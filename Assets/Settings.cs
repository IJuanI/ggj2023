using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectionMode {
    Keep,
    Next
}

public class Settings : MonoBehaviour
{

    // Singleton
    public static Settings instance;

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
    

    public float maxRootLength = 5f;

    public float minRootLength = 1f;

    public float saplingOffset = 0.5f;

    public float treeOffset = -0.2f;

    public SelectionMode selectionMode = SelectionMode.Next;
}
