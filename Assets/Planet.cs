using System;
using UnityEngine;

public enum PlanetType {
  Default
}

[Serializable]
public struct TreeItem {

  public String id;
  public ResourceCost unlockCost;
  public ResourceCost[] buyCost;
  public GameObject prefab;
  public Sprite shopItem;
  public float growthExpRate;
  
  public TreeItem(String id, ResourceCost unlockCost, ResourceCost[] buyCost,
    GameObject prefab, Sprite shopItem, float growthExpRate) {
    this.id = id;
    this.unlockCost = unlockCost;
    this.buyCost = buyCost;
    this.prefab = prefab;
    this.shopItem = shopItem;
    this.growthExpRate = growthExpRate;
  }

  public override bool Equals(object obj) {
    if (obj == null || GetType() != obj.GetType()) {
      return false;
    }

    TreeItem other = (TreeItem) obj;
    return id.Equals(other.id);
  }

  public override int GetHashCode() {
    return id.GetHashCode();
  }

}

[RequireComponent(typeof(CircleCollider2D))]
public class Planet : MonoBehaviour {

  public PlanetType type;

  public GameObject saplingPrefab;

  public TreeItem[] trees;


  [HideInInspector]
  public CircleCollider2D planetCollider;

  void Start() {
    planetCollider = GetComponent<CircleCollider2D>();
  }
  
}