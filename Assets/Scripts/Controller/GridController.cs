using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public static void GenerateGrid(){
        //create grid for the first time, after you can load data from saveandload
        int gridSize = PlayerController.gridSize;

        for(int a=0;a<gridSize;a++){
            for(int b=0;b<gridSize;b++){
                //create floor
                //((a*2)-2.1f)-((b*2)-2),-3.75f+((-b)+1)+((-a)+1)-2
                ObjectController.GenerateFloorObject((a+"-"+b),"Floor_01", 10, ((a-b)*2)-0.1f, -3.75f-b-a, null, null, null, 0, 0.0f, 0.0f, 0.0f, 0.0f);
                //floorName, floorPrice, floorCoordX, floorCoordY, floorChildType, floorChildGameObjectName, floorChildName, floorChildPrice, floorChildRotation, floorChildCoordCorrectionXA, floorChildCoordCorrectionYA, floorChildCoordCorrectionXB, floorChildCoordCorrectionYB
            }
            //create wall
            ObjectController.GenerateWallObject((a+1)+"-"+0+"-Wall" ,"Wall_09_", 0, "Wall_Deko_01_1_", 0, 0.1f,a+1, 0);
            ObjectController.GenerateWallObject(0+"-"+(a+1)+"-Wall" ,"Wall_09_", 1, null, 0, 0.1f,0, a+1);
        }

        ObjectController.GenerateObjectOnWall("Wall_Deko_02_1_", "0-2-Wall");//(floorChildName,WallName)
        ObjectController.GenerateObjectOnWall("Wall_Deko_02_1_", "2-0-Wall");//(floorChildName,WallName)




        ObjectController.GenerateObjectOnFloor("Deko", "Deko_11_1_a", 10, -0.2f, 2.4f, 0.15f, 2.4f, "5-5");//(type,spriteName,price,coordCoorXA...-coordCoorYB,FloorGameObjectName)
        ObjectController.RotateObjectOnFloor("5-5-Child");//(floorChildGameObjectName)
        ObjectController.GenerateObjectOnFloor("Deko", "Deko_11_1_a", 10, -0.2f, 2.4f, 0.15f, 2.4f, "7-1");
        ObjectController.GenerateObjectOnFloor("GOGO", "Deko_06_1_b", 100, -0.2f, 2.4f, 0.15f, 2.4f, "7-1");
        ObjectController.MoveObjectOnFloor("7-1-Child","1-1");//(floorChildGameObjectName,floorGameObjectName(neuer platz))
        ObjectController.RotateObjectOnFloor("1-1-Child");//(floorChildGameObjectName)
        ObjectController.MoveObjectOnFloor("5-5-Child","1-1");
        ObjectController.MoveObjectOnFloor("1-1-Child","0-5");
        ObjectController.GenerateObjectOnFloor("Deko", "Deko_04_1_a", 100, 0.0f, 2.95f, 0.0f, 2.95f, "0-0");
        ObjectController.RotateObjectOnFloor("0-0-Child");
        ObjectController.DestroyFloorChild("0-5-Child");//(floorChildGameObjectName)


        SaveAndLoadController.SavePlayerData();
    }
}
