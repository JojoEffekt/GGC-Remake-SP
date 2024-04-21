using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static string playerName;//Name
    public static int playerMoney;//0
    public static int playerGold;//0
    public static long playerXP;//0
    public static int gridSize;//8
    public static Dictionary<string, int> FoodItemDict = new Dictionary<string, int>();//Empty
    public static Dictionary<string, int> StorageItemDict = new Dictionary<string, int>();//Empty / speicher Informationen welches Objekt und wie oft auf reserve
    public static Dictionary<string, int> ObjectLimiterDict = new Dictionary<string, int>();//Oven:3;Fridge:1;Counter:2;Slushi:1 / speicher wieviele Fridges,Counter,Oven and Slushies you can place
    public static Dictionary<string, string> PlayerDict = new Dictionary<string, string>();//Gender:true;Hat:255-255-255-255;Face:255-255-255-255;Hair255-255-255-255;HairOverlay255-255-255-255;Leg255-255-255-255;LegOverlay255-255-255-255;Skin255-255-255-255;SkinOverlay255-255-255-255;Tshirt255-255-255-255;TshirtOverlay:255-255-255-255 /speichert die informationen der kleidung und gender (gender true=boy)

    /*  
    JojoEffekt
    1500
    10
    100
    7
    Empty
    Empty
    Oven:3;Fridge:1;Counter:2;Shlushi:1
    Gender:true;Hat:255-255-255-255;Face:255-255-255-255;Hair:255-255-255-255;HairOverlay:255-255-255-255;Leg:255-255-255-255;LegOverlay:255-255-255-255;Skin:255-255-255-255;SkinOverlay:255-255-255-255;Tshirt:255-255-255-255;TshirtOverlay:255-255-255-255
    */
     
    public static int playerLevel;//wird durch xp erzeugt
    public static int FridgeStoragePlace;//wird durch LoadFoodItemDict erzeugt/ platz im kühlschrank
    
    //referenzen auf main UI txtObjekte
    public static TMPro.TextMeshProUGUI tmpLevel;
    public static TMPro.TextMeshProUGUI tmpXp;
    public static TMPro.TextMeshProUGUI tmpMoney;
    public static TMPro.TextMeshProUGUI tmpGold;
    public static GameObject imgXPBar;

    //läd referenzen beim ersten start
    public void Start(){
        tmpLevel = GameObject.Find("txtLevelStatic").GetComponent<TMPro.TextMeshProUGUI>();
        tmpXp = GameObject.Find("txtXPStatic").GetComponent<TMPro.TextMeshProUGUI>();
        tmpMoney = GameObject.Find("txtMoneyStatic").GetComponent<TMPro.TextMeshProUGUI>();
        tmpGold = GameObject.Find("txtGoldStatic").GetComponent<TMPro.TextMeshProUGUI>();
        imgXPBar = GameObject.Find("XPProgressBarStatic");

        //löd die mainUI für xp,gold,xp,level
        ReloadPlayerStats();
    }

    //läd das current Level anhand aller gesammelten XP
    public static void LoadLevelByXp(long xp){
        if(xp<90){
            long val1 = xp / 10;
            double val2 = Math.Pow(val1 , 1 / 2);
            playerLevel = Convert.ToInt32(Math.Floor(val2));
        }else{
            long val1 = xp / 5;
            double val2 = Math.Pow(val1 , 1 / 3.72);
            playerLevel = Convert.ToInt32(Math.Floor(val2));
        }
    }

    //berechnet die gesamten xp die benötigt werden um das level abzuschließen
    public static int getXpForNextLevelByLevel(int lvl){
        int xpNeeded = 0;
        if(lvl<=2){
            xpNeeded = Convert.ToInt32(Math.Floor(Math.Pow(lvl,2)+0.99)*10);
        }else{
            xpNeeded = Convert.ToInt32(Math.Floor(Math.Pow(lvl,3.72)+0.99)*5);
        }
        return xpNeeded;
    }



    //läd liste
    public static void LoadFoodItemDict(string data){
        data = data.Trim();
        string[] items = data.Split(";");
        if(!data.Equals("Empty")){
            for(int a=0;a<items.Length;a++){
                string[] pair = items[a].Split(":");
                FoodItemDict.Add(pair[0], Int32.Parse(pair[1]));
            }
        }
        CountFoodItemsInFridge();
    }
    public static void LoadStorageItemDict(string data){
        data = data.Trim();
        string[] items = data.Split(";");
        if(!data.Equals("Empty")){
            for(int a=0;a<items.Length;a++){
                string[] pair = items[a].Split(":");
                StorageItemDict.Add(pair[0], Int32.Parse(pair[1]));
            }
        }
    }
    public static void LoadObjectLimiterDict(string data){
        data = data.Trim();
        string[] items = data.Split(";");
        if(!data.Equals("Empty")){
            for(int a=0;a<items.Length;a++){
                string[] pair = items[a].Split(":");
                ObjectLimiterDict.Add(pair[0], Int32.Parse(pair[1]));
            }
        }
    }

    //baut ein dict mit der gespeicherten infos über das spieler aussehen
    //generiert spieler char
    public static void LoadPlayerDict(string data){
        //CONTINUE save failback
        data = data.Trim();
        string[] items = data.Split(";");
        for(int a=0;a<items.Length;a++){
            string[] pair = items[a].Split(":");
            PlayerDict.Add(pair[0], pair[1]);
        }

        PlayerCharBuilder.Intizialisierer(PlayerDict);
    }


    //addiert 1 zu einem objekt 
    public static void AddFoodItem(string food){
        if(FoodItemDict.ContainsKey(food)){
            FoodItemDict[food] += 1;
        }else{
            FoodItemDict.Add(food, 1);
        }

        CountFoodItemsInFridge();
    }
    public static bool AddStorageItem(string storage){
        if(StorageItemDict.ContainsKey(storage)){
            StorageItemDict[storage] += 1;
            return true;
        }else{
            StorageItemDict.Add(storage, 1);
            return true;
        }
        return false;
    }

    //subtrahiert 1 von einem objekt 
    public static void RemoveFoodItem(string food){
        if(FoodItemDict.ContainsKey(food)){
            if(FoodItemDict[food] == 1){    
                FoodItemDict.Remove(food);
            }else{
                FoodItemDict[food] -= 1;
            }
        }

        CountFoodItemsInFridge();
    }
    public static bool RemoveStorageItem(string storage){
        if(StorageItemDict.ContainsKey(storage)){
            if(StorageItemDict[storage] == 1){    
                StorageItemDict.Remove(storage);
                return true;
            }else{
                StorageItemDict[storage] -= 1;
                return true;
            }
        }
        return false;
    }

    public static void CountFoodItemsInFridge(){//zählt die anzahl an items im fridge
        int count = 0;
        foreach(var item in FoodItemDict){
            count = count + item.Value;
        }
        FridgeStoragePlace = count;
    }



    //reload player Money,Gold,XP in main UI
    public static void ReloadPlayerStats(){
        //berechnet das level anhand gesamt XP neu
        LoadLevelByXp(playerXP);
        tmpLevel.text = ""+playerLevel;
        tmpXp.text = ""+playerXP;
        tmpMoney.text = ""+playerMoney;
        tmpGold.text = ""+playerGold;

        //berechnet xpProgressBar
        int val1 = getXpForNextLevelByLevel(playerLevel);   //aktuelles level benötigte xp gesamt
        int val2 = getXpForNextLevelByLevel(playerLevel+1); //höheres level benötigte xp gesamt
        int dif1 = Convert.ToInt32(playerXP - val1);        //xp seit level up
        int difToNextLvl = val2 - val1;                     //xp von aktuellen level bis zum nächsten
        float perc = (float)(dif1)/(float)(difToNextLvl);   //prozent bis level up (1=levelup)

        //berechne scaleauflösung der xpbar
        float xpBarXScale = 0.3f;//0.3 ist 100%
        imgXPBar.transform.localScale = new Vector2(xpBarXScale*perc, imgXPBar.transform.localScale.y);
    }



    //getter
    public static int getFoodItemCount(string itemName){
        if(FoodItemDict.ContainsKey(itemName)){
            return FoodItemDict[itemName];
        }
        return 0;
    }
    public static int getStorageItemCount(string itemName){
        if(StorageItemDict.ContainsKey(itemName)){
            return StorageItemDict[itemName];
        }
        return 0;
    }
    public static string getFoodItemDictInfo(){
        string info = "";
        foreach (var item in FoodItemDict){
            info += item.Key+":"+item.Value+";";
        }
        if(info!=""){
            info = info.Remove(info.Length - 1, 1);
            return info;
        }
        return "Empty";
    }
    public static string getStorageItemDictInfo(){
        string info = "";
        foreach (var item in StorageItemDict){
            info += item.Key+":"+item.Value+";";
        }
        if(info!=""){
            info = info.Remove(info.Length - 1, 1);
            return info;
        }
        return "Empty";
    }
    public static string getObjectLimiterDictInfo(){
        string info = "";
        foreach (var item in ObjectLimiterDict){
            info += item.Key+":"+item.Value+";";
        }
        if(info!=""){
            info = info.Remove(info.Length - 1, 1);
            return info;
        }
        return "Oven:3;Fridge:1;Counter:2;Slushi:1";
    }

    //gibt die anzahl der noch möglich zu platzierenden Objekte für ein Objecttyp wieder
    public static int getObjectLimiterDictInfoForObject(string objectType){

        //kriegt die nummer der max. möglich zu platzierenden objecte
        int typeMaxAnzahl = 0;
        if(ObjectLimiterDict.ContainsKey(objectType)){
            typeMaxAnzahl = ObjectLimiterDict[objectType];
        }else{
            return 99999;
        }

        int AnzahlAufSpielfeld = 0;

        //vergleiche die zahl mit der bereits existierenden anzahl auf dem spielfeld
        for(int a=0;a<gridSize;a++){
            for(int b=0;b<gridSize;b++){
               
                //suche jedes childFloor object in der Scene
                if(GameObject.Find(a+"-"+b+"-Child")!=null){
                    string childName = GameObject.Find(a+"-"+b+"-Child").gameObject.GetComponent<SpriteRenderer>().sprite.name;
                    string[] splitName = childName.Split("_");

                    //gucke obj childFloor object der gesuchte type ist
                    if(splitName[0].Equals(objectType)){
                        AnzahlAufSpielfeld = AnzahlAufSpielfeld + 1;
                    }
                }
            }
        }

        //gibt die anzahl der noch zu platzierenden objekte wieder
        return typeMaxAnzahl-AnzahlAufSpielfeld;
    }

    //gibt die aktuellen playerChar daten zurück
    public static string getPlayerDictInfo(){
        string info = "";
        foreach (var item in PlayerDict){
            info += item.Key+":"+item.Value+";";
        }
        if(info!=""){
            info = info.Remove(info.Length - 1, 1);
            return info;
        }
        return "Gender:true;Hat:255-255-255-255;Face:255-255-255-255;Hair:255-255-255-255;HairOverlay:255-255-255-255;Leg:255-255-255-255;LegOverlay:255-255-255-255;Skin:255-255-255-255;SkinOverlay:255-255-255-255;Tshirt:255-255-255-255;TshirtOverlay:255-255-255-255";
    }


    //setter
    public static void setPlayerName(string name){
        playerName = name;
        MainController.LoadName(name);
    }
}
