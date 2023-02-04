using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof (SpriteShapeController)/* , typeof(PolygonCollider2D) */)]
public class TreeRoot : MonoBehaviour
{

    public GameObject enabledHalo;

    TreeRoot parent;


    public void SetSelected(bool selected) {
        if (enabledHalo) {
            enabledHalo.SetActive(selected);
        }
    }

    public void spawnChild(Vector2 end) {

        Spline currSpline = GetComponent<SpriteShapeController>().spline;

        GameObject child = Instantiate(gameObject, transform.parent);
        Spline childSpline = child.GetComponent<SpriteShapeController>().spline;
        childSpline.Clear();
        childSpline.InsertPointAt(0, currSpline.GetPosition(1));
        childSpline.InsertPointAt(1, end);

        CircleCollider2D nodeCollider = child.GetComponentInChildren<CircleCollider2D>();
        nodeCollider.transform.position = end;

        TreeRoot childController = child.GetComponent<TreeRoot>();
        childController.parent = this;
    }

}
