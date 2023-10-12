using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{
    // private float maxOrthographicSize = 6.00F;
    // private float orthographicSize = 5.00F;
    // private float minOrthographicSize = 4.00F;
    // private float panSpeed = -0.1f;
    private Settings settings;
    // Start is called before the first frame update
    void Start()
    {
        // if (GetComponent<AudioSource>() != null) {
        //     AudioSource musicAudio = GetComponent<AudioSource>();
        //     settings = SaveGame.LoadSettings();
        //     if (settings.MusicOn == false) {
        //         musicAudio.volume = 0;
        //     } else {
        //         musicAudio.volume = 0.07F;
        //     }
        // }
        // GetComponent<Camera>().orthographicSize = 5 - 2;
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
        // Camera cam = GetComponent<Camera>();
        // if (Input.GetAxis("Mouse ScrollWheel") > 0 && cam.orthographicSize >= minOrthographicSize) {
        //     cam.orthographicSize = orthographicSize - panSpeed;
        //     orthographicSize = orthographicSize - panSpeed;
        // }
        // if (Input.GetAxis("Mouse ScrollWheel") < 0 && cam.orthographicSize <= maxOrthographicSize) {
        //     cam.orthographicSize = orthographicSize + panSpeed;
        //     orthographicSize = orthographicSize + panSpeed;
        // }
        // if (Input.touchCount == 2) {
        //     Touch touchZero = Input.GetTouch(0);
        //     Touch touchOne = Input.GetTouch(1);
           
        //     // Find the position in the previous frame of each touch.
        //     Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        //     Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
           
        //     // Find the magnitude of the vector (the distance) between the touches in each frame.
        //     float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        //     float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
           
        //     // Find the difference in the distances between each frame.
        //     float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
           
        //     // If the camera is orthographic...
        //     if (cam.orthographic)
        //     {
        //         // ... change the orthographic size based on the change in distance between the touches.
        //         cam.orthographicSize += deltaMagnitudeDiff * panSpeed;            }
        // }
        // // If the camera is orthographic...
        // if (cam.orthographic)
        // {
        //     // Make sure the orthographic size never drops below zero.
        //     cam.orthographicSize = Mathf.Max(cam.orthographicSize, minOrthographicSize);
            
        //     // Make sure the orthographic size never goes above original size.
        //     cam.orthographicSize = Mathf.Min(cam.orthographicSize, maxOrthographicSize);
        // }
    }
}
