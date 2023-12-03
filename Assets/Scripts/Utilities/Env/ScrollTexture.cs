using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    public MeshRenderer render;

    public float scrollSpeed = 0.5f;

    float offset;

    void Update()
    {
        offset -= (Time.deltaTime * scrollSpeed) / 10.0f;     
        
        render.material.mainTextureOffset = new Vector2(0, offset);
    }
}