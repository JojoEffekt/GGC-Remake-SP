using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectController : MonoBehaviour
{
    public static List<WallObject> WallObjectList = new List<WallObject>();
    public static List<FloorObject> FloorObjectList = new List<FloorObject>();

    public static List<DekorationObject> DekorationObjectList = new List<DekorationObject>();
    

    
    public static void GenerateWallObject(string wallName, int rotation, string wallChildName, int wallChildLength, float wallChildCoordCorrection, int xCoord, int yCoord){
        WallObjectList.Add(new WallObject(wallName, rotation, wallChildName, wallChildLength, wallChildCoordCorrection,xCoord, yCoord));
    }

    public static void GenerateFloorObject(int xCoord, int yCoord){
        FloorObjectList.Add(new FloorObject("standart_floor", 10, xCoord, yCoord));
    }

    public static void GenerateObjectOnFloor(string type, string objectName, int price, int coordX, int coordY){
        //check if floor empty
        //
        if(type=="Dekoration"){
            DekorationObjectList.Add(new DekorationObject(type, objectName, 1, price, coordX, coordY));

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
