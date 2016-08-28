using UnityEngine;
using System.Collections;
using UnityEditor;

public class Health : MonoBehaviour
{

    private Vector3 labelOffset = new Vector3(0.55f, -1f, 0f);
    private Entity _entity;
    private float _barDisplay;

    // Use this for initialization
    void Start()
    {
        _entity = GetComponent<Entity>();
    }

    void OnSceneGUI()
    {
        OnGUI();
    }


    // Update is called once per frame
    void OnGUI()
    {
        var origColor = GUI.color;
        GUI.color = Color.gray;
        Handles.Label(this.transform.position - labelOffset, "" + _entity.Health);
        GUI.color = origColor;

        var pos = this.transform.position - labelOffset;
        pos.z = -10.0f;

        GUI.color = Color.red;
        var boxSize = new Vector3(1, 0, 0);

        var origHandleColor = Handles.color;
        Handles.color = Color.green;
        Handles.DrawAAPolyLine(Texture2D.whiteTexture, 3.0f,
            pos,
            pos + boxSize*_barDisplay);

        Handles.color = Color.red;
        Handles.DrawAAPolyLine(Texture2D.whiteTexture, 3.0f,
            pos + boxSize*_barDisplay,
            pos + boxSize);


        Handles.color = origHandleColor;
    }

    void Update()
    {

        //the player's health
        if (_entity.HealthMax > 0)
        {
            _barDisplay = (float) _entity.Health/(float) _entity.HealthMax;
        }
    }


}
