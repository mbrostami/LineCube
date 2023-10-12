using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Tilemaps;

public class Loading : MonoBehaviour 
{
    private RectTransform loadingRect1;
    private RectTransform loadingRect2;
    void Start() {
        Image img1 = GameObject.Find("LoadingLogoMain").GetComponent<Image>();
        Image img2 = GameObject.Find("LoadingLogo").GetComponent<Image>();
        loadingRect1 = img1.rectTransform; 
        loadingRect2 = img2.rectTransform;

        StartCoroutine(LoadLevelAfterDelay());
    }    

    IEnumerator LoadLevelAfterDelay()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");
        
        while (!asyncLoad.isDone)
        {
            // Debug.Log("LOAD SCENE: Progress: " + asyncLoad.progress);
            yield return null;
        }
    }

    void Update() {
        loadingRect1.Rotate(0f, 0f, 200f * Time.deltaTime);
        loadingRect2.Rotate(0f, 0f, 100f * Time.deltaTime);
    }
}