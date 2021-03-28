using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum Clip
    {
        Walk,
        GoalReached,
        LevelComplete
    }

    public AudioSource source;
    public AudioClip[] clips;

    /// <summary>
    /// Plays a specified clip
    /// </summary>
    /// <param name="clip">The clip to play</param>
    public void PlayClip(Clip clip)
    {
        // Don't play if something else is playing right now.
        if (source.isPlaying)
            return;

        source.PlayOneShot(clips[(int)clip]);
    }

    /// <summary>
    /// Forces the playback of a specified clip
    /// </summary>
    /// <param name="clip">The clip to play</param>
    public void ForceClip(Clip clip)
    {
        source.PlayOneShot(clips[(int)clip]);
    }
}
