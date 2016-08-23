using UnityEngine;
using System.Collections;

/// <summary>
/// Common properties that every entity in the system should have.
/// </summary>
public class Entity : MonoBehaviour
{

    /// <summary>
    /// Cost for player to add this type of entity.
    /// </summary>
    public int IncomeCost;

    public int Health = 5;
    public int HealthMax = 5;

    // Use this for initialization

#if !EXCLUDE_HEALTH
    public bool HealthDisplay = true;
    public GUIStyle progress_empty;
    public GUIStyle progress_full;

    //current progress
    public float barDisplay;

    Vector2 pos = new Vector2(10, 50);
    Vector2 size = new Vector2(250, 50);

    private Texture2D emptyTex;
    private Texture2D fullTex;

    void Start()
    {
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

    void OnSceneGUI()
    {
        OnGUI();
    }

    void OnGUI()
    {
        if (HealthDisplay)
        {


            //draw the background:
            GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y), emptyTex, progress_empty);



            GUI.Box(new Rect(pos.x, pos.y, size.x, size.y), emptyTex, progress_full);

            //draw the filled-in part:
            GUI.BeginGroup(new Rect(0, 0, size.x*barDisplay, size.y));
            GUI.Box(new Rect(0, 0, size.x, size.y), fullTex, progress_full);

            GUI.EndGroup();
            GUI.EndGroup();
        }
    }

    void Update()
    {

        //the player's health
        if (HealthMax > 0)
        {
            barDisplay = (float) Health/ (float) HealthMax;
        }
    }
#endif
}


