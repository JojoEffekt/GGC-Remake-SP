using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public static string playerName;
    public static int playerMoney;
    public static int playerGold;
    public static long playerXP;
    public static int playerLevel;

    public static int gridSize;

    public static Dictionary<string, int> FoodItemDict = new Dictionary<string, int>();
    public static Dictionary<string, int> StorageItemDict = new Dictionary<string, int>();

    public static void LoadFoodItemDict(string data){
        data = data.Trim();
        string[] items = data.Split(";");
        if(data.Equals("Empty")==false){
            for(int a=0;a<items.Length;a++){
                string[] pair = items[a].Split(":");
                FoodItemDict.Add(pair[0], Int32.Parse(pair[1]));
                Debug.Log("data1: "+data);
            }
        }
    }
    public static void LoadStorageItemDict(string data){
        data = data.Trim();
        string[] items = data.Split(";");
        if(!data.Equals("Empty")){
            for(int a=0;a<items.Length;a++){
                string[] pair = items[a].Split(":");
                StorageItemDict.Add(pair[0], Int32.Parse(pair[1]));
                Debug.Log("data2: "+data);
            }
        }
    }

    public static bool AddFoodItem(string food){
        if(FoodItemDict.ContainsKey(food)){
            FoodItemDict[food] += 1;
            return true;
        }else{
            FoodItemDict.Add(food, 1);
            return true;
        }
        return false;
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

    public static bool RemoveFoodItem(string food){
        if(FoodItemDict.ContainsKey(food)){
            if(FoodItemDict[food] == 1){    
                FoodItemDict.Remove(food);
                return true;
            }else{
                FoodItemDict[food] -= 1;
                return true;
            }
        }
        return false;
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



    //getter
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
