using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuToggle : MonoBehaviour
{
    public Image topMenu = null;
    private bool startAnimate = false;
    // private bool startRotation = false;
    private bool closeMenu = false;
    private float originalY;
    // private float rotation = 0;
    // Start is called before the first frame update
    void Start()
    {
        originalY = topMenu.gameObject.GetComponent<RectTransform>().anchoredPosition.y;
        // Debug.Log("originalY:"+originalY);
        topMenu.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (originalY * -1));
    }

    public void ToggleMenu()
    {
        float y = topMenu.gameObject.GetComponent<RectTransform>().anchoredPosition.y;
        if (y == originalY) {
            closeMenu = true;
        } else {
            closeMenu = false;
        }
        // startRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (startAnimate == true) {
            float y = topMenu.gameObject.GetComponent<RectTransform>().anchoredPosition.y;
            if (closeMenu == true) {
                if (y >= (originalY * -1)) {
                    y = (originalY * -1);
                    startAnimate = false;
                } else {
                    y += 5F;
                }
            } else {
                if (y <= originalY) { 
                    y = originalY;
                    startAnimate = false;
                } else {
                    y -= 5F;
                }
            }
            topMenu.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
        }
    }
}
