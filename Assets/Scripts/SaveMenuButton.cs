// I honestly don't care anymore, I've been doing this project alone while the rest
// of my team were slacking off, since none of them know how to code. I'm tired.
// おやすみ

using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenuButton : MonoBehaviour
{
    public Text buttonText;
    private SaveState state;

    public void Set(SaveState state)
    {
        this.state = state;

        var module = ModulesManager.The.FindModule(moduleInfo =>
        {
            return moduleInfo.Directory == state.ModuleDirectory;
        });

        var level = ModulesManager.The.FindLevelFromModule(module, levelInfo =>
        {
            return levelInfo.Id == state.GameState.CurrentLevel;
        });

        buttonText.text = $"{level.Name} - {DateTimeOffset.FromUnixTimeSeconds((long)state.SaveTakenTimestamp)}";
    }

    public void Button_OnClick()
    {
        SaveManager.The.LoadSave(state);
    }

    public void Remove_OnClick()
    {
        SaveManager.The.RemoveSave(state);
        Destroy(gameObject);
    }
}
