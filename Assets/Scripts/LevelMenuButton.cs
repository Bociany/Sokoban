using UnityEngine;
using UnityEngine.UI;

public class LevelMenuButton : MonoBehaviour
{
    public Text levelName;
    public GameObject editButton;
    public GameObject removeButton;

    public ModuleInfo module;
    public LevelInfo level;

    /// <summary>
    /// Sets the current level info for this button
    /// </summary>
    /// <param name="moduleInfo">The module the level belongs to</param>
    /// <param name="levelInfo">The level meta info</param>
    public void SetCurrentLevel(ModuleInfo moduleInfo, LevelInfo levelInfo)
    {
        module = moduleInfo;
        level = levelInfo;

        levelName.text = level.Name;

        editButton.SetActive(moduleInfo.Editable);
        removeButton.SetActive(moduleInfo.Editable);
    }

    public void Button_OnClick()
    {
        WorldTraversalManager.The.LoadLevelFromModule(module, level);
    }

    public void Edit_OnClick()
    {
        WorldTraversalManager.The.EditLevel(module, level);
    }

    public void Remove_OnClick()
    {
        ModulesManager.The.RemoveLevel(module, level);
        Destroy(transform.parent.gameObject);
    }
}
