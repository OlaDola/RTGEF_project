
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class GridConnection: MonoBehaviour{
    public List<PlatformCell> platformCells = new();
    [SerializeField]
    private Vector3 cameraDirection;

    public List<Vector3> positions = new();
    public void LateUpdate(){
        cameraDirection = Camera.main.GetComponent<CameraControler>().Orientation;
    }

    public void AddPlatformCell(PlatformCell platformCell){
        platformCells.Add(platformCell);
        foreach(PlatformCell platformCell1 in platformCells){
            print(platformCell1.position);
        }
        positions.Add(platformCell.position);
    }

    public void RemovePlatformCell(PlatformCell platformCell){
        foreach(PlatformCell platformCell1 in platformCells){
            print(platformCell1.position);
        }
        foreach(PlatformCell platformCell1 in platformCells){
            if(platformCell1.position == platformCell.position){
                platformCells.Remove(platformCell1);
                positions.Remove(platformCell1.position);
                break;
            }
        }
    }

    public void UpdateConnections(){
        List<Vector3> positions = new ();
        var gridHandler = GameObject.Find("GridHandler").GetComponent<GridHandler>();
        var defaultPosition = cameraDirection.x > 0 || cameraDirection.z > 0 ?new Vector3(-1f, 0 , -1f) : new Vector3(gridHandler.width, 0f , gridHandler.width); 
        foreach(PlatformCell platformCell in platformCells){
            platformCell.frontConnection = (defaultPosition, false);
            platformCell.backConnection = (defaultPosition, false);
            platformCell.leftConnection = (defaultPosition, false);
            platformCell.rightConnection = (defaultPosition, false);
        }
        foreach(PlatformCell platformCell1 in platformCells){
            foreach(PlatformCell platformCell2 in platformCells){
                // if (positions.Contains(platformCell2.position)) continue;
                if (platformCell1 == platformCell2) continue;
                if(cameraDirection.x != 0 ){
                    if(platformCell1.position.z - platformCell2.position.z <0 && platformCell1.position.z - platformCell2.position.z > -1 && (int)(platformCell1.position.z - platformCell2.position.z) -1 == -1){
                        var max = platformCell1.backConnection.position.x;
                        var min = platformCell1.backConnection.position.x;
                        
                        if(cameraDirection.x > 0){
                            if(platformCell1.position.x > platformCell2.frontConnection.position.x)
                                platformCell2.frontConnection = (platformCell1.position, true);
                            if(platformCell2.position.x > max)
                                platformCell1.backConnection = (platformCell2.position, true);
                        }

                        if(cameraDirection.x < 0){
                            if(platformCell1.position.x < platformCell2.frontConnection.position.x)
                                platformCell2.frontConnection = (platformCell1.position, true);
                            if(platformCell2.position.x < min)
                                platformCell1.backConnection = (platformCell2.position, true);
                        }
                    }
                    if((int)(platformCell1.position.z - platformCell2.position.z) == 1){
                        var max = platformCell1.frontConnection.position.x;
                        var min = platformCell1.frontConnection.position.x;

                        if(cameraDirection.x > 0){
                            if(platformCell1.position.x > platformCell2.backConnection.position.x)
                                platformCell2.backConnection = (platformCell1.position, true);
                            if(platformCell2.position.x > max)
                                platformCell1.frontConnection = (platformCell2.position, true);
                        }

                        if(cameraDirection.x < 0){
                            if(platformCell1.position.x < platformCell2.backConnection.position.x)
                                platformCell2.backConnection = (platformCell1.position, true);
                            if(platformCell2.position.x < min)
                                platformCell1.frontConnection = (platformCell2.position, true);
                        }
                    }
                }
                if(cameraDirection.z != 0){
                    
                     if(platformCell1.position.x - platformCell2.position.x<0 && platformCell1.position.x - platformCell2.position.x>-1 && (int)(platformCell1.position.x - platformCell2.position.x) -1 == -1){
                        var max = platformCell1.rightConnection.position.z;
                        var min = platformCell1.rightConnection.position.z;

                        if(cameraDirection.z > 0){
                            if(platformCell1.position.z > platformCell2.leftConnection.position.z)
                                platformCell2.leftConnection = (platformCell1.position, true);
                            if(platformCell2.position.z > max)
                                platformCell1.rightConnection = (platformCell2.position, true);
                        }

                        if(cameraDirection.z < 0){
                            if(platformCell1.position.z < platformCell2.leftConnection.position.z)
                                platformCell2.leftConnection = (platformCell1.position, true);
                            if(platformCell2.position.z < min)
                                platformCell1.rightConnection = (platformCell2.position, true);
                        }
                    }
                    if((int)(platformCell1.position.x - platformCell2.position.x) == 1){
                        var max = platformCell1.leftConnection.position.z;
                        var min = platformCell1.leftConnection.position.z;
                        
                        if(cameraDirection.z > 0){
                            if(platformCell1.position.z > platformCell2.rightConnection.position.z)
                                platformCell2.rightConnection = (platformCell1.position, true);
                            if(platformCell2.position.z > max)
                                platformCell1.leftConnection = (platformCell2.position, true);
                        }

                        if(cameraDirection.z < 0){
                            if(platformCell1.position.z < platformCell2.rightConnection.position.z)
                                platformCell2.rightConnection = (platformCell1.position, true);
                            if(platformCell2.position.z < min)
                                platformCell1.leftConnection = (platformCell2.position, true);
                        }                    
                    }
                }
            }
            positions.Add(platformCell1.position);
        }
        foreach(PlatformCell platformCell in platformCells){
            print("Position:"+ platformCell.position);
            print("Front:"+ platformCell.frontConnection);
            print("Back:"+ platformCell.backConnection);
            print("Left:"+ platformCell.leftConnection);
            print("Right:"+ platformCell.rightConnection);
        }
    }

    public void DeleteConections(){
        foreach(PlatformCell platformCell in platformCells){
            platformCell.frontConnection = (platformCell.position, false);
            platformCell.backConnection = (platformCell.position, false);
            platformCell.leftConnection = (platformCell.position, false);
            platformCell.rightConnection = (platformCell.position, false);
        }
    }

}



public class PlatformCell {

    public PlatformCell(Vector3 position)
    {
        this.position = position;
        frontConnection = (position, false);
        backConnection = (position, false);
        leftConnection = (position, false);
        rightConnection = (position, false);
    }

    public Vector3 position{ get;}
    public (Vector3 position, bool connected) frontConnection{ get; set;}
    public (Vector3 position, bool connected) backConnection{ get; set;}
    public (Vector3 position, bool connected) leftConnection{ get; set;}
    public (Vector3 position, bool connected) rightConnection{ get; set;}
    
}

