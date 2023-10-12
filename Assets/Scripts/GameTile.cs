using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class GameTile: Tile
{
    public List<int> Ports;
    public int Type;
    public Vector3Int Position;
    public string OutDirection;
    public bool CanMove;

    public int Score()
    {
        if (Type == 0 || Type == 1) {
            return 4;
        } else if (Type == 3 || Type == 4 || Type == 5 || Type == 8 || Type == 9 || Type == 10) {
            return 2;
        } else if (Type == 2 || Type == 6 || Type == 7 || Type == 11) {
            return 3;
        }
        return 1;
    }
}
