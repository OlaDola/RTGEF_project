using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public enum Direction
{
    Front,
    Back,
    Left,
    Right
}

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float time = 0.25f;

    [SerializeField]
    private Vector3 direction;

    [SerializeField]
    private bool isMoving = false;

    [SerializeField]
    private Direction cameraDirection = Direction.Front;

    [SerializeField]
    private bool isOrtho = false;

    [SerializeField]
    public List<PlatformCell> gridConnection;

    // Update is called once per frame
    void Update()
    {
        if(GameObject.Find("GridHandler").GetComponent<GridHandler>().isPaused) return;
        isOrtho = Camera.main.GetComponent<CameraControler>().orthoOn;
        direction = Camera.main.GetComponent<CameraControler>().Orientation;
        gridConnection = GameObject.Find("GridHandler").GetComponent<GridConnection>().platformCells;
        switch(direction)
        {
            case Vector3 v when v == new Vector3(0, 0, -1):
                cameraDirection = Direction.Front;
                break;
            case Vector3 v when v == new Vector3(0, 0, 1):
                cameraDirection = Direction.Back;
                break;
            case Vector3 v when v == new Vector3(1, 0, 0):
                cameraDirection = Direction.Left;
                break;
            case Vector3 v when v == new Vector3(-1, 0, 0):
                cameraDirection = Direction.Right;
                break;
        }
        if(Input.GetKeyDown(KeyCode.W) && !isMoving){
            var corectDirection = isOrtho ? corectDirectionConversion(KeyCode.W) + new Vector3(0,0.392f,0): CorectPositionPerspective(transform.position-direction);
            StartCoroutine(MovePlayer(corectDirection));
        }
        if(Input.GetKeyDown(KeyCode.S) && !isMoving){
            var corectDirection = isOrtho ? corectDirectionConversion(KeyCode.S) + new Vector3(0,0.392f,0): CorectPositionPerspective(transform.position + direction);
            StartCoroutine(MovePlayer(corectDirection));
        }
        if(Input.GetKeyDown(KeyCode.A) && !isMoving)
        {
            if(direction.x!=0){
                var corectDirection = isOrtho ? corectDirectionConversion(KeyCode.A) + new Vector3(0,0.392f,0): CorectPositionPerspective(transform.position + new Vector3(0, 0, -direction.x));
                StartCoroutine(MovePlayer(corectDirection));
            }
            else{
                var corectDirection = isOrtho ? corectDirectionConversion(KeyCode.A) + new Vector3(0,0.392f,0): CorectPositionPerspective(transform.position + new Vector3(direction.z, 0, 0));
                StartCoroutine(MovePlayer(corectDirection));
            }
        }
        if(Input.GetKeyDown(KeyCode.D) && !isMoving)
        {
             if(direction.x!=0){
                var corectDirection = isOrtho ? corectDirectionConversion(KeyCode.D)+ new Vector3(0,0.392f,0): CorectPositionPerspective(transform.position + new Vector3(0, 0, direction.x));
                StartCoroutine(MovePlayer(corectDirection));
            }
            else{
                var corectDirection = isOrtho ? corectDirectionConversion(KeyCode.D)+ new Vector3(0,0.392f,0): CorectPositionPerspective(transform.position + new Vector3(-direction.z, 0, 0));
                StartCoroutine(MovePlayer(corectDirection));
            }
        }
    }

    private Vector3 CorectPositionPerspective(Vector3 direction)
    {
        foreach(PlatformCell platformCell in gridConnection)
        {
            if(platformCell.position == direction - new Vector3(0,0.392f,0))
            {
                return platformCell.position + new Vector3(0,0.392f,0);
            }
        }
        return transform.position;
    }

    private Vector3 corectDirectionConversion(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.W:
                switch(cameraDirection)
                {
                    case Direction.Front:
                        var (position, connected) = gridConnection.Find(x => x.position == transform.position - new Vector3(0,0.392f,0)).backConnection;
                        var corectPosition = connected ? position : transform.position - new Vector3(0,0.392f,0);
                        return corectPosition;
                    case Direction.Back:
                        var (position1, connected1) = gridConnection.Find(x => x.position == transform.position - new Vector3(0,0.392f,0)).frontConnection;
                        var corectPosition1 = connected1 ? position1 : transform.position - new Vector3(0,0.392f,0);
                        return corectPosition1;
                    case Direction.Left:
                        var (position2, connected2) = gridConnection.Find(x => x.position == transform.position - new Vector3(0,0.392f,0)).leftConnection;
                        var corectPosition2 = connected2 ? position2 : transform.position - new Vector3(0,0.392f,0);
                        return corectPosition2;
                    case Direction.Right:
                        var (position3, connected3) = gridConnection.Find(x => x.position == transform.position - new Vector3(0,0.392f,0)).rightConnection;
                        var corectPosition3 = connected3 ? position3 : transform.position - new Vector3(0,0.392f,0);
                        return corectPosition3;
                    default:
                        return transform.position - new Vector3(0,0.392f,0);
                }
                
            case KeyCode.S:
                switch(cameraDirection){
                    case Direction.Front:
                        var (position, connected) = gridConnection.Find(x => x.position == transform.position - new Vector3(0,0.392f,0)).frontConnection;
                        var corectPosition = connected ? position : transform.position - new Vector3(0,0.392f,0);
                        return corectPosition;
                    case Direction.Back:
                        var (position1, connected1) = gridConnection.Find(x => x.position == transform.position - new Vector3(0,0.392f,0)).backConnection;
                        var corectPosition1 = connected1 ? position1 : transform.position - new Vector3(0,0.392f,0);
                        return corectPosition1;
                    case Direction.Left:
                        var (position2, connected2) = gridConnection.Find(x => x.position == transform.position - new Vector3(0,0.392f,0)).rightConnection;
                        var corectPosition2 = connected2 ? position2 : transform.position - new Vector3(0,0.392f,0);
                        return corectPosition2;
                    case Direction.Right:
                        var (position3, connected3) = gridConnection.Find(x => x.position == transform.position - new Vector3(0,0.392f,0)).leftConnection;
                        var corectPosition3 = connected3 ? position3 : transform.position - new Vector3(0,0.392f,0);
                        return corectPosition3;
                    default:
                        return transform.position - new Vector3(0,0.392f,0);
                }
            case KeyCode.A:
                switch(cameraDirection){
                    case Direction.Front:
                        var (position, connected) = gridConnection.Find(x => x.position == transform.position - new Vector3(0,0.392f,0)).leftConnection;
                        var corectPosition = connected ? position : transform.position - new Vector3(0,0.392f,0);
                        return corectPosition;
                    case Direction.Back:
                        var (position1, connected1) = gridConnection.Find(x => x.position == transform.position - new Vector3(0,0.392f,0)).rightConnection;
                        var corectPosition1 = connected1 ? position1 : transform.position - new Vector3(0,0.392f,0);
                        return corectPosition1;
                    case Direction.Left:
                        var (position2, connected2) = gridConnection.Find(x => x.position == transform.position - new Vector3(0,0.392f,0)).frontConnection;
                        var corectPosition2 = connected2 ? position2 : transform.position - new Vector3(0,0.392f,0);
                        return corectPosition2;
                    case Direction.Right:
                        var (position3, connected3) = gridConnection.Find(x => x.position == transform.position - new Vector3(0,0.392f,0)).backConnection;
                        var corectPosition3 = connected3 ? position3 : transform.position - new Vector3(0,0.392f,0);
                        return corectPosition3;
                    default:
                        return transform.position - new Vector3(0,0.392f,0);
                }
            case KeyCode.D:
                switch(cameraDirection){
                    case Direction.Front:
                        var (position, connected) = gridConnection.Find(x => x.position == transform.position - new Vector3(0,0.392f,0)).rightConnection;
                        var corectPosition = connected ? position : transform.position - new Vector3(0,0.392f,0);
                        return corectPosition;
                    case Direction.Back:
                        var (position1, connected1) = gridConnection.Find(x => x.position == transform.position - new Vector3(0,0.392f,0)).leftConnection;
                        var corectPosition1 = connected1 ? position1 : transform.position - new Vector3(0,0.392f,0);
                        return corectPosition1;
                    case Direction.Left:
                        var (position2, connected2) = gridConnection.Find(x => x.position == transform.position - new Vector3(0,0.392f,0)).backConnection;
                        var corectPosition2 = connected2 ? position2 : transform.position - new Vector3(0,0.392f,0);
                        return corectPosition2;
                    case Direction.Right:
                        var (position3, connected3) = gridConnection.Find(x => x.position == transform.position - new Vector3(0,0.392f,0)).frontConnection;
                        var corectPosition3 = connected3 ? position3 : transform.position - new Vector3(0,0.392f,0);
                        return corectPosition3;
                    default:
                        return transform.position - new Vector3(0,0.392f,0);
                }
            default:
                return transform.position - new Vector3(0,0.392f,0);
        }

    }

    private IEnumerator MovePlayer(Vector3 direction)
    {   
        isMoving = true;
        transform.DOMove(direction, time);
        yield return new WaitForSeconds(time);
        isMoving = false;
    }
}
