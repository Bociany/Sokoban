using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    [Header("Important")]
    public GoalManager goalManager;
    public AudioManager audioManager;

    [Header("Tiles")]
    public Tilemap tilemap;
    public TileBase clearTile;
    public TileBase boxTile;

    private Vector3Int position;

    public int StepCount { get; private set; }

    /// <summary>
    /// Sets the current position of the player. This is passed in from the Level Generator script
    /// </summary>
    /// <param name="position">The position of the player entity.</param>
    public void SetCurrentPosition(Vector3Int position)
    {
        this.position = position;

        // Also reset the step count
        StepCount = 0;
    }

    /// <summary>
    /// Performs a move in the direction specified by the vector.
    /// </summary>
    /// <param name="vector">The movement vector</param>
    private void DoMove(Vector3Int vector)
    {
        var replaceTile = clearTile;

        // First, check if we're going to collide with something
        // If the tile on (position + vector) isn't a clear tile, check it.
        var nextTile = tilemap.GetTile(position + vector);
        if (nextTile != clearTile)
        {
            // Next, check if the next tile is a box tile
            // If so, try to move it. If we can't perform a move afterwards, abort.
            if (nextTile == boxTile && !DoMoveBox(position + vector, vector))
                return;

            // If the tile wasn't a box tile and isn't a goal tile, abort the movement.
            else if (nextTile != boxTile && nextTile != goalManager.GoalTile)
                return;
        }

        // Get the goal state at our current position, if we have any, 
        // we need to be mindful to reset it later if we're moving from the goal.
        var goalState = goalManager.GetGoalState(position);
        if (goalState.HasValue)
        {
            replaceTile = goalManager.GoalTile;
        }

        // Get the player tile on the current position
        var tileInfo = tilemap.GetTile(position);

        // Clear the old position
        tilemap.SetTile(position, replaceTile);

        // Move him to the new position
        position += vector;
        tilemap.SetTile(position, tileInfo);

        // Increase the step count.
        StepCount++;
        audioManager.PlayClip(AudioManager.Clip.Walk);
    }

    /// <summary>
    /// Tries to move a box in a specified direction.
    /// </summary>
    /// <param name="position">The current position of the box</param>
    /// <param name="position">The movement vector</param>
    /// <returns>Whether the player can move afterwards.</returns>
    private bool DoMoveBox(Vector3Int position, Vector3Int direction)
    {
        var newPosition = position + direction;
        var replaceTile = clearTile;

        // Firstly, try to get the goal state at this position
        var goalStatePrev = goalManager.GetGoalState(position);
        var goalState = goalManager.GetGoalState(newPosition);

        // Then, get the tile that the box is going to move to
        // and check whether it can make the move. If it can't,
        // we can't either.
        var nextTile = tilemap.GetTile(newPosition);
        if (nextTile != clearTile)
        {
            // If the goal state has a value, we have a goal there.
            // Ensure that the goal state is actually empty first!
            // Move the box and set the goal state to true.
            if (goalState.HasValue &&
                goalState.Value == GoalManager.GoalState.Empty)
                goalManager.SetGoalState(newPosition, GoalManager.GoalState.Fulfilled);

            // Otherwise just quit.
            else
                return false;
        }

        // If we've passed the initial check, we then need to check 
        // if we're moving from a goal. If that's the case, we need to
        // reset the tile to the goal tile, and unmark it in the goal manager.
        if (goalStatePrev.HasValue &&
            goalStatePrev.Value == GoalManager.GoalState.Fulfilled)
        {
            replaceTile = goalManager.GoalTile;
            goalManager.SetGoalState(position, GoalManager.GoalState.Empty);
        }

        // If it passed, move the box
        var boxTile = tilemap.GetTile(position);
        tilemap.SetTile(position + direction, boxTile);

        // Clear the box tile
        tilemap.SetTile(position, replaceTile);

        // Tell the player that they can move.
        return true;
    }

    /// <summary>
    /// Gets the movement as a -1 / +1 int on the specified axis
    /// </summary>
    /// <param name="positive">"Positive" element of the axis</param>
    /// <param name="negative">"Negative" element of the axis</param>
    /// <returns>1 for positive, -1 for negative and 0 for neither.</returns>
    private int GetMovementOn(string positive, string negative)
    {
        if (Input.GetButtonDown(positive))
            return 1;
        else if (Input.GetButtonDown(negative))
            return -1;

        return 0;
    }

    private void Update()
    {
        // Get the movement vectors
        // Ensure that we can go in only one direction at a time, never across.
        var horizontal = GetMovementOn("Right", "Left");
        var vertical = horizontal == 0 ? GetMovementOn("Up", "Down") : 0;
        var movement = new Vector3Int(horizontal, vertical, 0);

        // Check if we have movement in any direction, if so, perform it.
        if (movement.sqrMagnitude > float.Epsilon)
        {
            DoMove(movement);
        }
    }
}
