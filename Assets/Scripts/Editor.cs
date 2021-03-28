using System;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Editor : MonoBehaviour
{
    [Header("Tilemap Data")]
    public Tilemap actualTilemap;
    public Tilemap overlayTilemap;
    public TileSet tileSet;
    public Vector2Int levelBoundaries;

    [Header("Serialization")]
    public InputField levelNameField;

    private TileBase currentTile;
    private Camera sceneCamera;
    private Vector3Int previousPosition;

    private void Start()
    {
        // Cache the main camera, since accessing Camera.main allocates
        // garbage on the heap.
        sceneCamera = Camera.main;
    }

    /// <summary>
    /// Returns the path to the custom levels module
    /// </summary>
    private static string CustomLevelPath
    {
        get => Path.Combine(Application.streamingAssetsPath, "Modules", "CustomLevels");
    }

    /// <summary>
    /// Serializes the map into a Level data class.
    /// </summary>
    private Level SerializeMap()
    {
        // Create a new level
        var level = new Level
        {
            Boundaries = levelBoundaries,
            TileData = new int[levelBoundaries.x * levelBoundaries.y]
        };

        // Iterate over the level 
        for (var y = 0; y < levelBoundaries.y; y++)
            for (var x = 0; x < levelBoundaries.x; x++)
            {
                var tileIdx = 0;
                var adjustedPosition = new Vector3Int(x, 20 - y, 0);

                // Get the tile at this position
                var tile = actualTilemap.GetTile(adjustedPosition);

                // Find the tile index and write it.
                if (tile != null)
                {
                    tileIdx = Array.IndexOf(tileSet.Tiles, tile);
                }

                // If the tile is null, set the index to the index of the clear tile.
                else
                    tileIdx = Array.IndexOf(tileSet.Tiles, tileSet.ClearTile);

                level.TileData[(y * levelBoundaries.x) + x] = tileIdx;
            }

        return level;
    }

    /// <summary>
    /// Loads the level in the main scene for testing purposes
    /// </summary>
    private void TestLevel()
    {
        // Serialize the level itself
        var level = SerializeMap();

        // Load the scene
        WorldTraversalManager.The.LoadLevel(level, true);
        WorldTraversalManager.The.SetSceneVisibility("LevelEditor", false);
    }

    /// <summary>
    /// Saves the currently edited level
    /// </summary>
    public void SaveLevel()
    {
        // First, generate a random name for them
        // I'll use GUIDs for now, should be fine
        var guid = Guid.NewGuid();

        // Now, create the meta information file
        // and serialize the map
        var metaInfo = new LevelInfo
        {
            Name = levelNameField.text,
            Id = $"{guid}",
            NextLevel = "",
            Difficulty = Level.Difficulty.Easy
        };

        var level = SerializeMap();

        // Finally, write the data
        File.WriteAllText(
            Path.Combine(CustomLevelPath, $"{guid}.map"),
            JsonUtility.ToJson(level)
        );

        File.WriteAllText(
            Path.Combine(CustomLevelPath, $"{guid}.info"),
            JsonUtility.ToJson(metaInfo)
        );
    }

    /// <summary>
    /// Sets the current tile used by the editor
    /// </summary>
    /// <param name="tileIdx">The new tile</param>
    public void SetCurrentTile(Tile tile)
    {
        currentTile = tile;

        // Force refresh the editor by resetting the previous position
        previousPosition = new Vector3Int(-1000, -1000, -100);
    }

    void Update()
    {
        // Get the position on the tilemap from the mouse position
        var worldMousePos = sceneCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        var tilemapPos = overlayTilemap.WorldToCell(worldMousePos);

        // After that, make sure we're within the bounds
        if (tilemapPos.x < 0 || tilemapPos.x >= levelBoundaries.x ||
            tilemapPos.y < 1 || tilemapPos.y > levelBoundaries.y)
        {
            // Reset the previous tile.
            overlayTilemap.SetTile(previousPosition, null);
            return;
        }

        // Check if we've moved
        if (previousPosition != tilemapPos)
        {
            // If so, swap the previous position with nothing
            // and set the current tile on the new position
            overlayTilemap.SetTile(previousPosition, null);
            overlayTilemap.SetTile(tilemapPos, currentTile);

            previousPosition = tilemapPos;
        }

        // If we've clicked, place the tile
        if (Input.GetMouseButton(0))
        {
            // If the current tile is the blank tile, just null it out,
            // so we can see the grid underneath it
            if (currentTile == tileSet.ClearTile)
                actualTilemap.SetTile(tilemapPos, null);

            // Otherwise, set the current tile
            else
                actualTilemap.SetTile(tilemapPos, currentTile);
        }

        // If we've pressed enter, enable the test mode of the level editor
        if (Input.GetKeyDown(KeyCode.Return))
            TestLevel();
    }

    private void OnGUI()
    {
        GUILayout.Label($"Position: {previousPosition}");
    }
}
