using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LevelController : MonoBehaviour
{

    public Levels ActiveLevel;
    private Levels _activeLevel;

    public enum Levels
    {
        Level1,
        Level2
    }

    // Use this for initialization
    void Start()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }


    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        try
        {
            // Fix up connections after changing scenes
            // TBD: This is so hacky. maybe there is some way I don't need this.
            Toolbox.Instance.GameManager.RebuildTreeNodes();

            Toolbox.Instance.GameManager.GameGrid.RebuildMapConnection();

            Toolbox.Instance.GameManager.WavesController.RebuildConnections();
            Toolbox.Instance.GameManager.WavesController.StartLevel(Time.time);

        }
        catch (Exception ex)
        {
            Debug.LogWarning( "sceneloaded: " + ex.Message);
        }
    }

    // Update is called once per frame
    void Update () {
	    Debug.Log( "Switched Level");
        if (ActiveLevel != _activeLevel)
        {
            ChangeLevel(ActiveLevel, _activeLevel);
            _activeLevel = ActiveLevel;
        }
    }

    void OnValidate()
    {
    }

    private void ChangeLevel(Levels activeLevel, Levels oldActiveLevel)
    {
        Debug.Log("Change level");

        Toolbox.Instance.GameManager.WavesController.CancelActiveWaves();

        if (!Application.isPlaying)
        {
           // Debug.Assert(EditorSceneManager.GetActiveScene().name == oldActiveLevel.ToString());
          //  var scene = EditorSceneManager.GetSceneByName(oldActiveLevel.ToString());
            // ReSharper disable once AccessToStaticMemberViaDerivedType
         //   if (EditorSceneManager.CloseScene(scene, true))
         //   {
          //      // ReSharper disable once AccessToStaticMemberViaDerivedType
           //     EditorSceneManager.LoadScene(activeLevel.ToString(), LoadSceneMode.Additive);
           // }
        }
        else
        {
            // Only works at runtime.
           // Debug.Assert(SceneManager.GetActiveScene().name == oldActiveLevel.ToString());
            // ReSharper disable once AccessToStaticMemberViaDerivedType
            if (SceneManager.UnloadScene(oldActiveLevel.ToString()))
            {
                // ReSharper disable once AccessToStaticMemberViaDerivedType
                SceneManager.LoadScene(activeLevel.ToString(), LoadSceneMode.Additive);
            }

        }
    }

    private void MoveCameraToLevel(Level activeLevel)
    {
        var camera = FindCamera();
        Debug.Log(camera.tag);
        var newCamerPos = activeLevel.gameObject.transform.FindChild("Map").transform.position;
        newCamerPos.z = camera.transform.position.z;
        camera.transform.position = newCamerPos;
    }

    private static Camera FindCamera()
    {
        return
            Resources.FindObjectsOfTypeAll<Camera>().Where( cam => cam.name == "Main Camera")
                .ToList().FirstOrDefault();
    }


    private static Level[] FindAllLevels()
    {
        return
            Resources.FindObjectsOfTypeAll<Level>()
                .ToArray();
    }

}
