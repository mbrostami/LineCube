using SystemRandom = System.Random;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using UnityEngine.UI;

public class ButtonActions : MonoBehaviour
{
    public Tilemap tilemap;
    private LevelManagement levelManagement;
    private Settings settings;
    private AudioSource audioHint;
    private AudioSource audioHintFailed;
    // private Image BuyPopup;

    void Awake() {
        // BuyPopup = GameObject.Find("BuyPopUp").GetComponent<Image>();
        // BuyPopup.rectTransform.localScale = new Vector3(0, 0, 0);
        // BuyPopup.gameObject.SetActive(false);
    }

    void Start() {
        levelManagement = SaveGame.LoadLevel();
        settings = SaveGame.LoadSettings();
        audioHint = GetComponent<AudioSource>();
        AudioSource[] audioes = GetComponents<AudioSource>();
        audioHint = audioes[0];
        audioHintFailed = audioes[1];
        if (settings.SoundOn == false) {
            audioHint.volume = 0;
            audioHintFailed.volume = 0;
        } else {
            audioHint.volume = 0.7F;
            audioHintFailed.volume = 0.7F;
        }
    }

    // Update is called once per frame
    public void SolutionTilemap()
    {
        TilemapHandler tilemapHandler = TilemapHandler.instance;
        Tilemap tilemap = tilemapHandler.tileMap;
        tilemap.ClearAllTiles();
        tilemapHandler.tiles.Clear();
        foreach (KeyValuePair<Vector3Int, GameTile> item in tilemapHandler.originalTiles)
        {
            GameTile _gameTile = item.Value;
            _gameTile.Position = item.Key;
            tilemapHandler.tiles.Add(item.Key, _gameTile);
            tilemap.SetTile(item.Key, _gameTile);
        }
        tilemapHandler.FinishLevel();
    }

    public void UseHint()
    {
        TilemapHandler tilemapHandler = TilemapHandler.instance;
        if (tilemapHandler.UseHint()) {
            audioHint.Play(0);
        } else {
            audioHintFailed.Play(0);
        }
    }

    public void ClosePopup()
    {
        Text message = GameObject.Find("PaymentMessage").GetComponent<Text>();
        message.text = "";
        // BuyPopup.rectTransform.localScale = new Vector3(0, 0, 0);
        // BuyPopup.gameObject.SetActive(false);
    }

    public void BuyPopUp()
    {
        // BuyPopup.gameObject.SetActive(true);
        StartCoroutine("OpenPopup");
    }


    public void GenerateTiles()
    {
        SystemRandom random = new SystemRandom();
        TilemapHandler tilemapHandler = TilemapHandler.instance;
        tilemapHandler.init(tilemap);
        tilemapHandler.Start();
    }

    public void RecreateTiles()
    {
        TilemapHandler tilemapHandler = TilemapHandler.instance;
        tilemapHandler.RefreshLevel();
    }

    public void ShuffleTiles()
    {
        TilemapHandler tilemapHandler = TilemapHandler.instance;
        tilemapHandler.ShuffleTiles();
    }

    public void GoToNextLevel()
    {
        // Debug.Log("Can Play next level:"+levelManagement.CanPlayNextLevel());
        if (levelManagement.CanPlayNextLevel()) {
            levelManagement.GoNextLevel();
            SaveGame.SaveLevel(levelManagement);
            SceneManager.LoadScene("Loading"); // loading first
        }
        // Scenes.Load("Game", "level", levelManagement.GetCurrentLevel().Number.ToString());
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void LevelMenu()
    {
        SceneManager.LoadScene("Levels");
    }
}
