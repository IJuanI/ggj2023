using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof (SpriteShapeController))]
public class TreeRoot : MonoBehaviour
{

    public GameObject enabledHalo;

    TreeRoot parent;


    void SetSelected(bool selected) {
        enabledHalo.SetActive(selected);
    }

    void spawnChild(Vector2 end) {

        GameObject child = Instantiate(gameObject, transform.parent);
        Spline childSpline = child.GetComponent<SpriteShapeController>().spline;
        childSpline.Clear();
        childSpline.InsertPointAt(0, transform.position);
        childSpline.InsertPointAt(1, end);

        TreeRoot childController = child.GetComponent<TreeRoot>();
        childController.parent = this;
    }

}
