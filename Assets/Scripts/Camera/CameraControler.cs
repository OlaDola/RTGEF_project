using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    public bool orthoOn = false;

    [SerializeField]
    private Vector3 _previuscameraPositionTarget;

    public Transform _unfocusedCameraPositionTarget;
    
    public Vector3 CameraDirection;
    public Vector3 Orientation;

    private GridHandler _gridHandler;
    [SerializeField]
    private bool isOnStartMenu = true;

    public GameObject GridHandler;

    // private void Awake(){
    //     GridHandler.SetActive(true);
    //     _gridHandler = GameObject.Find("GridHandler").GetComponent<GridHandler>();
    // }

    public void LevelEditor()
    {
        isOnStartMenu = false;
        GridHandler.SetActive(true);
        _gridHandler = GameObject.Find("GridHandler").GetComponent<GridHandler>();
    }

    
    void LateUpdate(){
        OrientationSnap();
        if(isOnStartMenu) return;
        var gridPlane = _gridHandler.transform.GetComponentInChildren<GridPlane>();
        if(!gridPlane.focused && !orthoOn)
        {
            _unfocusedCameraPositionTarget.position = Camera.main.transform.position;
            _unfocusedCameraPositionTarget.rotation = Camera.main.transform.rotation;
        }
        CameraDirection = (_unfocusedCameraPositionTarget.position - _gridHandler.middlePoint).normalized;
        if(Input.GetKeyDown(KeyCode.Tab))
            ChangeProjection();
        if(!orthoOn)
            HandlePerspectiveRotation();
    }

    private void OrientationSnap()
    {
        float min = Mathf.Infinity;
        Vector3[] orientations = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        foreach (Vector3 orientation in orientations)
        {
            float distance = Vector3.Distance(CameraDirection, orientation);
            if (distance < min)
            {
                min = distance;
                Orientation = orientation;
            }
        }
    }

    public void ChangeProjectionChangeSize(){
        GetProjectionMatrices(out Matrix4x4 ortho, out Matrix4x4 perspective);
        var blender = GetComponent<MatrixBlender>();
        if(orthoOn){
            Camera.main.transform.DOMove(_gridHandler.middlePoint+Orientation*_gridHandler.width, 0.5f);
            Camera.main.transform.DORotateQuaternion(Quaternion.LookRotation(-Orientation), 0.5f);
            blender.BlendToMatrix(ortho, 0.5f);
            _gridHandler.transform.GetComponentInChildren<GridConnection>().UpdateConnections();
        }    
        else{
            Camera.main.transform.DOMove(_unfocusedCameraPositionTarget.position, 0.5f);
            Camera.main.transform.DORotateQuaternion(_unfocusedCameraPositionTarget.rotation, 0.5f);
            blender.BlendToMatrix(perspective, 0.5f);
            _gridHandler.transform.GetComponentInChildren<GridConnection>().DeleteConections();
        }
    }

    private void ChangeProjection()
    {
        if(_gridHandler.isPaused) return;
        orthoOn = !orthoOn;
        ChangeProjectionChangeSize();
    }

    private void GetProjectionMatrices(out Matrix4x4 ortho, out Matrix4x4 perspective)
    {
        var orthoSize = (_gridHandler.width + 1f)*0.5f;
        var aspect = (float)Screen.width / (float)Screen.height;
        var fov = Camera.main.fieldOfView;
        var near = Camera.main.nearClipPlane;
        var far = orthoSize * 5f;
        ortho = Matrix4x4.Ortho(-orthoSize* aspect, orthoSize * aspect, -orthoSize, orthoSize, near, far);
        perspective = Matrix4x4.Perspective(fov, aspect, near, far);
    }

    private void HandlePerspectiveRotation(){
        if(_gridHandler.isPaused) return;
        if(Input.GetMouseButtonDown(1))
            _previuscameraPositionTarget = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        
        if(Input.GetMouseButton(1)){
            Vector3 direction = _previuscameraPositionTarget - Camera.main.ScreenToViewportPoint(Input.mousePosition);
            var gridPlane = _gridHandler.transform.GetComponentInChildren<GridPlane>();
            var rotate_point = gridPlane.focused && !orthoOn ? gridPlane.focusedCell.transform.position : _gridHandler.middlePoint;
            
            Camera.main.transform.RotateAround(rotate_point, Vector3.up, (-direction.x) * 180 / _speed);
            _unfocusedCameraPositionTarget.RotateAround(_gridHandler.middlePoint, Vector3.up, (-direction.x) * 180 / _speed);
            
        }
    }
   
}
