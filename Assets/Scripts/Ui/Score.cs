using UnityEngine;


public class Score : MonoBehaviour {
    private SpriteRenderer _renderer;

    // Use this for initialization
	void Start ()
	{
	    _renderer = this.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
    private void OnGUI()
    {
        return; // ONGUITEST

        if (Event.current.type == EventType.Repaint)
        {
            var vector = new Vector3(_renderer.bounds.min.x, _renderer.bounds.max.y, _renderer.bounds.min.z) +
                         new Vector3(0.5f, -0.5f, 0);
            float lineSpacing = 0.45f;

            var gameGrid = Toolbox.Instance.GameManager.LevelController.CurrentLevel.GameGrid;

            gameGrid.DrawTextAtVector(vector, "Score: " + Toolbox.Instance.GameManager.ScoreController.Score, Color.blue);
            vector.Set(vector.x, vector.y - lineSpacing, vector.z);
            gameGrid.DrawTextAtVector(vector, "Build Score: " + Toolbox.Instance.GameManager.ScoreController.BuildScore, Color.blue);
            vector.Set(vector.x, vector.y - lineSpacing, vector.z);
            gameGrid.DrawTextAtVector(vector, "Grow Score: " + Toolbox.Instance.GameManager.ScoreController.GrowScore, Color.blue);
            vector.Set(vector.x, vector.y - lineSpacing, vector.z);
            gameGrid.DrawTextAtVector(vector, "Grow rate: " + Toolbox.Instance.GameManager.ScoreController.GrowRate, Color.blue);
        }
    }
}

