using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GoalManager : MonoBehaviour
{
    public enum GoalState
    {
        Empty,
        Fulfilled
    };

    public TileSet tileSet;
    public AudioManager audioManager;

    // This is where we'll store all of the goals that we have in the level.
    // All of them will also have their respective state attached to them.
    // Fulfilled meaning that there's a box overlapping the goal.
    private readonly Dictionary<Vector3Int, GoalState> goals = new Dictionary<Vector3Int, GoalState>();

    /// <summary>
    /// Clears the goals.
    /// </summary>
    public void ResetGoals()
    {
        goals.Clear();
    }

    /// <summary>
    /// Adds a goal at the specified position
    /// </summary>
    /// <param name="position">The position of the goal</param>
    public void AddGoal(Vector3Int position)
    {
        goals.Add(position, GoalState.Empty);
    }

    /// <summary>
    /// Gets the state of the goal at the given position.
    /// </summary>
    /// <param name="position">Position of the goal</param>
    /// <returns>Nullable enum. If the return value is a null, there's no goal at the given position. Otherwise it returns the state of the goal.</returns>
    public GoalState? GetGoalState(Vector3Int position)
    {
        if (goals.TryGetValue(position, out GoalState goalState))
        {
            return goalState;
        }

        return null;
    }

    /// <summary>
    /// Sets the goal state of a specified goal
    /// </summary>
    /// <param name="position">The position of the goal</param>
    /// <param name="state">The new state</param>
    public void SetGoalState(Vector3Int position, GoalState state)
    {
        // If we don't have a goal at this position, abort.
        if (!goals.ContainsKey(position))
            return;

        // If the state is fulfilled, play the clip.
        if (state == GoalState.Fulfilled)
            audioManager.PlayClip(AudioManager.Clip.GoalReached);

        goals[position] = state;
    }

    /// <summary>
    /// Returns the win state of the game.
    /// </summary>
    public bool HasWon
    {
        get
        {
            // Get every single goal state
            foreach (var value in goals.Values)
            {
                // If the goal state is empty, we know we haven't won yet. Quit.
                if (value == GoalState.Empty)
                    return false;
            }

            // Otherwise we have won.
            return true;
        }
    }

    /// <summary>
    /// Gets the goal tile.
    /// </summary>
    public TileBase GoalTile => tileSet.GoalTile;
}
