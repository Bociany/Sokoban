using System;
using UnityEngine;

[Serializable]
public class SaveState
{
    public string Id;

    public ulong SaveTakenTimestamp;
    public string ModuleDirectory;

    public GameState GameState;
    public TileDiff[] TileDiffs;
    
    public int StepsTaken;
    public float TimePassed;    
}

[Serializable]
public struct TileDiff
{
    public Vector3Int Position;
    public int TileId;
}