using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Algorithms;
using Assets.Scripts;
using UnityEngine.Assertions;

public class GameManagerScript : MonoBehaviour
{

    // Inspectors
    public string GameName;

    private static AudioPoolController AudioPoolController;
    public ScoreController ScoreController;
    public DataController DataController;
    public GameController GameController;
    public AtlasController AtlasController;
    public LevelController LevelController;

    public Selector _selector;
    public GameObject _dragBox;
    public GameObject _disallowed;

    public Selector GetSelector()
    {
        return _selector;
    }
    public GameObject GetDragBox()
    {
        return _dragBox;
    }
    public GameObject GetDisallowed()
    {
        return _disallowed;
    }

    // Use this for initialization (of me).
    void Awake()
    {
        // Need to be in AWAKE because other components refer to this.
        Toolbox.Instance.GameManager = this;
        Toolbox.Instance.DebugSys = this.GetComponent<DebugSystem>();
        AudioPoolController = GetComponent<AudioPoolController>();
        ScoreController = GetComponent<ScoreController>();
        DataController = GetComponent<DataController>();
        GameController = GetComponent<GameController>();
        AtlasController = GetComponent<AtlasController>();
        LevelController = GetComponent<LevelController>();
    }

    public void Start()
    {
        Cursor.visible = true;
        _selector = FindSelector();
    }

    private static Selector FindSelector()
    {
        return
            Resources.FindObjectsOfTypeAll<Selector>()
                .Where(go => go.gameObject.activeInHierarchy && go.gameObject.name == "Selector")
                .ToList()
                .FirstOrDefault();
    }


    public static void PlayClip(AudioClip clip)
    {
        //if (GameManager.Instance.PlayController.State == PlayController.States.Over)
        //{
        //    // No sound when not playing.
        //    return;
        //}
        if (AudioPoolController == null)
        {
            Debug.LogWarning("No audio pool controller.");
            return;
        }
        AudioPoolController.PlayClip(clip);
    }

}
