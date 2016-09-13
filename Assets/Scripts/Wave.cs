using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wave : MonoBehaviour
{

    public int WaveId;
    public float startDelayTime;
    public float endDelayTime;

    public Path Path;

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
