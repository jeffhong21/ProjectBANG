using UnityEngine;

/// <summary>
/// Base class for components that must behave as singletons - being unique in the scene and having a static Instance property for easy access.
/// </summary>
/// <typeparam name="T">The type of the deriving component.</typeparam>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
    public static T Instance { get; private set; }



    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarningFormat("Another instance of {0} has already been registered for this scene, destroying this one", this);
            Destroy(this);
            return;
        }

        Instance = (T)this;

    }
}