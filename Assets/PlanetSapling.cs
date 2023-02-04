using UnityEngine;

public class PlanetSapling : MonoBehaviour
{

    TreeRoot parent;

    public void OpenShop() {

        ShopManager.instance.OpenTreeShop(parent.properties.planet, this);
    }

    public void Grow(TreeItem tree) {

        Vector2 planetCenter = parent.properties.planet.transform.position;
        Vector2 spawnPosition = planetCenter
            + ((Vector2)transform.position - planetCenter).normalized
            * (parent.properties.planet.planetCollider.radius + Settings.instance.saplingOffset);

        Quaternion rotation = Quaternion.FromToRotation(
            Vector3.up, transform.position - parent.properties.planet.transform.position);
        GameObject treeInstance = Instantiate(
            tree.prefab, transform.position, rotation, parent.transform.parent);
        
        TreeRoot[] childRoots = treeInstance.GetComponentsInChildren<TreeRoot>();

        GameObject boundRoot = parent.ExpandRoot(childRoots[0].transform.position, true);
        boundRoot.transform.SetParent(childRoots[0].transform.parent);

        foreach (TreeRoot childRoot in childRoots) {
            childRoot.properties.planet = parent.properties.planet;
        }
        
        Destroy(childRoots[0].gameObject);
        Destroy(gameObject);
    }


    public static PlanetSapling CreateSlot(Vector2 position, TreeRoot parent) {

        Planet planet = parent.properties.planet;

        Vector2 planetCenter = planet.transform.position;
        Vector2 spawnPosition = planetCenter
            + (position - planetCenter).normalized
            * (planet.planetCollider.radius + Settings.instance.saplingOffset);

        GameObject sappling = Instantiate(
            planet.saplingPrefab, spawnPosition, Quaternion.identity, planet.transform);

        PlanetSapling sapplingScript = sappling.GetComponent<PlanetSapling>();
        sapplingScript.parent = parent;

        return sapplingScript;
    }

}
