using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ModulesManager : Singleton<ModulesManager>
{
    /// <summary>
    /// This multi-map array stores all links from a module to its levels
    /// </summary>
    private readonly Dictionary<ModuleInfo, List<LevelInfo>> moduleLevelMap = new Dictionary<ModuleInfo, List<LevelInfo>>();

    /// <summary>
    /// Adds a module to the module list
    /// </summary>
    /// <param name="module">The module info</param>
    public void AddModule(ModuleInfo module)
    {
        moduleLevelMap.Add(
            module,
            new List<LevelInfo>()
        );
    }

    /// <summary>
    /// Adds a level to the module multimap
    /// </summary>
    /// <param name="module">The module the level is in</param>
    /// <param name="level">The level name</param>
    public void AddLevel(ModuleInfo module, LevelInfo level)
    {
        // If the map doesn't contain the module, abort.
        if (!moduleLevelMap.ContainsKey(module))
            return;

        moduleLevelMap[module].Add(level);
        moduleLevelMap[module].Sort((l, r) =>
        {
            if (int.TryParse(l.Name, out int lInt) && 
                int.TryParse(r.Name, out int rInt))
                return lInt.CompareTo(rInt);

            return l.Name.CompareTo(r.Name);
        });
    }

    /// <summary>
    /// Gets all the levels of a selected module
    /// </summary>
    /// <param name="module">The module</param>
    /// <returns>An enumerator with all the levels.</returns>
    public IEnumerable<LevelInfo> GetAllLevelsFor(ModuleInfo module)
    {
        if (!moduleLevelMap.ContainsKey(module))
            yield break;

        foreach (var level in moduleLevelMap[module])
            yield return level;
    }

    /// <summary>
    /// Tries to find a level from a given module, with the predicate
    /// acting as a judge on which level to find.
    /// </summary>
    /// <param name="module">The module</param>
    /// <param name="predicate">The predicate with which we're finding the level</param>
    /// <returns>Null if the level wasn't found. LevelInfo if we found the level.</returns>
    public LevelInfo FindLevelFromModule(ModuleInfo module, Predicate<LevelInfo> predicate)
    {
        if (!moduleLevelMap.ContainsKey(module))
            return null;

        foreach (var level in moduleLevelMap[module])
            if (predicate(level))
                return level;

        return null;
    }

    /// <summary>
    /// Gets a random level with a given difficulty
    /// </summary>
    /// <param name="module">The module</param>
    /// <returns>Either a level or nothing.</returns>
    public LevelInfo GetRandomLevelOfDifficulty(ModuleInfo module, Level.Difficulty difficulty)
    {
        if (!moduleLevelMap.ContainsKey(module))
            return null;

        // Temporarily allocate an array to clone the module level set
        var asArray = moduleLevelMap[module].ToArray();
        var counter = 0;

        // I'll run for 50 tries
        while (counter < 50)
        {
            var level = asArray[UnityEngine.Random.Range(0, asArray.Length)];

            if (level.Difficulty == difficulty)
                return level;

            counter++;
        }

        return null;
    }

    /// <summary>
    /// Finds a module with the predicate.
    /// </summary>
    /// <param name="predicate">The predicate that decides on the module</param>
    /// <returns>Null if the module wasn't found. ModuleInfo if we found the level.</returns>
    public ModuleInfo FindModule(Predicate<ModuleInfo> predicate)
    {
        foreach (var module in moduleLevelMap.Keys)
            if (predicate(module))
                return module;

        return null;
    }

    /// <summary>
    /// Removes a level from a module
    /// </summary>
    /// <param name="module">The module the level is in</param>
    /// <param name="levelInfo">The level</param>
    public void RemoveLevel(ModuleInfo module, LevelInfo levelInfo)
    {
        var moduleFolderPath = Path.Combine(Application.streamingAssetsPath, "Modules", module.Directory);
        File.Delete(Path.Combine(moduleFolderPath, $"{levelInfo.Id}.info"));
        File.Delete(Path.Combine(moduleFolderPath, $"{levelInfo.Id}.map"));
    }
}
