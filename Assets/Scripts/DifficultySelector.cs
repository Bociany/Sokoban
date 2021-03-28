using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySelector : MonoBehaviour
{
    public GameObject moduleSelector;
    public GameObject difficultySelector;

    private ModuleInfo moduleInfo;

    /// <summary>
    /// Sets the current module the selector will load for
    /// </summary>
    /// <param name="module">The module info</param>
    public void SetModule(ModuleInfo module)
    {
        moduleInfo = module;
        moduleSelector.SetActive(false);
        difficultySelector.SetActive(true);
    }

    public void Easy_OnClick()
    {
        var level = ModulesManager.The.GetRandomLevelOfDifficulty(moduleInfo, Level.Difficulty.Easy);

        if (level != null)
            WorldTraversalManager.The.LoadLevelFromModule(moduleInfo, level);
    }

    public void Medium_OnClick()
    {
        var level = ModulesManager.The.GetRandomLevelOfDifficulty(moduleInfo, Level.Difficulty.Medium);

        if (level != null)
            WorldTraversalManager.The.LoadLevelFromModule(moduleInfo, level);
    }

    public void Advanced_OnClick()
    {
        var level = ModulesManager.The.GetRandomLevelOfDifficulty(moduleInfo, Level.Difficulty.Advanced);

        if (level != null)
            WorldTraversalManager.The.LoadLevelFromModule(moduleInfo, level);
    }
}
