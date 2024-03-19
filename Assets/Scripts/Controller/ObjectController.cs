using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectController : MonoBehaviour
{
    public static List<WallObject> WallObjectList = new List<WallObject>();

    public static List<FloorObject> FloorObjectList = new List<FloorObject>();
    public static List<StandardObject> standardObjectList = new List<StandardObject>();
    

    
    public static void GenerateWallObject(string wallGameObjectName, string wallSpriteName, int rotation, string wallChildName, int wallChildLength, float wallChildCoordCorrectionX, float wallChildCoordCorrectionY, int xCoord, int yCoord){
        WallObjectList.Add(new WallObject(wallGameObjectName, wallSpriteName, rotation, wallChildName, wallChildLength, wallChildCoordCorrectionX, wallChildCoordCorrectionY, xCoord, yCoord));
    }

    //render Deko Obj on Wall
    public static bool GenerateObjectOnWall(string wallChildName, string wallName, int wallChildLength, float wallChildCoordCorrectionX, float wallChildCoordCorrectionY){
        WallObject wallObject = getWallGOFromWallGOName(wallName);
        //wenn kein deko object auf WallObject ist, generiere deko obj
        if(wallChildLength == 3){//überprüfe ob es ein 2 teiliges child ist, wenn ja prüfe ob nachbar existiert + child hat (3==2teilig)
            //prüfe ob ein kleineren nachbarn hat, prüfe ob der nachbar frei ist
            WallObject neighbourWallobject = getSmallerNeighbourWallGOFromWallGOName(wallName);
            if(neighbourWallobject.wallGameObjectName!=null){//nachbar existiert(angenommen main wallobj ist 1-0)
                if(string.IsNullOrWhiteSpace(wallObject.WallChildName)&&string.IsNullOrWhiteSpace(neighbourWallobject.WallChildName)){//hat keine deko objecte
                    InstantiateObjectOnWall(wallObject, wallChildCoordCorrectionX, wallChildCoordCorrectionY, wallChildLength, wallChildName);
                    InstantiateObjectOnWall(neighbourWallobject, 0.0f, 0.0f, 2, "placeholder");
                    return true;
                }
            }
        }else if(wallChildLength == 1){
            Debug.Log("InstantiateObjectOnWall1");
            if(string.IsNullOrWhiteSpace(wallObject.WallChildName)){
                Debug.Log("InstantiateObjectOnWall2");
                //prüfe noch ob, wenn door generiert werden soll, keine vorhanden ist
                if(checkIfObjectIsDoor(wallChildName)==true){
                    if(checkIfDoorOnWallExists()==false){
                        //prüfe ob das floorObj auf dem die tür steht frei ist(kein deko obj etc)
                        if(checkPotentialDoorPlacementOnFloorFromWallGOName(wallObject.wallGameObjectName)==true){
                            InstantiateObjectOnWall(wallObject, wallChildCoordCorrectionX, wallChildCoordCorrectionY, wallChildLength, wallChildName);
                            return true;
                        }
                    }
                }else{
                    Debug.Log("InstantiateObjectOnWall3");
                    InstantiateObjectOnWall(wallObject, wallChildCoordCorrectionX, wallChildCoordCorrectionY, wallChildLength, wallChildName);
                    return true;
                }
            }else{
                Debug.Log("wallObject.WallChildName:"+wallObject.WallChildName);
            }
        }
        return false;
    }

    public static void InstantiateObjectOnWall(WallObject obj, float coordCoorX, float coordCoorY, int length, string name){
        obj.wallChildCoordCorrectionX = coordCoorX;
        obj.wallChildCoordCorrectionY = coordCoorY;
        obj.wallChildLength = length;
        obj.WallChildName = name;
    }

    public static void MoveObjectOnWall(string wallObjectChildName, string newWallObjectName){
        WallObject oldWallObject = getWallGOFromChildGOName(wallObjectChildName);
        WallObject newWallObject = getWallGOFromWallGOName(newWallObjectName);

        //krieg beide wallObjekte, gucke ob child existiert, gucke ob sprite 2 teilig ist
        //wenn sprite "wallChildLength==2" ist, setzt main als "oldWallObject"
        //speichere werte, lösche objekte
        //GenerateObjectOnWall, wenn nicht instantiiert wurde, generiere alte obj
        if(string.IsNullOrWhiteSpace(oldWallObject.WallChildName)==false){//AktuellesObjekt hat childsprite
            
            if(oldWallObject.wallChildLength == 2){
                //setzt vom 2 teiligen chil das main obj als referenz 
                oldWallObject = getGreaterNeighbourWallGOFromWallGOName(oldWallObject.wallGameObjectName);
            }

            //speichert child daten
            string OldWallChildName = oldWallObject.WallChildName;
            int oldWallChildLength = oldWallObject.wallChildLength;
            float oldWallChildCoordCorX = oldWallObject.wallChildCoordCorrectionX;
            float oldWallChildCoordCorY = oldWallObject.wallChildCoordCorrectionY;

            if(checkIfObjectIsDoor(OldWallChildName)==true){//check ob child ist Tür, dann zerstöre Tür direkt, da "DestroyObjectOnWall" nicht Tür zerstören kann
                oldWallObject.DeleteChild();//löscht alle child komponenten
            }else{
                DestroyObjectOnWall(oldWallObject.wallGameObjectName);
            }

            //guck ob Child am neuen Ort generiert wurde, wenn nicht, stelle altes her
            bool isTrue = GenerateObjectOnWall(OldWallChildName, newWallObject.wallGameObjectName, oldWallChildLength, oldWallChildCoordCorX, oldWallChildCoordCorY);
            if(isTrue==false){
                GenerateObjectOnWall(OldWallChildName, oldWallObject.wallGameObjectName, oldWallChildLength, oldWallChildCoordCorX, oldWallChildCoordCorY);
            }
        }
    }

    public static void DestroyObjectOnWall(string wallObjectName){
        WallObject wallObject = getWallGOFromWallGOName(wallObjectName);

        //Hat child? Ist nicht Tür? hat obj child? guck ob child zwei teilig ist? wenn nicht resete, ansonsten hole 2tes obj und resete beide
        if(wallObject.WallChildName!=null){
            if(checkIfObjectIsDoor(wallObject.WallChildName)==false){//darf nicht tür entfernen
                if(wallObject.wallChildLength==1){
                    wallObject.DeleteChild();//löscht alle child komponenten
                }else if(wallObject.wallChildLength==2){//get rechts liegendes obj zusätzlich
                    WallObject greaterWallObject = getGreaterNeighbourWallGOFromWallGOName(wallObject.wallGameObjectName);
                    greaterWallObject.DeleteChild();
                    wallObject.DeleteChild();//löscht alle child komponenten
                }else if(wallObject.wallChildLength==3){//get links liegendes obj zusätzlich
                    WallObject smallerWallObject = getSmallerNeighbourWallGOFromWallGOName(wallObject.wallGameObjectName);
                    smallerWallObject.DeleteChild();
                    wallObject.DeleteChild();//löscht alle child komponenten
                }
            }
        }
    }



    //generiert gespeicherte FloorObjects und deren childs
    public static void GenerateFloorObject(string floorGameObjectName, string floorName, int floorPrice, float floorCoordX, float floorCoordY, string floorChildType, string floorChildGameObjectName, string floorChildName, int floorChildPrice, float floorChildCoordCorrectionXA, float floorChildCoordCorrectionYA, float floorChildCoordCorrectionXB, float floorChildCoordCorrectionYB){
        FloorObjectList.Add(new FloorObject(floorGameObjectName, floorName, floorPrice, floorCoordX, floorCoordY, floorChildType, floorChildGameObjectName, floorChildName, floorChildPrice, floorChildCoordCorrectionXA, floorChildCoordCorrectionYA, floorChildCoordCorrectionXB, floorChildCoordCorrectionYB));

        //generiert child from floor wenn floor obj child hat
        if(string.IsNullOrWhiteSpace(floorChildType)==false){
            //Übergebe floor child parent coord name to create floor child go name as identifier
            FloorObject floorObject = getFloorGOFromFloorGOName(floorGameObjectName);
            standardObjectList.Add(floorObject.setChild(floorChildType, floorGameObjectName, floorChildName, floorChildPrice, floorChildCoordCorrectionXA, floorChildCoordCorrectionYA, floorChildCoordCorrectionXB, floorChildCoordCorrectionYB));
        
            //FloorChildExtraData wird erstellt
            FloorChildExtraDataController.InstantiateFCED(floorChildType+";"+floorGameObjectName);
            //FloorChildExtraDataController.getInfo();
        }
    }

    //create object on floor 
    public static void GenerateObjectOnFloor(string type, string objectName, int price, float floorChildCoordCorrectionXA, float floorChildCoordCorrectionYA, float floorChildCoordCorrectionXB, float floorChildCoordCorrectionYB, string floorNameToPlaceOn){
        //übergibt obj data to floor obj, generate new obj, return obj to save in "standardObjectList"
        FloorObject floorObject = getFloorGOFromFloorGOName(floorNameToPlaceOn);
        if(string.IsNullOrWhiteSpace(floorObject.floorChildType)==true&&checkFloorGONameHasDoor(floorNameToPlaceOn)==false){//wenn floorGameObject schon childgameObject hat, erzeuge nicht
            standardObjectList.Add(floorObject.setChild(type, floorNameToPlaceOn, objectName, price, floorChildCoordCorrectionXA, floorChildCoordCorrectionYA, floorChildCoordCorrectionXB, floorChildCoordCorrectionYB));   
        
            //FloorChildExtraData wird erstellt
            FloorChildExtraDataController.InstantiateFCED(type+";"+floorNameToPlaceOn);
            //FloorChildExtraDataController.getInfo();
        }
    }

    //setzt ein neues floorSprite
    public static void NewFloorSprite(string newFloorSpriteName, int floorPrice, string floorGOName){
        FloorObject floorObject = getFloorGOFromFloorGOName(floorGOName);
        if(floorObject.floorName.Equals(newFloorSpriteName)==false){//gucke ob FloorGO nicht schon diesen floor hat
            floorObject.setNewFloorSprite(newFloorSpriteName, floorPrice);
        }
    }

    //rotiert obj on floor obj
    public static void RotateObjectOnFloor(string floorChildGameObjectName){
        //get currentFloorParent from child
        string FloorGameObjectName = getFloorGONameFromChildGOName(floorChildGameObjectName);

        StandardObject child = getFloorGOChildFromChildGOName(floorChildGameObjectName);

        //get floor object durch FloorGameObjectName 
        FloorObject floorObject = getFloorGOFromFloorGOName(FloorGameObjectName);
        floorObject.RotateChild(child);
    }

    //platziert ein floorChildGameObject von ein floorGameObject auf einen anderes floorGameObject und löscht das alte
    public static void MoveObjectOnFloor(string floorChildGameObjectName, string newFloorGameObjectName){
        //get currentFloorParent from child
        string oldFloorGameObjectName = getFloorGONameFromChildGOName(floorChildGameObjectName);

        //hole floorchild data von floorParent
        FloorObject floorObject = getFloorGOFromFloorGOName(oldFloorGameObjectName);
        string floorChildInfo = floorObject.getInfo();
        string[] floorChildInfoItems = floorChildInfo.Split(";");

        //gucke ob neue position bereits ein childObject besitzt, wenn nicht platziere
        for(int c=0;c<FloorObjectList.Count;c++){
            if(FloorObjectList[c].floorGameObjectName.Equals(newFloorGameObjectName)){
                if(string.IsNullOrWhiteSpace(FloorObjectList[c].floorChildType)){

                    //gucke ob FloorObject unter tür ist, wenn ja platziere nicht
                    if(checkFloorGONameHasDoor(newFloorGameObjectName)==false){

                        //floorChildExtraData
                        //muss vor GenerateObjectOnFloor(...) passieren, da hier das FCED geklont wird und nicht neu ohne werte erzeugt wird
                        FloorChildExtraDataController.CloneFECD(floorChildGameObjectName, newFloorGameObjectName);

                        GenerateObjectOnFloor(floorChildInfoItems[5],floorChildInfoItems[7],Int32.Parse(floorChildInfoItems[8]),float.Parse(floorChildInfoItems[9]),float.Parse(floorChildInfoItems[10]),float.Parse(floorChildInfoItems[11]),float.Parse(floorChildInfoItems[12]),newFloorGameObjectName);
                        DestroyFloorChild(floorChildGameObjectName);
                    }
                }
            }
        }
    }

    //lösche FloorChildGameObject aus dem spiel FloorGameObject
    public static void DestroyFloorChild(string childGOName){//(floorChildGameObjectName)
        string floorGOName = getFloorGONameFromChildGOName(childGOName);

        //delete floorChild info von floorGameObject
        FloorObject floorObject = getFloorGOFromFloorGOName(floorGOName);
        floorObject.DeleteChild();

        //delete floorChild von standardObjectList
        for(int b=0;b<standardObjectList.Count;b++){
            if(standardObjectList[b].gameObjectName.Equals(childGOName)){
                standardObjectList.RemoveAt(b);
            }
        }
        //delete floorChild from scene
        Destroy(GameObject.Find(childGOName));

        //floorChildExtraData wird entfernt
        FloorChildExtraDataController.DeleteFECD(childGOName);
    }



    //getter
    public static WallObject getWallGOFromWallGOName(string wallGOName){
        WallObject obj = null;
        for(int a=0;a<WallObjectList.Count;a++){
            if(WallObjectList[a].wallGameObjectName.Equals(wallGOName)){
                obj = WallObjectList[a];
            }
        }
        return obj;
    }

    public static WallObject getWallGOFromChildGOName(string child){
        WallObject obj = null; 
        obj = getWallGOFromWallGOName(child);
        return obj;
    }

    public static WallObject getSmallerNeighbourWallGOFromWallGOName(string wallGOName){
        WallObject obj = new WallObject();
        string[] nameSlice = wallGOName.Split("-");//splitt name
        int val1 = Int32.Parse(nameSlice[0]);//gucke ob eine x,y coord größer als 1 ist
        int val2 = Int32.Parse(nameSlice[1]);
        if(val1>1){
            obj = getWallGOFromWallGOName((val1-1).ToString()+"-"+nameSlice[1]+"-Wall");
        }else if(val2>1){
            obj = getWallGOFromWallGOName(nameSlice[0]+"-"+(val2-1).ToString()+"-Wall");
        }
        return obj;
    }

    public static WallObject getGreaterNeighbourWallGOFromWallGOName(string wallGOName){
        WallObject obj = new WallObject();
        string[] nameSlice = wallGOName.Split("-");//splitt name
        int val1 = Int32.Parse(nameSlice[0]);//gucke ob eine x,y coord größer als 1 ist
        int val2 = Int32.Parse(nameSlice[1]);
        if(val1>1){
            obj = getWallGOFromWallGOName((val1+1).ToString()+"-"+nameSlice[1]+"-Wall");
        }else if(val2>1){
            obj = getWallGOFromWallGOName(nameSlice[0]+"-"+(val2+1).ToString()+"-Wall");
        }
        return obj;
    }

    public static bool checkIfObjectIsDoor(string objName){
        string[] nameSlice = objName.Split("_");//splitt name
        if(nameSlice.Length>2){
            if(nameSlice[1].Equals("Door")){
                return true;
            }
        }
        return false;
    }

    public static bool checkIfDoorOnWallExists(){
        for(int a=0;a<WallObjectList.Count;a++){
            if(string.IsNullOrWhiteSpace(WallObjectList[a].WallChildName)==false&&WallObjectList[a].WallChildName!="placeholder"){//placeholder ist 2tes teil von 2teiligen child
                string[] nameSlice = WallObjectList[a].WallChildName.Split("_");//splitt name
                if(nameSlice[1].Equals("Door")){
                    return true;
                }
            }
        }
        return false;
    }

    public static bool checkIfWallGOContainsDoor(WallObject obj){
        string[] objSlice = obj.WallChildName.Split("_");//splitt name
        if(objSlice.Length>=2){
            if(objSlice[1].Equals("Door")){
                return true;
            }
        }
        return false;
    }



    public static StandardObject getFloorGOChildFromChildGOName(string floorChildGameObjectName){
        StandardObject objChild = null;
        for(int a=0;a<standardObjectList.Count;a++){
            if(standardObjectList[a].gameObjectName.Equals(floorChildGameObjectName)){
                objChild = standardObjectList[a];
            }
        }
        return objChild;
    }

    public static FloorObject getFloorGOFromFloorGOName(string floorGameObjectName){
        FloorObject obj = null;
        for(int a=0;a<FloorObjectList.Count;a++){
            if(FloorObjectList[a].floorGameObjectName.Equals(floorGameObjectName)){
                obj = FloorObjectList[a];
            }
        }
        return obj;
    }

    public static string getFloorGONameFromChildGOName(string floorChildGameObjectName){
        string[] nameSlice = floorChildGameObjectName.Split("-");//splitt name
        string oldFloorGameObjectName = nameSlice[0]+"-"+nameSlice[1];//get old parent
        return oldFloorGameObjectName;
    }

    public static bool checkPotentialDoorPlacementOnFloorFromWallGOName(string wallName){
        string[] nameSlice = wallName.Split("-");//splitt name
        int val1 = Int32.Parse(nameSlice[0]);//gucke ob eine x,y coord größer als 0 ist
        int val2 = Int32.Parse(nameSlice[1]);
        if(val1>0){//generiere floorGOName 
            val1 = val1 - 1;
        }else if(val2>0){
            val2 = val2 - 1;
        }
        //prüft ob der boden auf dem die tür steht frei ist(kein child objecte)
        FloorObject floorObject = getFloorGOFromFloorGOName((val1).ToString()+"-"+(val2).ToString());
        if(string.IsNullOrWhiteSpace(floorObject.floorChildType)==true){
            return true;
        }
        return false;
    }

    public static bool checkFloorGONameHasDoor(string floorGameObjectName){
        //prüfe ob Tür auf FloorGO steht, wenn ja dann true
        string[] nameSlice = floorGameObjectName.Split("-");//splitt name
        int val1 = Int32.Parse(nameSlice[0]);
        int val2 = Int32.Parse(nameSlice[1]);
        if(val1==0&&val2>0){
            val2 = val2 + 1;
            WallObject wallObject = getWallGOFromWallGOName(val1+"-"+val2+"-Wall");
            if(string.IsNullOrWhiteSpace(wallObject.WallChildName)==false){
                if(checkIfWallGOContainsDoor(wallObject)==true){
                    return true;
                }
            }
        }else if(val2==0&&val1>0){
            val1 = val1 + 1;
            WallObject wallObject1 = getWallGOFromWallGOName(val1+"-"+val2+"-Wall");
            if(string.IsNullOrWhiteSpace(wallObject1.WallChildName)==false){
                if(checkIfWallGOContainsDoor(wallObject1)==true){
                    return true;
                }
            }
        }else if(val2==0&&val2==0){
            WallObject wallObject2 = getWallGOFromWallGOName(1+"-"+0+"-Wall");
            if(string.IsNullOrWhiteSpace(wallObject2.WallChildName)==false){
                if(checkIfWallGOContainsDoor(wallObject2)==true){
                    return true;
                }
            }
            WallObject wallObject3 = getWallGOFromWallGOName(0+"-"+1+"-Wall");
            if(string.IsNullOrWhiteSpace(wallObject3.WallChildName)==false){
                if(checkIfWallGOContainsDoor(wallObject3)==true){
                    return true;
                }    
            }
        }
        return false;
    }
}
