using UnityEngine;
using System.Collections;
using System.Net;
using Assets.Scripts;
using UnityEngine.UI;

public class MouseInput : MonoBehaviour
{

    public DragSource DraggingNow;


    public const int DRAG_BUTTON_ID = 0;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	    if (Input.mousePresent)
	    {

	        // We support BOTH DRAG...DROP and CLICK...DRAG...CLICK .

	        bool isDragging = (DraggingNow != null);
	        var leftButtonDown = Input.GetMouseButtonDown(DRAG_BUTTON_ID);
	        var leftButtonUp = Input.GetMouseButtonUp(DRAG_BUTTON_ID);
	        bool movedAwayFromClick = false;

	        var pos = GetCursorPosition();

            // Move the selector around, to chase the mouse
            Toolbox.Instance.GameManager.GameGrid.GetSelector().transform.position = pos;

            if (isDragging)
	        {
	            movedAwayFromClick = DraggingNow.IsMovedAwayFromClick();

	            DraggingNow.transform.position = pos;
	        }

	        if ((leftButtonDown && isDragging) || (leftButtonUp && isDragging && movedAwayFromClick))
	        {
	            // End dragging with a click.
	            DraggingNow.FinishOrCancelDragging();
	            DraggingNow = null;
	            
            }
    	    else if (leftButtonDown && !isDragging)
	        {
	            var dragSource = FindFirstDragSourceAtMousePositionOrNull();
	            if (dragSource != null)
	            {
                    DraggingNow = dragSource;
                    dragSource.StartDragging();


	            }
	        }
        }

    }

    private Vector3 GetCursorPosition()
    {
        var pos = Input.mousePosition;
        pos.z = -5;

        var gameGrid = Toolbox.Instance.GameManager.GameGrid;
        if (gameGrid != null)
        {
            var pos2 = gameGrid.MapScreenToMapPositionOrNull(pos);
            if (pos2.HasValue)
            {
                var pos3 = pos2.Value;
                pos3.z = 0;
                return pos3;
            }
            else
            {
                // We've gone off the grid! Yucky. Need better way to handle this.
                var pos3 = new Vector3(-10, -10);
                pos3.z = 0;
                return pos3;
            }
        }
        return new Vector3();
    }

    private DragTarget FindFirstDragTargetAtMousePositionOrNull()
    {
        Vector2 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D[] col = Physics2D.OverlapPointAll(v);

        if (col.Length > 0)
        {
            foreach (Collider2D c in col)
            {
                //Debug.Log("Collided with: " + c.collider2D.gameObject.name);

                var target = c.GetComponent<DragTarget>();
                if (target != null)
                {
                    // TBD: For now drag the first one.
                    return target;
                }
            }
        }
        return null;
    }

    private DragSource FindFirstDragSourceAtMousePositionOrNull()
    {
        Vector2 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D[] col = Physics2D.OverlapPointAll(v);

        if (col.Length > 0)
        {
            foreach (Collider2D c in col)
            {
                //Debug.Log("Collided with: " + c.collider2D.gameObject.name);

                var source = c.GetComponent<DragSource>();
                if (source != null)
                {
                    // TBD: For now drag the first one.
                    return source;
                }
            }
        }
        return null;
    }

    void OnMouseDown()
    {
        if ((Input.mousePresent) && (DraggingNow != null))
        {

        }
    }
}
