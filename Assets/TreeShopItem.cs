using UnityEngine;
using UnityEngine.UI;

public class TreeShopItem : MonoBehaviour
{

  public Image itemIcon;

  bool unlocked;
  TreeItem tree;
  PlanetSapling sapling;


  public void LoadItem(TreeItem tree, PlanetSapling sapling) {
    this.tree = tree;
    this.sapling = sapling;
    this.unlocked = ResourceManager.instance.IsTreeUnlocked(tree.id) || tree.unlockCost.type == ResourceType.None;

    itemIcon.sprite = tree.shopItem;
  }

  public void Buy() {

    if (unlocked) {

      ResourceCost[] costs = new ResourceCost[tree.buyCost.Length];
      for (int i = 0; i < tree.buyCost.Length; i++) {
        costs[i] = tree.buyCost[i];
        costs[i].amount *= Mathf.Pow(ResourceManager.instance.GetTreeCount(tree), tree.growthExpRate);
      }

      if (ResourceManager.instance.PayResources(costs)) {
        ShopManager.instance.CloseTreeShop();
        sapling.Grow(tree);
      }
    } else {
      if (ResourceManager.instance.PayResource(tree.unlockCost)) {
        ResourceManager.instance.UnlockTree(tree.id);
        unlocked = true;
      }
    }    
  }

}