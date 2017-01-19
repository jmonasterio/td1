using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Algorithms;
using Assets.Scripts;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * Additive scenes kinda suck for levels:
 *  1) They don't load in same way in editor as runtime.
 *  2) Lots of other problems.
 *  3) I don't want to rely on Unity's persistence... they keep losing my data.
 *  
 *  To load a level, we'll load in the "EmptyLevel" prefab.
 *  Then using data files for the level, we'll add everything onto it.
 *  
 *  Later we can make an level editor that will load and empty level, let you edit, and then save back to data.
 *  
 *  Instead of trying to use unity.
 */
[ExecuteInEditMode]
public class LevelController : MonoBehaviour
{
    public Levels ActiveLevelId;
    private Levels _activeLevel;
    private Level _currentLevel;

    public enum Levels
    {
        None,
        Level1,
        Level2
    }

    public Level CurrentLevel
    {
        get
        {
            return _currentLevel;
        }
    }

    // Use this for initialization
    void Start()
    {
        _activeLevel = Levels.None;
        _currentLevel = null; // _currentLevel = Resources.FindObjectsOfTypeAll<Level>().ToList().FirstOrDefault();
    }


    // Update is called once per frame
    void Update () {
        if (ActiveLevelId != _activeLevel)
        {
            Debug.Log("Switched Level");
            ChangeLevel(ActiveLevelId, _activeLevel);
            _activeLevel = ActiveLevelId;
        }
    }

    private void ChangeLevel(Levels activeLevel, Levels oldActiveLevel)
    {
        Debug.Log("Change level");

        if (!Application.isPlaying)
        {
            // Editor level switch

            // TBD: May want to save any changes to data file.
        }
        else
        {
            if (Toolbox.Instance.GameManager.LevelController.CurrentLevel != null)
            {
                Toolbox.Instance.GameManager.LevelController.CurrentLevel.WavesController.CancelActiveWaves();
            }

// Delete old level
            if (_currentLevel != null)
            {
                Destroy(_currentLevel.gameObject);
                _currentLevel = null;
            }

            // Runtime level switch.
            _currentLevel = Instantiate<Level>(Toolbox.Instance.GameManager.AtlasController.EmptyPrefabs.Level); // Make a copy, so we don't remove from tree and then we can run wave again.
            _currentLevel.gameObject.name = "Level";
            _currentLevel.transform.SetParent(Toolbox.Instance.GameManager.transform.parent);
            _currentLevel.transform.position = new Vector3(0f, 0f, 0f); // Important to center. Map Scale:(4.89, 3.67)
            _currentLevel.LevelId = ActiveLevelId;

            _activeLevel = ActiveLevelId;
        }
    }

}
