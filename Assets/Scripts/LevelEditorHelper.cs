using UnityEngine;

public class LevelEditorHelper : MonoBehaviour
{
    public GoalManager goalManager;

    private void Update()
    {
        // If we're in editor mode, we should quit the level testing if we either have
        // finished the level, or pressed enter

        if (goalManager.HasWon || Input.GetKeyDown(KeyCode.Return))
        {
            // Unload this scene and reenable the level editor
            WorldTraversalManager.The.SetSceneVisibility("LevelEditor", true);
            WorldTraversalManager.The.UnloadScene("MainScene");
        }
    }
}
