using System.Collections;
using UnityEngine;

public class Tree : MonoBehaviour
{

  public ResourceCost[] unkeeps;

  public ResourceCost[] generates;

  public ResourceCost[] storage;

  bool underCooldown = false;

  private void Start()
  {
    foreach (ResourceCost cost in storage)
    {
      ResourceManager.instance.AddCapacity(cost.type, (int) cost.amount);
    }
  }

  void Update()
  {
    if (!underCooldown)
    {
      foreach (ResourceCost cost in generates)
      {
        ResourceManager.instance.AddResource(cost.type, cost.amount);
      }

      foreach (ResourceCost cost in unkeeps)
      {
        ResourceManager.instance.RemoveResource(cost);
      }

      StartCoroutine(Cooldown());
    }
  }

  IEnumerator Cooldown()
  {
    underCooldown = true;
    yield return new WaitForSeconds(1);
    underCooldown = false;
  }
}