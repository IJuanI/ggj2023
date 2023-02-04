using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ResourceLand : MonoBehaviour
{
    public ResourceType resource;

    bool isOccupied = false, underCooldown = false;

    Resource resourceDefinition;

    void Start()
    {
        resourceDefinition = ResourceManager.instance.GetResourceDefinition(resource);
    }

    void Update()
    {
        if (isOccupied && !underCooldown)
        {
            ResourceManager.instance.AddResource(resource, resourceDefinition.drainRate);
            StartCoroutine(Cooldown());
        }
    }

    IEnumerator Cooldown()
    {
        underCooldown = true;
        yield return new WaitForSeconds(1);
        underCooldown = false;
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (!isOccupied && other.gameObject.CompareTag("Root"))
        {
            isOccupied = true;
        }
    }
}
