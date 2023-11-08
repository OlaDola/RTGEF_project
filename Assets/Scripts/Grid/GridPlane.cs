using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using System.IO;

public class GridPlane: MonoBehaviour
{   
    private readonly string SAVE_FOLDER =  Application.dataPath + "/Levels/";
    public int width = 2;
    private int currentWidth;

    public GameObject gridCell;

    private Grid _grid;
    private Camera _cam;
    public Transform _unfocusedCameraPositionTarget;

    private Vector3 _cameraPositionTarget;
    private float _cameraSizeTarget;

    [SerializeField]
    private bool _focused = false;
    [SerializeField]
    private GridCell _focusedCell;

    [SerializeField]
    private Bounds _bounds;

    private bool firstTime = true;
    

    public List<Vector3Int> currentCells;
    // public List<Vector3Int> newCells;
    // public List<Vector3Int> deleteCells;

    public void Awake(){
        _grid = GetComponent<Grid>();
        _cam = Camera.main;
        currentWidth = width;
        // if(GetComponentInParent<GridHandler>().isPlayMode) return;
        Generate();
    }

    // private void OnValidate() => _requiresGeneration = true;

    private void LateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {   
            if(_focused)
                DefucosCamera();
            else
                GetComponentInParent<GridHandler>().Pause();
        }
    }

    public void LoadLevel(string fileName){
        var jsonString = File.ReadAllText(SAVE_FOLDER + fileName);
        var levelData = JsonUtility.FromJson<LevelData>(jsonString);
        width = levelData.width;
        currentWidth = width;
        _grid.cellGap = Vector3.zero;
        var platformPositions = levelData.vector3s;
        _bounds = new Bounds();
        
        foreach(var coordinate in platformPositions){
            var cell = Instantiate(gridCell.transform, transform);
            cell.GetComponent<GridCell>().PlacePlatform();
            cell.name = coordinate.ToString();
            var intCordinate = new Vector3Int(Mathf.RoundToInt(coordinate.x), Mathf.RoundToInt(coordinate.y), Mathf.RoundToInt(coordinate.z));
            cell.transform.position = _grid.CellToWorld(intCordinate);
            currentCells.Add(intCordinate);
        }
        foreach(var cell in currentCells){
            _bounds.Encapsulate(_grid.CellToWorld(cell));
        }
        _bounds.Expand(2);
        SetCamera();
        // Generate();
        // var platformCells = transform.GetComponentInParent<GridConnection>().platformCells;
        // foreach(var cell in platformCells){
        //     transform.Find(cell.position.ToString()).gameObject.GetComponent<GridCell>().PlacePlatform();
        // }
    }

    private void DefucosCamera()
    {
        if (!_focused) return;
        _focused = false;
        _focusedCell = null;
        _cameraPositionTarget = _cam.GetComponent<CameraControler>()._unfocusedCameraPositionTarget.position;
        _cam.transform.DOMove(_cameraPositionTarget, 0.7f);
    }

    public void Play(){
        var platformCells = transform.GetComponentInParent<GridConnection>().platformCells;
        if(platformCells.Count == 0) return;
        GetComponentInParent<GridHandler>().isPlayMode = !GetComponentInParent<GridHandler>().isPlayMode;
        if(!GetComponentInParent<GridHandler>().isPlayMode){
            Generate();
            return;
        }
        _grid.cellGap = Vector3.zero;
        // _bounds = new Bounds();
        var tempCurentcells = new List<Vector3Int>(currentCells);
        foreach(Vector3Int cell in tempCurentcells){
            if(platformCells.Any(x => x.position == (Vector3)cell))
                continue;
            transform.Find(cell.ToString()).gameObject.GetComponent<GridCell>().DeleteCell();
            Destroy(transform.Find(cell.ToString()).gameObject);
            currentCells.Remove(cell);
        }

        foreach(Vector3Int cell in currentCells){
            transform.Find(cell.ToString()).DOMove(cell, 0.5f);
            // _bounds.Encapsulate(_grid.CellToWorld(cell));
        }
        // _bounds.Expand(2);
        SetCamera();
        GetComponentInParent<GridHandler>().playerStartPosition = currentCells[0];
    }

    public void Generate(){
        print("Generating");

        _grid.cellGap = new Vector3(0.1f, 0.1f, 0.1f);
        var coordinates = new List<Vector3Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {   
                var cell = new Vector3Int(x, 0, y);
                coordinates.Add(cell);
            }
        }
        
        var newCells = coordinates.Except(currentCells).ToList();
        var newCellsString = "New cells:"+ string.Join(",", newCells.Select(x => x.ToString()));
        print(newCellsString);
        var deleteCells = currentCells.Except(coordinates).ToList();
        var deleteCellsString = "Delete cells:"+ string.Join(",", deleteCells.Select(x => x.ToString()));
        print(deleteCellsString);
        foreach (Transform child in transform)
        {   
            foreach(var cell in deleteCells){
                if(child.name == cell.ToString())
                    {
                        Destroy(child.gameObject);
                        currentCells.Remove(cell);
                    }
            }
            
        }

        foreach(Vector3Int cell in currentCells){
            transform.Find(cell.ToString()).DOMove(_grid.CellToWorld(cell), 0.5f);
        }

        _bounds = new Bounds();
        
        foreach(var coordinate in newCells){
            var cell = Instantiate(gridCell.transform, transform);
            cell.name = coordinate.ToString();
            cell.transform.position = _grid.CellToWorld(coordinate);
            currentCells.Add(coordinate);
        }

        foreach(var cell in currentCells){
            _bounds.Encapsulate(_grid.CellToWorld(cell));
        }
        
        _bounds.Expand(2);
        SetCamera();
        foreach(Transform child in transform)
            if(_focused && !_focusedCell.gameObject.transform.Equals(child))
                DefucosCamera();
                
        firstTime = false;
    }
    
    private void SetCamera()
    {

        var vertical = _bounds.size.y;
        var horizontal = _bounds.size.x * _cam.pixelHeight / _cam.pixelWidth;
        Vector3 dir = !firstTime ? _cam.gameObject.GetComponent<CameraControler>().CameraDirection : new Vector3(0, 3/(width+2f), -1);
        _cameraPositionTarget = _bounds.center + dir*(width + 2f);

        _cameraSizeTarget = Mathf.Max(horizontal, vertical) * 0.5f;

        if(!_focused)
            _cam.transform.DOMove(_cameraPositionTarget, 0.3f);
        _cam.DOOrthoSize(_cameraSizeTarget, 0.3f);
        _cam.GetComponent<CameraControler>()._unfocusedCameraPositionTarget.position = _cameraPositionTarget;
    }

    public void changeWidth(int newWidth){
        if(currentWidth == newWidth) return;
        width = newWidth;
        currentWidth = newWidth;
        Generate();
    }
    
    public void Save(string fileName){
        var platformCells = transform.GetComponentInParent<GridConnection>().positions;
        var jsonString = JsonUtility.ToJson(new LevelData(width, platformCells));
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }
        File.WriteAllText(SAVE_FOLDER+ fileName +".json", jsonString);
    }

    public bool focused{
        get => _focused;
        set => _focused = value;
    }

    public GridCell focusedCell{
        get => _focusedCell;
        set => _focusedCell = value;
    }
}

public class LevelData
{
    public int width;
    public List<Vector3> vector3s;

    public LevelData(int width, List<Vector3> vector3s)
    {
        this.width = width;
        this.vector3s = vector3s;
    }
}