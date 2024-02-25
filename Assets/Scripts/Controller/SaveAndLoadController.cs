using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SaveAndLoadController : MonoBehaviour
{
    private static string playerDataFilePath = "playerData.txt";

    void Start()
    {
        LoadPlayerData();
    }

    public static void LoadPlayerData(){
        try{
            StreamReader source = new StreamReader(Application.dataPath + "/Data/" + playerDataFilePath);
            string fileContents = source.ReadToEnd();
            source.Close();
            string[] lines = fileContents.Split("\n"[0]);

            //load player stats
            PlayerController.playerName = lines[0];
            PlayerController.playerMoney = Int32.Parse(lines[1]);
            PlayerController.playerGold = Int32.Parse(lines[2]);
            PlayerController.playerXP = long.Parse(lines[3]);
            PlayerController.playerLevel = Int32.Parse(lines[4]);
            PlayerController.gridSize = Int32.Parse(lines[5]);

            //load grid
            //GridController.GenerateGrid();

            Debug.Log("successfully load data!");
        }catch(Exception e){
            Debug.Log("error, failed to load data!");
        }

        //load grid
            GridController.GenerateGrid();
    }

    public static void SavePlayerData(){
        try{
            StreamWriter source = new StreamWriter(Application.dataPath + "/Data/" + playerDataFilePath);

            //save player stats
            string n = PlayerController.playerName.Remove(PlayerController.playerName.Length-1);
            source.WriteLine(n);
            source.WriteLine(PlayerController.playerMoney);
            source.WriteLine(PlayerController.playerGold);
            source.WriteLine(PlayerController.playerXP);
            source.WriteLine(PlayerController.playerLevel);
            source.WriteLine(PlayerController.gridSize);
            
            source.Close();
            Debug.Log("successfully save data!");
        }catch(Exception e){
            Debug.Log("error, failed to save data!");
        }

        try{
            //save 
            StreamWriter source = new StreamWriter(Application.dataPath + "/Data/WallData.txt");
            for(int a=0;a<ObjectController.WallObjectList.Count;a++){
                string n = ObjectController.WallObjectList[a].getWallName();


            
                source.WriteLine(n);
                
            }
            source.Close();
        }catch(Exception e){

        } 
    }
}

public class WallData{

}
