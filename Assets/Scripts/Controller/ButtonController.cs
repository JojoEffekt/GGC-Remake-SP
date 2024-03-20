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
            string[] details = getObjectToCreateDetails(ObjectToCreate[0]);//holt sich die infos zum generieren

            //guckt welcher type Generiert werden soll
            if(details[0].Equals("Floor")){//wenn floor 
                string floorObj = getFloor(info);
                if(string.IsNullOrWhiteSpace(floorObj)==false){
                    NewFloorSprite(details[1], Int32.Parse(details[2]), floorObj);
                }
            }else if(details[0].Equals("Door")){//wenn door (muss nur sprite austauschen)
            //test
                ObjectController.ChangeDoorOnWall(details[1]);


                
            }else if(isWallObject(objectName)){
                if(details.Length!=0){
                    GenerateObjectOnWall(details[0], objectName, Int32.Parse(details[1]), float.Parse(details[2]), float.Parse(details[3]));
                }
            }else if(isFloorObject(objectName)){
                if(details.Length!=0){
                    GenerateObjectOnFloor(details[0], details[1], Int32.Parse(details[2]), float.Parse(details[3]), float.Parse(details[4]), float.Parse(details[5]), float.Parse(details[6]), objectName);
                }
            }
            //CONTINUE
            //bei tür muss die tür mir der aktuellen ausgetauscht werden
            //change wallsprite muss mehtode erstellt werden

            MouseAction = 0;//reset

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



        if(objName.Equals("Chair_18_a")){
            //counter object       type      sprite name    price coordCorX coordCorY coordCorX coordCorY
            details = new string[]{"Chair", "Chair_18_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_01_a")){
            details = new string[]{"Chair", "Chair_01_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_02_a")){
            details = new string[]{"Chair", "Chair_02_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_03_a")){
            details = new string[]{"Chair", "Chair_03_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_04_a")){
            details = new string[]{"Chair", "Chair_04_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_05_a")){
            details = new string[]{"Chair", "Chair_05_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_06_a")){
            details = new string[]{"Chair", "Chair_06_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_12_a")){
            details = new string[]{"Chair", "Chair_12_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_13_a")){
            details = new string[]{"Chair", "Chair_13_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_14_a")){
            details = new string[]{"Chair", "Chair_14_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_15_a")){
            details = new string[]{"Chair", "Chair_15_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_16_a")){
            details = new string[]{"Chair", "Chair_16_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_17_a")){
            details = new string[]{"Chair", "Chair_17_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_01_a_1")){
            details = new string[]{"Chair", "Chair_01_1_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_02_a_1")){
            details = new string[]{"Chair", "Chair_02_1_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_03_a_1")){
            details = new string[]{"Chair", "Chair_03_1_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_04_a_1")){
            details = new string[]{"Chair", "Chair_04_1_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_05_a_1")){
            details = new string[]{"Chair", "Chair_05_1_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_06_a_1")){
            details = new string[]{"Chair", "Chair_06_1_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_07_a_1")){
            details = new string[]{"Chair", "Chair_07_1_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_08_a_1")){
            details = new string[]{"Chair", "Chair_08_1_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_09_a_1")){
            details = new string[]{"Chair", "Chair_09_1_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_10_a_1")){
            details = new string[]{"Chair", "Chair_10_1_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_11_a_1")){
            details = new string[]{"Chair", "Chair_11_1_a", "10", "0,0", "0,75", "0,0", "0,75"};
        }



        if(objName.Equals("Deko_01_a")){
            details = new string[]{"Deko", "Deko_01_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_02_a")){
            details = new string[]{"Deko", "Deko_02_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_03_a")){
            details = new string[]{"Deko", "Deko_03_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_04_a")){
            details = new string[]{"Deko", "Deko_04_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_05_a")){
            details = new string[]{"Deko", "Deko_05_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_06_a")){
            details = new string[]{"Deko", "Deko_06_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_07_a")){
            details = new string[]{"Deko", "Deko_07_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_08_a")){
            details = new string[]{"Deko", "Deko_08_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_09_a")){
            details = new string[]{"Deko", "Deko_09_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_10_a")){
            details = new string[]{"Deko", "Deko_10_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_11_a")){
            details = new string[]{"Deko", "Deko_11_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_12_a")){
            details = new string[]{"Deko", "Deko_12_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_13_a")){
            details = new string[]{"Deko", "Deko_13_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_14_a")){
            details = new string[]{"Deko", "Deko_14_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_15_a")){
            details = new string[]{"Deko", "Deko_15_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_01_a_1")){
            details = new string[]{"Deko", "Deko_01_1_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_02_a_1")){
            details = new string[]{"Deko", "Deko_02_1_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_03_a_1")){
            details = new string[]{"Deko", "Deko_03_1_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_04_a_1")){
            details = new string[]{"Deko", "Deko_04_1_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_05_a_1")){
            details = new string[]{"Deko", "Deko_05_1_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_06_a_1")){
            details = new string[]{"Deko", "Deko_06_1_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_07_a_1")){
            details = new string[]{"Deko", "Deko_07_1_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_08_a_1")){
            details = new string[]{"Deko", "Deko_08_1_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_09_a_1")){
            details = new string[]{"Deko", "Deko_09_1_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_10_a_1")){
            details = new string[]{"Deko", "Deko_10_1_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_11_a_1")){
            details = new string[]{"Deko", "Deko_11_1_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_12_a_1")){
            details = new string[]{"Deko", "Deko_12_1_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_13_a_1")){
            details = new string[]{"Deko", "Deko_13_1_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_14_a_1")){
            details = new string[]{"Deko", "Deko_14_1_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_15_a_1")){
            details = new string[]{"Deko", "Deko_15_1_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_16_a_1")){
            details = new string[]{"Deko", "Deko_16_1_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_17_a_1")){
            details = new string[]{"Deko", "Deko_17_1_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_18_a_1")){
            details = new string[]{"Deko", "Deko_18_1_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_19_a_1")){
            details = new string[]{"Deko", "Deko_19_1_a", "10", "0,0", "1,75", "0,0", "1,75"};
        }


        if(objName.Equals("Floor_01")){
            details = new string[]{"Floor", "Floor_01", "99"};
        }
        if(objName.Equals("Floor_02")){
            details = new string[]{"Floor", "Floor_02", "99"};
        }
        if(objName.Equals("Floor_03")){
            details = new string[]{"Floor", "Floor_03", "99"};
        }
        if(objName.Equals("Floor_04")){
            details = new string[]{"Floor", "Floor_04", "99"};
        }
        if(objName.Equals("Floor_05")){
            details = new string[]{"Floor", "Floor_05", "99"};
        }
        if(objName.Equals("Floor_06")){
            details = new string[]{"Floor", "Floor_06", "99"};
        }
        if(objName.Equals("Floor_07")){
            details = new string[]{"Floor", "Floor_07", "99"};
        }
        if(objName.Equals("Floor_08")){
            details = new string[]{"Floor", "Floor_08", "99"};
        }
        if(objName.Equals("Floor_09")){
            details = new string[]{"Floor", "Floor_09", "99"};
        }
        if(objName.Equals("Floor_10")){
            details = new string[]{"Floor", "Floor_10", "99"};
        }
        if(objName.Equals("Floor_11")){
            details = new string[]{"Floor", "Floor_11", "99"};
        }
        if(objName.Equals("Floor_01_1")){
            details = new string[]{"Floor", "Floor_01_1", "99"};
        }
        if(objName.Equals("Floor_02_1")){
            details = new string[]{"Floor", "Floor_02_1", "99"};
        }
        if(objName.Equals("Floor_03_1")){
            details = new string[]{"Floor", "Floor_03_1", "99"};
        }
        if(objName.Equals("Floor_04_1")){
            details = new string[]{"Floor", "Floor_04_1", "99"};
        }
        if(objName.Equals("Floor_05_1")){
            details = new string[]{"Floor", "Floor_05_1", "99"};
        }
        if(objName.Equals("Floor_06_1")){
            details = new string[]{"Floor", "Floor_06_1", "99"};
        }
        if(objName.Equals("Floor_07_1")){
            details = new string[]{"Floor", "Floor_07_1", "99"};
        }
        if(objName.Equals("Floor_08_1")){
            details = new string[]{"Floor", "Floor_08_1", "99"};
        }
        if(objName.Equals("Floor_09_1")){
            details = new string[]{"Floor", "Floor_09_1", "99"};
        }
        if(objName.Equals("Floor_10_1")){
            details = new string[]{"Floor", "Floor_10_1", "99"};
        }
        if(objName.Equals("Floor_11_1")){
            details = new string[]{"Floor", "Floor_11_1", "99"};
        }
        if(objName.Equals("Floor_12_1")){
            details = new string[]{"Floor", "Floor_12_1", "99"};
        }
        if(objName.Equals("Floor_13_1")){
            details = new string[]{"Floor", "Floor_13_1", "99"};
        }
        if(objName.Equals("Floor_14_1")){
            details = new string[]{"Floor", "Floor_14_1", "99"};
        }
        if(objName.Equals("Floor_15_1")){
            details = new string[]{"Floor", "Floor_15_1", "99"};
        }
        if(objName.Equals("Floor_16_1")){
            details = new string[]{"Floor", "Floor_16_1", "99"};
        }
        if(objName.Equals("Floor_17_1")){
            details = new string[]{"Floor", "Floor_17_1", "99"};
        }
        if(objName.Equals("Floor_18_1")){
            details = new string[]{"Floor", "Floor_18_1", "99"};
        }



        if(objName.Equals("Fridge_03_a")){
            details = new string[]{"Fridge", "Fridge_03_a", "10", "0,0", "2,15", "0,0", "2,15"};
        }
        if(objName.Equals("Fridge_01_a_1")){
            details = new string[]{"Fridge", "Fridge_01_1_a", "10", "0,0", "2,15", "0,0", "2,15"};
        }
        if(objName.Equals("Fridge_02_a_1")){
            details = new string[]{"Fridge", "Fridge_02_1_a", "10", "0,0", "2,15", "0,0", "2,15"};
        }
        if(objName.Equals("Fridge_04_a_1")){
            details = new string[]{"Fridge", "Fridge_04_1_a", "10", "0,0", "2,15", "0,0", "2,15"};
        }
        if(objName.Equals("Fridge_05_a_1")){
            details = new string[]{"Fridge", "Fridge_05_1_a", "10", "0,0", "2,15", "0,0", "2,15"};
        }
        if(objName.Equals("Fridge_06_a_1")){
            details = new string[]{"Fridge", "Fridge_06_1_a", "10", "0,0", "2,15", "0,0", "2,15"};
        }
        if(objName.Equals("Fridge_07_a_1")){
            details = new string[]{"Fridge", "Fridge_07_1_a", "10", "0,0", "2,15", "0,0", "2,15"};
        }
        if(objName.Equals("Fridge_08_a_1")){
            details = new string[]{"Fridge", "Fridge_08_1_a", "10", "0,0", "2,15", "0,0", "2,15"};
        }



        if(objName.Equals("Oven_01_a")){
            details = new string[]{"Oven", "Oven_01_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Oven_07_a")){
            details = new string[]{"Oven", "Oven_07_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Oven_01_a_1")){
            details = new string[]{"Oven", "Oven_01_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Oven_02_a_1")){
            details = new string[]{"Oven", "Oven_02_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Oven_03_a_1")){
            details = new string[]{"Oven", "Oven_03_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Oven_04_a_1")){
            details = new string[]{"Oven", "Oven_04_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Oven_05_a_1")){
            details = new string[]{"Oven", "Oven_05_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Oven_06_a_1")){
            details = new string[]{"Oven", "Oven_06_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }



        if(objName.Equals("Shlushi_a")){
            details = new string[]{"Slushi", "Shlushi_01_a", "10", "0,0", "2,15", "0,0", "2,15"};
        }



        if(objName.Equals("Table_01_a")){
            details = new string[]{"Table", "Table_01_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_02_a")){
            details = new string[]{"Table", "Table_02_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_03_a")){
            details = new string[]{"Table", "Table_03_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_04_a")){
            details = new string[]{"Table", "Table_04_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_05_a")){
            details = new string[]{"Table", "Table_05_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_06_a")){
            details = new string[]{"Table", "Table_06_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_07_a")){
            details = new string[]{"Table", "Table_07_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_08_a")){
            details = new string[]{"Table", "Table_08_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_09_a")){
            details = new string[]{"Table", "Table_09_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_10_a")){
            details = new string[]{"Table", "Table_10_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_11_a")){
            details = new string[]{"Table", "Table_11_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_12_a")){
            details = new string[]{"Table", "Table_12_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_01_a_1")){
            details = new string[]{"Table", "Table_01_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_02_a_1")){
            details = new string[]{"Table", "Table_02_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_03_a_1")){
            details = new string[]{"Table", "Table_03_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_04_a_1")){
            details = new string[]{"Table", "Table_04_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_05_a_1")){
            details = new string[]{"Table", "Table_05_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_06_a_1")){
            details = new string[]{"Table", "Table_06_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_07_a_1")){
            details = new string[]{"Table", "Table_07_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_08_a_1")){
            details = new string[]{"Table", "Table_08_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_09_a_1")){
            details = new string[]{"Table", "Table_09_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_10_a_1")){
            details = new string[]{"Table", "Table_10_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_11_a_1")){
            details = new string[]{"Table", "Table_11_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }



        if(objName.Equals("Door_01_a")){
            details = new string[]{"Door", "Door_01_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Door_02_a")){
            details = new string[]{"Door", "Door_02_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Door_01_a_1")){
            details = new string[]{"Door", "Door_01_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Door_02_a_1")){
            details = new string[]{"Door", "Door_02_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Door_03_a_1")){
            details = new string[]{"Door", "Door_03_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Door_04_a_1")){
            details = new string[]{"Door", "Door_04_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Door_05_a_1")){
            details = new string[]{"Door", "Door_05_1_a", "10", "0,0", "0,85", "0,0", "0,85"};
        }
        /*
        ObjectList.Add(new Object("Wall_01_a", 900, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_02_a", 900, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_03_a", 800, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_04_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_05_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_06_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_07_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_08_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_09_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_10_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_11_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_12_a", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_13_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_14_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_15_a", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_16_a", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_17_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_01_a_1", 1500, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_02_a_1", 1000, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_03_a_1", 2500, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_04_a_1", 7500, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_05_a_1", 7000, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_06_a_1", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_07_a_1", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_08_a_1", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_09_a_1", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_10_a_1", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_11_a_1", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_12_a_1", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_13_a_1", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_14_a_1", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_15_a_1", 6000, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_16_a_1", 7000, 0, 0, true, 2, 23));
        */

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
