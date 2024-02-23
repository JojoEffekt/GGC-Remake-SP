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

            PlayerController.playerName = lines[0];
            PlayerController.playerMoney = Int32.Parse(lines[1]);
            PlayerController.playerGold = Int32.Parse(lines[2]);
            PlayerController.playerXP = long.Parse(lines[3]);
            PlayerController.playerLevel = Int32.Parse(lines[4]);
            Debug.Log("successfully load data!");
        }catch(Exception e){
            Debug.Log("error, failed to load data!");
        }
    }

    public static void SavePlayerData(){
        try{
            StreamWriter source = new StreamWriter(Application.dataPath + "/Data/" + playerDataFilePath);
            source.WriteLine(PlayerController.playerName);
            source.WriteLine(PlayerController.playerMoney);
            source.WriteLine(PlayerController.playerGold);
            source.WriteLine(PlayerController.playerXP);
            source.WriteLine(PlayerController.playerLevel);
            source.Close();
            Debug.Log("successfully save data!");
        }catch(Exception e){
            Debug.Log("error, failed to save data!");
        }
    }
}
