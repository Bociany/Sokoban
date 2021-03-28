/// <summary>
/// This class stores information about a module (collection of levels)
/// </summary>
public class ModuleInfo
{
    public enum ModuleType
    {
        ContinuousLevels,
        DifficultyBased
    }

    public string Name;
    public ModuleType Type;
    public string Directory;
    public bool Editable;
}
