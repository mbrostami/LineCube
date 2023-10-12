using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveGame
{
    public static string savePath = Application.persistentDataPath;
    public static LevelManagement levelManagement = null;
    public static LevelStateManager levelStateManager = null;
    public static Settings settings = null;

    public static void SaveSettings(Settings _settings)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(SaveGame.savePath + "/settings.save");
        bf.Serialize(file, _settings);
        file.Close();
    }

    public static Settings LoadSettings()
    {
        if (SaveGame.settings == null) { 
            if (File.Exists(SaveGame.savePath + "/settings.save")) {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(SaveGame.savePath + "/settings.save", FileMode.Open);
                SaveGame.settings = (Settings)bf.Deserialize(file);
                file.Close();
            }  else {
                if (Settings.instance == null) {
                    SaveGame.settings = new Settings();
                } else {
                    SaveGame.settings = Settings.instance;
                }
            }
        }
        return SaveGame.settings;
    }

    public static void SaveLevel(LevelManagement _levelManagement)
    {
        // Debug.Log("saveLevel");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(SaveGame.savePath + "/levels.save");
        bf.Serialize(file, _levelManagement);
        file.Close();
    }

    public static LevelManagement LoadLevel()
    {
        if (SaveGame.levelManagement == null) { 
            if (File.Exists(SaveGame.savePath + "/levels.save")) {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(SaveGame.savePath + "/levels.save", FileMode.Open);
                SaveGame.levelManagement = (LevelManagement)bf.Deserialize(file);
                file.Close();
            }  else {
                if (LevelManagement.instance == null) {
                    SaveGame.levelManagement = new LevelManagement();
                } else {
                    SaveGame.levelManagement = LevelManagement.instance;
                }
            }
        }
        return SaveGame.levelManagement;
    }

    public static void SaveLevelState(LevelStateManager _levelStateManager)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(SaveGame.savePath + "/levelstates.save");
        bf.Serialize(file, _levelStateManager);
        // Debug.Log("LevelStateStored!");
        file.Close();
    }
    
    public static void ClearLevelStateManager()
    {
        // Debug.Log("Clearing levelStates");
        File.Delete(SaveGame.savePath + "/levelstates.save");
        SaveGame.levelStateManager = null;
    }
    public static void ClearLevels()
    {
        // Debug.Log("Clearing levels");
        File.Delete(SaveGame.savePath + "/levels.save");
        SaveGame.levelManagement = null;
    }

    public static void ClearAll()
    {
        SaveGame.ClearLevelStateManager();
        SaveGame.ClearLevels();
    }

    public static LevelStateManager LoadLevelStateManager()
    {
        if (SaveGame.levelStateManager == null) { 
            if (File.Exists(SaveGame.savePath + "/levelstates.save")) {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(SaveGame.savePath + "/levelstates.save", FileMode.Open);
                SaveGame.levelStateManager = (LevelStateManager)bf.Deserialize(file);
                file.Close();
                // Debug.Log("LevelStateLoaded!");
            }  else {
                if (LevelStateManager.instance == null) {
                    SaveGame.levelStateManager = new LevelStateManager();
                } else {
                    SaveGame.levelStateManager = LevelStateManager.instance;
                }
            }
        }
        return SaveGame.levelStateManager;
    }
}