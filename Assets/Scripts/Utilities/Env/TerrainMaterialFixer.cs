using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMaterialFixer : MonoBehaviour
{
    public float scaler;
        
    void Awake()
    {
        transform.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(1, -(transform.localScale.z / scaler));
    }
}
