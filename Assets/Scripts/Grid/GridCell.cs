using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class GridCell: MonoBehaviour
{
    [SerializeField]
    private GameObject CenterPrefab;

    [SerializeField]
    private GameObject PlatformPrefab;

    private GameObject centerObject;
    private GameObject platformObject;

    private Grid _grid;
    private GridHandler gridHandler;

    private bool placed = false;
    private bool focused;
    [SerializeField]
    private GridCell _focusedCell;
    private GridConnection _gridConnection;

    public void Awake(){
        centerObject = Instantiate(CenterPrefab, transform);
        _grid = GetComponentInParent<Grid>();
        _gridConnection = GameObject.Find("GridHandler").GetComponent<GridConnection>();
        gridHandler = GameObject.Find("GridHandler").GetComponent<GridHandler>();
    }

    public void Update(){
        if (EventSystem.current.IsPointerOverGameObject()){
            OnMouseExit();
            return;
        }
        focused = GetComponentInParent<GridPlane>().focused;
        _focusedCell = GetComponentInParent<GridPlane>().focusedCell;
    }

    public void FocusCamera(){
        var camera = Camera.main;
        if(camera.gameObject.GetComponent<CameraControler>().orthoOn) return;
        print("Focusing camera");
        
        
        Vector3 dir = camera.gameObject.GetComponent<CameraControler>().CameraDirection;

        Vector3 focusedLocation = centerObject.transform.position + dir*3f;
        camera.transform.DOMove(focusedLocation, 0.5f);
    }

    public void PlacePlatform(){
        if(placed) return;
        centerObject.GetComponent<Animator>().SetBool("Destroy", true);
        platformObject = Instantiate(PlatformPrefab, transform);
        centerObject.SetActive(false);
        placed = true;
        transform.localScale = platformObject.transform.localScale;
        var localScale = new Vector3(centerObject.transform.localScale.x, centerObject.transform.localScale.y/1.1f, centerObject.transform.localScale.z);
        platformObject.transform.localScale = localScale;
        setPlatform();
    }

    public void DeletePlatform(){
        removePlatform();
        if(!placed) return;
        platformObject.GetComponent<Animator>().SetBool("Destroy", true);
        Destroy(platformObject);
        centerObject.GetComponent<Animator>().SetBool("Destroy", false);
        centerObject.SetActive(true);
        placed = false;
        transform.localScale = centerObject.transform.localScale;
    }

    public void DeleteCell(){
        centerObject.GetComponent<Animator>().SetBool("Destroy", true);
    }

    public void setPlatform(){

        float cellGap = _grid.cellGap.x;
        Vector3 position = new Vector3(
                    transform.position.x - (int)transform.position.x * cellGap,
                    transform.position.y - (int)transform.position.y * cellGap,
                    transform.position.z - (int)transform.position.z * cellGap);
        PlatformCell platformCell = new PlatformCell(position);
       _gridConnection.AddPlatformCell(platformCell);
    }

    public void removePlatform(){
        float cellGap = _grid.cellGap.x;
        Vector3 position = new Vector3(
                    transform.position.x - (int)transform.position.x * cellGap,
                    transform.position.y - (int)transform.position.y * cellGap,
                    transform.position.z - (int)transform.position.z * cellGap);
        PlatformCell platformCell = new PlatformCell(position);
        _gridConnection.RemovePlatformCell(platformCell);
    }

//////////////////////////////  EVENTS  //////////////////////////////
    public void OnMouseDown(){
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if(focused && _focusedCell.gameObject.Equals(gameObject)) return;
        if(Input.GetMouseButtonDown(0)){
            GetComponentInParent<GridPlane>().focused = true;
            GetComponentInParent<GridPlane>().focusedCell = this;
            FocusCamera();
        }
    }   

    public void OnMouseEnter(){
        var highlightedObject = placed ? platformObject : centerObject;
        var material = highlightedObject.GetComponent<MeshRenderer>().materials[1];
        material.SetFloat("_Scale", 1.06f);
    }

    public void OnMouseExit(){
        var highlightedObject = placed ? platformObject : centerObject;
        var material = highlightedObject.GetComponent<MeshRenderer>().materials[1];
        material.SetFloat("_Scale", 1f);
    }

}
