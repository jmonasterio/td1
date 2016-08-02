using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var scoreController = this.gameObject.GetComponent<ScoreController>();
	    if (scoreController.EnemyCaptureAllFlags() )
	    {
            //Debug.LogWarning("Should unload scene.");
            //SceneManager.LoadScene("GameOver");
	    }
	}
}
