using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectController : MonoBehaviour
{
    public static List<WallObject> WallObjectList = new List<WallObject>();
    public static List<FloorObject> FloorObjectList = new List<FloorObject>();

    public static List<StandardObject> standardObjectList = new List<StandardObject>();
    

    
    public static void GenerateWallObject(string wallName, int rotation, string wallChildName, int wallChildLength, float wallChildCoordCorrection, int xCoord, int yCoord){
        WallObjectList.Add(new WallObject(wallName, rotation, wallChildName, wallChildLength, wallChildCoordCorrection,xCoord, yCoord));
    }



    //generiert gespeicherte FloorObjects und deren childs
    public static void GenerateFloorObject(string floorGameObjectName, string floorName, int floorPrice, float floorCoordX, float floorCoordY, string floorChildType, string floorChildGameObjectName, string floorChildName, int floorChildPrice, float floorChildCoordCorrectionXA, float floorChildCoordCorrectionYA, float floorChildCoordCorrectionXB, float floorChildCoordCorrectionYB){
        FloorObjectList.Add(new FloorObject(floorGameObjectName, floorName, floorPrice, floorCoordX, floorCoordY, floorChildType, floorChildGameObjectName, floorChildName, floorChildPrice, floorChildCoordCorrectionXA, floorChildCoordCorrectionYA, floorChildCoordCorrectionXB, floorChildCoordCorrectionYB));

        //generiert child from floor wenn floor obj child hat
        if(string.IsNullOrWhiteSpace(floorChildType)==false){
            //Übergebe floor child parent coord name to create floor child go name as identifier

            FloorObject floorObject = getFloorGOFromFloorGOName(floorGameObjectName);
            standardObjectList.Add(floorObject.setChild(floorChildType, floorGameObjectName, floorChildName, floorChildPrice, floorChildCoordCorrectionXA, floorChildCoordCorrectionYA, floorChildCoordCorrectionXB, floorChildCoordCorrectionYB));
        }
    }

    //create object on floor 
    public static void GenerateObjectOnFloor(string type, string objectName, int price, float floorChildCoordCorrectionXA, float floorChildCoordCorrectionYA, float floorChildCoordCorrectionXB, float floorChildCoordCorrectionYB, string floorNameToPlaceOn){
        //übergibt obj data to floor obj, generate new obj, return obj to save in "standardObjectList"
        FloorObject floorObject = getFloorGOFromFloorGOName(floorNameToPlaceOn);
        if(string.IsNullOrWhiteSpace(floorObject.floorChildType)==true){//wenn floorGameObject schon childgameObject hat, erzeuge nicht
            standardObjectList.Add(floorObject.setChild(type, floorNameToPlaceOn, objectName, price, floorChildCoordCorrectionXA, floorChildCoordCorrectionYA, floorChildCoordCorrectionXB, floorChildCoordCorrectionYB));   
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
                    GenerateObjectOnFloor(floorChildInfoItems[5],floorChildInfoItems[7],Int32.Parse(floorChildInfoItems[8]),float.Parse(floorChildInfoItems[9]),float.Parse(floorChildInfoItems[10]),float.Parse(floorChildInfoItems[11]),float.Parse(floorChildInfoItems[12]),newFloorGameObjectName);
                    DestroyFloorChild(floorChildGameObjectName);
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
}
