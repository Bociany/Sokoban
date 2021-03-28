using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public Player player;
    public GoalManager goalManager;
    public GameManager gameManager;
    public TileSet tileSet;

    /// <summary>
    /// Generates the tilemap from level data.
    /// </summary>
    /// <param name="level">Level data to parse</param>
    public void GenerateFrom(Level level)
    {
        if (goalManager != null)
            goalManager.ResetGoals();

        for (var y = 0; y < 20; y++)
            for (var x = 0; x < 30; x++)
            {
                var currentPos = new Vector3Int(x, 20 - y, 0);

                // Get the tile index and the tile.
                var tileIdx = level.TileData[(y * 30) + x];
                var tile = tileSet.Tiles[tileIdx];

                // If the tile is a player tile, inform the Player script of it.
                if (tile == tileSet.PlayerTile && player != null)
                    player.SetCurrentPosition(currentPos);

                // Otherwise, if the tile is a goal tile, add it to the goal manager.
                else if (tile == tileSet.GoalTile && goalManager != null)
                    goalManager.AddGoal(currentPos);

                tilemap.SetTile(currentPos, tile);
            }
    }

    /// <summary>
    /// Generates a level with the saved state
    /// </summary>
    /// <param name="module">The module the level belongs to</param>
    /// <param name="level">The level info</param>
    /// <param name="state">The saved state</param>
    public void LoadSavedState(ModuleInfo module, LevelInfo level, SaveState state)
    {
        GenerateFromModuleLevel(module, level, state.GameState);

        // Apply the diff tiles
        foreach (var tileDiff in state.TileDiffs)
        {
            var tile = tileSet.Tiles[tileDiff.TileId];

            if (tile == tileSet.PlayerTile && player != null)
                player.SetCurrentPosition(tileDiff.Position);

            else if (tile != tileSet.GoalTile && goalManager != null && goalManager.GetGoalState(tileDiff.Position).HasValue)
                goalManager.SetGoalState(tileDiff.Position, GoalManager.GoalState.Fulfilled);

            tilemap.SetTile(tileDiff.Position, tile);
        }
    }

    /// <summary>
    /// Enables editor mode for the level
    /// </summary>
    /// <param name="editorMode">Whether editor mode is enabled</param>
    public void SetEditorMode(bool editorMode)
    {
        if (editorMode)
        {
            // Add the editor helper and set it up
            var editorHelper = gameObject.AddComponent<LevelEditorHelper>();
            editorHelper.goalManager = goalManager;
        }
    }

    /// <summary>
    /// Generate a level from a given level info
    /// </summary>
    /// <param name="module">The module the level belongs to</param>
    /// <param name="level">The level info</param>
    public void GenerateFromModuleLevel(ModuleInfo module, LevelInfo level, GameState state)
    {
        // First, load the level data
        var levelPath = Path.Combine(Application.streamingAssetsPath, "Modules", module.Directory, $"{level.Id}.map");
        var levelData = File.ReadAllText(levelPath);
        var map = JsonUtility.FromJson<Level>(levelData);

        // Generate the level
        GenerateFrom(map);

        // Set the data in the game manager
        if (gameManager != null)
            gameManager.SetCurrentModuleLevel(module, level, state);
    }
}
