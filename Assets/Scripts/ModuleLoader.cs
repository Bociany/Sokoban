using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ModuleLoader : MonoBehaviour
{
    [Header("Modules")]
    public GameObject moduleButtonPrefab;
    public Transform moduleButtonContainer;

    [Header("Levels")]
    public GameObject levelButtonPrefab;
    public Transform levelButtonContainer;
    public Text levelSelectionHeader;

    [Header("Etc")]
    public DifficultySelector diffSelector;

    private void Awake()
    {
        // Get a list of all modules in the module directory
        var moduleDirectory = Path.Combine(Application.streamingAssetsPath, "Modules");
        foreach (var module in Directory.GetFiles(moduleDirectory, "*.json"))
        {
            // Try to read and parse it
            var moduleData = File.ReadAllText(module);
            var moduleInfo = JsonUtility.FromJson<ModuleInfo>(moduleData);

            // Instantiate a new prefab for the module
            var button = Instantiate(moduleButtonPrefab, moduleButtonContainer)
                                    .GetComponent<ModuleMenuButton>();

            // Set the module of the button
            button.SetCurrentModule(this, moduleInfo);
            button.selector = diffSelector;

            // Load the meta info for all levels
            LoadLevelsFromModule(moduleInfo);
        }
    }

    /// <summary>
    /// Loads the meta info for all levels of a given module.
    /// </summary>
    /// <param name="module">The module to load info from.</param>
    public void LoadLevelsFromModule(ModuleInfo module)
    {
        // Add the current module to the modules manager.
        ModulesManager.The.AddModule(module);

        var levelDirectory = Path.Combine(Application.streamingAssetsPath, "Modules", module.Directory);
        foreach (var levelInfoPath in Directory.GetFiles(levelDirectory, "*.info"))
        {
            // Read the data and parse it
            var levelInfoData = File.ReadAllText(levelInfoPath);
            var levelInfo = JsonUtility.FromJson<LevelInfo>(levelInfoData);

            // Add the level to the manager
            ModulesManager.The.AddLevel(module, levelInfo);
        }
    }

    /// <summary>
    /// Loads the level list for a module and displays it in the main menu
    /// </summary>
    /// <param name="module">The module for which we're showing the level list</param>
    public void LoadLevelListFor(ModuleInfo module)
    {
        levelSelectionHeader.text = module.Name;

        // Instantiate a button for every level
        foreach (var level in ModulesManager.The.GetAllLevelsFor(module))
        {
            var button = Instantiate(levelButtonPrefab, levelButtonContainer)
                                    .GetComponent<LevelMenuButton>();

            button.SetCurrentLevel(module, level);
        }

        // Show the level selection
        moduleButtonContainer.gameObject.SetActive(false);
        levelButtonContainer.gameObject.SetActive(true);
    }
}
