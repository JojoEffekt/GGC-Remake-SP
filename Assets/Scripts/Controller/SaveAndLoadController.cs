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
    private static string floorChildExtraDataFilePath = "FloorChildExtraData.txt";

    void Start()
    {
        LoadPlayerData();
    }

    public static void LoadPlayerData(){
        try{
            string[] lines = ReadStream(playerDataFilePath);

            //LOAD PLAYERSTATS
            PlayerController.playerName = lines[0];
            PlayerController.playerMoney = Int32.Parse(lines[1]);
            PlayerController.playerGold = Int32.Parse(lines[2]);
            PlayerController.playerXP = long.Parse(lines[3]);
            PlayerController.playerLevel = Int32.Parse(lines[4]);
            PlayerController.gridSize = Int32.Parse(lines[5]);

            //Debug.Log("successfully load Player-data!");
        }catch(Exception e){
            Debug.Log("error, failed to load Player-data!");
        }

        //wenn wallfile/floorfile exist, lade wallfile/floorfile, ansonsten generiere grid
       // try{
            if(File.Exists(Application.dataPath+"/Data/"+wallDataFilePath)==true&&File.Exists(Application.dataPath+"/Data/"+floorDataFilePath)==true){
                
                //LOAD FLOOREXTRADATA
                //muss zu erst geladen werden, da beim Instanziieren der FloorObjekte sonst leere FCED erzeugt werden
                lines = ReadStream(floorChildExtraDataFilePath);
                //lines.Length-1;// -1 because WriteLine generates an empty line on bottom
                for(int a=0;a<lines.Length-1;a++){
                    string[] lineItem = lines[a].Split(";");

                    if(lineItem[0].Equals("Chair")){
                        FloorChildExtraDataController.InstantiateFCED(lines[a]);
                    }
                    if(lineItem[0].Equals("Table")){
                        FloorChildExtraDataController.InstantiateFCED(lines[a]); 
                    }
                    if(lineItem[0].Equals("Counter")){
                        FloorChildExtraDataController.InstantiateFCED(lines[a]);
                    }
                    if(lineItem[0].Equals("Oven")){
                        FloorChildExtraDataController.InstantiateFCED(lines[a]);
                    }
                    if(lineItem[0].Equals("Slushi")){
                        FloorChildExtraDataController.InstantiateFCED(lines[a]);
                    }
                }
                //Debug.Log("successfully load FloorExtra-data!");


                //LOAD WALL
                string[] lines = ReadStream(wallDataFilePath);
                //lines.Length-1: -1 because WriteLine generates an empty line on bottom
                for(int a=0;a<lines.Length-1;a++){
                    string[] lineItem = lines[a].Split(";");
                    ObjectController.GenerateWallObject(lineItem[0], lineItem[1],Int32.Parse(lineItem[2]),lineItem[3],Int32.Parse(lineItem[4]),float.Parse(lineItem[5]),float.Parse(lineItem[6]),Int32.Parse(lineItem[7]),Int32.Parse(lineItem[8]));
                }
                //Debug.Log("successfully load Wall-data!");



                //LOAD FLOOR
                lines = ReadStream(floorDataFilePath);
                //lines.Length-1: -1 because WriteLine generates an empty line on bottom
                for(int a=0;a<lines.Length-1;a++){
                    string[] lineItem = lines[a].Split(";");
                    ObjectController.GenerateFloorObject(lineItem[0],lineItem[1],Int32.Parse(lineItem[2]),float.Parse(lineItem[3]),float.Parse(lineItem[4]),lineItem[5],lineItem[6],lineItem[7],Int32.Parse(lineItem[8]),float.Parse(lineItem[9]),float.Parse(lineItem[10]),float.Parse(lineItem[11]),float.Parse(lineItem[12]));
                }
                //Debug.Log("successfully load Floor-data!");



                //testing save data
                SavePlayerData();
            }else{
                //load grid if no save exist
                Debug.Log("no Wall/Floor-data, generate grid for the first time!");
                GridController.GenerateGrid();
            }
        /*}catch(Exception e){
            Debug.Log("error, failed to load Wall/Floor-data!");
        }*/
    }

    public static void SavePlayerData(){
        //SAVE PLAYERSTATS
        try{
            StreamWriter source = new StreamWriter(Application.dataPath + "/Data/" + playerDataFilePath);
            string n = PlayerController.playerName.Remove(PlayerController.playerName.Length-1);
            source.WriteLine(n);
            source.WriteLine(PlayerController.playerMoney);
            source.WriteLine(PlayerController.playerGold);
            source.WriteLine(PlayerController.playerXP);
            source.WriteLine(PlayerController.playerLevel);
            source.WriteLine(PlayerController.gridSize);
            
            source.Close();
            //Debug.Log("successfully save Player-data!");
        }catch(Exception e){
            Debug.Log("error, failed to save Player-data!");
        }
        
        //SAVE WALL
        try{
            StreamWriter source = new StreamWriter(Application.dataPath + "/Data/" + wallDataFilePath);
            for(int a=0;a<ObjectController.WallObjectList.Count;a++){
                string wallObject = ObjectController.WallObjectList[a].getInfo();
                source.WriteLine(wallObject);
            }
            source.Close();
            //Debug.Log("successfully save Wall-data!");
        }catch(Exception e){
            Debug.Log("error, failed to save Wall-data!");
        }

        //SAVE FLOOR
        try{
            StreamWriter source = new StreamWriter(Application.dataPath + "/Data/" + floorDataFilePath);
            for(int a=0;a<ObjectController.FloorObjectList.Count;a++){
                string floorObject = ObjectController.FloorObjectList[a].getInfo();
                source.WriteLine(floorObject);
            }
            source.Close();
            //Debug.Log("successfully save Floor-data!");
        }catch(Exception e){
            Debug.Log("error, failed to save Floor-data!");
        }

        //SAVE FLOORCHILDEXTRADATA
        SaveFloorChildExtraData(); //NICHT GETESTET
    }

    private static void SaveFloorChildExtraData(){
        StreamWriter source = new StreamWriter(Application.dataPath + "/Data/" + floorChildExtraDataFilePath);
        for(int a=0;a<FloorChildExtraDataController.ChairList.Count;a++){
            source.WriteLine(FloorChildExtraDataController.ChairList[a].Info());
        }
        for(int b=0;b<FloorChildExtraDataController.TableList.Count;b++){
            source.WriteLine(FloorChildExtraDataController.TableList[b].Info());
        }
        for(int c=0;c<FloorChildExtraDataController.CounterList.Count;c++){
            source.WriteLine(FloorChildExtraDataController.CounterList[c].Info());
        }
        for(int d=0;d<FloorChildExtraDataController.OvenList.Count;d++){
            source.WriteLine(FloorChildExtraDataController.OvenList[d].Info());
        }
        for(int e=0;e<FloorChildExtraDataController.SlushiList.Count;e++){
            source.WriteLine(FloorChildExtraDataController.SlushiList[e].Info());
        }
        source.Close();
    }

    private static string[] ReadStream(string pathFile){
        StreamReader source = new StreamReader(Application.dataPath + "/Data/" + pathFile);
        string fileContents = source.ReadToEnd();
        source.Close();
        string[] lines = fileContents.Split("\n"[0]);
        return lines;
    }
}
