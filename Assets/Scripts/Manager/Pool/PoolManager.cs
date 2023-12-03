using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    [HideInInspector]
    public List<GameObject> pool_Parents = new List<GameObject>();  

    private void Awake()
    {
        instance = this;
    }

    public GameObject GetItem(GameObject i_parent)
    {
        GameObject o_pooledObj = null;

        foreach(Transform child in i_parent.transform)
        {           
            if (!child.gameObject.activeInHierarchy)
            {
                o_pooledObj = child.gameObject;
            }
        }

        return o_pooledObj;
    }

    public GameObject InstantiateInPool(GameObject i_obj, GameObject i_parent)
    {
        GameObject inst = Instantiate(i_obj, i_parent.transform.position, i_obj.transform.rotation, i_parent.transform);       
        return inst;
    }
}