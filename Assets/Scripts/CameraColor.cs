using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraColor : MonoBehaviour
{
    // public Text text = null;
    // Start is called before the first frame update
    void Start()
    {
        Settings settings = SaveGame.LoadSettings();
        if (settings.DarkModeOn == true) {
            Camera camera = GetComponent<Camera>();
            Color myColor = new Color();
            ColorUtility.TryParseHtmlString("#222222", out myColor);
            camera.backgroundColor = myColor;
            // if (text != null) {
            //     Color textColor = new Color();
            //     ColorUtility.TryParseHtmlString("#F4F7F5", out textColor);
            //     text.color = textColor;
            // }
        } else {
            Camera camera = GetComponent<Camera>();
            Color myColor = new Color();
            ColorUtility.TryParseHtmlString("#F4F7F5", out myColor);
            camera.backgroundColor = myColor;
            // if (text != null) {
            //     Color textColor = new Color();
            //     ColorUtility.TryParseHtmlString("#2D3142", out textColor);
            //     text.color = textColor;
            // }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
