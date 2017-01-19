using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Algorithms;
using Assets.Scripts;

[ExecuteInEditMode]
public class Level : MonoBehaviour
{
    public WavesController WavesController;
    public LevelController.Levels LevelId;

    public struct TreeNodes
    {
        public Transform Backgrounds;
        public Transform Bullets;
        public Transform Enemies;
        public Transform Towers;
        public Transform Robots;
        public Transform Humans;
        public Transform Map;
        public Transform Level; // Self
        public Transform Paths;
    }


    // State
    private TreeNodes _nodes;

    public TreeNodes Nodes
    {
        get { return _nodes; }
    }

    [Multiline]
    [TextArea(3,10)]
    public string LevelNotes;


    void Awake()
    {
        // Need to be in AWAKE because other components refer to this.
        WavesController = GetComponent<WavesController>();
    }

    // Use this for initialization
    void Start ()
    {

        RebuildTreeNodes();
        PopulateLevel();

        MakeBackgroundSprite();
    }

    private void PopulateLevel()
    {
        LevelData = Toolbox.Instance.GameManager.DataController.LoadLevelData(LevelId);

        // Backgrounds
        var backgroundData = Toolbox.Instance.GameManager.DataController.BackgroundClasses;
        foreach (var backgroundLevelData in LevelData.BackgroundLevelData)
        {
            var backgroundDatum = backgroundData.FirstOrDefault(_ => _.EntityId == backgroundLevelData.EntityId);
            IntantiateBackgroundFromData(backgroundDatum, backgroundLevelData);
        }

        // Robots
        var robotData = Toolbox.Instance.GameManager.DataController.RobotClasses;
        var robotDatum = robotData.FirstOrDefault(_ => _.EntityId == LevelData.RobotLevelData.EntityId);
        InstantiateRobotFromData(robotDatum, LevelData.RobotLevelData); // Looks up correct prefab + sets some data.

        // Towers
        var towerData = Toolbox.Instance.GameManager.DataController.TowerClasses;
        foreach (var towerLevelData in LevelData.TowerLevelData)
        {
            var towerDatum = towerData.FirstOrDefault(_ => _.EntityId == towerLevelData.EntityId);
            InstantiateTowerFromData(towerDatum, towerLevelData); // Looks up correct prefab + sets some data.
        }

        // Humans

        // TBD: Lookup the humans characteristic data.
        var humanData = Toolbox.Instance.GameManager.DataController.HumanClasses;

        foreach (var humanLevelData in LevelData.HumanLevelData)
        {
            var humanDatum = humanData.FirstOrDefault(_ => _.EntityId == humanLevelData.EntityId);

            // Make human from
            InstantiateHumanFromData(humanDatum, humanLevelData); // Looks up correct prefab + sets some data.
        }

        // Paths
        foreach (var pathLevelData in LevelData.PathLevelData)
        {
            InstantiatePathFromData(pathLevelData);
        }

        // Enemies.

        // Enemies get added by the wave controller.


        // TBD: TEMP Toolbox.Instance.GameManager.LevelController.CurrentLevel.GameGrid.InitCellMapFromLevelEntities(this);

    }

    public DataController.LevelData LevelData { get; set; }

    private void IntantiateBackgroundFromData(BackgroundPoco backgroundDatum, DataController.BackgroundLevelData levelDataBackground)
    {      
        // TBD: This prefab should be coming from the ATLAS?
        var prefab = Toolbox.Instance.GameManager.AtlasController.EmptyPrefabs.Block;
        var background = Instantiate<Block>(prefab);
        background.gameObject.name = "Bg" + backgroundDatum.EntityId;
        background.transform.SetParent(Nodes.Backgrounds);
        background.transform.position = GameGrid.MapGridPointToVector(levelDataBackground.BaseLevelData.GridPoint);
        background.transform.localScale = new Vector3(1,1,1);
    }

    private void InstantiateTowerFromData(TowerPoco towerDatum, DataController.TowerLevelData towerLevelData)
    {
        // TBD: This prefab should be coming from the ATLAS?
        var prefab = Toolbox.Instance.GameManager.AtlasController.EmptyPrefabs.Tower;
        var tower = Instantiate<Tower>(prefab);
        tower.gameObject.name = "Tower" + towerDatum.EntityId;
        tower.transform.SetParent(Nodes.Towers);
        tower.transform.position = GameGrid.MapGridPointToVector(towerLevelData.BaseLevelData.GridPoint);
    }

