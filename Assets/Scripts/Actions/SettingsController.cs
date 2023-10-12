using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsController: MonoBehaviour
{
    public Camera cam;
    private float changeColorSpeed = 1.1F;
    private Settings settings;
    
    void Start() {
        settings = SaveGame.LoadSettings();
        Toggle soundToggle = GameObject.Find("SoundToggle").GetComponent<Toggle>();
        Toggle musicToggle = GameObject.Find("MusicToggle").GetComponent<Toggle>();
        Toggle darkModeToggle = GameObject.Find("DarkModeToggle").GetComponent<Toggle>();
        soundToggle.isOn = settings.SoundOn;
        musicToggle.isOn = settings.MusicOn;
        darkModeToggle.isOn = settings.DarkModeOn;
        soundToggle.onValueChanged.AddListener(delegate {
            ToggleSound(soundToggle);
        });
        musicToggle.onValueChanged.AddListener(delegate {
            ToggleMusic(musicToggle);
        });
        darkModeToggle.onValueChanged.AddListener(delegate {
            ToggleDarkMode(darkModeToggle);
        });
    }

    public void ToggleSound(Toggle toggle)
    {
        settings.SoundOn = toggle.isOn;
        SaveGame.SaveSettings(settings);
    }
    public void ToggleMusic(Toggle toggle)
    {
        settings.MusicOn = toggle.isOn;
        if (toggle.isOn == true) {
            GameObject.FindGameObjectWithTag("Music").GetComponent<Music>().PlayMusic();
        } else {
            GameObject.FindGameObjectWithTag("Music").GetComponent<Music>().StopMusic();
        }
        SaveGame.SaveSettings(settings);
    }
    public void ToggleDarkMode(Toggle toggle)
    {
        changeColorSpeed = 0.1F;
        settings.DarkModeOn = toggle.isOn;
        SaveGame.SaveSettings(settings);
    }

    public void Back() {  
        SceneManager.LoadScene("Menu");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Back();
        }
        if (changeColorSpeed <= 1.01F) {
            Color destColor = new Color();
            if (settings.DarkModeOn == true) {
                ColorUtility.TryParseHtmlString("#222222", out destColor);
            } else {
                ColorUtility.TryParseHtmlString("#F4F7F5", out destColor);
            }
            cam.backgroundColor = Color.Lerp(cam.backgroundColor, destColor, changeColorSpeed);
            changeColorSpeed += 0.1F;
        }
    }
}
