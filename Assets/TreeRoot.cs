using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[Serializable]
public struct RootProperties {

    public Planet planet;
    public RootType type;
    public ResourceCost cost;
    public int vitality;
    public bool growOnPlanet;

}

[Serializable]
public struct RootSkin {
    public Sprite sprite;
    public Color color;

    public RootSkin(Sprite sprite, Color color) {
        this.sprite = sprite;
        this.color = color;
    }
}

[RequireComponent(typeof (SpriteShapeController), typeof(EdgeCollider2D))]
public class TreeRoot : MonoBehaviour
{

    private static RootSkin emptySkin = new RootSkin(null, new Color(1, 1, 1, 0));

    public RootSkin defaultSkin;
    public RootSkin selectedSkin;

    [SerializeField]
    public RootProperties properties;
    public TreeRoot parent;
    public Tree tree;
    List<TreeRoot> children = new List<TreeRoot>();

    bool selected = false, visible = false;
    Spline currSpline;

    SpriteRenderer _skinRender;
    SpriteRenderer skinRender {
        get {
            if (_skinRender == null) {
                _skinRender = GetComponentInChildren<SpriteRenderer>();
            }
            return _skinRender;
        }
    }

    void Start()
    {
        tree = GetComponentInParent<Tree>();
        currSpline = GetComponent<SpriteShapeController>().spline;

        UpdateCameraPivots(gameObject);
        ResourceManager.instance.AddRoot();
    }

    public void SetVisible(bool visible) {
        this.visible = visible;

        if (selected) {
            return;
        }

        if (visible || parent == null) {
            SetSkin(defaultSkin);
        } else {
            SetSkin(emptySkin);
        }
    }

    public void SetSelected(bool selected) {
        this.selected = selected;
        if (selected) {
            SetSkin(selectedSkin);
        } else {
            if (visible || parent == null) {
                SetSkin(defaultSkin);
            } else {
                SetSkin(emptySkin);
            }
        }
    }

    void SetSkin(RootSkin skin) {
        skinRender.sprite = skin.sprite;
        skinRender.color = skin.color;
    }

    public void LockRoots(int lockCount) {
        if (properties.vitality > 0 && lockCount <= 0) {
            return;
        }

        Lock();
        if (children.Count > 0) {
            children[0].LockRoots(lockCount - 1);
        }

        if (parent != null) {
            parent.LockRoots(lockCount - 1);
        }
    }

    public Vector2 GetEnd() {
        return transform.TransformPoint(currSpline.GetPosition(1));
    }

    public void Lock() {
        ResourceCost cost = properties.cost;
        ResourceManager.instance.AddResource(cost.type, Mathf.Ceil(cost.amount / 2));
        properties.vitality = 0;
    }

    public void Relocate(Vector2 end) {
        Vector2 start = transform.TransformPoint(currSpline.GetPosition(0));
        transform.position = end;
        currSpline.SetPosition(0, transform.InverseTransformPoint(start));
        currSpline.SetPosition(1, transform.InverseTransformPoint(end));

        foreach (TreeRoot child in children) {
            child.currSpline.SetPosition(0, end);
        }
    }

    public GameObject ExpandRoot(Vector2 end, bool force = false) {

        bool outOfPlanet = false;
        if (!force && (!CheckRequisites() || !ComputeConstraints(ref end, out outOfPlanet))) {
            return null;
        }

        GameObject child = InstantiateChild(end);
        TreeRoot childRoot = child.GetComponent<TreeRoot>();

        if (outOfPlanet && properties.growOnPlanet) {
            PlanetSapling.CreateSlot(end, childRoot);
        }

        UpdateSelection(child);
        return child;
    }


    bool CheckRequisites() {
        return properties.vitality > 0
            && ResourceManager.instance.PayResource(
                new ResourceCost(properties.cost.type, Mathf.Floor(properties.cost.amount
                    * Mathf.Pow(ResourceManager.instance.GetRootCount(),
                    Settings.instance.rootCostExpGrowth))));
    }

