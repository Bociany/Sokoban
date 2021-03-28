using UnityEngine.SceneManagement;
using System.Collections;

public class WorldTraversalManager : Singleton<WorldTraversalManager>
{
    /// <summary>
    /// Opens the level editor with the given level
    /// </summary>
    /// <param name="level">The level</param>
    public void EditLevel(ModuleInfo module, LevelInfo level)
    {
        StartCoroutine(LoadEditorWithLevel(module, level));
    }

    /// <summary>
    /// Load a level from some data.
    /// </summary>
    /// <param name="level">The level data.</param>
    /// <param name="editorMode">Whether to load the level in editor mode</param>
    public void LoadLevel(Level level, bool editorMode)
    {
        StartCoroutine(LoadLevelAsync(level, editorMode));
    }

    /// <summary>
    /// Loads a level from a specified module
    /// </summary>
    /// <param name="module">The module the level belongs to</param>
    /// <param name="level">The level info</param>
    public void LoadLevelFromModule(ModuleInfo module, LevelInfo level)
    {
        StartCoroutine(LoadLevelFromModuleAsync(module, level));
    }

    /// <summary>
    /// Loads a level from a module and applies saved state onto it.
    /// </summary>
    /// <param name="module">The module the level belongs to</param>
    /// <param name="level">The level info</param>
    /// <param name="state">The save state</param>
    public void LoadLevelFromModuleWithSavedState(ModuleInfo module, LevelInfo level, SaveState state)
    {
        StartCoroutine(LoadLevelFromModuleWithSaveAsync(module, level, state));
    }

    /// <summary>
    /// Loads a scene synchronously by the given name
    /// </summary>
    /// <param name="sceneName">The name of the scene</param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Unloads a scene with the given name
    /// </summary>
    /// <param name="sceneName">The scene to unload</param>
    public void UnloadScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }

    /// <summary>
    /// Sets the visibility of a given scene
    /// </summary>
    /// <param name="name">The name of the scene</param>
    public void SetSceneVisibility(string name, bool active)
    {
        foreach (var sceneObject in SceneManager.GetSceneByName(name).GetRootGameObjects())
        {
            sceneObject.SetActive(active);
        }
    }

    private IEnumerator LoadLevelAsync(Level level, bool editorMode)
    {
        // Start loading the scene asynchronously
        var asyncLoad = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Additive);

        // Yield until it's done
        while (!asyncLoad.isDone)
            yield return null;

        // When it's done, find the level generator and generate the level.
        var levelGenerator = FindObjectOfType<LevelGenerator>();
        levelGenerator.GenerateFrom(level);

        // If we have editor mode enabled, set the appropriate flag
        levelGenerator.SetEditorMode(editorMode);
    }

    private IEnumerator LoadLevelFromModuleAsync(ModuleInfo module, LevelInfo levelInfo)
    {
        // Start loading the scene asynchronously
        var asyncLoad = SceneManager.LoadSceneAsync("MainScene");

        // Yield until it's done
        while (!asyncLoad.isDone)
            yield return null;

        // When it's done, find the level generator and generate the level.
        var levelGenerator = FindObjectOfType<LevelGenerator>();
        levelGenerator.GenerateFromModuleLevel(module, levelInfo, null);
    }

    private IEnumerator LoadLevelFromModuleWithSaveAsync(ModuleInfo module, LevelInfo levelInfo, SaveState state)
    {
        // Start loading the scene asynchronously
        var asyncLoad = SceneManager.LoadSceneAsync("MainScene");

        // Yield until it's done
        while (!asyncLoad.isDone)
            yield return null;

        // When it's done, find the level generator and generate the level.
        var levelGenerator = FindObjectOfType<LevelGenerator>();
        levelGenerator.LoadSavedState(module, levelInfo, state);
    }

    private IEnumerator LoadEditorWithLevel(ModuleInfo module, LevelInfo level)
    {
        // Start loading the scene asynchronously
        var asyncLoad = SceneManager.LoadSceneAsync("LevelEditor");

        // Yield until it's done
        while (!asyncLoad.isDone)
            yield return null;

        // When it's done, find the level generator and generate the level.
        var levelGenerator = FindObjectOfType<LevelGenerator>();
        levelGenerator.GenerateFromModuleLevel(module, level, null);
    }
}
