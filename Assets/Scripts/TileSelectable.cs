using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelectable : MonoBehaviour
{
    public Editor editor;

    private Tile selfTile;

    /// <summary>
    /// Set the tile that will be used once the item is clicked.
    /// </summary>
    /// <param name="tile">The tile</param>
    public void SetTile(Tile tile)
    {
        selfTile = tile;
    }

    public void OnClicked()
    {
        editor.SetCurrentTile(selfTile);
    }
}
