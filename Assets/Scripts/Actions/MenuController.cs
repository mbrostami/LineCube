using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

public class MenuController: MonoBehaviour
{
    private LevelManagement levelManagement;
    void Awake() {
        levelManagement = SaveGame.LoadLevel();
        // Debug.Log("StartLevel:"+levelManagement.GetCurrentLevel().Number);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }
    
    public void StartGame() {  
        // Scenes.Load("Game", "level", levelManagement.GetCurrentLevel().Number.ToString());
        SceneManager.LoadScene("Loading");
    }

    public void Settings() {  
        SceneManager.LoadScene("Settings");
    }

    public void Donate() {  
        SceneManager.LoadScene("Donate");
    }

    public void Leaderboard() {  
        Social.ShowLeaderboardUI();
        // PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkIjKPc_tMNEAIQAQ");
    }

    public void ResetGame() {
        SaveGame.ClearAll(); 
        TilemapHandler.initialized = false;
        Application.Quit();
        // Scenes.Load("Game", "level", levelManagement.GetLevel(1).Number.ToString());
    }

    public void ExitGame() {
        Application.Quit();   
    }
}
