using UnityEngine;

public class Toolbox : Singleton<Toolbox>
{
    

    protected Toolbox()
    {
        //_gm = Instance.GameManager; 
    } // guarantee this will be always a singleton only - can't use the constructor!
    //private GameManagerScript _gm;

    public Language language = new Language();

    public GameManagerScript GameManager;
    public DebugSystem DebugSys;

    //public ScoreController SC
   // {
   //     get { return _gm.ScoreController; }
   // }

    void Awake()
    {
        // Your initialization code here
    }

    // (optional) allow runtime registration of global objects
    static public T RegisterComponent<T>() where T : Component
    {
        return Instance.GetOrAddComponent<T>();
    }
}

static public class MethodExtensionForMonoBehaviourTransform
{
    /// <summary>
    /// Gets or add a component. Usage example:
    /// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
    /// </summary>
    static public T GetOrAddComponent<T>(this Component child) where T : Component
    {
        T result = child.GetComponent<T>();
        if (result == null)
        {
            result = child.gameObject.AddComponent<T>();
        }
        return result;
    }
}

[System.Serializable]
public class Language
{
    public string current;
    public string lastLang;
}