using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectController : MonoBehaviour
{
    public static List<WallObject> WallObjectList = new List<WallObject>();
    public static List<FloorObject> FloorObjectList = new List<FloorObject>();

    public static List<StandartObject> standartObjectList = new List<StandartObject>();
    

    
    public static void GenerateWallObject(string wallName, int rotation, string wallChildName, int wallChildLength, float wallChildCoordCorrection, int xCoord, int yCoord){
        WallObjectList.Add(new WallObject(wallName, rotation, wallChildName, wallChildLength, wallChildCoordCorrection,xCoord, yCoord));
    }

    public static void GenerateFloorObject(string floorGameObjectName, string floorName, int floorPrice, float floorCoordX, float floorCoordY, string floorChildType, string floorChildName, int floorChildPrice, int floorChildRotation, float floorChildCoordX, float floorChildCoordY){
        FloorObjectList.Add(new FloorObject(floorGameObjectName, floorName, floorPrice, floorCoordX, floorCoordY, floorChildType, floorChildName, floorChildPrice, floorChildRotation, floorChildCoordX, floorChildCoordY));
    }

    //create object on floor 
    public static void GenerateObjectOnFloor(string type, string objectName, int rotation, int price){
        string parentFloorObject;
        float parentFloorCoordX = 0;
        float parentFloorCoordY = 0;
        for(int a=0;a<FloorObjectList.Count;a++){
            if(FloorObjectList[a].getFloorGameObjectName().Equals("5-5")){
                parentFloorCoordX = FloorObjectList[a].getCoordX();
                parentFloorCoordY = FloorObjectList[a].getCoordY();
            }
        }
        standartObjectList.Add(new StandartObject(type, objectName, rotation, price, parentFloorCoordX, parentFloorCoordY));
        //FloorObjectList[a].setChild(type, objectName, price, rotation, coordX, coordY);
        //continue save childobject in dekolist
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
