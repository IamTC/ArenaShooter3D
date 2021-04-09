using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasRenderInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Canvas a = GetComponent<Canvas>();
        a.renderMode = RenderMode.ScreenSpaceCamera;
        a.worldCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        a.planeDistance = 2;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
