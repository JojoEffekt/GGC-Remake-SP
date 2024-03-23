using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class ButtonController : MonoBehaviour
{
    public int MouseAction = 0; //0=nothing,1=rotate,2=create,3=remove,4=replace

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
    
        //Rotate Abfrage
        if(MouseAction==1){
            if(isFloorChildObject(objectName)){
                RotateFloorChild(objectName);
            }
        //Create Abfrage
        }else if(MouseAction==2){
            string[] details = getObjectToCreateDetails(ObjectToCreate[0]);//holt sich die infos zum generieren

            //guckt welcher type Generiert werden soll
            //Type Floor 
            if(details[0].Equals("Floor")){
                string floorObj = getFloor(info);
                if(string.IsNullOrWhiteSpace(floorObj)==false){
                    //Neuer Floor wird versucht zu Generieren, und Abzurechnen und eingelagert
                    NewFloorSprite(details[1], Int32.Parse(details[2]), floorObj);
                }
            //Type Wall
            }else if(details[0].Equals("Wall")){
                string wallObj = getWall(info);
                if(string.IsNullOrWhiteSpace(wallObj)==false){
                    //Neue wand wird erstellt, Abgerechnet und eingelagert
                    GenerateNewWallSprite(wallObj, details[1], Int32.Parse(details[2]), Int32.Parse(details[3]));
                }
            //Wall Deko
            }else if(isWallObject(objectName)){
                //prüft noch auf genügent parameter
                if(details.Length==6){
                    //Neue wand wird erstellt, Abgerechnet und eingelagert
                    GenerateObjectOnWall(details[0], objectName, Int32.Parse(details[1]), float.Parse(details[2]), float.Parse(details[3]), Int32.Parse(details[4]), Int32.Parse(details[5]));
                }
            //Type FloorObjects (include Oven, Fridge, FloorDeko etc...)
            }else if(isFloorObject(objectName)){
                //prüft noch auf genügent parameter
                if(details.Length==8){
                    GenerateObjectOnFloor(details[0], details[1], Int32.Parse(details[2]), Int32.Parse(details[3]), float.Parse(details[4]), float.Parse(details[5]), float.Parse(details[6]), float.Parse(details[7]), objectName);
                }
            }

            //Kein Handler ist Aktiviert, Nichts kann verändert werden
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

    public void GenerateObjectOnWall(string spriteName, string wallName, int wallChildLength, float coordCorrectionX, float coordCorrectionY, int priceGold, int priceMoney){
        //(wallchilName,WallName,wallChildLength,coordCorrectionX,coordCorrectionY)
        //wenn das WallObject generiert wurde, rechne Ab
        if(ObjectController.GenerateObjectOnWall(spriteName, wallName, wallChildLength, coordCorrectionX, coordCorrectionY)){

            //Abrechnen
            PlayerController.playerGold = PlayerController.playerGold - priceGold;
            PlayerController.playerMoney = PlayerController.playerMoney - priceMoney;

            //updated die mainUI player stats
            PlayerController.ReloadPlayerStats();
        }
    }
    public void GenerateObjectOnFloor(string type, string spriteName, int priceGold, int priceMoney, float coordCoorXA, float coordCoorYA, float coordCoorXB, float coordCoorYB, string wallName){
        //(type,spriteName,price,coordCoorXA...-coordCoorYB,FloorGameObjectName)
        //wenn das FloorChildObject generiert wurde, rechne Ab
        if(ObjectController.GenerateObjectOnFloor(type, spriteName, priceGold, coordCoorXA, coordCoorYA, coordCoorXB, coordCoorYB, wallName)){

            //Abrechnen
            PlayerController.playerGold = PlayerController.playerGold - priceGold;
            PlayerController.playerMoney = PlayerController.playerMoney - priceMoney;

            //updated die mainUI player stats
            PlayerController.ReloadPlayerStats();
        }
    }
    public void GenerateNewWallSprite(string wallName, string spriteName, int priceGold, int priceMoney){
        ObjectController.ChangeWallSprite(wallName, spriteName, priceGold, priceMoney);
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

    public string getWall(RaycastHit2D[] info){//guckt ob beim raycast eine Wall gecatch wurde
        string objectName = null;
        for(int a=0;a<info.Length;a++){
            string[] splitName = info[a].collider.name.Split("-");
            if(splitName.Length==3){
                if(splitName[2].Equals("Wall")){
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
            //walldeko object        sprite name    length coordCorX coordCorY preisgold preismoney
            details = new string[]{"Wall_Deko_01_", "1", "0,15", "1,0", "0", "800"};
        }
        if(objName.Equals("Wall_Deko_02_a")){
            details = new string[]{"Wall_Deko_02_", "1", "0,15", "1,0", "0", "400"};
        }
        if(objName.Equals("Wall_Deko_03_a")){
            details = new string[]{"Wall_Deko_03_", "1", "0,25", "1,0", "4", "0"};
        }
        if(objName.Equals("Wall_Deko_04_a")){
            details = new string[]{"Wall_Deko_04_", "1", "0,2", "1,0", "5", "0"};
        }
        if(objName.Equals("Wall_Deko_05_a")){
            details = new string[]{"Wall_Deko_05_", "1", "0,1", "1,0", "0", "700"};
        }
        if(objName.Equals("Wall_Deko_06_a")){
            details = new string[]{"Wall_Deko_06_", "1", "0,75", "1,0", "0", "11000"};
        }
        if(objName.Equals("Wall_Deko_07_a")){
            details = new string[]{"Wall_Deko_07_", "1", "0,0", "1,0", "2", "0"};
        }
        if(objName.Equals("Wall_Deko_08_a")){
            details = new string[]{"Wall_Deko_08_", "1", "0,1", "1,0", "3", "0"};
        }
        if(objName.Equals("Wall_Deko_09_a")){
            details = new string[]{"Wall_Deko_09_", "3", "1", "1,0", "0", "6000"};
        }
        if(objName.Equals("Wall_Deko_10_a")){
            details = new string[]{"Wall_Deko_10_", "3", "1,1", "1,0", "0", "8000"};
        }
        if(objName.Equals("Wall_Deko_11_a")){
            details = new string[]{"Wall_Deko_11_", "3", "1,2", "1,0", "0", "12500"};
        }
        if(objName.Equals("Wall_Deko_12_a")){
            details = new string[]{"Wall_Deko_12_", "3", "1,3", "1,0", "4", "0"};
        }
        if(objName.Equals("Wall_Deko_13_a")){
            details = new string[]{"Wall_Deko_13_", "3", "0,75", "1,0", "4", "0"};
        }
        if(objName.Equals("Wall_Deko_14_a")){
            details = new string[]{"Wall_Deko_14_", "1", "0,0", "1,0", "0", "5000"};
        }
        if(objName.Equals("Wall_Deko_15_a")){
            details = new string[]{"Wall_Deko_15_", "1", "1,25", "1,0", "0", "10000"};
        }
        if(objName.Equals("Wall_Deko_01_a_1")){
            details = new string[]{"Wall_Deko_01_1_", "1", "0,0", "1,0", "0", "2500"};
        }
        if(objName.Equals("Wall_Deko_02_a_1")){
            details = new string[]{"Wall_Deko_02_1_", "1", "0,75", "1,0", "0", "1500"};
        }
        if(objName.Equals("Wall_Deko_03_a_1")){
            details = new string[]{"Wall_Deko_03_1_", "1", "0,25", "1,0", "0", "500"};
        }
        if(objName.Equals("Wall_Deko_04_a_1")){
            details = new string[]{"Wall_Deko_04_1_", "1", "0,1", "1,0", "0", "7000"};
        }
        if(objName.Equals("Wall_Deko_05_a_1")){
            details = new string[]{"Wall_Deko_05_1_", "3", "1,2", "1,0", "0", "15000"};
        }
        if(objName.Equals("Wall_Deko_06_a_1")){
            details = new string[]{"Wall_Deko_06_1_", "1", "0,25", "1,0", "3", "0"};
        }
        if(objName.Equals("Wall_Deko_07_a_1")){
            details = new string[]{"Wall_Deko_07_1_", "1", "0,1", "1,0", "0", "10000"};
        }
        if(objName.Equals("Wall_Deko_08_a_1")){
            details = new string[]{"Wall_Deko_08_1_", "1", "0,1", "1,0", "999", "0"};//CONTINUE
        }
        if(objName.Equals("Wall_Deko_09_a_1")){
            details = new string[]{"Wall_Deko_09_1_", "1", "0,15", "1,0", "999", "0"};//CONTINUE
        }
        if(objName.Equals("Wall_Deko_10_a_1")){
            details = new string[]{"Wall_Deko_10_1_", "1", "0,4", "1,0", "999", "0"};//CONTINUE
        }
        if(objName.Equals("Wall_Deko_11_a_1")){
            details = new string[]{"Wall_Deko_11_1_", "1", "0,5", "1,0", "999", "0"};//CONTINUE
        }
        if(objName.Equals("Wall_Deko_12_a_1")){
            details = new string[]{"Wall_Deko_12_1_", "1", "0,75", "1,0", "999", "0"};//CONTINUE
        }
        if(objName.Equals("Wall_Deko_13_a_1")){
            details = new string[]{"Wall_Deko_13_1_", "1", "0,2", "1,0", "999", "0"};//CONTINUE
        }
        if(objName.Equals("Wall_Deko_14_a_1")){
            details = new string[]{"Wall_Deko_14_1_", "1", "0,1", "1,0", "4", "0"};
        }
        if(objName.Equals("Wall_Deko_15_a_1")){
            details = new string[]{"Wall_Deko_15_1_", "1", "0,2", "1,0", "2", "0"};
        }
        if(objName.Equals("Wall_Deko_16_a_1")){
            details = new string[]{"Wall_Deko_16_1_", "1", "0,4", "1,0", "5", "0"};
        }
        if(objName.Equals("Wall_Deko_17_a_1")){
            details = new string[]{"Wall_Deko_17_1_", "1", "0,7", "1,0", "5", "0"};
        }


        if(objName.Equals("Counter_01_a")){
            //counter object       type      sprite name priceGold priceMoney coordCorX coordCorY coordCorX coordCorY
            details = new string[]{"Counter", "Counter_01_a", "999", "0", "0,0", "0,75", "0,0", "0,75"};//CONTINUE
        }
        if(objName.Equals("Counter_02_a")){
            details = new string[]{"Counter", "Counter_02_a", "0", "500", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Counter_03_a")){
            details = new string[]{"Counter", "Counter_03_a", "999", "0", "0,0", "0,75", "0,0", "0,75"};//CONTINUE
        }
        if(objName.Equals("Counter_04_a")){
            details = new string[]{"Counter", "Counter_04_a", "999", "0", "0,0", "0,75", "0,0", "0,75"};//CONTINUE
        }
        if(objName.Equals("Counter_05_a")){
            details = new string[]{"Counter", "Counter_05_a", "0", "1000", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Counter_06_a")){
            details = new string[]{"Counter", "Counter_06_a", "0", "500", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Counter_01_a_1")){
            details = new string[]{"Counter", "Counter_01_1_a", "0", "10000", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Counter_02_a_1")){
            details = new string[]{"Counter", "Counter_02_1_a", "999", "0", "0,0", "0,75", "0,0", "0,75"};//CONTINUE
        }
        if(objName.Equals("Counter_03_a_1")){
            details = new string[]{"Counter", "Counter_03_1_a", "999", "0", "0,0", "0,75", "0,0", "0,75"};//CONTINUE
        }
        if(objName.Equals("Counter_04_a_1")){
            details = new string[]{"Counter", "Counter_04_1_a", "3", "0", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Counter_05_a_1")){
            details = new string[]{"Counter", "Counter_05_1_a", "999", "0", "0,0", "0,75", "0,0", "0,75"};//CONTINUE
        }
        if(objName.Equals("Counter_06_a_1")){
            details = new string[]{"Counter", "Counter_06_1_a", "999", "0", "0,0", "0,75", "0,0", "0,75"};//CONTINUE
        }



        if(objName.Equals("Chair_18_a")){
            //counter object       type      sprite name  priceGold PriceMoney coordCorX coordCorY coordCorX coordCorY
            details = new string[]{"Chair", "Chair_18_a", "1", "0", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_01_a")){
            details = new string[]{"Chair", "Chair_01_a", "0", "100", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_02_a")){
            details = new string[]{"Chair", "Chair_02_a", "0", "100", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_03_a")){
            details = new string[]{"Chair", "Chair_03_a", "0", "100", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_04_a")){
            details = new string[]{"Chair", "Chair_04_a", "0", "100", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_05_a")){
            details = new string[]{"Chair", "Chair_05_a", "0", "100", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_06_a")){
            details = new string[]{"Chair", "Chair_06_a", "0", "100", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_12_a")){
            details = new string[]{"Chair", "Chair_12_a", "0", "1500", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_13_a")){
            details = new string[]{"Chair", "Chair_13_a", "0", "2800", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_14_a")){
            details = new string[]{"Chair", "Chair_14_a", "0", "2000", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_15_a")){
            details = new string[]{"Chair", "Chair_15_a", "0", "1700", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_16_a")){
            details = new string[]{"Chair", "Chair_16_a", "0", "1600", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_17_a")){
            details = new string[]{"Chair", "Chair_17_a", "0", "9000", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_01_a_1")){
            details = new string[]{"Chair", "Chair_01_1_a", "1", "0", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_02_a_1")){
            details = new string[]{"Chair", "Chair_02_1_a", "0", "5000", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_03_a_1")){
            details = new string[]{"Chair", "Chair_03_1_a", "1", "0", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_04_a_1")){
            details = new string[]{"Chair", "Chair_04_1_a", "1", "0", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_05_a_1")){
            details = new string[]{"Chair", "Chair_05_1_a", "0", "4000", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_06_a_1")){
            details = new string[]{"Chair", "Chair_06_1_a", "999", "0", "0,0", "0,75", "0,0", "0,75"};//CONTINUE
        }
        if(objName.Equals("Chair_07_a_1")){
            details = new string[]{"Chair", "Chair_07_1_a", "999", "0", "0,0", "0,75", "0,0", "0,75"};//CONTINUE
        }
        if(objName.Equals("Chair_08_a_1")){
            details = new string[]{"Chair", "Chair_08_1_a", "999", "0", "0,0", "0,75", "0,0", "0,75"};//CONTINUE
        }
        if(objName.Equals("Chair_09_a_1")){
            details = new string[]{"Chair", "Chair_09_1_a", "0", "2000", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_10_a_1")){
            details = new string[]{"Chair", "Chair_10_1_a", "0", "3000", "0,0", "0,75", "0,0", "0,75"};
        }
        if(objName.Equals("Chair_11_a_1")){
            details = new string[]{"Chair", "Chair_11_1_a",  "999", "0", "0,0", "0,75", "0,0", "0,75"};//CONTINUE
        }



        if(objName.Equals("Deko_01_a")){          //priceGold priceMoney
            details = new string[]{"Deko", "Deko_01_a", "0", "700", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_02_a")){
            details = new string[]{"Deko", "Deko_02_a", "3", "0", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_03_a")){
            details = new string[]{"Deko", "Deko_03_a", "0", "500", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_04_a")){
            details = new string[]{"Deko", "Deko_04_a", "0", "8000", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_05_a")){
            details = new string[]{"Deko", "Deko_05_a", "2", "0", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_06_a")){
            details = new string[]{"Deko", "Deko_06_a", "999", "0", "0,0", "1,75", "0,0", "1,75"};//CONTINUE
        }
        if(objName.Equals("Deko_07_a")){
            details = new string[]{"Deko", "Deko_07_a", "0", "20000", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_08_a")){
            details = new string[]{"Deko", "Deko_08_a", "0", "1500", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_09_a")){
            details = new string[]{"Deko", "Deko_09_a", "3", "0", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_10_a")){
            details = new string[]{"Deko", "Deko_10_a", "0", "1750", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_11_a")){
            details = new string[]{"Deko", "Deko_11_a", "0", "2000", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_12_a")){
            details = new string[]{"Deko", "Deko_12_a", "5", "0", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_13_a")){
            details = new string[]{"Deko", "Deko_13_a", "5", "0", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_14_a")){
            details = new string[]{"Deko", "Deko_14_a", "3", "0", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_15_a")){
            details = new string[]{"Deko", "Deko_15_a", "3", "0", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_01_a_1")){
            details = new string[]{"Deko", "Deko_01_1_a", "5", "0", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_02_a_1")){
            details = new string[]{"Deko", "Deko_02_1_a", "5", "0", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_03_a_1")){
            details = new string[]{"Deko", "Deko_03_1_a", "4", "0", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_04_a_1")){
            details = new string[]{"Deko", "Deko_04_1_a", "3", "0", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_05_a_1")){
            details = new string[]{"Deko", "Deko_05_1_a", "3", "0", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_06_a_1")){
            details = new string[]{"Deko", "Deko_06_1_a", "999", "0", "0,0", "1,75", "0,0", "1,75"};//CONTINUE
        }
        if(objName.Equals("Deko_07_a_1")){
            details = new string[]{"Deko", "Deko_07_1_a", "999", "0", "0,0", "1,75", "0,0", "1,75"};//CONTINUE
        }
        if(objName.Equals("Deko_08_a_1")){
            details = new string[]{"Deko", "Deko_08_1_a", "999", "0", "0,0", "1,75", "0,0", "1,75"};//CONTINUE
        }
        if(objName.Equals("Deko_09_a_1")){
            details = new string[]{"Deko", "Deko_09_1_a", "20", "0", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_10_a_1")){
            details = new string[]{"Deko", "Deko_10_1_a", "12", "0", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_11_a_1")){
            details = new string[]{"Deko", "Deko_11_1_a", "3", "0", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_12_a_1")){
            details = new string[]{"Deko", "Deko_12_1_a", "3", "0", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_13_a_1")){
            details = new string[]{"Deko", "Deko_13_1_a", "999", "0", "0,0", "1,75", "0,0", "1,75"};//CONTINUE
        }
        if(objName.Equals("Deko_14_a_1")){
            details = new string[]{"Deko", "Deko_14_1_a", "3", "4000", "0,0", "1,75", "0,0", "1,75"};
        }
        if(objName.Equals("Deko_15_a_1")){
            details = new string[]{"Deko", "Deko_15_1_a", "999", "0", "0,0", "1,75", "0,0", "1,75"};//CONTINUE
        }
        if(objName.Equals("Deko_16_a_1")){
            details = new string[]{"Deko", "Deko_16_1_a", "999", "0", "0,0", "1,75", "0,0", "1,75"};//CONTINUE
        }
        if(objName.Equals("Deko_17_a_1")){
            details = new string[]{"Deko", "Deko_17_1_a", "999", "0", "0,0", "1,75", "0,0", "1,75"};//CONTINUE
        }
        if(objName.Equals("Deko_18_a_1")){
            details = new string[]{"Deko", "Deko_18_1_a", "999", "0", "0,0", "1,75", "0,0", "1,75"};//CONTINUE
        }
        if(objName.Equals("Deko_19_a_1")){
            details = new string[]{"Deko", "Deko_19_1_a", "999", "0", "0,0", "1,75", "0,0", "1,75"};//CONTINUE
        }


        if(objName.Equals("Floor_01")){
            details = new string[]{"Floor", "Floor_01", "20"};
        }
        if(objName.Equals("Floor_02")){
            details = new string[]{"Floor", "Floor_02", "20"};
        }
        if(objName.Equals("Floor_03")){
            details = new string[]{"Floor", "Floor_03", "20"};
        }
        if(objName.Equals("Floor_04")){
            details = new string[]{"Floor", "Floor_04", "100"};
        }
        if(objName.Equals("Floor_05")){
            details = new string[]{"Floor", "Floor_05", "20"};
        }
        if(objName.Equals("Floor_06")){
            details = new string[]{"Floor", "Floor_06", "100"};
        }
        if(objName.Equals("Floor_07")){
            details = new string[]{"Floor", "Floor_07", "100"};
        }
        if(objName.Equals("Floor_08")){
            details = new string[]{"Floor", "Floor_08", "20"};
        }
        if(objName.Equals("Floor_09")){
            details = new string[]{"Floor", "Floor_09", "20"};
        }
        if(objName.Equals("Floor_10")){
            details = new string[]{"Floor", "Floor_10", "20"};
        }
        if(objName.Equals("Floor_11")){
            details = new string[]{"Floor", "Floor_11", "100"};
        }
        if(objName.Equals("Floor_01_1")){
            details = new string[]{"Floor", "Floor_01_1", "1000"};
        }
        if(objName.Equals("Floor_02_1")){
            details = new string[]{"Floor", "Floor_02_1", "1500"};
        }
        if(objName.Equals("Floor_03_1")){
            details = new string[]{"Floor", "Floor_03_1", "2000"};
        }
        if(objName.Equals("Floor_04_1")){
            details = new string[]{"Floor", "Floor_04_1", "999"};//CONTINUE
        }
        if(objName.Equals("Floor_05_1")){
            details = new string[]{"Floor", "Floor_05_1", "999"};//CONTINUE
        }
        if(objName.Equals("Floor_06_1")){
            details = new string[]{"Floor", "Floor_06_1", "999"};//CONTINUE
        }
        if(objName.Equals("Floor_07_1")){
            details = new string[]{"Floor", "Floor_07_1", "999"};//CONTINUE
        }
        if(objName.Equals("Floor_08_1")){
            details = new string[]{"Floor", "Floor_08_1", "999"};//CONTINUE
        }
        if(objName.Equals("Floor_09_1")){
            details = new string[]{"Floor", "Floor_09_1", "999"};//CONTINUE
        }
        if(objName.Equals("Floor_10_1")){
            details = new string[]{"Floor", "Floor_10_1", "999"};//CONTINUE
        }
        if(objName.Equals("Floor_11_1")){
            details = new string[]{"Floor", "Floor_11_1", "100"};
        }
        if(objName.Equals("Floor_12_1")){
            details = new string[]{"Floor", "Floor_12_1", "999"};//CONTINUE
        }
        if(objName.Equals("Floor_13_1")){
            details = new string[]{"Floor", "Floor_13_1", "100"};
        }
        if(objName.Equals("Floor_14_1")){
            details = new string[]{"Floor", "Floor_14_1", "250"};
        }
        if(objName.Equals("Floor_15_1")){
            details = new string[]{"Floor", "Floor_15_1", "999"};//CONTINUE
        }
        if(objName.Equals("Floor_16_1")){
            details = new string[]{"Floor", "Floor_16_1", "1000"};
        }
        if(objName.Equals("Floor_17_1")){
            details = new string[]{"Floor", "Floor_17_1", "1000"};
        }
        if(objName.Equals("Floor_18_1")){
            details = new string[]{"Floor", "Floor_18_1", "1000"};
        }



        if(objName.Equals("Fridge_03_a")){
            details = new string[]{"Fridge", "Fridge_03_a", "999", "0", "0,0", "2,15", "0,0", "2,15"};//CONTINUE
        }
        if(objName.Equals("Fridge_01_a_1")){
            details = new string[]{"Fridge", "Fridge_01_1_a", "7", "0", "0,0", "2,15", "0,0", "2,15"};
        }
        if(objName.Equals("Fridge_02_a_1")){
            details = new string[]{"Fridge", "Fridge_02_1_a", "8", "0", "0,0", "2,15", "0,0", "2,15"};
        }
        if(objName.Equals("Fridge_04_a_1")){
            details = new string[]{"Fridge", "Fridge_04_1_a", "4", "0", "0,0", "2,15", "0,0", "2,15"};
        }
        if(objName.Equals("Fridge_05_a_1")){
            details = new string[]{"Fridge", "Fridge_05_1_a", "999", "0", "0,0", "2,15", "0,0", "2,15"};//CONTINUE
        }
        if(objName.Equals("Fridge_06_a_1")){
            details = new string[]{"Fridge", "Fridge_06_1_a", "6", "0", "0,0", "2,15", "0,0", "2,15"};
        }
        if(objName.Equals("Fridge_07_a_1")){
            details = new string[]{"Fridge", "Fridge_07_1_a", "999", "0", "0,0", "2,15", "0,0", "2,15"};//CONTINUE
        }
        if(objName.Equals("Fridge_08_a_1")){
            details = new string[]{"Fridge", "Fridge_08_1_a", "4", "0", "0,0", "2,15", "0,0", "2,15"};
        }



        if(objName.Equals("Oven_01_a")){
            details = new string[]{"Oven", "Oven_01_a", "0", "1000", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Oven_07_a")){
            details = new string[]{"Oven", "Oven_07_a", "0", "12000", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Oven_01_a_1")){
            details = new string[]{"Oven", "Oven_01_1_a", "3", "0", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Oven_02_a_1")){
            details = new string[]{"Oven", "Oven_02_1_a", "0", "15000", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Oven_03_a_1")){
            details = new string[]{"Oven", "Oven_03_1_a", "7", "0", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Oven_04_a_1")){
            details = new string[]{"Oven", "Oven_04_1_a", "999", "0", "0,0", "0,85", "0,0", "0,85"};//CONTINUE
        }
        if(objName.Equals("Oven_05_a_1")){
            details = new string[]{"Oven", "Oven_05_1_a", "4", "0", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Oven_06_a_1")){
            details = new string[]{"Oven", "Oven_06_1_a", "5", "0", "0,0", "0,85", "0,0", "0,85"};
        }



        if(objName.Equals("Shlushi_a")){
            details = new string[]{"Slushi", "Shlushi_01_a", "0", "500", "0,0", "2,15", "0,0", "2,15"};
        }



        if(objName.Equals("Table_01_a")){
            details = new string[]{"Table", "Table_01_a", "0", "200", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_02_a")){
            details = new string[]{"Table", "Table_02_a", "0", "200", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_03_a")){
            details = new string[]{"Table", "Table_03_a", "0", "200", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_04_a")){
            details = new string[]{"Table", "Table_04_a", "0", "200", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_05_a")){
            details = new string[]{"Table", "Table_05_a", "0", "200", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_06_a")){
            details = new string[]{"Table", "Table_06_a", "0", "200", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_07_a")){
            details = new string[]{"Table", "Table_07_a", "0", "1500", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_08_a")){
            details = new string[]{"Table", "Table_08_a", "0", "1500", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_09_a")){
            details = new string[]{"Table", "Table_09_a", "0", "2000", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_10_a")){
            details = new string[]{"Table", "Table_10_a", "0", "7500", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_11_a")){
            details = new string[]{"Table", "Table_11_a", "1", "0", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_12_a")){
            details = new string[]{"Table", "Table_12_a", "1", "0", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_01_a_1")){
            details = new string[]{"Table", "Table_01_1_a", "1", "0", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_02_a_1")){
            details = new string[]{"Table", "Table_02_1_a", "0", "8000", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_03_a_1")){
            details = new string[]{"Table", "Table_03_1_a", "2", "0", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_04_a_1")){
            details = new string[]{"Table", "Table_04_1_a", "999", "0", "0,0", "0,85", "0,0", "0,85"};//CONTINUE
        }
        if(objName.Equals("Table_05_a_1")){
            details = new string[]{"Table", "Table_05_1_a", "999", "0", "0,0", "0,85", "0,0", "0,85"};//CONTINUE
        }
        if(objName.Equals("Table_06_a_1")){
            details = new string[]{"Table", "Table_06_1_a", "999", "0", "0,0", "0,85", "0,0", "0,85"};//CONTINUE
        }
        if(objName.Equals("Table_07_a_1")){
            details = new string[]{"Table", "Table_07_1_a", "3", "0", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_08_a_1")){
            details = new string[]{"Table", "Table_08_1_a", "2", "0", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_09_a_1")){
            details = new string[]{"Table", "Table_09_1_a", "2", "0", "0,0", "0,85", "0,0", "0,85"};
        }
        if(objName.Equals("Table_10_a_1")){
            details = new string[]{"Table", "Table_10_1_a", "999", "0", "0,0", "0,85", "0,0", "0,85"};//CONTINUE
        }
        if(objName.Equals("Table_11_a_1")){
            details = new string[]{"Table", "Table_11_1_a", "1", "0", "0,0", "0,85", "0,0", "0,85"};
        }



        if(objName.Equals("Wall_01_a")){
            //                      type    spritename pricegold pricemoney
            details = new string[]{"Wall", "Wall_01_", "0", "900"};
        }
        if(objName.Equals("Wall_02_a")){
            details = new string[]{"Wall", "Wall_02_", "0", "900"};
        }
        if(objName.Equals("Wall_03_a")){
            details = new string[]{"Wall", "Wall_03_", "0", "800"};
        }
        if(objName.Equals("Wall_04_a")){
            details = new string[]{"Wall", "Wall_04_", "0", "20"};
        }
        if(objName.Equals("Wall_05_a")){
            details = new string[]{"Wall", "Wall_05_", "0", "20"};
        }
        if(objName.Equals("Wall_06_a")){
            details = new string[]{"Wall", "Wall_06_", "0", "20"};
        }
        if(objName.Equals("Wall_07_a")){
            details = new string[]{"Wall", "Wall_07_", "0", "20"};
        }
        if(objName.Equals("Wall_08_a")){
            details = new string[]{"Wall", "Wall_08_", "0", "20"};
        }
        if(objName.Equals("Wall_09_a")){
            details = new string[]{"Wall", "Wall_09_", "0", "20"};
        }
        if(objName.Equals("Wall_10_a")){
            details = new string[]{"Wall", "Wall_10_", "0", "20"};
        }
        if(objName.Equals("Wall_11_a")){
            details = new string[]{"Wall", "Wall_11_", "0", "20"};
        }
        if(objName.Equals("Wall_12_a")){
            details = new string[]{"Wall", "Wall_12_", "999", "0"};//CONTINUE
        }
        if(objName.Equals("Wall_13_a")){
            details = new string[]{"Wall", "Wall_13_", "0", "20"};
        }
        if(objName.Equals("Wall_14_a")){
            details = new string[]{"Wall", "Wall_14_", "0", "20"};
        }
        if(objName.Equals("Wall_15_a")){
            details = new string[]{"Wall", "Wall_15_", "999", "0"};//CONTINUE
        }
        if(objName.Equals("Wall_16_a")){
            details = new string[]{"Wall", "Wall_16_", "999", "0"};//CONTINUE
        }
        if(objName.Equals("Wall_17_a")){
            details = new string[]{"Wall", "Wall_17_", "0", "20"};
        }
        if(objName.Equals("Wall_01_a_1")){
            details = new string[]{"Wall", "Wall_01_1_", "0", "1500"};
        }
        if(objName.Equals("Wall_02_a_1")){
            details = new string[]{"Wall", "Wall_02_1_", "0", "1000"};
        }
        if(objName.Equals("Wall_03_a_1")){
            details = new string[]{"Wall", "Wall_03_1_", "0", "2500"};
        }
        if(objName.Equals("Wall_04_a_1")){
            details = new string[]{"Wall", "Wall_04_1_", "0", "7500"};
        }
        if(objName.Equals("Wall_05_a_1")){
            details = new string[]{"Wall", "Wall_05_1_", "0", "7000"};
        }
        if(objName.Equals("Wall_06_a_1")){
            details = new string[]{"Wall", "Wall_06_1_", "999", "0"};//CONTINUE
        }
        if(objName.Equals("Wall_07_a_1")){
            details = new string[]{"Wall", "Wall_07_1_", "999", "0"};//CONTINUE
        }
        if(objName.Equals("Wall_08_a_1")){
            details = new string[]{"Wall", "Wall_08_1_", "999", "0"};//CONTINUE
        }
        if(objName.Equals("Wall_09_a_1")){
            details = new string[]{"Wall", "Wall_09_1_", "999", "0"};//CONTINUE
        }
        if(objName.Equals("Wall_10_a_1")){
            details = new string[]{"Wall", "Wall_10_1_", "999", "0"};//CONTINUE
        }
        if(objName.Equals("Wall_11_a_1")){
            details = new string[]{"Wall", "Wall_11_1_", "999", "0"};//CONTINUE
        }
        if(objName.Equals("Wall_12_a_1")){
            details = new string[]{"Wall", "Wall_12_1_", "999", "0"};//CONTINUE
        }
        if(objName.Equals("Wall_13_a_1")){
            details = new string[]{"Wall", "Wall_13_1_", "999", "0"};//CONTINUE
        }
        if(objName.Equals("Wall_14_a_1")){
            details = new string[]{"Wall", "Wall_14_1_", "999", "0"};//CONTINUE
        }
        if(objName.Equals("Wall_15_a_1")){
            details = new string[]{"Wall", "Wall_15_1_", "0", "6000"};
        }
        if(objName.Equals("Wall_16_a_1")){
            details = new string[]{"Wall", "Wall_16_1_", "0", "7000"};
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