    bool ComputeConstraints(ref Vector2 end, out bool outOfPlanet) {
        
        Vector2 start = transform.TransformPoint(currSpline.GetPosition(1));
        Vector2 direction = end - start;
        outOfPlanet = false;

        var distance = Vector2.Distance(start, end);
        if (distance > Settings.instance.maxRootLength) {
            end = start + (direction).normalized * Settings.instance.maxRootLength;
            direction = end - start;
        }

        bool alteredLength = false;
        bool isInsidePlanet = properties.planet.planetCollider.OverlapPoint(end);

        // If end is not inside of planet, move it to the edge
        if (!isInsidePlanet) {
            // Calculate collision point between circle and line (start, end)
            Vector2 center = properties.planet.transform.position;
            Vector2 line = direction;
            float a = line.sqrMagnitude;
            float b = 2 * Vector2.Dot(line, start - center);
            float c = Vector2.Dot(start - center, start - center)
                - properties.planet.planetCollider.radius * properties.planet.planetCollider.radius;
            float delta = b * b - 4 * a * c;
            float t1 = (-b + Mathf.Sqrt(delta)) / (2 * a);
            float t2 = (-b - Mathf.Sqrt(delta)) / (2 * a);
            float t = Mathf.Min(Math.Abs(t1), Math.Abs(t2));
            end = start + line * t;
            direction = end - start;
            alteredLength = true;
            outOfPlanet = true;
        }

        RaycastHit2D[] obstacleHits = Physics2D.RaycastAll(
            start + direction.normalized * .1f, direction,
            direction.magnitude, LayerMask.GetMask("Obstacle"));

        bool bypassableObstacle = false;
        Vector2 closestPoint = Vector2.positiveInfinity;

        foreach (var obstacleHit in obstacleHits) {

            if (obstacleHit.transform == transform) {
                continue;
            }

            Obstacle obstacleScript = obstacleHit.transform.GetComponent<Obstacle>();
            if (obstacleScript != null && obstacleScript.IsBypassable(properties.type)) {
                continue;
            }

            if (obstacleHit.point.sqrMagnitude < closestPoint.sqrMagnitude) {
                closestPoint = obstacleHit.point;
            }

            bypassableObstacle = true;
        }

        if (bypassableObstacle) {
            end = closestPoint;
            direction = end - start;
            alteredLength = true;
        }

        distance = Vector2.Distance(start, end);
        if (distance < Settings.instance.minRootLength) {
            if (alteredLength) {
                return false;
            }

            end = start + (direction).normalized * Settings.instance.minRootLength;
            direction = end - start;
        }

        return true;
    }

    GameObject InstantiateChild(Vector2 end) {
        GameObject child = Instantiate(gameObject, transform.parent);
        child.transform.position = end;

        Spline childSpline = child.GetComponent<SpriteShapeController>().spline;
        childSpline.Clear();
        childSpline.InsertPointAt(0, child.transform
            .InverseTransformPoint(transform.TransformPoint(currSpline.GetPosition(1))));
        childSpline.SetHeight(0, (properties.vitality + 1) * Settings.instance.rootHeightScale);
        childSpline.InsertPointAt(1, child.transform.InverseTransformPoint(end));
        childSpline.SetHeight(1, (properties.vitality) * Settings.instance.rootHeightScale);
        
        TreeRoot childController = child.GetComponent<TreeRoot>();
        children.Add(childController);
        childController.parent = this;
        childController.properties = properties;
        --childController.properties.vitality;

        return child;
    }

    void UpdateCameraPivots(GameObject childs) {
        if (transform.position.y < CameraController.instance.minPivot.transform.position.y) {
            Vector2 pivotPos = CameraController.instance.minPivot.transform.position;
            pivotPos.y = transform.position.y;
            CameraController.instance.minPivot.transform.position = pivotPos;
        }

        if (transform.position.y > CameraController.instance.maxPivot.transform.position.y) {
            Vector2 pivotPos = CameraController.instance.maxPivot.transform.position;
            pivotPos.y = transform.position.y;
            CameraController.instance.maxPivot.transform.position = pivotPos;
        }

        if (transform.position.x < CameraController.instance.minPivot.transform.position.x) {
            Vector2 pivotPos = CameraController.instance.minPivot.transform.position;
            pivotPos.x = transform.position.x;
            CameraController.instance.minPivot.transform.position = pivotPos;
        }

        if (transform.position.x > CameraController.instance.maxPivot.transform.position.x) {
            Vector2 pivotPos = CameraController.instance.maxPivot.transform.position;
            pivotPos.x = transform.position.x;
            CameraController.instance.maxPivot.transform.position = pivotPos;
        }
    }

    void UpdateSelection(GameObject childInstance) {

        TreeRoot childController = childInstance.GetComponent<TreeRoot>();

        TreeInput.instance.SelectRoot(
            Settings.instance.selectionMode == SelectionMode.Keep ? this : childController);        
    }

}
