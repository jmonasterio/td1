using UnityEngine;
using System.Collections;

public class DebugSystem : MonoBehaviour
{

    public bool ShowXOnPath = false;
    public bool ShowGridAtRuntime = false;

    public static void DebugAssert(bool condition, string message)
    {

        if (!condition)
        {
            Debug.Log(message);
            Debug.Break();
        }
    }
}
