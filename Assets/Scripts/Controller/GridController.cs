using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public static void GenerateGrid(){
        int gridSize = PlayerController.gridSize;

        for(int a=0;a<gridSize;a++){
            for(int b=0;b<gridSize;b++){
                //create floor
                ObjectController.GenerateFloorObject(a, b);
            }
            //create wall
            ObjectController.GenerateWallObject("Wall_05_1_", 0, "Wall_Deko_06_1_", 0, 0.1f,a+1, 0);
            ObjectController.GenerateWallObject("Wall_05_1_", 1, "Wall_Deko_06_1_", 0, 0.1f,0, a+1);
        }


        //ObjectController.PrintWallObjectList();
        //ObjectController.PrintFloorObjectList();
        //ObjectController.WallObjectList[5].setWallName("Wall_05_1_");
        ObjectController.WallObjectList[5].setWallChildName("Wall_Deko_10_1_");
    }
}