    private void InstantiateHumanFromData(HumanPoco humanDatum, DataController.HumanLevelData humanLevelData)
    {
        // TBD: This prefab should be coming from the ATLAS?
        var prefab = Toolbox.Instance.GameManager.AtlasController.EmptyPrefabs.Human;
        switch (humanDatum.EntityId)
        {
            default:
                prefab = Toolbox.Instance.GameManager.AtlasController.EmptyPrefabs.Human;
                break;
        }
        var human = Instantiate<Human>(prefab);
        human.gameObject.name = "Human1";
        human.transform.SetParent(Nodes.Humans);
        human.transform.position = GameGrid.MapGridPointToVector(humanLevelData.BaseLevelData.GridPoint);
    }

    private void InstantiateRobotFromData(RobotPoco robotDatum, DataController.RobotLevelData robotLevelData)
    {
        // TBD: This prefab should be coming from the AtlasController?
        var prefab = Toolbox.Instance.GameManager.AtlasController.EmptyPrefabs.Robot;
        switch (robotDatum.EntityId)
        {
            default:
                prefab = Toolbox.Instance.GameManager.AtlasController.EmptyPrefabs.Robot;
                break;
        }
        var human = Instantiate<Robot>(prefab);
        human.gameObject.name = "Robot1";
        human.transform.SetParent(Nodes.Level);
        human.transform.position = GameGrid.MapGridPointToVector(robotLevelData.BaseLevelData.GridPoint);
    }

    private void InstantiatePathFromData(DataController.PathLevelData pathLevelData)
    {
        var emptyPrefabs = Toolbox.Instance.GameManager.AtlasController.EmptyPrefabs;
        
        var pathContainer = Instantiate<Path>(emptyPrefabs.PathContainer);
        pathContainer.name = pathLevelData.EntityId;
        pathContainer.transform.SetParent(Nodes.Paths);

        var path = pathContainer.GetComponent<Path>();

        var start = Instantiate<Waypoint>(emptyPrefabs.Start);
        start.transform.SetParent(Nodes.Paths);
        start.transform.SetParent(pathContainer.transform);
        start.transform.position = GameGrid.MapGridPointToVector(pathLevelData.Start);
        start.GetComponent<Waypoint>().WaypointType = Waypoint.WaypointTypes.Start;

        path.StartWaypoint = start;

        int idx = 0;
        var midpoints = new List<Waypoint>();
        foreach (var wp in pathLevelData.Midpoints)
        {
            var mid = Instantiate<Waypoint>(emptyPrefabs.Midpoint);
            mid.transform.SetParent(Nodes.Paths);
            mid.transform.SetParent(pathContainer.transform);
            mid.transform.position = GameGrid.MapGridPointToVector(wp);
            mid.GetComponent<Waypoint>().WaypointIndex = idx++;
            mid.GetComponent<Waypoint>().WaypointType = Waypoint.WaypointTypes.Waypoint;
            midpoints.Add(mid);
        }

        path.MidWaypoints = midpoints;

        var end = Instantiate<Waypoint>(emptyPrefabs.End);
        end.transform.SetParent(Nodes.Paths);
        end.transform.SetParent(pathContainer.transform);
        end.transform.position = GameGrid.MapGridPointToVector(pathLevelData.End);
        end.GetComponent<Waypoint>().WaypointType = Waypoint.WaypointTypes.End;

        path.EndWaypoint = end;

    }





    // Update is called once per frame
    void OnGUI()
        {
        return; // ONGUITEST

        if (Event.current.type.Equals(EventType.Repaint))
            {
                var gameGrid = Toolbox.Instance.GameManager.LevelController.CurrentLevel.GameGrid;

                gameGrid.DrawTextAtVector(this.transform.position + new Vector3(-5.0f, +10.0f, 0f),
                    "" + Toolbox.Instance.GameManager.LevelController.ActiveLevelId.ToString(), Color.black);
            }
        }

    private static List<T> GetSceneObjectsInTranform<T>(Transform t) where T : UnityEngine.Object
    {
        var ret = new List<T>();
        if (t == null)
        {
            return ret;
        }
        foreach (Transform x in t)
        {
            if (t.hideFlags == HideFlags.None)
            {
                var ent = x.gameObject.GetComponent<T>();

                if (ent != null)
                {
                    ret.Add(ent);
                }
            }
        }
        return ret;
    }




