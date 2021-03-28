using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Advanced
    }

    public Vector2 Boundaries;
    public int[] TileData;
}
