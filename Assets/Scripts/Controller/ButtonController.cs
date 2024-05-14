using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public GameObject RebuildUI;//referenz auf RebuildUIController script
    public GameObject MainController;//referenz auf MainController script
    public GameObject IngredientsAndFridgeUIController; //referenz auf IngredientsAndFridgeUIController script

    public GameObject FCEDController; //referenz auf das FloorChildExtraData Script 

    public int MouseAction = 0; //0=nothing,1=rotate,2=create,3=remove,4=replace
    public bool isRebuildShopOpen = false;//wenn der rebuild shop offen ist, wahr

    public string ObjectToMove = "";
    public string[] ObjectToCreate;//referenzobjekt(spriteName, goldpreis, moneypreis) wird übergeben und in diesem script zum bekommen der InstantiateDetails benutzt

    public GameObject ItemSettingsPrefab; //referenz auf das dynamische UISetting für die Items
    public GameObject DynamicPrefab; //referenzprefab für das erstellte UIStetting eines items

    public static bool DinnerUIShopOpenByOven = false; //wird auf true gesetzt, wenn ein oven anklickt wurde, sorgt dafür das item cook btn sichtbar werden kann

    //methodes
    void Update(){
        Mouse mouse = Mouse.current;
        if (mouse.leftButton.wasPressedThisFrame){//check if mouse left down
            RaycastHit2D[] hitInfo = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);//get all objects

            //guckt das irgendwas angeklickt und erfasst wurde
            if(hitInfo.Length!=0){
                
                //Überprüft ob Mouse Position nicht über UI ist, damit wird verhindert, dass nicht durch den shop geklickt wird
                if(EventSystem.current.IsPointerOverGameObject()==false){
                    
                    //guckt das rebuild shop offen ist
                    if(isRebuildShopOpen==true){

                        //objekte könnnen bearbeitet werden
                        MouseHandler(hitInfo);
                    }else{
                    //wenn nicht, gucke ob Oven, Counter, Fridge oder Slushi angeklickt wurde

                        //objekte könnnen angeklickt werden
                        MouseHandler2(hitInfo);
                    }
                }
            }
        }
    }

    //wird durch SettingsUI benutzt und rotiert gegebenes Objekt
    public void RotateObjectByUISetting(string name){
        RotateFloorChild(name);

        //muss nach jeder shop aktion neu ausgeführt werder um bsp zu gucken ob der player noch genug geld für objekte hat und entsprechend 
        //buy buttons ausblenden
        RebuildUI.GetComponent<RebuildUIController>().DeleteItems();
        RebuildUI.GetComponent<RebuildUIController>().RenderShop();

        //nach jeder action muss neu gespeichert werden
        SaveAndLoadController.SavePlayerData();
    }

    //wird durch SettingsUI aufgerufen, Lagert Objekt wieder ein 
    public void StoreObjectByUISetting(string name){
        if(isWallObject(name)==true){
            DestroyObjectOnWall(name);
        }
        if(isFloorChildObject(name)==true){
            DestroyFloorChild(name);
        }

        //schließe ui
        Destroy(DynamicPrefab);

        //muss nach jeder shop aktion neu ausgeführt werder um bsp zu gucken ob der player noch genug geld für objekte hat und entsprechend 
        //buy buttons ausblenden
        RebuildUI.GetComponent<RebuildUIController>().DeleteItems();
        RebuildUI.GetComponent<RebuildUIController>().RenderShop();

        //nach jeder action muss neu gespeichert werden
        SaveAndLoadController.SavePlayerData();
    }

    //wird durch SettingsUI aufgerufen, Replaced Objekt
    //1te sequenz der replace funktion
    public void ReplaceObjectByUISetting(string name){
        //im ersten schritt prüfe ob ein floorChild Object angeklickt wurde
        if(isFloorChildObject(name)==true){
            ObjectToMove = name;
        //im ersten schritt prüfe ob ein wallObject angeklickt wurde
        }else if(isWallObject(name)==true){
            ObjectToMove = name;
        }
        MouseAction = 4;

        //muss nach jeder shop aktion neu ausgeführt werder um bsp zu gucken ob der player noch genug geld für objekte hat und entsprechend 
        //buy buttons ausblenden
        RebuildUI.GetComponent<RebuildUIController>().DeleteItems();
        RebuildUI.GetComponent<RebuildUIController>().RenderShop();

        //nach jeder action muss neu gespeichert werden
        SaveAndLoadController.SavePlayerData();
    }

    //Guckt welches Object erstellt wird, anhand dessen, welches Object angeklickt wurde
    public void MouseHandler(RaycastHit2D[] info){

        string objectName = getPrioritizedObjectName(info);
        
        //zertstört dynamicPrefab damit immer nur 1 existiert
        //hiermit können einzelaktionen für bestimmte Gameobjecte benutzt werden
        Destroy(DynamicPrefab);

        //ItemSettingUI
        //rendert die HandlerUI für das jeweilige angeklickte Object
        //Replace, Sell, Rotate
        if(MouseAction==0&&isRebuildShopOpen==true){
            //überprüft auf welches angeklickte Object der fokus liegt
            // muss floorchilds sein      oder        wallchild mit sprite    
            if(isFloorChildObject(objectName)==true||(isWallObject(objectName)&&isWallChildObjectEmpty(objectName)==false)){
            
                //hole die referenz auf das geklickte Object
                GameObject KlickedObject = GameObject.Find(objectName);

                //generiere für das referenzobject das prefab
                DynamicPrefab = Instantiate(ItemSettingsPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                DynamicPrefab.transform.parent = KlickedObject.transform;
                DynamicPrefab.GetComponent<Canvas>().sortingOrder = 100;
                DynamicPrefab.transform.position = new Vector2(KlickedObject.transform.position.x, KlickedObject.transform.position.y+3);
                
                //guck welche BTNs für welches item generiert werden kann (Wall kein rotate)
                //Gucke nach Rotatebutton             
                if(isFloorChildObject(objectName)){
                    //Gibt einen Btn eine AddListener Funktion die beim drücken eine Methode in ButtonController aktiviert und den ObjectNamen übergibt
                    DynamicPrefab.transform.GetChild(1).gameObject.SetActive(true);
                    DynamicPrefab.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(delegate{RotateObjectByUISetting(objectName);});
                }
                //Gucke nach SellButton 
                if(!isWallChildObjectEmpty(objectName)||isFloorChildObject(objectName)){
                    //object darf nicht Door sein
                    if(!isWallDoorObject(objectName)){
                        DynamicPrefab.transform.GetChild(2).gameObject.SetActive(true);
                        DynamicPrefab.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(delegate{StoreObjectByUISetting(objectName);});
                    }
                }
                //Gucke nach ReplaceButton     
                if(!isWallChildObjectEmpty(objectName)||isFloorChildObject(objectName)){
                    DynamicPrefab.transform.GetChild(3).gameObject.SetActive(true);
                    DynamicPrefab.transform.GetChild(3).gameObject.GetComponent<Button>().onClick.AddListener(delegate{ReplaceObjectByUISetting(objectName);});
                }
            }
        }
        
        //Create Object Abfrage
        //wird durch shopkauf aufgerufen
        if(MouseAction==2){
            string[] details = getObjectToCreateDetails(ObjectToCreate[0]);//holt sich die infos zum generieren

            //guckt welcher type Generiert werden soll
            //Type Floor , wird replaced
            if(details[0].Equals("Floor")){
                string floorObj = getFloor(info);
                if(string.IsNullOrWhiteSpace(floorObj)==false){
                    //Neuer Floor wird versucht zu Generieren, und Abzurechnen und eingelagert
                    NewFloorSprite(details[1], Int32.Parse(details[2]), floorObj);
                }
            //Type Wall , wird replaced
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

        }

        //replace Object
        //wird durch dynamicSettingsUI aufgerufen
        //ist die 2te sequenz der replace funktion
        if(MouseAction==4){
            //im zweiten schritt prüfe ob ein floorChild Object vorhanden ist und ob das neue Object floor ist
            if(ObjectToMove.Equals("")==false&&isFloorObject(objectName)==true&&isFloorChildObject(ObjectToMove)==true){
                MoveObjectOnFloor(ObjectToMove, objectName);                                                            
                ObjectToMove = "";
            //im zweiten schritt prüfe ob ein wall Object vorhanden ist und ob das neue Object wall ist
            }else if(ObjectToMove.Equals("")==false&&isWallObject(objectName)==true&&isWallObject(ObjectToMove)==true){
                MoveObjectOnWall(ObjectToMove,objectName);
                ObjectToMove = "";
            }
            //breche replace sequenz ab
            ObjectToMove = "";
            MouseAction = 0;
        }

        //muss nach jeder shop aktion neu ausgeführt werder um bsp zu gucken ob der player noch genug geld für objekte hat und entsprechend 
        //buy buttons ausblenden
        RebuildUI.GetComponent<RebuildUIController>().DeleteItems();
        RebuildUI.GetComponent<RebuildUIController>().RenderShop();

        //nach jeder action muss neu gespeichert werden
        SaveAndLoadController.SavePlayerData();
    }

    

    public void DestroyObjectOnWall(string objectName){

        //hier wird das object als schon gekauft gespeichert 
        //speicher als "..."_a
        string[] spriteName = GameObject.Find(objectName).gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite.name.Split("_");
        if(spriteName.Length == 4){
            PlayerController.AddStorageItem(spriteName[0]+"_"+spriteName[1]+"_"+spriteName[2]+"_a");
        }else if(spriteName.Length == 5){
            PlayerController.AddStorageItem(spriteName[0]+"_"+spriteName[1]+"_"+spriteName[2]+"_a_1");
        }

        //muss nach jeder shop aktion neu ausgeführt werder um bsp zu gucken ob der player noch genug geld für objekte hat und entsprechend 
        //buy buttons ausblenden, läd shop neu
        RebuildUI.GetComponent<RebuildUIController>().DeleteItems();
        RebuildUI.GetComponent<RebuildUIController>().RenderShop();

        //Anschließend zerstört
        ObjectController.DestroyObjectOnWall(objectName);//(WallGOName)
    }

    //Zerstört Object und speichert das Object im ObjectKatalog
    public void DestroyFloorChild(string objectName){

        //hier wird das object als schon gekauft gespeichert 
        //speicher als "..."_a
        string[] spriteName = GameObject.Find(objectName).gameObject.GetComponent<SpriteRenderer>().sprite.name.Split("_");
        if(spriteName.Length == 3){
            //slushi als ausnahme
            if(GameObject.Find(objectName).gameObject.GetComponent<SpriteRenderer>().sprite.name.Equals("Shlushi_01_a")){
                PlayerController.AddStorageItem(spriteName[0]+"_a");
            }else{
                PlayerController.AddStorageItem(spriteName[0]+"_"+spriteName[1]+"_a");
            }
        }else if(spriteName.Length == 4){
            PlayerController.AddStorageItem(spriteName[0]+"_"+spriteName[1]+"_a_1");
        }

        //muss nach jeder shop aktion neu ausgeführt werder um bsp zu gucken ob der player noch genug geld für objekte hat und entsprechend 
        //buy buttons ausblenden, läd shop neu
        RebuildUI.GetComponent<RebuildUIController>().DeleteItems();
        RebuildUI.GetComponent<RebuildUIController>().RenderShop();

        //Anschließend zerstört
        ObjectController.DestroyFloorChild(objectName);//(FloorChildGOName)
    }

    public void RotateFloorChild(string objectName){
        ObjectController.RotateObjectOnFloor(objectName);//(floorChildGOName)
    }

    //generiert ein object auf der Wand und rechnent ab
    public void GenerateObjectOnWall(string spriteName, string wallName, int wallChildLength, float coordCorrectionX, float coordCorrectionY, int priceGold, int priceMoney){
        //(wallchilName,WallName,wallChildLength,coordCorrectionX,coordCorrectionY)
        //wenn das WallObject generiert wurde, rechne Ab
        if(ObjectController.GenerateObjectOnWall(spriteName, wallName, wallChildLength, coordCorrectionX, coordCorrectionY)){



            //Item wird Abgerechnet
            //wenn in backup, rechne erst das ab
            if(Int32.Parse(ObjectToCreate[3])>0){
                //guckt nach den richtigen spriteNamen und zieht diesen aus dem backup ab
                string[] splitString = spriteName.Split("_");
                if(splitString.Length==4){
                    //1 item wird aus backup entfernt
                    PlayerController.RemoveStorageItem(splitString[0]+"_"+splitString[1]+"_"+splitString[2]+"_a");
                }else if(splitString.Length==5){
                    PlayerController.RemoveStorageItem(splitString[0]+"_"+splitString[1]+"_"+splitString[2]+"_a_1");
                }
            }else{
                //nicht in backup also rechne normalen preis ab
                PlayerController.playerMoney = PlayerController.playerMoney - priceMoney;
                PlayerController.playerGold = PlayerController.playerGold - priceGold;
            }



            //muss nach jeder shop aktion neu ausgeführt werder um bsp zu gucken ob der player noch genug geld für objekte hat und entsprechend 
            //buy buttons ausblenden, läd shop neu
            RebuildUI.GetComponent<RebuildUIController>().DeleteItems();
            RebuildUI.GetComponent<RebuildUIController>().RenderShop();

            //updated die mainUI player stats
            PlayerController.ReloadPlayerStats();
        }
    }

    //Generiert ein neues Object auf dem floor
    public void GenerateObjectOnFloor(string type, string spriteName, int priceGold, int priceMoney, float coordCoorXA, float coordCoorYA, float coordCoorXB, float coordCoorYB, string wallName){
        //(type,spriteName,price,coordCoorXA...-coordCoorYB,FloorGameObjectName)
        //wenn das FloorChildObject generiert wurde, rechne Ab
        if(ObjectController.GenerateObjectOnFloor(type, spriteName, priceGold, coordCoorXA, coordCoorYA, coordCoorXB, coordCoorYB, wallName)){




            //Abrechnen und gucken ob gegebenes Object in backup ist
            string[] splitName = spriteName.Split("_");
            if(Int32.Parse(ObjectToCreate[3])>0){
                if(splitName.Length == 3){
                    //slushi als ausnahme
                    if(spriteName.Equals("Shlushi_01_a")){
                        PlayerController.RemoveStorageItem(splitName[0]+"_a");
                    }else{
                        PlayerController.RemoveStorageItem(splitName[0]+"_"+splitName[1]+"_a");
                    }
                }else if(splitName.Length == 4){
                    PlayerController.RemoveStorageItem(splitName[0]+"_"+splitName[1]+"_a_1");
                }
            }else{
                PlayerController.playerGold = PlayerController.playerGold - priceGold;
                PlayerController.playerMoney = PlayerController.playerMoney - priceMoney;
            }




            //muss nach jeder shop aktion neu ausgeführt werder um bsp zu gucken ob der player noch genug geld für objekte hat und entsprechend 
            //buy buttons ausblenden, läd shop neu
            RebuildUI.GetComponent<RebuildUIController>().DeleteItems();
            RebuildUI.GetComponent<RebuildUIController>().RenderShop();

            //updated die mainUI player stats
            PlayerController.ReloadPlayerStats();
        }
    }

    //Verändert die hintergrundTextur der Wand
    public void GenerateNewWallSprite(string wallName, string spriteName, int priceGold, int priceMoney){

        //wenn wall gechanged wurde
        if(ObjectController.ChangeWallSprite(wallName, spriteName, priceGold, priceMoney)){

            //Item wird Abgerechnet
            //wenn in backup, rechne erst das ab
            if(Int32.Parse(ObjectToCreate[3])>0){
                //guckt nach den richtigen spriteNamen und zieht diesen aus dem backup ab
                string[] splitString = spriteName.Split("_");
                if(splitString.Length==3){
                    PlayerController.RemoveStorageItem(splitString[0]+"_"+splitString[1]+"_a");
                }else if(splitString.Length==4){
                    PlayerController.RemoveStorageItem(splitString[0]+"_"+splitString[1]+"_a_1");
                }
            }else{
                //nicht in backup also rechne normalen preis ab
                PlayerController.playerMoney = PlayerController.playerMoney - priceMoney;
                PlayerController.playerGold = PlayerController.playerGold - priceGold;
            }

            //muss nach jeder shop aktion neu ausgeführt werder um bsp zu gucken ob der player noch genug geld für objekte hat und entsprechend 
            //buy buttons ausblenden, läd shop neu
            RebuildUI.GetComponent<RebuildUIController>().DeleteItems();
            RebuildUI.GetComponent<RebuildUIController>().RenderShop();

            //updated die mainUI player stats
            PlayerController.ReloadPlayerStats();
        }
    }

    public void MoveObjectOnWall(string wallNameOld, string wallNameNew){
        ObjectController.MoveObjectOnWall(wallNameOld, wallNameNew);//(curWallName,newWallName)
    }
    public void MoveObjectOnFloor(string objectName, string floorName){
        ObjectController.MoveObjectOnFloor(objectName, floorName);//(floorChildGameObjectName,floorGameObjectName(neuer platz))
    }

    //verändert die boden textur
    public void NewFloorSprite(string newFloorSpriteName, int floorPrice, string floorGOName){
        if(ObjectController.NewFloorSprite(newFloorSpriteName, floorPrice, floorGOName)){//(newFloorSprite,floorPrice,floorGOName)

            //Item wird Abgerechnet
            //wenn in backup, rechne erst das ab
            if(Int32.Parse(ObjectToCreate[3])>0){
                PlayerController.RemoveStorageItem(newFloorSpriteName);
            }else{
                PlayerController.playerMoney = PlayerController.playerMoney - floorPrice;
            }

            //updated die mainUI player stats
            PlayerController.ReloadPlayerStats();

            //muss nach jeder shop aktion neu ausgeführt werder um bsp zu gucken ob der player noch genug geld für objekte hat und entsprechend 
            //buy buttons ausblenden, läd shop neu
            RebuildUI.GetComponent<RebuildUIController>().DeleteItems();
            RebuildUI.GetComponent<RebuildUIController>().RenderShop();
        }
    }






    //Wenn RebuildShop nicht geöffnet ist, empfängt alle mouseclicks
    //gucke auf welche objecte geklickt wurde
    public void MouseHandler2(RaycastHit2D[] info){

        //sucht das priorisierte item
        string objectName = getPrioritizedObjectName(info);

        //gucke ob es ein Floor child object ist
        if(isFloorChildObject(objectName)){
            //gucke ob angeklicktes object Fridge,Oven,Counter oder Slushi ist
            if(getTypeFromObject(objectName).Equals("Fridge")){

                //öffne fridge
                //MainController.buttonPressed(1) //öffnet ingredientsstore 
                //IngredientsAndFridgeUIController.IsFridgeOpen(true) //öffnet fridge
                MainController.GetComponent<MainController>().buttonPressed(1);
                IngredientsAndFridgeUIController.GetComponent<IngredientsUIController>().IsFridgeOpen(true);

            }else if(getTypeFromObject(objectName).Equals("Oven")){

                //CONTINUE 
                //guck ob nicht schon ein gericht auf dem oven angebaut wurde (wenn step = 0, dann kein gericht auf oven)
                int stepAnzahl = FCEDController.GetComponent<FloorChildExtraDataController>().getOvenStep(objectName);
                if(stepAnzahl>0){
                    Debug.Log("StepAnzahl: "+stepAnzahl);

                    //CONTINUE stepAnzahl soll abgearbeitet werden...
                    //spieler soll zum oven gehen 
                    //step anzahl um -1 reduzieren
                    //UI beim dinner cooking updaten

                    //EXPERIMENTAL
                    DinnerControlller.ReduceStepCount_SendPlayer(objectName, stepAnzahl);

                //kein gericht, also öffne oven shop
                }else{
                    Debug.Log("Öffne shop weil: "+stepAnzahl);
                    
                    //öffne sonst gerichteshop und generiere die Cookbtns
                    //speichert vorübergehend den objektnamen des angeklickten ovens auf dem gekocht werden soll
                    DinnerController.ovenToCookOnByOpenDinnerShopUI = objectName;

                    //wenn nicht öffne denn dinner shop
                    //shop kann buy btn generieren
                    DinnerUIShopOpenByOven = true;

                    //öffne shop
                    MainController.GetComponent<MainController>().buttonPressed(3);
                }
                


            }else if(getTypeFromObject(objectName).Equals("Counter")){
                //zeige anzahl der gerichte auf counter
            }else if(getTypeFromObject(objectName).Equals("Slushi")){
                //öffne slushi shop
            }
        //Floor angeklickt, Gucke ob der Spieler sich bewegen kann
        }else if(isFloorObject(objectName)){
            //übergibt die angeklickte position an den movementcontroller
            PlayerMovementController.MovePlayer(new int[]{Int32.Parse(objectName.Split("-")[0]),Int32.Parse(objectName.Split("-")[1])});
        }
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

    //guckt welcher typ der übergebene string ist
    public string getTypeFromObject(string name){
        GameObject gameObject = GameObject.Find(name);
        string spriteType = gameObject.GetComponent<SpriteRenderer>().sprite.name.Split("_")[0];
        if(spriteType.Equals("Fridge")){
            return "Fridge";
        }else if(spriteType.Equals("Oven")){
            return "Oven";
        }else if(spriteType.Equals("Shlushi")){
            return "Slushi";
        }else if(spriteType.Equals("Counter")){
            return "Counter";
        }
        return "None";
    }

    public bool isWallObject(string objectName){
        string[] splitName = objectName.Split("-");
        if(splitName[splitName.Length-1].Equals("Wall")){
            return true;
        }
        return false;
    }

    public bool isWallChildObjectEmpty(string objectName){
        string[] splitName = objectName.Split("-");
        if(splitName[splitName.Length-1].Equals("Wall")){
            GameObject wallGO = GameObject.Find(objectName);
            
            //überprüft ob das gameobject kein sprite hat
            if(Object.ReferenceEquals(wallGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite, null)){
                return true;
            }
        }
        return false;
    }

    public bool isWallDoorObject(string objectName){
        //find alle wall objecte in der scene und gucke ob unterobject wall ist
        string[] splitName = objectName.Split("-");
        if(splitName[splitName.Length-1].Equals("Wall")){
            GameObject wallGO = GameObject.Find(objectName);
            
            //überprüft ob das gameobject unterobject ein sprite hat
            if(!Object.ReferenceEquals(wallGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite, null)){
                if(wallGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite.name.Split("_")[1].Equals("Door")){
                    return true;
                }
            }
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
            details = new string[]{"Slushi", "Shlushi_01_a", "0", "500", "0,1", "2,70", "0,1", "2,70"};
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
