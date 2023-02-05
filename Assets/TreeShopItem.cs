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
      if (ResourceManager.instance.PayResources(tree.buyCost)) {
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