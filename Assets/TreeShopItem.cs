using UnityEngine;
using UnityEngine.UI;

public class TreeShopItem : MonoBehaviour
{

  public Image itemIcon;
  public GameObject blockedObj;
  public GameObject pricePanel;
  public GameObject ResourceCostPrefab;

  bool unlocked;
  TreeItem tree;
  PlanetSapling sapling;


  public void LoadItem(TreeItem tree, PlanetSapling sapling)
  {
    this.tree = tree;
    this.sapling = sapling;
    this.unlocked = ResourceManager.instance.IsTreeUnlocked(tree.id) || tree.unlockCost.type == ResourceType.None;
    this.blockedObj.SetActive(!unlocked);

    if (!unlocked)
    {
      DisplayCost(new ResourceCost[] { tree.unlockCost });
    }
    else
    {
      ResourceCost[] costs = new ResourceCost[tree.buyCost.Length];
      for (int i = 0; i < tree.buyCost.Length; i++)
      {
        costs[i] = tree.buyCost[i];
        costs[i].amount = Mathf.Floor(costs[i].amount * Mathf.Pow(ResourceManager.instance.GetTreeCount(tree), tree.growthExpRate));
      }

      DisplayCost(costs);
    }

    itemIcon.sprite = tree.shopItem;
  }

  public void Buy()
  {

    if (unlocked)
    {

      ResourceCost[] costs = new ResourceCost[tree.buyCost.Length];
      for (int i = 0; i < tree.buyCost.Length; i++)
      {
        costs[i] = tree.buyCost[i];
        costs[i].amount = Mathf.Floor(costs[i].amount * Mathf.Pow(ResourceManager.instance.GetTreeCount(tree), tree.growthExpRate));
      }

      if (ResourceManager.instance.PayResources(costs))
      {
        ShopManager.instance.CloseTreeShop();
        sapling.Grow(tree);
      }
    }
    else
    {
      if (ResourceManager.instance.PayResource(tree.unlockCost))
      {
        ResourceCost[] costs = new ResourceCost[tree.buyCost.Length];
        for (int i = 0; i < tree.buyCost.Length; i++)
        {
          costs[i] = tree.buyCost[i];
          costs[i].amount = Mathf.Floor(costs[i].amount * Mathf.Pow(ResourceManager.instance.GetTreeCount(tree), tree.growthExpRate));
        }

        DisplayCost(costs);

        ResourceManager.instance.UnlockTree(tree.id);
        unlocked = true;
        this.blockedObj.SetActive(!unlocked);
      }
    }
  }

  void DisplayCost(ResourceCost[] costs)
  {
    foreach (Transform child in pricePanel.transform)
    {
      Destroy(child.gameObject);
    }

    foreach (ResourceCost cost in costs)
    {
      GameObject costObj = Instantiate(ResourceCostPrefab, pricePanel.transform);
      ResourceHud hudScript = costObj.GetComponent<ResourceHud>();
      hudScript.SetIcon(ResourceManager.instance.GetResourceDefinition(cost.type).icon);
      hudScript.SetAmount((int)cost.amount);
    }
  }

}