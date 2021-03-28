using UnityEngine;
using UnityEngine.UI;

public class TileSelector : MonoBehaviour
{
    public GameObject tileSelectionPrefab;
    public Transform tileSelectionParent;
    public TileSet tileSet;
    public Editor editor;

    void Start()
    {
        // Generate all of the UI buttons for the tile selection.
        for (var i = 0; i < tileSet.Tiles.Length; i++)
        {
            var tileSelectionObject = Instantiate(tileSelectionPrefab, tileSelectionParent);

            // Get the image and button components
            var tileSelectionButton = tileSelectionObject.GetComponent<TileSelectable>();
            var tileSelectionImage = tileSelectionObject.GetComponent<Image>();

            // Bind the button to change the current tile
            tileSelectionButton.editor = editor;
            tileSelectionButton.SetTile(tileSet.Tiles[i]);

            // Set the image to the tile
            tileSelectionImage.sprite = tileSet.Tiles[i].sprite;
        }
    }
}
