using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class TreeInput : MonoBehaviour
{

    public SpriteShapeController arbolito;

    public void OnMainAction (InputAction.CallbackContext eventData)
    {
        if (eventData.performed) {
            Spline s = arbolito.spline;
            s.InsertPointAt(1, Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
            // Debug.Log(Mouse.current.position.ReadValue());
        }
    }
}

