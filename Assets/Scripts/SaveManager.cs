using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SaveManager : Singleton<SaveManager>
{
    public List<SaveState> Saves { get; private set; } = new List<SaveState>();

    private string SavesPath
    {
        get
        {
            return Path.Combine(Application.persistentDataPath, "saves");
        }
    }

    public override void Awake()
    {
        base.Awake();

        // Firstly, check if the saves directory exists
        // If not, create it.
        if (!Directory.Exists(SavesPath))
            Directory.CreateDirectory(SavesPath);

        // Now, load all of the saves
        ReloadSaves();
    }

    /// <summary>
    /// Reloads all the save files
    /// </summary>
    public void ReloadSaves()
    {
        Saves.Clear();

        foreach (var file in Directory.GetFiles(SavesPath, "*.sav"))
        {
            // Read the file
            var data = File.ReadAllText(file);
            var saveState = JsonUtility.FromJson<SaveState>(data);

            if (!Saves.Contains(saveState))
                Saves.Add(saveState);
        }

        // Finally, sort the saves descendingly from the highest timestamp
        Saves.Sort((l, r) =>
        {
            return r.SaveTakenTimestamp.CompareTo(l.SaveTakenTimestamp);
        });
    }

    /// <summary>
    /// Loads a level from the save state
    /// </summary>
    /// <param name="state">The state</param>
    public void LoadSave(SaveState state)
    {
        var module = ModulesManager.The.FindModule(moduleInfo =>
        {
            return moduleInfo.Directory == state.ModuleDirectory;
        });

        var level = ModulesManager.The.FindLevelFromModule(module, levelInfo =>
        {
            return levelInfo.Id == state.GameState.CurrentLevel;
        });

        WorldTraversalManager.The.LoadLevelFromModuleWithSavedState(module, level, state);
    }

    /// <summary>
    /// Removes a save
    /// </summary>
    /// <param name="state">The state to remove</param>
    public void RemoveSave(SaveState state)
    {
        Debug.Assert(!string.IsNullOrEmpty(state.Id));

        File.Delete(Path.Combine(SavesPath, $"{state.Id}.sav"));
        ReloadSaves();
    }

    /// <summary>
    /// Creates a new save
    /// </summary>
    /// <param name="gameManager">The current game manager</param>
    public void PerformSave(GameManager gameManager)
    {
        var levelData = File.ReadAllText
        (
            Path.Combine
            (
                Application.streamingAssetsPath, 
                "Modules", 
                gameManager.CurrentModule.Directory, 
                $"{gameManager.CurrentLevel.Id}.map"
            )
        );

        // Gather some helpers
        var level = JsonUtility.FromJson<Level>(levelData);
        var tilemap = gameManager.generator.tilemap;
        var tileSet = gameManager.generator.tileSet;

        var differingTileList = new List<TileDiff>();

        // First, gather all of the differing tiles.
        for (var y = 0; y < 20; y++)
            for (var x = 0; x < 30; x++)
            {
                var currentPos = new Vector3Int(x, 20 - y, 0);

                // Get the tile index and the tile.
                var tileIdx = level.TileData[(y * 30) + x];
                var setTile = tilemap.GetTile(currentPos);
                var setTileIdx = System.Array.IndexOf(tileSet.Tiles, setTile);

                if (tileIdx != setTileIdx)
                {
                    differingTileList.Add(new TileDiff
                    {
                        Position = currentPos,
                        TileId = setTileIdx
                    });
                }

            }

        var guid = Guid.NewGuid();

        // Create the save state
        var save = new SaveState
        {
            Id = $"{guid}",

            GameState = gameManager.GameState,
            TileDiffs = differingTileList.ToArray(),

            StepsTaken = gameManager.player.StepCount,
            TimePassed = gameManager.SecondsSinceLevelStarted,
            
            ModuleDirectory = gameManager.CurrentModule.Directory,
            SaveTakenTimestamp = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()
        };

        // Get a random name via GUIDs
        var saveFileName = $"{guid}.sav";

        // Write it
        File.WriteAllText(Path.Combine(SavesPath, saveFileName), JsonUtility.ToJson(save));

        ReloadSaves();
    }
}
