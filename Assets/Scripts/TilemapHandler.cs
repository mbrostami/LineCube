using SystemRandom = System.Random;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class TilemapHandler
{

    public Dictionary<Vector3Int, List<int>> RequiredPorts;
    public Dictionary<Vector3Int, GameTile> originalTiles;
    public List<Vector3Int> originalTilesPositions;
    public Dictionary<Vector3Int, GameTile> tiles;
    public static bool initialized = false;
    public static bool levelCompletedTrigger = false;
    public static TilemapHandler instance = null;
    public Tilemap tileMap;
    private int ind = 0;
    public Dictionary<string, int> Limitations;
    public GameTile EntryBox;
    private LevelManagement levelManagement;
    private LevelStateManager levelStateManager;
    private Image nextLevelImageButton;
    public static AudioSource audioLevelCompleted;
    public LevelState levelState;

    public TilemapHandler()
    {
        if (TilemapHandler.initialized == false) {
            TilemapHandler.initialized = true;
            levelManagement   = SaveGame.LoadLevel();
            levelStateManager = SaveGame.LoadLevelStateManager();
            if (instance == null) {
                instance = this;
            }
        }
    }

    public void init(Tilemap _tileMap)
    {   
        tileMap       = _tileMap;
        tiles         = new Dictionary<Vector3Int, GameTile>();
        originalTiles = new Dictionary<Vector3Int, GameTile>();
        Limitations   = new Dictionary<string, int>();
        RequiredPorts = new Dictionary<Vector3Int, List<int>>();
        originalTilesPositions = new List<Vector3Int>();
    }

    public void NextLevel()
    {
        SystemRandom random = new SystemRandom();
        levelManagement.LevelCompleted();
        // Debug.Log("levelManagement:completed:"+levelManagement.GetLevel(levelManagement.GetCurrentLevel().Number - 1).Completed);
        if (levelManagement.GetNextIncompletedLevel() != -1) {
            tileMap.ClearAllTiles();
            init(tileMap);
            Start();
            renderUiElements();
        } else {
            Debug.Log("Game IS FINISHED!!!!");
        }
    }

    public void RefreshLevel()
    {
        GoToLevel(tileMap, levelManagement.GetCurrentLevel().Number);
    }

    public void GoToLevel(Tilemap _tileMap, int level)
    {
        if (!levelManagement.CanPlayNextLevel()) {
            // HideNextLevelButton();
        }
        SystemRandom random = new SystemRandom();
        if (levelManagement.GetLevel(level).CanPlay) {
            levelState = levelStateManager.LoadLevelState(level);
            if (levelState.IsEmpty == true) {
                // Debug.Log("levelStateIsEmpty");
                _tileMap.ClearAllTiles();
                init(_tileMap);
                Start();
                ShuffleTiles();
                saveLevelState(false);
            } else {
                fillLevelTiles(_tileMap, levelState);
            }
            validateRoute();
            renderUiElements();
        } else {
            Debug.Log("Can not play this level: "+ level);
        }
    }

    public void ShuffleTiles()
    {
        SystemRandom random = new SystemRandom();
        foreach (KeyValuePair<Vector3Int, GameTile> item in originalTiles)
        {
            if (item.Value.name == "box" && item.Value.CanMove == true) {
                Vector3Int source = new Vector3Int();
                Vector3Int destination = new Vector3Int();
                destination.x = random.Next(1, levelState.MaxXPos - 1);
                destination.y = random.Next(1, levelState.MaxYPos - 1);
                source.x = item.Key.x;
                source.y = item.Key.y;
                ReplaceBox(source, destination);
            }
        }
    }
    private void fillLevelTiles(Tilemap _tileMap, LevelState _levelState)
    {
        // Debug.Log("Filling By levelState");
        _tileMap.ClearAllTiles();
        init(_tileMap);
        var _tiles = _levelState.tiles;
        originalTiles.Clear();
        originalTilesPositions.Clear();
        foreach (KeyValuePair<int[], StorableGameTile> item in _tiles)
        {   
            if (item.Value.name == "entryBox") {
                CreateEntryBox(item.Key[0], item.Key[1]);
                EntryBox.OutDirection = item.Value.OutDirection;
            } else if (item.Value.name == "fixedBox") {
                Vector3Int vector3Int = new Vector3Int(item.Key[0], item.Key[1], 0); 
                CreateFixedBox(vector3Int);
            } else {
                Vector3Int vector3Int = new Vector3Int(item.Key[0], item.Key[1], 0); 
                CreateBoxFromStorage(vector3Int, item.Value, true);
            }
        }
        var _originalTiles = _levelState.originalTiles;
        foreach (KeyValuePair<int[], StorableGameTile> item in _originalTiles)
        {
            Vector3Int _vector3Int = new Vector3Int(item.Key[0], item.Key[1], 0); 
            keepOriginal(CreateBoxFromStorage(_vector3Int, item.Value, false));
        }
        Limitations = _levelState.Limitations;
        // Debug.Log("Filled originalTiles");
    }

    private GameTile CreateBoxFromStorage(Vector3Int vector3Int, StorableGameTile storableGameTile, bool applyTile)
    {
        GameTile gameTile = ScriptableObject.CreateInstance<GameTile>();
        gameTile.name     = "box";
        gameTile.Position = vector3Int;
        gameTile.Type     = storableGameTile.type;
        gameTile.Ports    = getPorts(storableGameTile.type);
        gameTile.CanMove  = storableGameTile.CanMove;
        gameTile.OutDirection  = storableGameTile.OutDirection;
        gameTile.CanMove  = storableGameTile.CanMove;
        // Debug.Log("Storable:CanMove:"+gameTile.Position.x+":"+gameTile.Position.y+":"+gameTile.CanMove);
        if (gameTile.CanMove == false) {
            gameTile.sprite   = getHintedSprite(storableGameTile.type);
        } else {
            gameTile.sprite   = getSprite(storableGameTile.type);
        }
        if (applyTile == true) {
            tiles.Add(vector3Int, gameTile);
            tileMap.SetTile(vector3Int, gameTile);
        }
        return gameTile;
    }

    private void saveLevelState(bool completed)
    {
        // Debug.Log("Going to save level state");
        int level = levelManagement.GetCurrentLevel().Number;
        levelStateManager.SaveLevelState(level, completed, tiles, originalTiles);
    }

    public void Hover(Tilemap _hoverTileMap, GameTile selectedTile, Vector3Int vector3Int)
    {
        if (IsInsideLimitations(vector3Int)) { 
            _hoverTileMap.ClearAllTiles();
            bool canHover = true;
            if (tileMap.HasTile(vector3Int)) {
                GameTile _etile = ScriptableObject.CreateInstance<GameTile>();
                tiles.TryGetValue(vector3Int, out _etile);
                if (_etile.CanMove == false) {
                    canHover = false;
                }
            }
            if (canHover == true) {
                Tile gameTile  = ScriptableObject.CreateInstance<Tile>();
                GameTile _tile = ScriptableObject.CreateInstance<GameTile>();
                // Debug.Log("Trying to hover:"+selectedTile.Position.x+":"+selectedTile.Position.y);
                if (tiles.TryGetValue(selectedTile.Position, out _tile)) {
                    gameTile.sprite = Resources.Load<Sprite>("boxHover"+_tile.Type);
                    _hoverTileMap.SetTile(vector3Int, gameTile);
                    GameTile _selectTile = ScriptableObject.CreateInstance<GameTile>();
                    _selectTile.sprite = Resources.Load<Sprite>("selectBox");
                    _hoverTileMap.SetTile(selectedTile.Position, _selectTile);
                } else {
                    // Debug.Log("tile not found");
                }
            }
        }
    }

    public void Start()
    {
        if (!levelManagement.CanPlayNextLevel()) {
            // HideNextLevelButton();
        }
        levelState = levelStateManager.LoadLevelState(levelManagement.GetCurrentLevel().Number);
        // Debug.Log("Entry:"+levelState.entryX+":"+levelState.entryY);
        CreateEntryBox(levelState.entryX, levelState.entryY);
        Limitations = levelState.Limitations;

        List<string> directions = new List<string>();
        directions.Add("up");
        directions.Add("right");
        directions.Add("down");
        directions.Add("left");
        MockGameTile firstBox = selectRandomAvailableDirection(EntryBox, directions);
        if (firstBox.Initialized) {
            List<int> _requredPorts = getRequiredPortsByPos(firstBox.Position);
            makeBoxRoute(firstBox, _requredPorts, 100);
        }
    }

    /**
    _requredPorts Make box based on this requredPorts and use MockBox position
     */
    public void makeBoxRoute(MockGameTile _mockBox, List<int> _requredPorts, int level)
    {
        if (level < 1) {
            return;
        }
        bool shapeSelected = false; 
        List<int> excludedShapeIds = new List<int>();
        int shapeId;
        do
        {
            shapeId = selectRandomShapeId(_requredPorts, excludedShapeIds);
            if (shapeId != 0) {
                _mockBox.Ports = getPorts(shapeId);
                shapeSelected = checkOpenPortsNextPositions(_mockBox);
                if (shapeSelected == false) {
                    excludedShapeIds.Add(shapeId);
                } else {
                    addRequiredPortsByPreviousBlock(_mockBox);
                }
            }
        } while (shapeSelected == false && shapeId != 0);
        level = level - 1;
        if (shapeId == 0) {
            CreateFixedBox(_mockBox.Position);
            return;
        }
        // Create RealBox based on mockBox
        CreateBox(_mockBox.Position, shapeId);
        foreach (int _port in _mockBox.Ports)
        {
            if (_mockBox.InPort != _port) {
                string _direction = GetDirectionByPort(_port);
                MockGameTile _nextMockBox = getBlockPositionByDirection(_mockBox, _direction);
                if (isBlockEmpty(_nextMockBox.Position) == true) {
                    List<int> _nexBlockRequredPorts = getRequiredPortsByPos(_nextMockBox.Position);
                    makeBoxRoute(_nextMockBox, _nexBlockRequredPorts, level);
                }
            }
        }
        return;
    }

    private void addRequiredPortsByPreviousBlock(MockGameTile _mockBox)
    {
        foreach (int _port in _mockBox.Ports)
        {
            if (_mockBox.InPort != _port) {
                string _direction = GetDirectionByPort(_port);
                MockGameTile _nextToMockBox = getBlockPositionByDirection(_mockBox, _direction);
                if (isBlockEmpty(_nextToMockBox.Position) == true) {
                    addRequiredPortsByPos(_nextToMockBox.Position, _nextToMockBox.InPort);
                }
            }    
        }
    }

    private bool checkOpenPortsNextPositions(MockGameTile _mockBox)
    {
        foreach (int _port in _mockBox.Ports)
        {
            if (_mockBox.InPort != _port) {
                string _direction = GetDirectionByPort(_port);
                MockGameTile _nextToMockBox = getBlockPositionByDirection(_mockBox, _direction);
                if (isBlockEmpty(_nextToMockBox.Position) == false) { // if next selected block is not empty check compatibility by ports
                    bool compatibleShape = false;
                    GameTile _gameTileInNextPosition = ScriptableObject.CreateInstance<GameTile>();
                    if (tiles.TryGetValue(_nextToMockBox.Position, out _gameTileInNextPosition)) {
                        int reversePort = ReversePort(_port);
                        if (_gameTileInNextPosition.Ports.Contains(reversePort)) {
                            compatibleShape = true;
                        }
                    }
                    if (compatibleShape == false) { // This mockBox is not fillable here, cause there is another box around it which is required another port
                        return false;
                    }
                }
            }
        }
        return true;
    }

    private int selectRandomShapeId(List<int> _requredPorts, List<int> excludedShapeIds)
    {
        List<int> validShapeIds = new List<int>();
        for (int shapeId = 1; shapeId <= 11; shapeId++)
        {
            if (excludedShapeIds.Contains(shapeId) == false) {
                List<int> shapePorts = getPorts(shapeId);
                bool _selectable = true;
                foreach (int requredPort in _requredPorts)
                {
                    _selectable = _selectable && shapePorts.Contains(requredPort);
                }
                if (_selectable == true) {
                    validShapeIds.Add(shapeId);
                }
            }
        }
        if (validShapeIds.Count > 0) {
            // Select random from available shapes which matched with required ports
            SystemRandom random = new SystemRandom();
            return validShapeIds[random.Next(validShapeIds.Count)];
        }
        return 0;
    }

    private List<int> getRequiredPortsByPos(Vector3Int _vector3Int)
    {
        List<int> ports = new List<int>();
        if (RequiredPorts.TryGetValue(_vector3Int, out ports)) {
            return ports;
        }
        return new List<int>();
    }
    
    private MockGameTile selectRandomAvailableDirection(GameTile box, List<string> directions)
    {
        if (directions == null || directions.Count < 1) {
            return ScriptableObject.CreateInstance<MockGameTile>();
        }
        SystemRandom random = new SystemRandom();
        string direction = directions[random.Next(directions.Count)];
        if (checkDirectionIsEmpty(box, direction) == false) {
            directions.Remove(direction);
            return selectRandomAvailableDirection(box, directions);
        }
        //newBlock;
        MockGameTile mockGameTile = getBlockPositionByDirection(box, direction);
        EntryBox.OutDirection = direction;
        addRequiredPortsByPos(mockGameTile.Position, mockGameTile.InPort);
        return mockGameTile;
    }

    
    private void addRequiredPortsByPos(Vector3Int _vector3Int, int inPort)
    {
        List<int> ports = new List<int>();
        if (RequiredPorts.TryGetValue(_vector3Int, out ports)) {
            ports.Add(inPort);
        } else {
            List<int> _ports = new List<int>();
            _ports.Add(inPort);
            RequiredPorts.Add(_vector3Int, _ports);
        }
    }

    /**
    Read only
     */
    private MockGameTile getBlockPositionByDirection(GameTile box, string _direction)
    {
        MockGameTile mockGameTile = ScriptableObject.CreateInstance<MockGameTile>();
        mockGameTile.Initialized  = true;
        mockGameTile.InPort       = ReversePort(GetPortByDirection(_direction));
        mockGameTile.Position     = getNewPositionByDirection(box.Position, _direction);
        return mockGameTile;
    }

    private Vector3Int getNewPositionByDirection(Vector3Int _vector3Int, string _direction)
    {
        Vector3Int vector3Int = new Vector3Int();
        if (_direction == "up") {
            vector3Int.x = _vector3Int.x;
            vector3Int.y = _vector3Int.y + 1;
        } else if (_direction == "right") {
            vector3Int.x = _vector3Int.x + 1;
            vector3Int.y = _vector3Int.y;
        } else if (_direction == "down") {
            vector3Int.x = _vector3Int.x;
            vector3Int.y = _vector3Int.y - 1;
        } else if (_direction == "left") {
            vector3Int.x = _vector3Int.x - 1;
            vector3Int.y = _vector3Int.y;
        }
        return vector3Int;
    }

    private bool checkDirectionIsEmpty(GameTile _gameTile, string _direction)
    {
        MockGameTile _box = getBlockPositionByDirection(_gameTile, _direction);
        return isBlockEmpty(_box.Position);
    }

    private bool isBlockEmpty(Vector3Int vector3Int)
    {
        if (vector3Int.x > Limitations["maxX"] 
            || vector3Int.x < Limitations["minX"] 
            || vector3Int.y > Limitations["maxY"] 
            || vector3Int.y < Limitations["minY"])
        {
            return false;
        }
        return !tileMap.HasTile(vector3Int);
    }

    private bool IsInsideLimitations(Vector3Int vector3Int)
    {
        if (vector3Int.x > levelState.MaxXPos
            || vector3Int.x < 0
            || vector3Int.y > levelState.MaxYPos
            || vector3Int.y < 0)
        {
            return false;
        }
        return true;
    }

    public bool CanMoveTo(Vector3Int source, Vector3Int destination)
    {
        return !tileMap.HasTile(destination);
    }

    public int ReversePort(int port)
    {
        if (port == 1) {
            return 3;
        } else if (port == 2) {
            return 4;
        } else if (port == 3) {
            return 1;
        }
        return 2;
    }
    
    public string GetDirectionByPort(int port)
    {
        if (port == 1) {
            return "up";
        } else if (port == 2) {
            return "right";
        } else if (port == 3) {
            return "down";
        }
        return "left";
    }

    public int GetPortByDirection(string direction)
    {
        if (direction == "up") {
            return 1;
        } else if (direction == "right") {
            return 2;
        } else if (direction == "down") {
            return 3;
        }
        return 4;
    }

    public void MoveBox(Vector3Int source, Vector3Int destination)
    {
        // Debug.Log("moving:xy:"+source.x+":"+source.y+":To:xy:"+destination.x+":"+destination.y);
        // Debug.Log("Level:"+levelManagement.GetCurrentLevel().Number);
        if (CanMoveTo(source, destination)) {
            GameTile gameTile = ScriptableObject.CreateInstance<GameTile>();
            if (tiles.TryGetValue(source, out gameTile)) {
                gameTile.Position = destination;
                tileMap.SetTile(source, null);
                tileMap.SetTile(destination, gameTile);
                tiles.Remove(source);
                tiles.Add(destination, gameTile);
                if (validateRoute()) {
                    levelState = levelStateManager.LoadLevelState(levelManagement.GetCurrentLevel().Number);
                    levelManagement.EnableNextLevel(levelState.levelScore);
                    // ShowNextLevelButton();
                    audioLevelCompleted.Play(0);
                    saveLevelState(true);
                } else {
                    saveLevelState(false);
                }
            }
        }
    }

    private string getStartDirectionFromEntryPoint(string _direction)
    {
        List<string> directions = new List<string>();
        directions.Add("up");
        directions.Add("right");
        directions.Add("down");
        directions.Add("left");
        SystemRandom random = new SystemRandom();
        do
        {
            Vector3Int newVector3Int = getNewPositionByDirection(EntryBox.Position, _direction);
            if (tileMap.HasTile(newVector3Int)) {
                GameTile _foundTile = ScriptableObject.CreateInstance<GameTile>();
                if (tiles.TryGetValue(newVector3Int, out _foundTile)) {
                    int requiredPort = ReversePort(GetPortByDirection(_direction));
                    if (_foundTile.Ports.Contains(requiredPort)) {
                        return _direction;
                    }
                }
            }
            directions.Remove(_direction);
            if (directions.Count > 0) {
                _direction = directions[random.Next(directions.Count)];
            }
        } while (directions.Count > 0);
        return "";
    }

    private bool validateRoute()
    {
        Tilemap _shiningTilemap = GameObject.Find("Shining").GetComponent<Tilemap>();
        _shiningTilemap.ClearAllTiles();
        string direction = getStartDirectionFromEntryPoint(EntryBox.OutDirection);
        if (direction == "") { // means entry box has no output
            // Debug.Log("Start Direction is empty");
            return false;
        }
        // Debug.Log("StartValidatingFrom:"+EntryBox.Position.x+":"+EntryBox.Position.y+":"+EntryBox.OutDirection+":"+direction);
        Vector3Int newVector3Int = getNewPositionByDirection(EntryBox.Position, direction);
        List<Vector3Int> checkedPositions = new List<Vector3Int>();
        checkedPositions.Add(EntryBox.Position);
        if (tileMap.HasTile(newVector3Int)) {
            GameTile _gameTile = ScriptableObject.CreateInstance<GameTile>();
            tiles.TryGetValue(newVector3Int, out _gameTile);
            MakeTileShining(_gameTile);
            bool isValid = validateTileConnection(_gameTile, direction, checkedPositions);
            if (isValid) {
                foreach (KeyValuePair<Vector3Int, GameTile> item in tiles)
                {
                    if (!checkedPositions.Contains(item.Key)) {
                        // Debug.Log("NotFound:"+item.Value.Position.x+":"+item.Value.Position.y);
                        // One of the tiles is not in the checkedPositions
                        return false;
                    }
                }
            }
            return isValid;
        }
        return false;
    }

    public bool HasCorrectTileByDirection(Vector3Int vector3Int, string direction)
    {
        Vector3Int newVector3Int = getNewPositionByDirection(vector3Int, direction);
        if (tileMap.HasTile(newVector3Int)) {
            GameTile _gameTile = ScriptableObject.CreateInstance<GameTile>();
            tiles.TryGetValue(newVector3Int, out _gameTile);
            if (_gameTile.name == "box") {
                int mustHavePort = ReversePort(GetPortByDirection(direction));
                // Debug.Log("HasCorrectTileByDirection:box:"+newVector3Int.x+":"+newVector3Int.y+":"+mustHavePort);
                return _gameTile.Ports.Contains(mustHavePort);
            }
            // Debug.Log("HasCorrectTileByDirection:nextEmpty:"+_gameTile.Position.x+":"+_gameTile.Position.y+":"+direction);
            return true;
        }
        // Debug.Log("HasCorrectTileByDirection:false:"+newVector3Int.x+":"+newVector3Int.y+":"+direction);
        return false;
    }

    /**
    * Check if tile route is valid
    */
    public bool validateTileConnection(GameTile gameTile, string direction, List<Vector3Int> checkedPositions)
    {
        if (checkedPositions.Contains(gameTile.Position)) {
            return true;
        }
        // Debug.Log("checkTileConnection:"+gameTile.Position.x+":"+gameTile.Position.y+":"+gameTile.Type+":"+gameTile.name);
        if (gameTile.name == "box") {
            int InputPort = ReversePort(GetPortByDirection(direction));
            foreach (int _port in gameTile.Ports) {
                if (InputPort != _port) {
                    string _direction = GetDirectionByPort(_port);
                    if (!HasCorrectTileByDirection(gameTile.Position, _direction)) {
                        // Debug.Log("HasCorrectTileByDirectionFalse"+gameTile.Position.x+":"+gameTile.Position.y);
                        return false;
                    }
                }
            }
            bool isRouteValid = true;
            checkedPositions.Add(gameTile.Position);
            foreach (int _port in gameTile.Ports) {
                // Debug.Log("CheckingPort:"+_port+":"+InputPort);
                if (InputPort != _port) {
                    string _direction = GetDirectionByPort(_port);
                    Vector3Int nextVector3Int = getNewPositionByDirection(gameTile.Position, _direction);
                    if (!tileMap.HasTile(nextVector3Int)) { // no tile connected to this tile
                        // Debug.Log("NoTileFound:"+nextVector3Int.x+":"+nextVector3Int.y);
                        isRouteValid = false;
                    } else {
                        GameTile _gameTile = ScriptableObject.CreateInstance<GameTile>();
                        tiles.TryGetValue(nextVector3Int, out _gameTile);
                        if (_gameTile.name == "box") {
                            if (_gameTile.Ports.Contains(ReversePort(_port))) {
                                if (!validateTileConnection(_gameTile, _direction, checkedPositions)) {
                                    // Debug.Log("InValid:"+_gameTile.Position.x+":"+_gameTile.Position.y);
                                    isRouteValid = false;
                                }
                                // Check if next tile has correct input port, then make it shining
                                MakeTileShining(_gameTile);
                            } else {
                                isRouteValid = false;
                            }
                        } else { // do not validate fixedBoxes. just add to the checkedPositions
                            checkedPositions.Add(_gameTile.Position);
                        }
                        // Debug.Log("CheckingPortValid:"+_port);
                    }
                }
            }
            return isRouteValid;
        } else {
            checkedPositions.Add(gameTile.Position);
        }
        return true;
    }

    private void MakeTileShining(GameTile gameTile)
    {
        if (gameTile.name == "box") {
            Tilemap _shiningTilemap = GameObject.Find("Shining").GetComponent<Tilemap>();
            Tile shiningTile = ScriptableObject.CreateInstance<Tile>();
            shiningTile.sprite = getShiningSprite(gameTile.Type);
            _shiningTilemap.SetTile(gameTile.Position, shiningTile);
        }
    }

    public bool ReplaceBox(Vector3Int source, Vector3Int destination)
    {
        if (source == destination) {
            // Debug.Log("Source and destination are same!!");
            return false;
        }
        GameTile sourceGameTile = ScriptableObject.CreateInstance<GameTile>();
        if (tiles.TryGetValue(source, out sourceGameTile)) {
            if (sourceGameTile.CanMove == false) {
                return false;
            }
            bool validMove = false;
            GameTile destGameTile = ScriptableObject.CreateInstance<GameTile>();
            if (tiles.TryGetValue(destination, out destGameTile)) {
                if (destGameTile.name != "box" || destGameTile.CanMove == false) {
                    return false; // skip replacing non boxes
                }
                ind = ind + 1;
                // destGameTile.Ports.ForEach(i => Debug.Log("Dest:s:"+ind+":"+destGameTile.Type+":"+i));
                // sourceGameTile.Ports.ForEach(i => Debug.Log("Sour:s:"+ind+":"+sourceGameTile.Type+":"+i));
                // Debug.Log("Sour:xy:s:"+source.x+":"+source.y);
                // Debug.Log("Dest:xy:s:"+destination.x+":"+destination.y);

                destGameTile.Position   = source;
                sourceGameTile.Position = destination;
                
                tiles.Remove(source);
                tiles.Remove(destination);
                tiles.Add(destination, sourceGameTile);
                tiles.Add(source, destGameTile);

                tileMap.SetTile(source, null);
                tileMap.SetTile(destination, null);
                Vector3Int[] vectors = new Vector3Int[2];
                vectors.SetValue(source, 0);
                vectors.SetValue(destination, 1);
                GameTile[] gameTiles = new GameTile[2];
                gameTiles.SetValue(destGameTile, 0);
                gameTiles.SetValue(sourceGameTile, 1);
                tileMap.SetTiles(vectors, gameTiles);
                tileMap.RefreshAllTiles();
                validMove = true;
            } else if (IsInsideLimitations(destination)) {
                sourceGameTile.Position = destination;
                tileMap.SetTile(source, null);
                tileMap.SetTile(destination, sourceGameTile);
                tiles.Remove(source);
                tiles.Add(destination, sourceGameTile);
                validMove = true;
            }
            // Debug.Log("validMove:"+validMove);
            if (validMove) {
                if (validateRoute()) {
                    // Debug.Log("ValidRoute");
                    //if (levelManagement.CanPlayNextLevel() == false) {
                    LevelState _levelState = levelStateManager.LoadLevelState(levelManagement.GetCurrentLevel().Number);
                    levelManagement.EnableNextLevel(_levelState.levelScore);
                    // ShowNextLevelButton();
                    audioLevelCompleted.Play(0);
                    TilemapHandler.levelCompletedTrigger = true;
                    // }
                    saveLevelState(true);
                } else {
                    // Debug.Log("Next level not available now!");
                    saveLevelState(false);
                }
            }
        }
        return true;
    }

    private bool CreateEntryBox(int x, int y)
    {
        Vector3Int vector3Int = new Vector3Int();
        vector3Int.x      = x;
        vector3Int.y      = y;
        GameTile gameTile = ScriptableObject.CreateInstance<GameTile>();
        gameTile.name     = "entryBox";
        gameTile.Position = vector3Int;
        gameTile.Type     = 0;
        gameTile.sprite   = getSprite(0);
        gameTile.Ports    = getPorts(0);
        gameTile.CanMove  = false;
        EntryBox          = gameTile;
        tiles.Add(vector3Int, gameTile);
        tileMap.SetTile(vector3Int, gameTile);
        keepOriginal(gameTile);
        // Debug.Log("EntryBox:xy:"+x+":"+y);
        return true;
    }

    private bool CreateFixedBox(Vector3Int vector3Int)
    {
         GameTile gameTile = ScriptableObject.CreateInstance<GameTile>();
         gameTile.name     = "fixedBox";
         gameTile.Position = vector3Int;
         gameTile.Type     = 0;
         gameTile.sprite   = getSprite(0);
         gameTile.Ports    = getPorts(0);
         gameTile.CanMove  = false;
         tiles.Add(vector3Int, gameTile);
         tileMap.SetTile(vector3Int, gameTile);
         keepOriginal(gameTile);
         // Debug.Log("NewFixedBox:xy:"+vector3Int.x+":"+vector3Int.y);
         return true;
    }

    private bool CreateBox(Vector3Int vector3Int, int type)
    {
        GameTile gameTile = makeBox(vector3Int, type);
        tiles.Add(vector3Int, gameTile);
        tileMap.SetTile(vector3Int, gameTile);
        keepOriginal(gameTile);
        // Debug.Log("NewBox:xy:"+vector3Int.x+":"+vector3Int.y);
        return true;
    }

    private GameTile makeBox(Vector3Int vector3Int, int type)
    {
        GameTile gameTile = ScriptableObject.CreateInstance<GameTile>();
        gameTile.name     = "box";
        gameTile.Position = vector3Int;
        gameTile.Type     = type;
        gameTile.Ports    = getPorts(type);
        if (levelState.hints > 0) {
            gameTile.CanMove  = false; // make this box static
            gameTile.sprite   = getHintedSprite(type);
            levelState.hints = levelState.hints - 1;
        } else {
            gameTile.sprite   = getSprite(type);
            gameTile.CanMove  = true;
        }
        return gameTile;
    }

    private void keepOriginal(GameTile _gameTile)
    {
        Vector3Int vector3Int = new Vector3Int();
        vector3Int.x = _gameTile.Position.x;
        vector3Int.y = _gameTile.Position.y;
        GameTile gameTileOriginal = ScriptableObject.CreateInstance<GameTile>();
        gameTileOriginal.name     = _gameTile.name;
        gameTileOriginal.Type     = _gameTile.Type;
        gameTileOriginal.sprite   = _gameTile.sprite;
        gameTileOriginal.CanMove  = _gameTile.CanMove;
        gameTileOriginal.Ports    = _gameTile.Ports;
        gameTileOriginal.Position = vector3Int;
        originalTilesPositions.Add(vector3Int);
        originalTiles.Add(vector3Int, gameTileOriginal);
    }

    private Sprite getSprite(int type)
    {
        return Resources.Load<Sprite>("box"+type);
    }

    private Sprite getHintedSprite(int type)
    {
        return Resources.Load<Sprite>("boxHint"+type);
    }

    private Sprite getShiningSprite(int type)
    {
        return Resources.Load<Sprite>("box"+type+"complete");
    }

    private List<int> getPorts(int type)
    {
        List<int> portList = new List<int>();
        switch (type)
        {
            case 1:
                portList.Add(1);
                portList.Add(2);
                portList.Add(3);
                portList.Add(4);
                break;
            case 2:
                portList.Add(1);
                portList.Add(2);
                portList.Add(3);
                break;
            case 3:
                portList.Add(1);
                portList.Add(2);
                break;
            case 4:
                portList.Add(1);
                portList.Add(3);
                break;
            case 5:
                portList.Add(1);
                portList.Add(4);
                break;
            case 6:
                portList.Add(1);
                portList.Add(3);
                portList.Add(4);
                break;
            case 7:
                portList.Add(2);
                portList.Add(3);
                portList.Add(4);
                break;
            case 8:
                portList.Add(2);
                portList.Add(3);
                break;
            case 9:
                portList.Add(2);
                portList.Add(4);
                break;
            case 10:
                portList.Add(3);
                portList.Add(4);
                break;
            case 11:
                portList.Add(1);
                portList.Add(2);
                portList.Add(4);
                break;
            default:
            case 0:
                break;
        }
        return portList;
    }

    private void HideNextLevelButton()
    {
        if (!levelManagement.CanPlayNextLevel()) {
            if (nextLevelImageButton == null) {
                nextLevelImageButton = GameObject.Find("NextLevel").GetComponent<Image>();
            }
            nextLevelImageButton.gameObject.SetActive(false);
        }
    }

    public bool UseHint()
    {
        if (levelManagement.totalHints > 0) {
            if (LockTileAsHint() == true) {
                levelManagement.totalHints -= 1;
                renderUiElements();
                levelManagement.SaveLevel();
                saveLevelState(false);
                return true;
            }
        }
        return false;
    }

    /** 
    * Select an item and lock in place as hint
    */
    private bool LockTileAsHint()
    {
        SystemRandom rand = new SystemRandom();
        int startIndex = 0;
        do
        {
            Vector3Int vector3Int = originalTilesPositions[rand.Next(startIndex, originalTilesPositions.Count)];
            startIndex++;
            GameTile _originalTile = ScriptableObject.CreateInstance<GameTile>();
            if (originalTiles.TryGetValue(vector3Int, out _originalTile)) {
                if (_originalTile.CanMove == false) {
                    continue;
                }
                GameTile _gameTile = ScriptableObject.CreateInstance<GameTile>();
                if (tiles.TryGetValue(vector3Int, out _gameTile)) {
                    if (_gameTile.Type == _originalTile.Type) {
                        _originalTile.CanMove = false;
                        _originalTile.sprite = getHintedSprite(_gameTile.Type);
                        _gameTile.CanMove = false;
                        _gameTile.sprite = getHintedSprite(_gameTile.Type);
                        tileMap.RefreshTile(_gameTile.Position);
                        return true;
                    } else {
                        foreach (KeyValuePair<Vector3Int, GameTile> _tile in tiles)
                        {
                            if (_tile.Value.CanMove == true && _tile.Value.Type == _originalTile.Type) {
                                if (ReplaceBox(_gameTile.Position, _tile.Value.Position)) {
                                    _originalTile.CanMove = false;
                                    _originalTile.sprite = getHintedSprite(_gameTile.Type);
                                    _tile.Value.CanMove = false;
                                    _tile.Value.sprite = getHintedSprite(_tile.Value.Type);
                                    tileMap.RefreshTile(_tile.Value.Position);
                                    return true;
                                }
                            }
                        }
                    }
                } else {
                    foreach (KeyValuePair<Vector3Int, GameTile> _tile in tiles)
                    {
                        if (_tile.Value.CanMove == true && _tile.Value.Type == _originalTile.Type) {
                            if (ReplaceBox(_tile.Value.Position, vector3Int)) {
                                _originalTile.CanMove = false;
                                _originalTile.sprite = getHintedSprite(_tile.Value.Type);
                                _tile.Value.CanMove = false;
                                _tile.Value.sprite  = getHintedSprite(_tile.Value.Type);
                                tileMap.RefreshTile(vector3Int);
                                return true;
                            }
                        }
                    }
                }
            }
        } while (startIndex <= originalTilesPositions.Count);
        return false;
    }

    private void ShowNextLevelButton()
    {
        if (levelManagement.CanPlayNextLevel()) {
            if (nextLevelImageButton == null) {
                nextLevelImageButton = GameObject.Find("NextLevel").GetComponent<Image>();
            }
            nextLevelImageButton.gameObject.SetActive(true);
        }
    }

    private void renderUiElements()
    {
        Text levelText = GameObject.Find("Level").GetComponent<Text>();
        levelText.text = "#" + levelManagement.GetCurrentLevel().Number;

        Text hintsText = GameObject.Find("Hints").GetComponent<Text>();
        hintsText.text = "" + levelManagement.totalHints;

        Text scoreText = GameObject.Find("Score").GetComponent<Text>();
        scoreText.text = "Score: " + levelManagement.totalScore;
    }
}
