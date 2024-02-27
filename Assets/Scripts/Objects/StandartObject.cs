using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandartObject : MonoBehaviour
{   
    //attributes
    private string type;
    private string name;
    private int price;
    private int rotation;
    private float coordX;
    private float coordY;

    //folgende vars werden automatisch erstellt
    private GameObject FloorPrefab;
    private GameObject FloorGameObject;

    //constructor
    public StandartObject(string type, string name, int price, int rotation, float coordX, float coordY){
        this.type = type;
        this.name = name;
        this.price = price;
        this.rotation = rotation;
        this.coordX = coordX;
        this.coordY = coordY;

        GenerateObject();
    }

    //methods
    public void Info(){
        Debug.Log("StandartObject-"+type+": ["+coordX+","+coordY+"], rotation:"+rotation+", objectName:"+name+", price:"+price);
    }

    //methods
    private void GenerateObject(){
        FloorPrefab = Resources.Load("Prefabs/FloorPrefab") as GameObject;
        FloorGameObject = Instantiate(FloorPrefab, new Vector2(coordX,coordY), Quaternion.identity);
        Debug.Log("cretaed!!!!!!!!!!!!!! "+FloorGameObject.name);
    }

    //getters
    public string getType(){
        return this.type;
    }
    public string getObjectName(){
        return this.name;
    }
    public int getPrice(){
        return this.price;
    }
    public int getRotation(){
        return this.rotation;
    }
    public float getCoordX(){
        return this.coordX;
    }
    public float getCoordY(){
        return this.coordY;
    }

    //setters
    public void setType(string type){
        this.type = type;
    }
    public void setObjectName(string name){
        this.name = name;
    }
    public void setPrice(int price){
        this.price = price;
    }
    public void setRotation(int rotation){
        this.rotation = rotation;
    }
    public void setCoordX(float coordX){
        this.coordX = coordX;
    }
    public void setCoordY(float coordY){
        this.coordY = coordY;
    }
}