    public void RebuildTreeNodes()
    {
        _nodes = new TreeNodes();
        _nodes.Bullets = this.transform.FindChild("Bullets").transform;
        _nodes.Enemies = this.transform.FindChild("Enemies").transform;
        _nodes.Towers = this.transform.FindChild("Towers").transform;
        _nodes.Backgrounds = this.transform.FindChild("Backgrounds").transform;
        _nodes.Humans = this.transform.FindChild("Humans").transform;
        _nodes.Map = this.transform.FindChild("Map").transform;
        _nodes.Level = this.transform;
        _nodes.Paths = this.transform.FindChild("Paths").transform;
    }

    public GameGrid GameGrid
    {
        get
        {
            var ret = this.GetComponent<GameGrid>();
            return ret;

        }
    }



    /// <summary>
    /// TBD-JM: Next two methods are probably really slow.
    /// </summary>
    /// <returns></returns>
    public List<Tower> Towers()
    {
        return GetSceneObjectsInTranform<Tower>(_nodes.Towers);
    }

    public List<Tower> Robot()
    {
        return GetSceneObjectsInTranform<Tower>(_nodes.Robots);
    }

    public List<Block> Backgrounds()
    {
        return GetSceneObjectsInTranform<Block>(_nodes.Backgrounds);
    }

    public List<Human> Humans()
    {
        return GetSceneObjectsInTranform<Human>(_nodes.Humans);
    }

    public List<Path> Paths()
    {
        return GetSceneObjectsInTranform<Path>(_nodes.Paths);
    }

    public List<Enemy> Enemies()
    {
        return GetSceneObjectsInTranform<Enemy>(_nodes.Enemies);
    }

    public void Update()
    {
        /*
        var mapRenderer = _nodes.Map.GetComponent<SpriteRenderer>();

        float scaleX = Mathf.Cos(Time.time) * 0.5F + 1;
        float scaleY = Mathf.Sin(Time.time) * 0.5F + 1;
        mapRenderer.sprite.mainTextureScale = new Vector2(scaleX, scaleY);
        */

    }

    public void MakeBackgroundSprite()
    {
        var mapRenderer = _nodes.Map.GetComponent<SpriteRenderer>();
        var old = mapRenderer.sprite.texture;
        //var rc = BoundsToScreenRect(mapRenderer.bounds);
        //var newText = new Texture2D((int) (rc.width), (int) rc.height, old.format, false);

        //Debug.Assert(newText.width == (int) rc.width);
        //Debug.Assert(newText.height == (int)rc.height);

        var newText = new Texture2D((int) (old.width), (int) old.height, old.format, false);

        var spriteGrounds = Toolbox.Instance.GameManager.AtlasController.GroundSprites;

        var tileDim = spriteGrounds[0].texture.width;
        Color[][] colorCopies = new Color[10][];
        for (int ii = 0; ii < spriteGrounds.Length; ii++)
        {
            colorCopies[ii] = spriteGrounds[ii].texture.GetPixels(0, 0, tileDim, tileDim);
        }

        for (int xx = 0; xx <= newText.width; xx += tileDim)
        {
            for (int yy = 0; yy <= newText.height; yy += tileDim)
            {
                var colors = colorCopies[Random.Range(0, 9)];
                var w = tileDim;
                if (xx + w > newText.width)
                {
                    w = newText.width - xx;
                }
                var h = tileDim;
                if (yy + h > newText.height)
                {
                    h = newText.height - yy;
                }
                newText.SetPixels(xx, yy, w, h, colors);
            }
        }
        newText.Apply();

        Sprite sprite = Sprite.Create(newText,
            new Rect(0, 0, newText.width, newText.height),
            new Vector2(0.5f, 0.5f), mapRenderer.sprite.pixelsPerUnit);

        mapRenderer.sprite = sprite;

#if OLD
        Color[] colors = old.GetPixels(0, 0, (int)(old.width), old.height);
        left.SetPixels(colors);
        left.Apply();
        Debug.Log("Old Bounds: " + GetComponent<Renderer>().sprite.bounds + " Rect: " + GetComponent<Renderer>().sprite.rect + " TexRect: " + GetComponent<Renderer>().sprite.textureRect);
        Debug.Log("Bounds: " + sprite.bounds + " Rect: " + sprite.rect + " TexRect: " + sprite.textureRect);
        GetComponent<Renderer>().sprite = sprite;
#endif
    }

    public Rect BoundsToScreenRect(Bounds bounds)
    {
        // Get mesh origin and farthest extent (this works best with simple convex meshes)
        Vector3 origin = Camera.main.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, 0f));
        Vector3 extent = Camera.main.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, 0f));

        // Create rect in screen space and return - does not account for camera perspective
        return new Rect(origin.x, Screen.height - origin.y, extent.x - origin.x, origin.y - extent.y);
    }

}
