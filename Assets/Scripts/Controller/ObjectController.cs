using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectController : MonoBehaviour
{
    public static List<WallObject> WallObjectList = new List<WallObject>();
    public static List<FloorObject> FloorObjectList = new List<FloorObject>();

    public static List<StandardObject> standartObjectList = new List<StandardObject>();
    

    
    public static void GenerateWallObject(string wallName, int rotation, string wallChildName, int wallChildLength, float wallChildCoordCorrection, int xCoord, int yCoord){
        WallObjectList.Add(new WallObject(wallName, rotation, wallChildName, wallChildLength, wallChildCoordCorrection,xCoord, yCoord));
    }

    public static void GenerateFloorObject(string floorGameObjectName, string floorName, int floorPrice, float floorCoordX, float floorCoordY, string floorChildType, string floorChildName, int floorChildPrice, int floorChildRotation, float floorChildCoordCorrectionX, float floorChildCoordCorrectionY){
        FloorObjectList.Add(new FloorObject(floorGameObjectName, floorName, floorPrice, floorCoordX, floorCoordY, floorChildType, floorChildName, floorChildPrice, floorChildRotation, floorChildCoordCorrectionX, floorChildCoordCorrectionY));

        //generiert child from floor wenn floor obj child hat
        if(string.IsNullOrWhiteSpace(floorChildType)==false){
            //
            //Continue
            //Übergebe floor child parent coord name to create floor child go name as identifier
            //
            GenerateObjectOnFloor(floorChildType, floorChildName, floorChildRotation, floorChildPrice, floorChildCoordCorrectionX, floorChildCoordCorrectionY, floorGameObjectName);
        }
    }



    //create object on floor 
    public static void GenerateObjectOnFloor(string type, string objectName, int rotation, int price, float floorChildCoordCorrectionX, float floorChildCoordCorrectionY, string floorNameToPlaceOn){
        //get floor obj, give floor obj data, create obj in floor obj
        for(int a=0;a<FloorObjectList.Count;a++){
            if(FloorObjectList[a].floorGameObjectName.Equals(floorNameToPlaceOn)){
                //übergibt obj data to floor obj, generate new obj, return obj to save in "standartObjectList"
                standartObjectList.Add(FloorObjectList[a].setChild(type, objectName, rotation, price, floorChildCoordCorrectionX, floorChildCoordCorrectionY));
            }
        }
    }

    //rotiert obj on floor obj
    public static void RotateObjectOnFloor(string objectName){
        for(int a=0;a<standartObjectList.Count;a++){
            if(standartObjectList[a].objectName.Equals(objectName)){
                standartObjectList[a].Rotate();
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
