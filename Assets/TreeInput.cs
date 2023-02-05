using UnityEngine;
using UnityEngine.InputSystem;

public class TreeInput : MonoBehaviour
{

  // Singleton

  public static TreeInput instance;

  void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
    else
    {
      Destroy(gameObject);
    }
  }

  private TreeRoot selectedRoot = null;


  #region Input Actions

  public void OnMainAction(InputAction.CallbackContext eventData)
  {
    if (eventData.performed)
    {

      Transform selection = FindNearSelectable(Mouse.current.position.ReadValue());
      if (selection != null)
      {
        TreeRoot root = selection.GetComponentInParent<TreeRoot>();
        if (root != null) {
            SelectRoot(root);
        }

        PlanetSapling sapling = selection.GetComponent<PlanetSapling>();
        if (sapling != null) {
            sapling.OpenShop();
        }
      }
    }
  }

  public void OnAltAction(InputAction.CallbackContext eventData)
  {
    if (eventData.performed)
    {
      if (selectedRoot != null)
      {
        selectedRoot.ExpandRoot(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
      }
    }
  }

  #endregion


  #region Business Logic

  Transform FindNearSelectable(Vector2 pos)
  {

    RaycastHit2D hit;
    Ray ray = Camera.main.ScreenPointToRay(pos);

    LayerMask mask = LayerMask.GetMask("SelectLayer");
    if (hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, mask))
    {
        return hit.transform;
    }

    return null;
  }

  public void SelectRoot(TreeRoot root)
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

