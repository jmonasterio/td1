using UnityEngine;

public class RandomMove : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    this.transform.position.Set( this.transform.position.x + Random.Range(0f,0.1f), this.transform.position.y, this.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
