using UnityEngine;
using System.Collections;
using UnityEditor;

public class Health : MonoBehaviour {

    private Vector3 labelOffset = new Vector3(0.55f, -1f, 0f);
    private Entity _entity;

    public GUIStyle progress_empty;
    public GUIStyle progress_full;

    //current progress
    public float barDisplay;


    private Texture2D emptyTex;
    private Texture2D fullTex;
    private Texture2D _redHealth;

    // Use this for initialization
    void Start()
    {
        _entity = GetComponent<Entity>();
        {
            var t = Texture2D.whiteTexture;
            var c2 = t.GetPixel(0, 1);
            var c3 = t.GetPixel(1, 0);

            // 1x2 needed for antialiasing with alpah.
            _redHealth = new Texture2D(1, 2, TextureFormat.RGBA32, true);
            _redHealth.alphaIsTransparency = false;
            _redHealth.SetPixel(0, 0, new Color(1.0f, 0.0f, 0.0f, 1.0f));
            _redHealth.SetPixel(0, 1, new Color(1.0f, 0.0f, 0.0f, 1.0f));


            emptyTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);

            // set the pixel values
            emptyTex.SetPixel(0, 0, Color.red);

            // Apply all SetPixel calls
            emptyTex.Apply();

            fullTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);

            // set the pixel values
            fullTex.SetPixel(0, 0, Color.red);

            // Apply all SetPixel calls
            fullTex.Apply();
        }
    }

    void OnSceneGUI()
    {
        OnGUI();
    }


    // Update is called once per frame
    void OnGUI () {
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
            pos + boxSize * barDisplay);

        Handles.color = Color.red;
        Handles.DrawAAPolyLine(Texture2D.whiteTexture, 3.0f,
            pos + boxSize * barDisplay,
            pos + boxSize);


        Handles.color = origHandleColor;


        //draw the background:
        //GUI.BeginGroup(new Rect(pos, boxSize), emptyTex , progress_empty);
        //GUI.Box(new Rect( pos, boxSize), emptyTex, progress_full);

        //draw the filled-in part:
        //Vector2 barSize = new Vector3(boxSize.x*barDisplay, boxSize.y, -10.0f);
        //GUI.BeginGroup(new Rect(pos, barSize));
        //GUI.Box(new Rect(pos, barSize), fullTex, progress_full);

        //GUI.EndGroup();
        //GUI.EndGroup();
    }
    
        void Update()
        {

            //the player's health
            if (_entity.HealthMax > 0)
            {
                barDisplay = (float) _entity.Health/(float) _entity.HealthMax;
            }
        }


    }
