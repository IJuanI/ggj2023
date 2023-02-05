using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{

    // Singleton
    public static ShopManager instance;

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
    

    public GameObject treeShop;
    public Transform treeShopContent;
    public Sprite[] treeShopFrames;
    public GameObject treeShopItemPrefab;


    public void OpenTreeShop(Planet planet, PlanetSapling sapling) {

      foreach (Transform child in treeShopContent) {
        Destroy(child.gameObject);
      }

      foreach (TreeItem tree in planet.trees) {

        GameObject shopItem = Instantiate(treeShopItemPrefab, treeShopContent);
        shopItem.GetComponent<Image>().sprite
          = treeShopFrames[Random.Range(0, treeShopFrames.Length)];

        shopItem.GetComponent<TreeShopItem>().LoadItem(tree, sapling);
      }

      treeShop.SetActive(true);
    }

    public void CloseTreeShop() {
      treeShop.SetActive(false);
    }

}
