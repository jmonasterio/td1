using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LevelController : MonoBehaviour
{

    public Level ActiveLevel;
    private Level _activeLevel;

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnValidate()
    {
        if (ActiveLevel != _activeLevel)
        {
            _activeLevel = ActiveLevel;
            ChangeLevel(ActiveLevel);
        }
    }

    private void ChangeLevel(Level activeLevel)
    {
        Debug.Log("Change level");
        DeactivateAllLevels();
        activeLevel.gameObject.SetActive(true);
        MoveCameraToLevel(activeLevel);
    }

    private void MoveCameraToLevel(Level activeLevel)
    {
        var camera = FindCamera();
        Debug.Log(camera.tag);
        var newCamerPos = activeLevel.gameObject.transform.FindChild("Map").transform.position;
        newCamerPos.z = camera.transform.position.z;
        camera.transform.position = newCamerPos;
    }

    private void DeactivateAllLevels()
    {
        foreach (var level in FindAllLevels())
        {
            level.gameObject.SetActive(false);
        }
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
