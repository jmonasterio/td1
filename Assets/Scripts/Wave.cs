using UnityEngine;
using System.Collections;

public class Wave : MonoBehaviour
{

    public string WaveName;
    public float startDelayTime;
    public float endDelayTime;

    public Waypoint StartWaypoint;
    public Waypoint EndWaypoint;

    private Coroutine _coroutine; // So you can cancel

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void CancelWaveCoroutine()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    public void SetCoroutine(Coroutine coroutine)
    {
        _coroutine = coroutine;
    }
}
