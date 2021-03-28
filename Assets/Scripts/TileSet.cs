using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileSet", menuName = "Sokoban/TileSets", order = 1)]
public class TileSet : ScriptableObject
{
    public Tile[] Tiles;

    public Tile ClearTile;
    public Tile PlayerTile;
    public Tile GoalTile;
}
