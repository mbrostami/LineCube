using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SocialPlatforms;
using SystemRandom = System.Random;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GridHolder : MonoBehaviour
{
    public Tilemap Preview;
    public Tilemap HoverTilemap;
    public Tilemap Tilemap;
    private TilemapHandler tilemapHandler;
    private GameTile movingTile;
    private bool isMoving = false;

    private LevelManagement levelManagement;
    private AudioSource audioSelect;
    private AudioSource audioReplace;
    private AudioSource audioLevelCompleted;
    private bool freeze = false;
    private Settings settings;
    private RectTransform loadingRect;

    private int mouseX = 0;
    private int mouseY = 0;
    // private float changeColorSpeed = 1.1F;
    public Camera cam;

    private bool changeColorStarted = false;
    // public Image BuyPopup;

    void Start()
    {
        freeze = false;
        settings = SaveGame.LoadSettings();
        AudioSource[] audioes = GetComponents<AudioSource>();
        audioSelect = audioes[0];
        audioReplace = audioes[1];
        audioLevelCompleted = audioes[2];
        if (settings.SoundOn == false) {
            audioSelect.volume = 0;
            audioReplace.volume = 0;
            audioLevelCompleted.volume = 0;
        } else {
            audioSelect.volume = 0.6F;
            audioReplace.volume = 0.7F;
            audioLevelCompleted.volume = 0.6F;
        }
        TilemapHandler.audioLevelCompleted = audioLevelCompleted;
        if (TilemapHandler.initialized == false) {
            tilemapHandler = new TilemapHandler();
        } else {
            tilemapHandler = TilemapHandler.instance;
        }
        TilemapHandler.levelCompletedTrigger = false;
        levelManagement = SaveGame.LoadLevel();
        // Debug.Log("Start Game with level:"+levelManagement.GetCurrentLevel().Number);
        if (levelManagement.CanPlayNextLevel()) {
            levelManagement.GoNextLevel();
        }
        levelManagement.ShowAdHint();
        tilemapHandler.GoToLevel(
            Tilemap, 
            levelManagement.GetCurrentLevel().Number
        );
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mousePosition.x == 0 || Input.mousePosition.y == 0 || Input.mousePosition.x >= Screen.width - 1 || Input.mousePosition.y >= Screen.height - 1) 
        {
            return;
        }
        // if (BuyPopup.IsActive() == true) {
        //     return;
        // }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("Menu");
            return;
        }
        if (freeze == true) {
            if (Input.GetMouseButtonDown(0))
            {
                if (!levelManagement.NextLevelExists()) { // if levels finished, generate new levels
                    levelManagement.AddNewLevel(true);
                }
                if (levelManagement.CanPlayNextLevel()) {
                    levelManagement.GoNextLevel();
                    SaveGame.SaveLevel(levelManagement);
                    TilemapHandler.levelCompletedTrigger = false;
                    SceneManager.LoadScene("Loading"); // loading first
                }
            }
            return;
        }
        if (TilemapHandler.levelCompletedTrigger == true) { 
            if (changeColorStarted == false) {
                StartCoroutine("ChangeBGColor");
                changeColorStarted = true;
            }
            if (tilemapHandler.originalTilesPositions.Count > 0) {
                SystemRandom rand = new SystemRandom();
                GameTile completeGameTile = ScriptableObject.CreateInstance<GameTile>();
                Vector3Int vector3Int = tilemapHandler.originalTilesPositions[rand.Next(0, tilemapHandler.originalTilesPositions.Count)];
                if (tilemapHandler.tiles.TryGetValue(vector3Int, out completeGameTile)) {
                    if (completeGameTile.Type == 0) {
                        completeGameTile.sprite = Resources.Load<Sprite>("box"+completeGameTile.Type+"complete");
                        HoverTilemap.SetTile(vector3Int, completeGameTile);
                    }
                    levelManagement.IncreaseScore(completeGameTile.Score());
                }
                tilemapHandler.originalTilesPositions.Remove(vector3Int);
            } else {
                levelManagement.IncreaseScore(tilemapHandler.levelState.levelScore);
                levelManagement.IncreaseScore(levelManagement.totalHints * 10);
                levelManagement.IncreaseHints(1);
                levelManagement.SaveScore();
                freeze = true;
                // Advertisement.Show();
            }
            return;
        }
        if (!isMoving) {
            // #if UNITY_EDITOR
            
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var worldPoint = new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), 0);
                Debug.Log("start Moving"+worldPoint.x+":"+worldPoint.y);
                if (Tilemap.HasTile(worldPoint))
                {
                    movingTile = ScriptableObject.CreateInstance<GameTile>();
                    if (tilemapHandler.tiles.TryGetValue(worldPoint, out movingTile)) {
                        // Debug.Log("start box Moving"+movingTile.Position.x+":"+movingTile.Position.y);
                        if (movingTile.CanMove == true) {
                            ShowPreview(movingTile);
                            mouseX = worldPoint.x;
                            mouseY = worldPoint.y;
                            audioSelect.Play(0);
                        }
                    }
                }
            
            } else if (movingTile != null && movingTile.CanMove == true) {
                Vector3 point  = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var worldPoint = new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), 0);
                if (mouseX != worldPoint.x || mouseY != worldPoint.y) { // clear preview when position changed
                    Preview.ClearAllTiles();
                }
                tilemapHandler.Hover(HoverTilemap, movingTile, worldPoint);
            }
            if (Input.GetMouseButtonUp(0)) {
                Preview.ClearAllTiles();
                if (movingTile != null && movingTile.CanMove == true) {
                    Vector3 point  = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var worldPoint = new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), 0);
                    
                    isMoving = true;
                    HoverTilemap.ClearAllTiles();
                    if (tilemapHandler.ReplaceBox(movingTile.Position, worldPoint)) {
                        audioReplace.Play(0);
                    }
                    //tilemapHandler.MoveBox(movingTile.Position, worldPoint);
                    isMoving = false;
                    
                    movingTile = null;
                }
            }
            /* #else
            for (int i = 0; i < Input.touchCount; ++i)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    // Construct a ray from the current touch coordinates
                    Vector3 point = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
                    var worldPoint = new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), 0);
                    if (Tilemap.HasTile(worldPoint))
                    {
                        movingTile = ScriptableObject.CreateInstance<GameTile>();
                        TileBase tile = Tilemap.GetTile(worldPoint);
                        movingTile.Position = worldPoint;
                        movingTile.name = tile.name;
                    }
                } else if (Input.GetTouch(i).phase == TouchPhase.Ended && movingTile != null && movingTile.name == "box") {
                    Vector3 point  = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
                    var worldPoint = new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), 0);
                
                    isMoving = true;
                    tilemapHandler.MoveBox(movingTile.Position, worldPoint);
                    isMoving = false;
                
                    movingTile = null;
                }
            }
            #endif
            */
        }
    }

    private void ShowPreview(GameTile movingTile)
    {
        GameTile _tile = ScriptableObject.CreateInstance<GameTile>();
        _tile.sprite = Resources.Load<Sprite>("boxHover"+movingTile.Type);
        Vector3Int vector3Int = new Vector3Int(0, 5, 0);
        Preview.ClearAllTiles();
        Preview.SetTile(vector3Int, _tile);
    }

    IEnumerator ChangeBGColor()
    {
        float changeColorSpeed = 0F;
        while (changeColorSpeed <= 1.01F) {
            Color destColor = new Color();
            if (settings.DarkModeOn == true) {
                ColorUtility.TryParseHtmlString("#F4AC45", out destColor);
            } else {
                ColorUtility.TryParseHtmlString("#F4AC45", out destColor);
            }
            cam.backgroundColor = Color.Lerp(cam.backgroundColor, destColor, changeColorSpeed);
            changeColorSpeed += 0.05F;
            yield return null;
        }
    }
}