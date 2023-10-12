[System.Serializable]
public class Settings
{
    public bool SoundOn;
    public bool MusicOn;
    public bool DarkModeOn;
    public static string DarkColor = "222222"; 
    public static string LightColor = "F4F7F5"; 
    public static Settings instance;

    public Settings()
    {
        SoundOn = true;
        MusicOn = true;
        DarkModeOn = false;
        if (Settings.instance == null) {
            Settings.instance = this;
        }
    }
}