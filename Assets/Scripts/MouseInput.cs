using UnityEngine;
using System.Collections;
using System.Net;
using Assets.Scripts;
using UnityEditor;
using UnityEngine.UI;

public class MouseInput : MonoBehaviour
{

    public DragSource DraggingNow;
    private GameObject _selector;
    private GameObject _dragBox;
    private DragSource _attachOnNextUpdate;


    public const int DRAG_BUTTON_ID = 0;

    // Use this for initialization
    void Start ()
    {
        _selector = Toolbox.Instance.GameManager.GameGrid.GetSelector();
        _dragBox = Toolbox.Instance.GameManager.GameGrid.GetDragBox();
    }

#if TEST
    void OnGUI()
    {
        if (DraggingNow != null)
        {
            var p2 = transform.position + new Vector3(1, 1, 1);
            var rect = new Rect(transform.position, p2);
            //GUIHelper.BeginGroup(rect);
            GUIHelper.DrawLine(new Vector2(transform.position.x, transform.position.y), new Vector2(p2.x, p2.y), Color.red);
            //GUIHelper.EndGroup();
        }
    }
#endif

    // Update is called once per frame
    void Update () {

	    if (Input.mousePresent)
	    {

            // We support BOTH DRAG...DROP and CLICK...DRAG...CLICK .
            if (_attachOnNextUpdate != null)
            {
                Debug.Assert(DraggingNow == null, "already dragging.");
                DraggingNow = _attachOnNextUpdate;
                _attachOnNextUpdate = null;
                DraggingNow.StartDragging();
                return;
            }

            bool isDragging = (DraggingNow != null);
	        var leftButtonDown = Input.GetMouseButtonDown(DRAG_BUTTON_ID);
	        var leftButtonUp = Input.GetMouseButtonUp(DRAG_BUTTON_ID);
	        bool movedAwayFromClick = false;


            var pos = GetCursorPosition();

            // Move the selector around, to chase the mouse

            _selector.transform.position = pos;

	        if (isDragging)
	        {
	            movedAwayFromClick = DraggingNow.IsMovedAwayFromClick();

	            DraggingNow.transform.position = pos;
	            _dragBox.transform.position = pos;


	        }
	        else
	        {
	            Vector3 OFF_SCREEN = new Vector3(-10,-10,-10);
	            _dragBox.transform.position = OFF_SCREEN;
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

    public void StartDraggingSpawnedObject(DragSource dragSource)
    {
        Debug.Assert(dragSource != null, "missing drag source.");
        Debug.Assert(_attachOnNextUpdate == null, "already dragging");
        _attachOnNextUpdate = dragSource;
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
