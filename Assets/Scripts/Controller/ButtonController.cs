using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonController : MonoBehaviour
{
    public int MouseAction = 0; //0=nothing,1=rotate,2=create,3=remove,4=replace
    
    //methodes
    void Update(){
        Mouse mouse = Mouse.current;
        if (mouse.leftButton.wasPressedThisFrame){//check if mouse left down
            RaycastHit2D[] hitInfo = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);//get all objects

            if(hitInfo.Length!=0){
                MouseHandler(hitInfo);
            }
        }
    }

    public void MouseHandler(RaycastHit2D[] info){
        string objectName = getPrioritizedObjectName(info);
    
        if(MouseAction==3){
            if(isWallObject(objectName)==true){
                DestroyObjectOnWall(objectName);
            }
            if(isFloorChildObject(objectName)==true){
                DestroyFloorChild(objectName);
            }
        }
        //continue

        //nach jeder action muss neu gespeichert werden
        SaveAndLoadController.SavePlayerData();
    }

    public void DestroyObjectOnWall(string objectName){
        ObjectController.DestroyObjectOnWall(objectName);//(WallGOName)
    }

    public void DestroyFloorChild(string objectName){
        ObjectController.DestroyFloorChild(objectName);//(FloorChildGOName)
    }
    


    //getter
    public string getPrioritizedObjectName(RaycastHit2D[] info){
        string objectName = null;
        for(int a=0;a<info.Length;a++){
            //guck welches Objekt priorisiert wird und als handlung genutzt wird
            //floorChild über Floor/Wall
            objectName = info[0].collider.name;

            string[] splitName = info[a].collider.name.Split("-");
            if(splitName.Length==3){
                if(splitName[2].Equals("Child")){
                    return info[a].collider.name;
                }
            }
        }
        return objectName;
    }

    public bool isWallObject(string objectName){
        string[] splitName = objectName.Split("-");
        if(splitName[splitName.Length-1].Equals("Wall")){
            return true;
        }
        return false;
    }

    public bool isFloorChildObject(string objectName){
        string[] splitName = objectName.Split("-");
        if(splitName[splitName.Length-1].Equals("Child")){
            return true;
        }
        return false;
    }



    //setter
    public void setMouseAction(int action){//set mouseAction active
        if(MouseAction==action){
            MouseAction = 0;
        }else{
            MouseAction = action;
        }
    }
}
