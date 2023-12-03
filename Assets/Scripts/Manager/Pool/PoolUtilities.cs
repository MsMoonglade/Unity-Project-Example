using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolUtilities : MonoBehaviour
{
    public static PoolUtilities instance;

    public List<PooledObject> pool = new List<PooledObject>();
    [HideInInspector]
    public List<GameObject> poolParent = new List<GameObject>();

    private void Awake()
    {
         instance = this; 

        foreach (PooledObject p in pool)
        {
            GameObject parent = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity, transform);
            parent.transform.name = p.obj_Parent_Name;

            poolParent.Add(parent);

            GeneratePool(p.obj_Prefs, p.initial_quantity, parent);
        }
    }

    public GameObject GetPooledItem(GameObject i_obj)
    {
        GameObject o_obj = null;

        //what pool to take from
        GameObject objParent = null;

        for(int i = 0; i < pool.Count; i++)
        {
            if (i_obj == pool[i].obj_Prefs)
            {
                objParent = poolParent[i];
                break;
            }
        }

        //get pooled object from correct pool         
        o_obj = PoolManager.instance.GetItem(objParent);

        //if not null return
        if (o_obj != null)
            return o_obj;

        //else instantiate new one and put in pool
        else
        {
            GameObject newObj = InstantiateNewObject(i_obj , objParent);
            return newObj;
        }
    }

    private GameObject InstantiateNewObject(GameObject i_obj , GameObject i_parent)
    { 
        GameObject o = PoolManager.instance.InstantiateInPool(i_obj, i_parent);
        o.gameObject.SetActive(false);
        
        return o;
    }

    private void GeneratePool(GameObject i_obj , int i_quantity , GameObject i_parent)
    {
        for (int i = 0; i < i_quantity; i++)
        {
            GameObject o = PoolManager.instance.InstantiateInPool(i_obj, i_parent);
            o.SetActive(false);
        }

        PoolManager.instance.pool_Parents.Add(i_parent);
    }
}

[System.Serializable]
public struct PooledObject
{   
    public GameObject obj_Prefs;
    public string obj_Parent_Name;
    public int initial_quantity;
}