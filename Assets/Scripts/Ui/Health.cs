using Assets.Scripts;
using UnityEditor;
using UnityEngine;

public class Health : MonoBehaviour
{
    private readonly Vector3 barOffset = new Vector3(0.0f, 1, 0f);
    private readonly Vector3 barScale = new Vector3(2,1,0);
    private readonly Vector3 textOffset = new Vector3(-0.3f, 1.8f, -1f);
    private readonly Vector3 textScale = new Vector3(0.5f, 0.5f, 0.0f);
    private Entity _entity;
    private SpriteRenderer _healthControl;
    private TextMesh _healthText;
    private int _lastDrawnHealth;

    // Use this for initialization
    void Start()
    {
        _entity = GetComponent<Entity>();
        var go = new GameObject("HealthControl");
        go.transform.SetParent(this.transform);
        _healthControl = go.AddComponent<SpriteRenderer>();
        _healthControl.sprite = Toolbox.Instance.GameManager.AtlasController.DefaultHealthSprites.Health4;
        _healthControl.transform.localPosition = barOffset; // relative (above)
        _healthControl.transform.localScale = barScale;
        _healthControl.sortingLayerName = "Active"; // or won't draw. 

        _lastDrawnHealth = -1; // So we draw.

        var go2 = new GameObject("HealthText");
        go2.transform.SetParent(this.transform);
        go2.AddComponent<MeshRenderer>();
        _healthText = go2.AddComponent<TextMesh>();
        _healthText.text = "TEST";
        _healthText.richText = false;
        _healthText.characterSize = 1;
        _healthText.color = Color.gray;
        _healthText.transform.localPosition = textOffset;
        _healthText.transform.localScale = textScale;
    }

    void Update()
    {

        //the player's health
        if (_entity.HealthMax > 0)
        {
            int barDisplay = (int)(((float) _entity.Health/(float) _entity.HealthMax)*10);
            if (barDisplay != _lastDrawnHealth)
            {
                _healthControl.enabled = true;
                switch (10-barDisplay)
                {
                    case 0:
                        _healthControl.sprite = Toolbox.Instance.GameManager.AtlasController.DefaultHealthSprites.Health0;
                        break;
                    case 1:
                        _healthControl.sprite = Toolbox.Instance.GameManager.AtlasController.DefaultHealthSprites.Health1;
                        break;
                    case 2:
                        _healthControl.sprite = Toolbox.Instance.GameManager.AtlasController.DefaultHealthSprites.Health2;
                        break;
                    case 3:
                        _healthControl.sprite = Toolbox.Instance.GameManager.AtlasController.DefaultHealthSprites.Health3;
                        break;
                    case 4:
                        _healthControl.sprite = Toolbox.Instance.GameManager.AtlasController.DefaultHealthSprites.Health4;
                        break;
                    case 5:
                        _healthControl.sprite = Toolbox.Instance.GameManager.AtlasController.DefaultHealthSprites.Health5;
                        break;
                    case 6:
                        _healthControl.sprite = Toolbox.Instance.GameManager.AtlasController.DefaultHealthSprites.Health6;
                        break;
                    case 7:
                        _healthControl.sprite = Toolbox.Instance.GameManager.AtlasController.DefaultHealthSprites.Health7;
                        break;
                    case 8:
                        _healthControl.sprite = Toolbox.Instance.GameManager.AtlasController.DefaultHealthSprites.Health8;
                        break;
                    case 9:
                        _healthControl.sprite = Toolbox.Instance.GameManager.AtlasController.DefaultHealthSprites.Health9;
                        break;
                    case 10:
                        _healthControl.sprite = Toolbox.Instance.GameManager.AtlasController.DefaultHealthSprites.Health10;
                        break;
                }
                _lastDrawnHealth = barDisplay;
                _healthText.text = "" + barDisplay;
            }
        }
        else
        {
            _healthControl.enabled = false;
        }
    }


}
