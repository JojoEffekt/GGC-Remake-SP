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
                //(a*2-2.1f)-(b*2-2),-3.75f-b-a
                ObjectController.GenerateFloorObject((a+"-"+b),"Floor_01", 10, ((a-b)*2)-0.1f, -3.75f-b-a, null, null, 0, 1, 0.0f, 0.0f);
                //floorName, floorPrice, floorCoordX, floorCoordY, floorChildType, floorChildName, floorChildPrice, floorChildRotation, floorChildCoordCorrectionX, floorChildCoordCorrectionY
            }
            //create wall
            ObjectController.GenerateWallObject("Wall_09_", 0, null, 0, 0.1f,a+1, 0);
            ObjectController.GenerateWallObject("Wall_09_", 1, null, 0, 0.1f,0, a+1);
        }

        ObjectController.GenerateObjectOnFloor("Deko", "Deko_11_1_a", 1, 10, -0.2f, 2.4f, "5-5");
        ObjectController.RotateObjectOnFloor("Deko_11_1_a");
        ObjectController.RotateObjectOnFloor("Deko_11_1_a");
        ObjectController.GenerateObjectOnFloor("Deko", "Deko_11_1_a", 1, 10, -0.2f, 2.4f, "7-1");


        SaveAndLoadController.SavePlayerData();
    }
}