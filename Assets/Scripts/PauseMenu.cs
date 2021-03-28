using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuObject;
    public GameManager gameManager;

    private void Update()
    {
        // If we have escape, toggle the visibility of the pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenuObject.SetActive(!pauseMenuObject.activeSelf);
        }
    }

    public void Save_OnClick()
    {
        SaveManager.The.PerformSave(gameManager);
    }

    public void Return_OnClick()
    {
        WorldTraversalManager.The.LoadScene("MainMenu");
    }
}
