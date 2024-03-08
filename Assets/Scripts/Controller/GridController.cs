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
                //floorName, floorPrice, floorCoordX, floorCoordY, floorChildType, floorChildGameObjectName, floorChildName, floorChildPrice, floorChildRotation, floorChildCoordCorrectionXA, floorChildCoordCorrectionYA, floorChildCoordCorrectionXB, floorChildCoordCorrectionYB, typeData
            }
            //create wall
            ObjectController.GenerateWallObject((a+1)+"-"+0+"-Wall" ,"Wall_09_", 0, null, 0, 0.0f, 0.0f,a+1, 0);
            ObjectController.GenerateWallObject(0+"-"+(a+1)+"-Wall" ,"Wall_09_", 1, null, 0, 0.0f, 0.0f, 0, a+1);
        }
        ObjectController.GenerateObjectOnWall("Wall_Door_01_1_", "6-0-Wall", 1, 0.2f, -0.5f);
        ObjectController.GenerateObjectOnWall("Wall_Door_06_1_", "0-6-Wall", 1, 0.1f, -0.75f);
        ObjectController.GenerateObjectOnWall("Wall_Deko_02_1_", "0-2-Wall", 1, 0.75f, 1.0f);//(floorChildName,WallName,wallChildLength,coordCorrectionX,coordCorrectionY)
        ObjectController.GenerateObjectOnWall("Wall_Deko_09_", "0-5-Wall", 3, 1.15f, 1.0f);//(floorChildName,WallName,wallChildLength,coordCorrectionX,coordCorrectionY)
        ObjectController.GenerateObjectOnWall("Wall_Deko_10_1_", "4-0-Wall", 1, 0.75f, 1.0f);
        ObjectController.GenerateObjectOnWall("Wall_Deko_09_", "5-0-Wall", 3, 1.15f, 1.0f);//(floorChildName,WallName,wallChildLength,coordCorrectionX,coordCorrectionY)
        ObjectController.GenerateObjectOnWall("Wall_Deko_02_1_", "2-0-Wall", 1, 0.75f, 1.0f);//(floorChildName,WallName,wallChildLength,coordCorrectionX,coordCorrectionY)
        ObjectController.GenerateObjectOnWall("Wall_Deko_09_", "1-0-Wall", 3, 1.15f, 1.0f);
        ObjectController.DestroyObjectOnWall("0-2-Wall");//(WallName)




        ObjectController.GenerateObjectOnFloor("Deko", "Deko_11_1_a", 10, -0.2f, 2.05f, 0.15f, 2.05f, "5-5");//(type,spriteName,price,coordCoorXA...-coordCoorYB,FloorGameObjectName)
        ObjectController.GenerateObjectOnFloor("Fridge", "Fridge_04_1_a", 10, 0.0f, 2.1f, 0.0f, 2.1f, "2-4");//(type,spriteName,price,coordCoorXA...-coordCoorYB,FloorGameObjectName)
        ObjectController.GenerateObjectOnFloor("Counter", "Counter_06_1_d", 10, 0.0f, 0.75f, 0.0f, 0.75f, "1-7");
        ObjectController.GenerateObjectOnFloor("Oven", "Oven_04_1_a", 10, 0.0f, 1.0f, 0.0f, 1.0f, "7-3");
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


        ObjectController.MoveObjectOnWall("0-4-Wall","8-0-Wall");//(curWallName,newWallName)
        ObjectController.MoveObjectOnWall("6-0-Wall","8-0-Wall");//(curWallName,newWallName)
        ObjectController.MoveObjectOnWall("6-0-Wall","0-8-Wall");//(curWallName,newWallName)
        ObjectController.MoveObjectOnWall("0-8-Wall","0-1-Wall");//(curWallName,newWallName)


        SaveAndLoadController.SavePlayerData();
    }
}
