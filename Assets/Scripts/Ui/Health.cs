using Assets.Scripts;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Health : MonoBehaviour
{

    private Vector3 labelOffset = new Vector3(0.0f, -0.9f, 0f);
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
        if( Event.current.type != EventType.Repaint)
        {
            return;
        }



        var origColor = GUI.color;
        GuiExtension.GuiLabel(this.transform.position - labelOffset, "" + _entity.Health, Color.gray);

        var pos = this.transform.position - labelOffset;
        pos.z = -10.0f;

        pos = GuiExtension.MapPositionToScreen(pos);

        GUI.color = Color.red;
        var boxSize = new Vector3(0.005f, 0.001f, 0);
        //boxSize = GuiExtension.MapWorldToScreenRect(boxSize);

        var rc = new Rect(pos, boxSize * _barDisplay);
        DrawQuad(rc, Color.green);
        rc = new Rect(pos + boxSize * _barDisplay, boxSize - boxSize * _barDisplay);
        DrawQuad(rc, Color.red);

        GUI.color = origColor;

    }

    private void DrawQuad(Rect pos, Color color)
    {
        var texture = Texture2D.whiteTexture;
        texture.SetPixel(0,0,color);
        texture.Apply();
        GUI.skin.box.normal.background = texture;
        GUI.Box(pos, GUIContent.none);
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
