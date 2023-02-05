using UnityEngine;

public class Obstacle : MonoBehaviour {

  public RootType[] bypassTypes;

  public bool IsBypassable(RootType type) {
    foreach (RootType bypassType in bypassTypes) {
      if (bypassType == type) {
        return true;
      }
    }

    return false;
  }

}