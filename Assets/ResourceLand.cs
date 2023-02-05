using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ResourceLand : MonoBehaviour
{
    public ResourceType resource;
    public int maxTrees = 3;

    bool underCooldown = false;
    List<Tree> trees = new List<Tree>();

    Resource resourceDefinition;

    void Start()
    {
        resourceDefinition = ResourceManager.instance.GetResourceDefinition(resource);
    }

    void Update()
    {
        if (trees.Count > 0 && !underCooldown)
        {
            ResourceManager.instance.AddResource(
                resource, resourceDefinition.drainRate * Mathf.Min(trees.Count, maxTrees));
            StartCoroutine(Cooldown());
        }
    }

    IEnumerator Cooldown()
    {
        underCooldown = true;
        yield return new WaitForSeconds(1);
        underCooldown = false;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Root"))
        {
            TreeRoot root = other.gameObject.GetComponent<TreeRoot>();
            if (root != null)
            {
                Tree tree = root.tree;
                if (!trees.Contains(tree))
                {
                    trees.Add(tree);
                }
            }
        }
    }
}
