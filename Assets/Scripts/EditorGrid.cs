using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class EditorGrid : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var rect = this.gameObject.GetComponent<SpriteRenderer>().bounds;
        Vector3 start = rect.min;
        Vector3 end = rect.max;

        for (int xx = (int)start.x; xx <= (int)end.x; xx++)
        {
            Vector3 ss = new Vector3( xx-0.5f, (int) start.y-0.5f, 0);
            Vector3 ee = new Vector3( xx-0.5f, (int) end.y+0.5f, 0);

	        Debug.DrawLine( ss,ee, Color.green, 0.1f);
	    }

	    for (int yy = (int) start.y; yy <= (int) end.y; yy++)
	    {
	        Vector3 ss = new Vector3((int) start.x - 0.5f, yy - 0.5f, 0);
	        Vector3 ee = new Vector3((int) end.x - 0.5f, yy - 0.5f, 0);

	        Debug.DrawLine(ss, ee, Color.green, 0.1f);
	    }
	    Debug.DrawLine( start, end, Color.magenta, 2f);
        Debug.DrawLine( new Vector3(0,0,0), end, Color.red );
    }
}
