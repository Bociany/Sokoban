using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LevelGenerator generator;
    public GoalManager goalManager;
    public AudioManager audioManager;
    public GameObject endOverlay;
    public Player player;

    private bool gameStarted;
    private TimeSince timeSinceLevelStarted;

    private const int BASE_SCORE = 10000;
    private const int SCORE_LOSE_PER_SECOND = 25;
    private const int SCORE_LOSE_PER_MOVE = 10;

    public ModuleInfo CurrentModule { get; private set; }
    public LevelInfo CurrentLevel { get; private set; }
    public GameState GameState { get; private set; }

    public float SecondsSinceLevelStarted => timeSinceLevelStarted;

    private void Update()
    {
        // We can only start this if the game has started.
        if (!gameStarted)
            return;

        // Check if we've won and we haven't enabled the overlay yet.
        if (goalManager.HasWon &&
            !endOverlay.activeInHierarchy)
        {
            // Enable the overlay and play the tune
            endOverlay.SetActive(true);
            audioManager.ForceClip(AudioManager.Clip.LevelComplete);

            // Add points to game state
            GameState.PointsCollected += CalculateScore();

            gameStarted = false;
        }
    }

    /// <summary>
    /// Sets the current module and level info.
    /// </summary>
    /// <param name="moduleInfo">The module the level is in.</param>
    /// <param name="levelInfo">The level</param>
    public void SetCurrentModuleLevel(ModuleInfo moduleInfo, LevelInfo levelInfo, GameState state)
    {
        CurrentModule = moduleInfo;
        CurrentLevel = levelInfo;

        gameStarted = true;

        timeSinceLevelStarted = 0f;

        if (state == null)
        {
            GameState = new GameState
            {
                CurrentLevel = CurrentLevel.Id,
                PointsCollected = 0
            };
        }
        else
        {
            GameState = state;
        }
    }

    /// <summary>
    /// Calculates the end-level score.
    /// </summary>
    /// <returns>The score for this level</returns>
    private int CalculateScore()
    {
        var score = BASE_SCORE;
        score -= SCORE_LOSE_PER_SECOND * (int)timeSinceLevelStarted;
        score -= SCORE_LOSE_PER_MOVE * player.StepCount;

        return Mathf.Clamp(score, 0, BASE_SCORE);
    }

    public void NextLevel_Click()
    {
        endOverlay.SetActive(false);

        switch (CurrentModule.Type)
        {
            case ModuleInfo.ModuleType.ContinuousLevels:
                {
                    // Check if we have a level present in the next level field
                    if (!string.IsNullOrEmpty(CurrentLevel.NextLevel))
                    {
                        // Get the next level's meta info
                        var info = ModulesManager.The.FindLevelFromModule(CurrentModule, levelInfo =>
                        {
                            return CurrentLevel.NextLevel == levelInfo.Id;
                        });

                        GameState.CurrentLevel = info.Id;

                        // If it doesn't exist, load the same level
                        if (info == null)
                            generator.GenerateFromModuleLevel(CurrentModule, CurrentLevel, GameState);

                        // Otherwise, load the new level
                        else
                            generator.GenerateFromModuleLevel(CurrentModule, info, GameState);
                    }

                    // Otherwise, load the same level
                    else
                    {
                        generator.GenerateFromModuleLevel(CurrentModule, CurrentLevel, GameState);
                    }
                    return;
                }

            case ModuleInfo.ModuleType.DifficultyBased:
                {
                    var level = ModulesManager.The.GetRandomLevelOfDifficulty(CurrentModule, CurrentLevel.Difficulty);
                    GameState.CurrentLevel = level.Id;

                    // If nothing exists, load the same level
                    if (level == null)
                        generator.GenerateFromModuleLevel(CurrentModule, CurrentLevel, GameState);

                    // Otherwise, load the new level
                    else
                        generator.GenerateFromModuleLevel(CurrentModule, level, GameState);
                    return;
                }
        }
    }

    public void Return_Click()
    {
        WorldTraversalManager.The.LoadScene("MainMenu");
    }
}
