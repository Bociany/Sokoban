using UnityEngine;
using UnityEngine.UI;

public class ModuleMenuButton : MonoBehaviour
{
    public Text buttonText;
    public ModuleLoader loader;
    public DifficultySelector selector;

    private ModuleInfo module;

    /// <summary>
    /// Sets the current module of this button
    /// </summary>
    /// <param name="moduleInfo">The module info</param>
    /// <param name="moduleLoader">The module loader</param>
    public void SetCurrentModule(ModuleLoader moduleLoader, ModuleInfo moduleInfo)
    {
        loader = moduleLoader;
        module = moduleInfo;
        buttonText.text = module.Name;
    }

    public void Button_OnClick()
    {
        if (module.Type == ModuleInfo.ModuleType.ContinuousLevels)
            loader.LoadLevelListFor(module);
        else
            selector.SetModule(module);
    }
}
