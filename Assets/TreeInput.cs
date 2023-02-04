using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class TreeInput : MonoBehaviour
{

  private TreeRoot selectedRoot = null;


  #region Input Actions

  public void OnMainAction(InputAction.CallbackContext eventData)
  {
    if (eventData.performed)
    {

      TreeRoot root = FindNearRoot(Mouse.current.position.ReadValue());
      if (root != null)
      {
        SelectRoot(root);
      }
    }
  }

  public void OnAltAction(InputAction.CallbackContext eventData)
  {
    if (eventData.performed)
    {
      if (selectedRoot != null)
      {
        selectedRoot.spawnChild(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
      }
    }
  }

  #endregion


  #region Business Logic

  TreeRoot FindNearRoot(Vector2 pos)
  {

    RaycastHit2D hit;
    Ray ray = Camera.main.ScreenPointToRay(pos);

    LayerMask mask = LayerMask.GetMask("RootNode");
    if (hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, mask))
    {
      TreeRoot root = hit.transform.GetComponentInParent<TreeRoot>();
      return root;
    }

    return null;
  }

  void SelectRoot(TreeRoot root)
  {
    if (selectedRoot != null)
    {
      selectedRoot.SetSelected(false);
    }

    selectedRoot = root;
    selectedRoot.SetSelected(true);

  }

  #endregion
}

