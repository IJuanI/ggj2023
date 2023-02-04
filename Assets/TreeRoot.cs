using System;
using UnityEngine;
using UnityEngine.U2D;

[Serializable]
public struct RootProperties {

    public Planet planet;
    public RootType type;
    public ResourceType resource;
    public float cost;

    public RootProperties(Planet planet, RootType type, ResourceType resource, float cost) {
        this.planet = planet;
        this.type = type;
        this.resource = resource;
        this.cost = cost;
    }
}

[Serializable]
public struct RootSkin {
    public Sprite sprite;
    public Color color;
}

[RequireComponent(typeof (SpriteShapeController), typeof(EdgeCollider2D))]
public class TreeRoot : MonoBehaviour
{

    public RootSkin defaultSkin;
    public RootSkin selectedSkin;

    [SerializeField]
    RootProperties properties;
    TreeRoot parent;

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
        currSpline = GetComponent<SpriteShapeController>().spline;
    }


    public void SetSelected(bool selected) {
        if (selected) {
            skinRender.sprite = selectedSkin.sprite;
            skinRender.color = selectedSkin.color;
        } else {
            skinRender.sprite = defaultSkin.sprite;
            skinRender.color = defaultSkin.color;
        }
    }

    public void ExpandRoot(Vector2 end) {

        if (!BuyRoot() || !ComputeConstraints(ref end)) {
            return;
        }

        GameObject child = InstantiateChild(end);

        UpdateSelection(child);
    }


    bool BuyRoot() {
        return ResourceManager.instance.PayResource(properties.resource, properties.cost);
    }

    bool ComputeConstraints(ref Vector2 end) {
        
        Vector2 start = currSpline.GetPosition(1);
        Vector2 direction = end - start;

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
        }

        RaycastHit2D obstacleHit = Physics2D.Raycast(
            start + direction.normalized * .1f, direction,
            direction.magnitude, LayerMask.GetMask("Obstacle"));
        if (obstacleHit.collider != null) {
            end = obstacleHit.point;
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

        Spline childSpline = child.GetComponent<SpriteShapeController>().spline;
        childSpline.Clear();
        childSpline.InsertPointAt(0, currSpline.GetPosition(1));
        childSpline.InsertPointAt(1, end);

        foreach (Transform grandchild in child.transform) {
            grandchild.transform.position = childSpline.GetPosition(1);
        }
        
        TreeRoot childController = child.GetComponent<TreeRoot>();
        childController.parent = this;
        childController.properties = properties;

        return child;
    }

    void UpdateSelection(GameObject childInstance) {

        TreeRoot childController = childInstance.GetComponent<TreeRoot>();

        TreeInput.instance.SelectRoot(
            Settings.instance.selectionMode == SelectionMode.Keep ? this : childController);        
    }

}
