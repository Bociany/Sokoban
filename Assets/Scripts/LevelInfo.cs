/// <summary>
/// This class contains meta information about the level which we can load
/// without loading the actual level file.
/// </summary>
public class LevelInfo
{
    public string Name;
    public string Id;
    public string NextLevel;
    public bool IsFinalLevel;
    public Level.Difficulty Difficulty;
}
