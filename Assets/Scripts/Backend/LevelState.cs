
using System.Collections.Generic;
using UnityEngine;
using SystemRandom = System.Random;

[System.Serializable]
public class LevelState
{
    public int Number {get;}
    public int entryX = 7;
    public int entryY = 5;
    public int extendX = 1;
    public int extendY = 1;
    public int hints = 1;
    public bool IsEmpty;
    public bool Completed;
    public int levelScore;
    public Dictionary<int[], StorableGameTile> tiles;
    public Dictionary<int[], StorableGameTile> originalTiles;
    public Dictionary<string, int> Limitations;
    public int MaxXPos = 6;
    public int MaxYPos = 11;

    public LevelState(int _number, int dynamicMaxX, int dynamicMaxY)
    {
        Number        = _number;
        IsEmpty       = true;
        Completed     = false;
        tiles         = new Dictionary<int[], StorableGameTile>();
        originalTiles = new Dictionary<int[], StorableGameTile>();
        Limitations   = new Dictionary<string, int>();
        if (dynamicMaxX > 0) {
            MaxXPos = dynamicMaxX;
        }
        if (dynamicMaxY > 0) {
            MaxYPos = dynamicMaxY;
        }
        CalculateDifficulty();
    }

    private void CalculateDifficulty()
    {
        int levelGroup = (Number / 20) + 1;
        hints = levelGroup; // help player with locked boxes
        if (Number == 1 || Number == 2 || Number == 3) {
            extendX = 1;
            extendY = 1;
        } else if (levelGroup < 2) {
            extendX = 2;
            extendY = 1;
        } else if (levelGroup < 4) {
            extendX = 2;
            extendY = 2;
        } else if (levelGroup < 6) {
            extendX = 3;
            extendY = 2;
        } else {
            extendX = 3;
            extendY = 3;
        }
        levelScore = levelGroup * 20;
        SystemRandom rand = new SystemRandom();
        // Debug.Log("MaxXPos:"+MaxXPos);
        // Debug.Log("MaxYPos:"+MaxYPos);
        entryX = rand.Next(3, MaxXPos - 2);
        entryY = rand.Next(4, MaxYPos - 3);
        CalculateLimitations();
    }
    
    private void CalculateLimitations()
    {
        // Limitations.Add("maxX", entryX + extendX);
        // Limitations.Add("minX", entryX - extendX);
        // Limitations.Add("maxY", entryY + extendY);
        // Limitations.Add("minY", entryY - extendY);
        
        if (entryX + extendX > MaxXPos) {
            Limitations.Add("maxX", entryX + extendX - ((entryX + extendX) - MaxXPos));
            Limitations.Add("minX", entryX - extendX + ((entryX + extendX) - MaxXPos));
        } else if (entryX - extendX < 0) {
            Limitations.Add("maxX", entryX + extendX - ((entryX - extendX) * -1));
            Limitations.Add("minX", entryX - extendX + ((entryX - extendX) * -1));
        } else {
            Limitations.Add("maxX", entryX + extendX);
            Limitations.Add("minX", entryX - extendX);
        }
        if (entryY + extendY > MaxYPos) { // max visible tile
            Limitations.Add("maxY", entryY + extendY - ((entryY + extendY) - MaxYPos));
            Limitations.Add("minY", entryY - extendY + ((entryY + extendY) - MaxYPos));
        } else if (entryY - extendY < 0) {
            Limitations.Add("maxY", entryY + extendY - ((entryY - extendY) * -1));
            Limitations.Add("minY", entryY - extendY + ((entryY - extendY) * -1));
        } else {
            Limitations.Add("maxY", entryY + extendY);
            Limitations.Add("minY", entryY - extendY);
        }
        // Debug.Log("maxX:"+Limitations["maxX"]);
        // Debug.Log("maxY:"+Limitations["maxY"]);
        // Debug.Log("minX:"+Limitations["minX"]);
        // Debug.Log("minY:"+Limitations["minY"]);
    }
}