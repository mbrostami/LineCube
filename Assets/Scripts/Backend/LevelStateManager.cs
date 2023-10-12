using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelStateManager
{
    private Dictionary<int, LevelState> levels; 
    public static LevelStateManager instance = null;

    public static int DynamicMaxX = 0;
    public static int DynamicMaxY = 0;

    public LevelStateManager()
    {
        if (instance == null) {
            instance = this;
            levels = new Dictionary<int, LevelState>();
        }
    }

    public LevelState LoadLevelState(int level)
    {
        if (levels.ContainsKey(level)) {
            return levels[level];
        }
        LevelState levelState = new LevelState(level, LevelStateManager.DynamicMaxX, LevelStateManager.DynamicMaxY);
        levels.Add(level, levelState);
        return levelState;
    }
    
    public bool SaveLevelState(
        int level,
        bool completed,
        Dictionary<Vector3Int, GameTile> tiles,
        Dictionary<Vector3Int, GameTile> originalTiles)
    {
        if (levels.ContainsKey(level)) {
            LevelState _levelState = levels[level];
            _levelState.Completed  = completed;
            _levelState.tiles.Clear();
            foreach (KeyValuePair<Vector3Int, GameTile> item in tiles)
            {
                int[] position = new int[2] {item.Key.x, item.Key.y}; 
                _levelState.tiles.Add(position, makeStorableGameTile(item.Value));
            }
            if (_levelState.IsEmpty == true) {
                // Save originalTiles for boxes, cause other fixed box are in tiles and positions are not going to change
                foreach (KeyValuePair<Vector3Int, GameTile> item in originalTiles)
                {
                    if (item.Value.name == "box") {
                        int[] position = new int[2] {item.Key.x, item.Key.y}; 
                        _levelState.originalTiles.Add(position, makeStorableGameTile(item.Value));
                    }
                }
            }
            _levelState.IsEmpty = false;
            Store();
            return true;
        }
        return false;
    }

    private StorableGameTile makeStorableGameTile(GameTile gameTile)
    {
        StorableGameTile storableGameTile = new StorableGameTile();
        storableGameTile.type = gameTile.Type;
        storableGameTile.name = gameTile.name;
        storableGameTile.OutDirection = gameTile.OutDirection;
        storableGameTile.CanMove = gameTile.CanMove;
        // Debug.Log("Save:Storable:CanMove:"+storableGameTile.CanMove);
        return storableGameTile;
    }

    private void Store()
    {
        SaveGame.SaveLevelState(this);
    }
}