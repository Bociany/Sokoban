using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoader : MonoBehaviour
{
    public GameObject saveButtonPrefab;
    public Transform saveButtonContainer;

    private void Start()
    {
        foreach (var save in SaveManager.The.Saves)
        {
            var saveButton = Instantiate(saveButtonPrefab, saveButtonContainer).GetComponent<SaveMenuButton>();
            saveButton.Set(save);
        }
    }
}
