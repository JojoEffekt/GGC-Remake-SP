using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public static string playerName;//Name
    public static int playerMoney;//0
    public static int playerGold;//0
    public static long playerXP;//0
    public static int gridSize;//8

    public static int playerLevel;//wird durch xp erzeugt
    public static int FridgeStoragePlace;//wird durch LoadFoodItemDict erzeugt/ platz im kühlschrank

    public static Dictionary<string, int> FoodItemDict = new Dictionary<string, int>();//Empty
    public static Dictionary<string, int> StorageItemDict = new Dictionary<string, int>();//Empty / speicher Informationen welches Objekt und wie oft auf reserve
 

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
    public static void LoadLevelByXp(long xp){
        long val1 = xp / 10;
        double val2 = Math.Pow(val1 , 0.5);
        playerLevel = Convert.ToInt32(Math.Floor(val2));
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



    //getter
    public static int getFoodItemCount(string itemName){
        if(FoodItemDict.ContainsKey(itemName)){
            return FoodItemDict[itemName];
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
}
