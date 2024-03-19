using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class ButtonController : MonoBehaviour
{
    public int MouseAction = 0; //0=nothing,1=rotate,2=create,3=remove,4=replace,5=newFloor

    public string ObjectToMove = "";
    public string[] ObjectToCreate;//referenzobjekt(spriteName, goldpreis, moneypreis) wird übergeben und in diesem script zum bekommen der InstantiateDetails benutzt
    
    //methodes
    void Update(){
        Mouse mouse = Mouse.current;
        if (mouse.leftButton.wasPressedThisFrame){//check if mouse left down
            RaycastHit2D[] hitInfo = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);//get all objects

            if(hitInfo.Length!=0){
                MouseHandler(hitInfo);
                //CONTINUE CHECK das kein shop geöffnet ist
                //
                //
                //
            }
        }
    }

    public void MouseHandler(RaycastHit2D[] info){
        string objectName = getPrioritizedObjectName(info);
    
        if(MouseAction==1){//rotate
            if(isFloorChildObject(objectName)){
                RotateFloorChild(objectName);
            }
        }else if(MouseAction==2){//create
            if(isWallObject(objectName)){
                //data übergeben
                string[] details = getObjectToCreateDetails(ObjectToCreate[0]);//holt sich die infos zum generieren
                if(details.Length!=0){
                    GenerateObjectOnWall(details[0], objectName, Int32.Parse(details[1]), float.Parse(details[2]), float.Parse(details[3]));
                }
                MouseAction = 0;//reset
            }
            if(isFloorObject(objectName)){
                string[] details = getObjectToCreateDetails(ObjectToCreate[0]);
                if(details.Length!=0){
                    GenerateObjectOnFloor(details[0], details[1], Int32.Parse(details[2]), float.Parse(details[3]), float.Parse(details[4]), float.Parse(details[5]), float.Parse(details[6]), objectName);
                }
                MouseAction = 0;//reset
            }
        }else if(MouseAction==3){//destroy
            if(isWallObject(objectName)==true){
                DestroyObjectOnWall(objectName);
            }
            if(isFloorChildObject(objectName)==true){
                DestroyFloorChild(objectName);
            }
        }else if(MouseAction==4){//replace
            if(ObjectToMove.Equals("")==false&&isFloorObject(objectName)==true&&isFloorChildObject(ObjectToMove)==true){//im zweiten schritt prüfe ob ein floorChild Object vorhanden ist
                MoveObjectOnFloor(ObjectToMove, objectName);                                                            //und ob das neue Object floor ist
                ObjectToMove = "";
            }else if(isFloorChildObject(objectName)==true){//im ersten schritt prüfe ob ein floorChild Object angeklickt wurde
                ObjectToMove = objectName;
            }else if(ObjectToMove.Equals("")==false&&isWallObject(objectName)==true&&isWallObject(ObjectToMove)==true){
                MoveObjectOnWall(ObjectToMove,objectName);
                ObjectToMove = "";
            }else if(isWallObject(objectName)==true){//im ersten schritt prüfe ob ein wallObject angeklickt wurde
                ObjectToMove = objectName;
            }else{//wenn was anderes angklickt wurde, breche ab
                ObjectToMove = "";
            }
        }else if(MouseAction==5){//new Floor
            string floorObj = getFloor(info);
            if(string.IsNullOrWhiteSpace(floorObj)==false){
                Debug.Log("floorObj: "+floorObj);
                NewFloorSprite("Floor_06_1", 99, floorObj);
            }
        }

        //nach jeder action muss neu gespeichert werden
        SaveAndLoadController.SavePlayerData();
    }

    public void DestroyObjectOnWall(string objectName){
        ObjectController.DestroyObjectOnWall(objectName);//(WallGOName)
    }

    public void DestroyFloorChild(string objectName){
        ObjectController.DestroyFloorChild(objectName);//(FloorChildGOName)
    }

    public void RotateFloorChild(string objectName){
        ObjectController.RotateObjectOnFloor(objectName);//(floorChildGOName)
    }

    public void GenerateObjectOnWall(string spriteName, string wallName, int wallChildLength, float coordCorrectionX, float coordCorrectionY){
        ObjectController.GenerateObjectOnWall(spriteName, wallName, wallChildLength, coordCorrectionX, coordCorrectionY);//(floorChildName,WallName,wallChildLength,coordCorrectionX,coordCorrectionY)
    }
    public void GenerateObjectOnFloor(string type, string spriteName, int price, float coordCoorXA, float coordCoorYA, float coordCoorXB, float coordCoorYB, string wallName){
        ObjectController.GenerateObjectOnFloor(type, spriteName, price, coordCoorXA, coordCoorYA, coordCoorXB, coordCoorYB, wallName);//(type,spriteName,price,coordCoorXA...-coordCoorYB,FloorGameObjectName)
    }

    public void MoveObjectOnWall(string wallNameOld, string wallNameNew){
        ObjectController.MoveObjectOnWall(wallNameOld, wallNameNew);//(curWallName,newWallName)
    }
    public void MoveObjectOnFloor(string objectName, string floorName){
        ObjectController.MoveObjectOnFloor(objectName, floorName);//(floorChildGameObjectName,floorGameObjectName(neuer platz))
    }

    public void NewFloorSprite(string newFloorSpriteName, int floorPrice, string floorGOName){
        ObjectController.NewFloorSprite(newFloorSpriteName, floorPrice, floorGOName);//(newFloorSprite,floorPrice,floorGOName)
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

    public string getFloor(RaycastHit2D[] info){//guckt ob beim raycast ein floor gecatch wurde
        string objectName = null;
        for(int a=0;a<info.Length;a++){
            string[] splitName = info[a].collider.name.Split("-");
            if(splitName.Length==2){
                return info[a].collider.name;
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

    public bool isFloorObject(string objectName){
        string[] splitName = objectName.Split("-");
        if(splitName.Length==2){
            return true;
        }
        return false;
    }

    //holt anhand des spritenamen die details zum instanziieren
    public string[] getObjectToCreateDetails(string objName){
        string[] details = new string[]{};
        if(objName.Equals("Wall_Deko_01_a")){
            //walldeko object        sprite name    length coordCorX coordCorY
            details = new string[]{"Wall_Deko_01_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_02_a")){
            details = new string[]{"Wall_Deko_02_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_03_a")){
            details = new string[]{"Wall_Deko_03_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_04_a")){
            details = new string[]{"Wall_Deko_04_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_05_a")){
            details = new string[]{"Wall_Deko_05_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_06_a")){
            details = new string[]{"Wall_Deko_06_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_07_a")){
            details = new string[]{"Wall_Deko_07_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_08_a")){
            details = new string[]{"Wall_Deko_08_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_09_a")){
            details = new string[]{"Wall_Deko_09_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_10_a")){
            details = new string[]{"Wall_Deko_10_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_11_a")){
            details = new string[]{"Wall_Deko_11_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_12_a")){
            details = new string[]{"Wall_Deko_12_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_13_a")){
            details = new string[]{"Wall_Deko_13_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_14_a")){
            details = new string[]{"Wall_Deko_14_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_15_a")){
            details = new string[]{"Wall_Deko_15_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_01_a_1")){
            details = new string[]{"Wall_Deko_01_1_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_02_a_1")){
            details = new string[]{"Wall_Deko_02_1_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_03_a_1")){
            details = new string[]{"Wall_Deko_03_1_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_04_a_1")){
            details = new string[]{"Wall_Deko_04_1_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_05_a_1")){
            details = new string[]{"Wall_Deko_05_1_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_06_a_1")){
            details = new string[]{"Wall_Deko_06_1_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_07_a_1")){
            details = new string[]{"Wall_Deko_07_1_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_08_a_1")){
            details = new string[]{"Wall_Deko_08_1_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_09_a_1")){
            details = new string[]{"Wall_Deko_09_1_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_10_a_1")){
            details = new string[]{"Wall_Deko_10_1_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_11_a_1")){
            details = new string[]{"Wall_Deko_11_1_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_12_a_1")){
            details = new string[]{"Wall_Deko_12_1_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_13_a_1")){
            details = new string[]{"Wall_Deko_13_1_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_14_a_1")){
            details = new string[]{"Wall_Deko_14_1_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_15_a_1")){
            details = new string[]{"Wall_Deko_15_1_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_16_a_1")){
            details = new string[]{"Wall_Deko_16_1_", "1", "0,75", "1,0"};
        }
        if(objName.Equals("Wall_Deko_17_a_1")){
            details = new string[]{"Wall_Deko_17_1_", "1", "0,75", "1,0"};
        }


        if(objName.Equals("Counter_01_a")){
            //counter object       type      sprite name    price coordCorX coordCorY coordCorX coordCorY
            details = new string[]{"Counter", "Counter_01_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Counter_02_a")){
            details = new string[]{"Counter", "Counter_02_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Counter_03_a")){
            details = new string[]{"Counter", "Counter_03_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Counter_04_a")){
            details = new string[]{"Counter", "Counter_04_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Counter_05_a")){
            details = new string[]{"Counter", "Counter_05_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Counter_06_a")){
            details = new string[]{"Counter", "Counter_06_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Counter_01_a_1")){
            details = new string[]{"Counter", "Counter_01_1_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Counter_02_a_1")){
            details = new string[]{"Counter", "Counter_02_1_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Counter_03_a_1")){
            details = new string[]{"Counter", "Counter_03_1_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Counter_04_a_1")){
            details = new string[]{"Counter", "Counter_04_1_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Counter_05_a_1")){
            details = new string[]{"Counter", "Counter_05_1_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Counter_06_a_1")){
            details = new string[]{"Counter", "Counter_06_1_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        
        return details;
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
