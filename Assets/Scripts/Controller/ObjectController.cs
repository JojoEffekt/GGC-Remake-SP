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

            for(int a=0;a<FloorObjectList.Count;a++){
                if(FloorObjectList[a].floorGameObjectName.Equals(floorGameObjectName)){//sucht floorObject anhand von floorGameObjectName
                    standardObjectList.Add(FloorObjectList[a].setChild(floorChildType, floorGameObjectName, floorChildName, floorChildPrice, floorChildCoordCorrectionXA, floorChildCoordCorrectionYA, floorChildCoordCorrectionXB, floorChildCoordCorrectionYB));
                }
            }
        }
    }
    //create object on floor 
    public static void GenerateObjectOnFloor(string type, string objectName, int price, float floorChildCoordCorrectionXA, float floorChildCoordCorrectionYA, float floorChildCoordCorrectionXB, float floorChildCoordCorrectionYB, string floorNameToPlaceOn){
        //get floor obj, give floor obj data, create obj in floor obj
        for(int a=0;a<FloorObjectList.Count;a++){
            if(FloorObjectList[a].floorGameObjectName.Equals(floorNameToPlaceOn)){//sucht floorObject anhand von floorGameObjectName
                if(string.IsNullOrWhiteSpace(FloorObjectList[a].floorChildType)==true){//wenn floorGameObject schon childgameObject hat, erzeuge nicht
                    //übergibt obj data to floor obj, generate new obj, return obj to save in "standardObjectList"
                    standardObjectList.Add(FloorObjectList[a].setChild(type, floorNameToPlaceOn, objectName, price, floorChildCoordCorrectionXA, floorChildCoordCorrectionYA, floorChildCoordCorrectionXB, floorChildCoordCorrectionYB));   
                }
            }
        }
    }
    //rotiert obj on floor obj
    public static void RotateObjectOnFloor(string floorChildGameObjectName){
        //cut string to get floorGameObject
        string[] nameSlice = floorChildGameObjectName.Split("-");
        StandardObject child = null;

        //get floorchild object durch floorChildGameObjectName
        for(int a=0;a<standardObjectList.Count;a++){
            if(standardObjectList[a].gameObjectName.Equals(floorChildGameObjectName)){
                child = standardObjectList[a];
            }
        }
        //get floor object durch floorChildGameObjectName
        for(int b=0;b<FloorObjectList.Count;b++){
            if((nameSlice[0]+"-"+nameSlice[1]).Equals(FloorObjectList[b].floorGameObjectName)){
                //Debug.Log("rotate: "+FloorObjectList[b].getInfo());
                FloorObjectList[b].RotateChild(child);
            }
        }
    }

    //platziert ein floorChildGameObject von ein floorGameObject auf einen anderes floorGameObject
    public static void MoveObjectOnFloor(string floorChildGameObjectName, string newFloorGameObjectName){
        string[] nameSlice = floorChildGameObjectName.Split("-");//splitt name
        string floorChildInfo = "";
        string oldFloorGameObjectName = nameSlice[0]+"-"+nameSlice[1];//get old parent
        string[] floorChildInfoItems = new string[100];

        //hole floorchild data von floorParent
        for(int b=0;b<FloorObjectList.Count;b++){
            if(FloorObjectList[b].floorGameObjectName.Equals(oldFloorGameObjectName)){
                floorChildInfo = FloorObjectList[b].getInfo();
                floorChildInfoItems = floorChildInfo.Split(";");
            }
        }

        //check if new FloorGameObject hat bereits FloorChildGameObject, wenn nicht generiere neuen child
        for(int c=0;c<FloorObjectList.Count;c++){
            if(FloorObjectList[c].floorGameObjectName.Equals(newFloorGameObjectName)){
                if(string.IsNullOrWhiteSpace(FloorObjectList[c].floorChildType)){
                    Debug.Log("Empty, can place new child");
                    
                    //platziere FloorChildGameObject all data neu
                    /*


                    Falsch oder neu nmachen
                    entweder neues Object generiernen und altes vollständig löschen,

                    oder object moven, dann aber neue methoden in Standartobject


                    */
                    GenerateObjectOnFloor(floorChildInfoItems[5],floorChildInfoItems[7],Int32.Parse(floorChildInfoItems[8]),float.Parse(floorChildInfoItems[9]),float.Parse(floorChildInfoItems[10]),float.Parse(floorChildInfoItems[11]),float.Parse(floorChildInfoItems[12]),newFloorGameObjectName);
                    //löscht child von alten floorGO
                    DeleteChildFromFloor(oldFloorGameObjectName);
                }else{
                    Debug.Log("cant place new child");
                }
            }
        }
    }

    //lösche FloorChildGameObject aus FloorGameObject
    public static void DeleteChildFromFloor(string floorGOName){//(floorGameObjectName)
        for(int a=0;a<FloorObjectList.Count;a++){
            if(FloorObjectList[a].floorGameObjectName.Equals(floorGOName)){
                FloorObjectList[a].DeleteChild();
                Debug.Log("delete child from old obj");
            }
        }
    }

    

    public static void PrintWallObjectList(){
        for(int a=0;a<WallObjectList.Count;a++){
            WallObjectList[a].Info();
        }
    }

    public static void PrintFloorObjectList(){
        for(int a=0;a<FloorObjectList.Count;a++){
            FloorObjectList[a].Info();
        }
    }
}
