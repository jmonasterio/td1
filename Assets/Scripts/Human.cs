using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Human : MonoBehaviour
{
    public enum HumanClasses
    {
        Standard = 1,
        Gatherer = 2, // Gathers's go get body of dead aliens. Humans get converted into gatherer.
    }


    public int IncomeValue = 10;
    private Entity _entity;
    private Wander _wander;
    private float _dropTime;

    // Use this for initialization
	void Start ()
	{
	    _entity = GetComponent<Entity>();
	    _wander = GetComponent<Wander>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    const float DELAY_BEFORE_NEXT_PATH_MOVE = 0.3f;
        // After being dropped, wait 0.5 seconds before wandering again.
        if (_dropTime != 0 && Time.time > _dropTime + DELAY_BEFORE_NEXT_PATH_MOVE)
	    {
	        _wander.RestartWandering();
            _dropTime = 0.0f;
	    }

	}

    public void DropAt(Vector3 mapExactDrop)
    {
        _dropTime = Time.time;
        _wander.StopWandering();
        this.transform.position = mapExactDrop;
        this.gameObject.layer = GameGrid.BACKGROUND_LAYER;

    }
}
