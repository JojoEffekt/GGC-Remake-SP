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
                ObjectController.GenerateFloorObject("standart_floor", 0, ((a-b)*2)-0.1f, -3.75f-b-a);
            }
            //create wall
            ObjectController.GenerateWallObject("Wall_05_1_", 0, "Wall_Deko_06_1_", 0, 0.1f,a+1, 0);
            ObjectController.GenerateWallObject("Wall_05_1_", 1, "Wall_Deko_06_1_", 0, 0.1f,0, a+1);
        }
        SaveAndLoadController.SavePlayerData();
    }
}
