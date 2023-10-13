using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSizeController : MonoBehaviour
{
    public Camera cam;

    // Start is called before the first frame update
    void Awake()
    {
        if (Screen.width > Screen.height && Screen.width > 1300) {
            cam.orthographicSize = 5;
        }
        // var upperLeftScreen  = new Vector3(0, Screen.height, 0);
        var upperRightScreen = new Vector3(Screen.width, Screen.height, 0);
        var lowerLeftScreen  = new Vector3(0, 0, 0);
        // var lowerRightScreen = new Vector3(Screen.width, 0, 0);
    
        //Corner locations in world coordinates
        // var upperLeft  = camera.ScreenToWorldPoint(upperLeftScreen);
        var lowerLeft  = cam.ScreenToWorldPoint(lowerLeftScreen);
        // var lowerRight = camera.ScreenToWorldPoint(lowerRightScreen);
        // g.transform.position.Set(lowerLeft.x, lowerLeft.y, 0);
        lowerLeft.z = 2;
        // transform.position = lowerLeft;
        
        cam.transform.position = new Vector3Int((int)(lowerLeft.x * -1), (int)(lowerLeft.y * -1) + 1, 0);
        Vector3 upperRight = cam.ScreenToWorldPoint(upperRightScreen);
        // var bottomLeftWorld = cam.ViewportToWorldPoint(Vector3.zero);
        var topRightWorld = cam.ViewportToWorldPoint(Vector3.one);
        // Debug.Log("bottomLeftWorld:"+bottomLeftWorld);
        // Debug.Log("topRightWorld:"+topRightWorld);
        // Debug.Log("Start Game with level:"+lowerLeft);
        // Debug.Log("upperRightScreen:"+upperRightScreen);
        // Debug.Log("upperRight:"+upperRight);
        LevelStateManager.DynamicMaxX = Mathf.FloorToInt(upperRight.x) - 1;
        // Debug.Log("LevelStateManager.DynamicMaxX:"+LevelStateManager.DynamicMaxX);
        if (Screen.width > Screen.height) {
            LevelStateManager.DynamicMaxY = Mathf.FloorToInt(upperRight.y) - 2;// - 3;
        } else {
            if (upperRight.x % 1.0f < 0.8f) {
                LevelStateManager.DynamicMaxY = Mathf.FloorToInt(upperRight.y) - 2;
            } else {
                LevelStateManager.DynamicMaxY = Mathf.FloorToInt(upperRight.y) - 1;
            }
        }
        // Debug.Log("LevelStateManager.DynamicMaxY:"+LevelStateManager.DynamicMaxY);
        // Debug.Log("LastPositionOfCamera:"+upperRight.x+":"+upperRight.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
