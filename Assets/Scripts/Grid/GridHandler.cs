using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject gridPlane;

    [SerializeField]
    private GameObject Canvas;

    [SerializeField]
    private GameObject PauseMenu;

    [Range(2, 6)]
    public int width;

    public Vector3 middlePoint;

    [SerializeField]
    private GameObject SaveLevelText;

    public bool isPlayMode = false;

    [SerializeField]
    private GameObject player;

    public Vector3 playerStartPosition;

    public bool isPaused = false;

    void Awake()
    {   
        Instantiate(gridPlane.transform, transform);
        Canvas = GameObject.Find("Editor");
        ChangeMiddlePoint();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {   
            if(isPaused) return;
            // if(Camera.main.GetComponent<CameraControler>().orthoOn) return;
            // if(!isPlayMode)
            //     SaveLevelText.SetActive(true);
            ChangeModeButton();
        }
    }

    private void ChangeMiddlePoint()
    {
        var _gridGap = GetComponentInChildren<Grid>().cellGap;
        var _gridSize = GetComponentInChildren<Grid>().cellSize;
        var _gridWidth = (width - 1f) * (_gridGap.x + _gridSize.x);
        middlePoint = new Vector3(_gridWidth / 2f, 0, _gridWidth / 2f);
    }

    public void AddButton(){
        if(isPlayMode) return;
        var focusedCell = GetComponentInChildren<GridPlane>().focusedCell;
        if(focusedCell != null)
            focusedCell.PlacePlatform();
    }

    public void DeleteButton(){
        if(isPlayMode) return;
        var focusedCell = GetComponentInChildren<GridPlane>().focusedCell;
        if(focusedCell != null)
            focusedCell.DeletePlatform();
    }

    public void IncreaseButton(){
        if(isPlayMode) return;
        width++;
        if(width > 6) width = 6;
        var gridPlane = transform.GetChild(0);
        gridPlane.GetComponent<GridPlane>().changeWidth(width);
        ChangeMiddlePoint();
        Camera.main.GetComponent<CameraControler>().ChangeProjectionChangeSize();
    }

    public void DecreaseButton(){
        if(isPlayMode) return;
        width--;
        if(width < 2) width = 2;
        var gridPlane = transform.GetChild(0);
        gridPlane.GetComponent<GridPlane>().changeWidth(width);
        ChangeMiddlePoint();
        Camera.main.GetComponent<CameraControler>().ChangeProjectionChangeSize();
    }

    public void ChangeModeButton(){
        if(isPaused) return;
        GetComponentInChildren<GridPlane>().Play();
        if(isPlayMode)
        {
            Canvas.SetActive(false);
            Instantiate(player, playerStartPosition+new Vector3(0,0.392f,0), Quaternion.identity);
        }
        else
        {   
            Canvas.SetActive(true);
            Destroy(GameObject.Find("Player(Clone)"));
        }
    }



    public void SaveButton(string LevelName){
        if(isPaused) return;
        SaveLevelText.SetActive(false);
        GetComponentInChildren<GridPlane>().Save(LevelName);
        ChangeModeButton();
    }

    public void Pause(){
        isPaused = !isPaused;
        if(isPaused)
            PauseMenu.SetActive(true);
        else
            PauseMenu.SetActive(false);
    }

    public void LoadButton(string LevelName){
        if(isPaused) return;
        GetComponentInChildren<GridPlane>().LoadLevel(LevelName);
        ChangeModeButton();
    }
}
