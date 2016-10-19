using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using Assets.Scripts;

public class ScoreController : MonoBehaviour {

    /// <summary>
    /// Overall score from killing enemies.
    /// </summary>
    public int Score;

    /// <summary>
    /// All the money you have available for building
    /// </summary>
    public float BuildScore;

    /// <summary>
    /// All the money you have available for growing humans.
    /// </summary>
    public float GrowScore;
    public float GrowRate;

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
}
