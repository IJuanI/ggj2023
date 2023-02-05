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
            tree.prefab, transform.position, rotation, transform.parent);
        
        TreeRoot[] childRoots = treeInstance.GetComponentsInChildren<TreeRoot>();

        Vector2 parentPos = parent.parent.GetEnd();
        float distA = Vector2.Distance(parentPos, childRoots[0].transform.position);
        float distB = Vector2.Distance(parentPos, childRoots[2].transform.position);

        TreeRoot closestRoot = distA < distB ? childRoots[0] : childRoots[2];

        parent.Relocate(closestRoot.transform.position);
        parent.LockRoots(Settings.instance.saplingRootLock);

        foreach (TreeRoot childRoot in childRoots) {
            childRoot.properties.planet = parent.properties.planet;
        }
        
        Destroy(closestRoot.gameObject);
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
