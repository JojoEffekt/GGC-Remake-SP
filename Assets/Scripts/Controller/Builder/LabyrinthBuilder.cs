using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LabyrinthBuilder : MonoBehaviour
{
    /*
    
        /\
       /\/\
    Y /\/\/\ X
     / .... \
    /        \
    
    */

    //makiert die position wo die tür steht
    public static int[] startPos = new int[]{1,1};

    //erstellt das grid indem der player laufen darf
    //muss nach jeder veränderung des Spielfeldes aufgerufen werden
    public static void GenerateGrid(){

        startPos = FindDoor();
        Debug.Log(startPos[0]+":"+startPos[1]);

        //erstellt die grid größe
        int gridSize = PlayerController.gridSize;
        int[,] gridMap = new int[gridSize,gridSize]; 

        //sucht alle FloorChildObjecte in der scene und markiert diese in der gridMap
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach(GameObject item in allObjects){
            string[] split = item.name.Split("-");
            if(split.Length==3&&split[2].Equals("Child")){
                //1=Object, cant walk there, makiert die position wo der spieler nicht laufen darf
                gridMap[Int32.Parse(split[0]),Int32.Parse(split[1])] = 1;
            }
        }

        for(int a=0;a<gridSize;a++){
            for(int b=0;b<gridSize;b++){
                Debug.Log(a+":"+b+"; "+gridMap[a,b]);
            }
        }
    }

    //sucht die startPos anhander der Tür an der Wand
    public static int[] FindDoor(){
        int[] pos = new int[]{0,0}; 
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach(GameObject item in allObjects){
            string[] split = item.name.Split("-");
            if(split.Length==3&&split[2].Equals("Wall")){
                if(!Object.ReferenceEquals(item.GetComponent<SpriteRenderer>().sprite, null)){
                    string[] spriteName = item.GetComponent<SpriteRenderer>().sprite.name.Split("_");
                    if(spriteName.Length>1){
                        if(spriteName[1].Equals("Door")){
                            if(Int32.Parse(item.name.Split("-")[0])!=0){
                                pos[0] = Int32.Parse(item.name.Split("-")[0])-1;
                                pos[1] = Int32.Parse(item.name.Split("-")[1]);
                            }else{
                                pos[0] = Int32.Parse(item.name.Split("-")[0]);
                                pos[1] = Int32.Parse(item.name.Split("-")[1])-1;
                            }
                        }
                    }
                }
            }
        }
        return pos;
    }
}
