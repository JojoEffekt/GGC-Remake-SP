using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SaveAndLoadController : MonoBehaviour
{
    public static string playerDataFilePath = "playerData.txt";
    public static string wallDataFilePath = "WallData.txt";
    public static string floorDataFilePath = "FloorData.txt";
    public static string floorChildExtraDataFilePath = "FloorChildExtraData.txt";

    public enum DataSheets {
        PlayerData,
        WallData,
        FloorData,
        FloorChildExtraData
    }

    // backupdaten für waiter
    public static string waiterDataBackup; 

    void Start()
    {
        LoadPlayerData();
    }

    public static void LoadPlayerData(){
        //try{
            string[] lines = ReadStream(playerDataFilePath);

            //LOAD PLAYERSTATS
            PlayerController.setPlayerName(lines[0]);
            PlayerController.playerMoney = Int32.Parse(lines[1]);
            PlayerController.playerGold = Int32.Parse(lines[2]);
            PlayerController.playerXP = long.Parse(lines[3]);
            PlayerController.LoadLevelByXp(long.Parse(lines[3]));//load playerlevel by converting xp
            PlayerController.gridSize = Int32.Parse(lines[4]);
            PlayerController.LoadFoodItemDict(lines[5]);
            PlayerController.LoadStorageItemDict(lines[6]);
            PlayerController.LoadObjectLimiterDict(lines[7]);
            PlayerController.LoadPlayerDict(lines[8]);

            //Debug.Log("successfully load Player-data!");
        /*}catch(Exception e){
            Debug.Log("error, failed to load Player-data!");
        }*/

        //wenn wallfile/floorfile exist, lade wallfile/floorfile, ansonsten generiere grid
        //try{
            if(File.Exists(Application.dataPath+"/Data/"+wallDataFilePath)==true&&File.Exists(Application.dataPath+"/Data/"+floorDataFilePath)==true&&File.Exists(Application.dataPath+"/Data/"+floorChildExtraDataFilePath)==true){
                
                //LOAD FLOOREXTRADATA
                //muss zu erst geladen werden, da beim Instanziieren der FloorObjekte sonst leere FCED erzeugt werden
                string[] lines1 = ReadStream(floorChildExtraDataFilePath);
                //lines.Length-1;// -1 because WriteLine generates an empty line on bottom
                for(int a=0;a<lines1.Length-1;a++){
                    string[] lineItem = lines1[a].Split(";");

                    if(lineItem[0].Equals("Chair")){
                        FloorChildExtraDataController.InstantiateFCED(lines1[a]);
                    }
                    if(lineItem[0].Equals("Table")){
                        FloorChildExtraDataController.InstantiateFCED(lines1[a]); 
                    }
                    if(lineItem[0].Equals("Counter")){
                        FloorChildExtraDataController.InstantiateFCED(lines1[a]);
                    }
                    if(lineItem[0].Equals("Oven")){
                        FloorChildExtraDataController.InstantiateFCED(lines1[a]);
                    }
                    if(lineItem[0].Equals("Slushi")){
                        FloorChildExtraDataController.InstantiateFCED(lines1[a]);
                    }
                }
                //Debug.Log("successfully load FloorExtra-data!");


                //LOAD WALL
                lines1 = ReadStream(wallDataFilePath);
                //lines.Length-1: -1 because WriteLine generates an empty line on bottom
                for(int a=0;a<lines1.Length-1;a++){
                    string[] lineItem = lines1[a].Split(";");
                    ObjectController.GenerateWallObject(lineItem[0], lineItem[1],Int32.Parse(lineItem[2]),lineItem[3],Int32.Parse(lineItem[4]),float.Parse(lineItem[5]),float.Parse(lineItem[6]),Int32.Parse(lineItem[7]),Int32.Parse(lineItem[8]));
                }
                //Debug.Log("successfully load Wall-data!");



                //LOAD FLOOR
                lines1 = ReadStream(floorDataFilePath);
                //lines.Length-1: -1 because WriteLine generates an empty line on bottom
                for(int a=0;a<lines1.Length-1;a++){
                    string[] lineItem = lines1[a].Split(";");
                    ObjectController.GenerateFloorObject(lineItem[0],lineItem[1],Int32.Parse(lineItem[2]),float.Parse(lineItem[3]),float.Parse(lineItem[4]),lineItem[5],lineItem[6],lineItem[7],Int32.Parse(lineItem[8]),float.Parse(lineItem[9]),float.Parse(lineItem[10]),float.Parse(lineItem[11]),float.Parse(lineItem[12]));
                }
                //Debug.Log("successfully load Floor-data!");




                //Load FCED dinners on oven/counter
                //lade zum ende die FCED dinners auf dem oven/counter, da floorObjecte benötigt werden
                //kriege alle FCED, gucke ob oven object, lade oven object mit DinnerController.CreateDinnerPrefabOnOven()...
                lines1 = ReadStream(floorChildExtraDataFilePath);
                //lines.Length-1;// -1 because WriteLine generates an empty line on bottom
                for(int a=0;a<lines1.Length-1;a++){
                    string[] lineItem = lines1[a].Split(";");
                    //FCED muss oven sein und darf nicht 0 als step anzahl haben(sonst ist es leer und hat kein dinner drauf)
                    if(lineItem[0].Equals("Oven")&&(Int32.Parse(lineItem[2])!=0)){
                        DinnerController.CreateDinnerPrefabOnOven((lines1[a].Split(";")[1]+"-Child"), lines1[a].Split(";")[3], Int32.Parse(lines1[a].Split(";")[2]));
                    }
                    //FCED des counters, muss essen enthalten
                    if(lineItem[0].Equals("Counter")&&(!lineItem[3].Equals(""))){
                        CounterController.AddDinnerOnCounter(lineItem[1], lineItem[3]);
                    }
                }




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

        //rendert spieler
        PlayerMovementController.LoadPlayer();

        //rendert die waiters
        GameObject.Find("WaiterController").GetComponent<WaiterManager>().InitialisiereWaiter(lines[9]);

        //LADE INITIALDATEN WENN ALLE OBJECTE ETC GELADEN SIND
        //speichert alle geladenene tables/chairs für npcs in einer liste
        GameObject.Find("NPCController").GetComponent<NPCManager>().CollectAllChairsAndTablesInList();
        //läd das grid für npc/player movement
        LabyrinthBuilder.GenerateGrid();
    }

    public static void SavePlayerData(){
        //SAVE PLAYERSTATS
        //try{
            StreamWriter source = new StreamWriter(Application.dataPath + "/Data/" + playerDataFilePath);
            string n = PlayerController.playerName.Remove(PlayerController.playerName.Length-1);
            source.WriteLine(n);
            source.WriteLine(PlayerController.playerMoney);
            source.WriteLine(PlayerController.playerGold);
            source.WriteLine(PlayerController.playerXP);
            source.WriteLine(PlayerController.gridSize);
            source.WriteLine(PlayerController.getFoodItemDictInfo());
            source.WriteLine(PlayerController.getStorageItemDictInfo());
            source.WriteLine(PlayerController.getObjectLimiterDictInfo());
            source.WriteLine(PlayerController.getPlayerDictInfo());
            string waiterData = GameObject.Find("WaiterController").GetComponent<WaiterManager>().WaiterDataToSave();
            if(waiterData.Length!=0){//nur speichern wenn der string nicht leer ist//passiert wenn baumodus offen und kein waiter vorhanden, dann wird leerer string erstellt. in dem fall zwischengespeicherterte daten speihern
                source.WriteLine(waiterData);
                waiterDataBackup = waiterData;
            }else{
                source.WriteLine(waiterDataBackup);
            }
            
            
            source.Close();
            //Debug.Log("successfully save Player-data!");
        /*}catch(Exception e){
            Debug.Log("error, failed to save Player-data!");
        }*/
        
        //SAVE WALL
        try{
            source = new StreamWriter(Application.dataPath + "/Data/" + wallDataFilePath);
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
            source = new StreamWriter(Application.dataPath + "/Data/" + floorDataFilePath);
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
            source.Write(FloorChildExtraDataController.ChairList[a].getData()+"\n");
        }
        for(int b=0;b<FloorChildExtraDataController.TableList.Count;b++){
            source.Write(FloorChildExtraDataController.TableList[b].getData()+"\n");
        }
        for(int c=0;c<FloorChildExtraDataController.CounterList.Count;c++){
            source.Write(FloorChildExtraDataController.CounterList[c].getData()+"\n");
        }
        for(int d=0;d<FloorChildExtraDataController.OvenList.Count;d++){
            source.Write(FloorChildExtraDataController.OvenList[d].getData()+"\n");
        }
        for(int e=0;e<FloorChildExtraDataController.SlushiList.Count;e++){
            source.Write(FloorChildExtraDataController.SlushiList[e].getData()+"\n");
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

    public static string ReadSingleLine(string fileString, int line)
    {
        DataSheets file;
        Enum.TryParse<DataSheets>(fileString, out file);
        switch(file)
        {
            case DataSheets.PlayerData:
                string[] lines = ReadStream(playerDataFilePath);

                if(line<lines.Length)
                {
                    return lines[line];
                }
                break;
        }

        return "";
    }
}
