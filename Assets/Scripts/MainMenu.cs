using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject moduleContainer;
    public GameObject mainButtonContainer;
    public GameObject savesContainer;

    public void LevelEditor_OnButtonClick()
    {
        WorldTraversalManager.The.LoadScene("LevelEditor");
    }

    public void Play_OnButtonClick()
    {
        mainButtonContainer.SetActive(false);
        moduleContainer.SetActive(true);
    }

    public void Saves_OnButtonClick()
    {
        mainButtonContainer.SetActive(false);
        savesContainer.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            moduleContainer.SetActive(false);
            savesContainer.SetActive(false);
            mainButtonContainer.SetActive(true);
        }
    }
}
