using UnityEngine;

public enum PlanetType {
  Default
}


[RequireComponent(typeof(CircleCollider2D))]
public class Planet : MonoBehaviour {

  public PlanetType type;

  [HideInInspector]
  public CircleCollider2D planetCollider;

  void Start() {
    planetCollider = GetComponent<CircleCollider2D>();
  }
  
}