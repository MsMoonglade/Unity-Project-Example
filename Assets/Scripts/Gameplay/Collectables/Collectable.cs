using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public int value;

    protected bool taken = false;
      
    public virtual void Take()
    {
        taken = true;
    }
}