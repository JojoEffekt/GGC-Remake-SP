using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SaveAndLoadController : MonoBehaviour
{
    private static string playerDataFilePath = "playerData.txt";
    private static string wallDataFilePath = "WallData.txt";
    private static string floorDataFilePath = "FloorData.txt";

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

            Debug.Log("successfully load Player-data!");
        }catch(Exception e){
            Debug.Log("error, failed to load Player-data!");
        }

        //wenn wallfile/floorfile exist, lade wallfile/floorfile, ansonsten genriere grid
        try{
            if(File.Exists(Application.dataPath+"/Data/"+wallDataFilePath)==true&&File.Exists(Application.dataPath+"/Data/"+floorDataFilePath)==true){
                StreamReader source = new StreamReader(Application.dataPath + "/Data/" + wallDataFilePath);
                string fileContents = source.ReadToEnd();
                source.Close();
                string[] lines = fileContents.Split("\n"[0]);
                //load wall data
                //lines.Length-1: -1 because WriteLine generates an empty line on bottom
                for(int a=0;a<lines.Length-1;a++){
                    string[] lineItem = lines[a].Split(";");
                    ObjectController.GenerateWallObject(lineItem[0],Int32.Parse(lineItem[1]),lineItem[2],Int32.Parse(lineItem[3]),float.Parse(lineItem[4]),Int32.Parse(lineItem[5]),Int32.Parse(lineItem[6]));
                }
                Debug.Log("successfully load Wall-data!");



                source = new StreamReader(Application.dataPath + "/Data/" + floorDataFilePath);
                fileContents = source.ReadToEnd();
                source.Close();
                lines = fileContents.Split("\n"[0]);
                //load floor data
                //lines.Length-1: -1 because WriteLine generates an empty line on bottom
                for(int a=0;a<lines.Length-1;a++){
                    string[] lineItem = lines[a].Split(";");
                    ObjectController.GenerateFloorObject(lineItem[0],lineItem[1],Int32.Parse(lineItem[2]),float.Parse(lineItem[3]),float.Parse(lineItem[4]),lineItem[5],lineItem[6],Int32.Parse(lineItem[7]),Int32.Parse(lineItem[8]),float.Parse(lineItem[9]),float.Parse(lineItem[10]));
                }
                Debug.Log("successfully load Floor-data!");



                //testing save data
                SavePlayerData();
            }else{
                //load grid if no save exist
                GridController.GenerateGrid();
                Debug.Log("no Wall/Floor-data, generate grid for the first time!");
            }
        }catch(Exception e){
            Debug.Log("error, failed to load Wall/Floor-data!");
        }
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
            Debug.Log("successfully save Player-data!");
        }catch(Exception e){
            Debug.Log("error, failed to save Player-data!");
        }

        try{
            //save wall info
            StreamWriter source = new StreamWriter(Application.dataPath + "/Data/" + wallDataFilePath);
            for(int a=0;a<ObjectController.WallObjectList.Count;a++){
                string wallObject = ObjectController.WallObjectList[a].getInfo();
                source.WriteLine(wallObject);
            }
            source.Close();
            Debug.Log("successfully save Wall-data!");
        }catch(Exception e){
            Debug.Log("error, failed to save Wall-data!");
        }

        try{
            //save floor info
            StreamWriter source = new StreamWriter(Application.dataPath + "/Data/" + floorDataFilePath);
            for(int a=0;a<ObjectController.FloorObjectList.Count;a++){
                string floorObject = ObjectController.FloorObjectList[a].getInfo();
                source.WriteLine(floorObject);
            }
            source.Close();
            Debug.Log("successfully save Floor-data!");
        }catch(Exception e){
            Debug.Log("error, failed to save Floor-data!");
        } 
    }
}
