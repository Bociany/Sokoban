using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper singleton base class
/// </summary>
public class Singleton<T> : MonoBehaviour where T: MonoBehaviour
{
    public static T The { get; private set; }

    public virtual void Awake()
    {
        // Ensure that we don't destroy ourselves on load.
        DontDestroyOnLoad(gameObject);

        // Set ourselves as the singleton instance.
        if (The == null)
            The = this as T;

        // If The isn't null, destroy ourselves
        // We don't want two copies of a singleton
        else
            Destroy(gameObject);
    }
}
