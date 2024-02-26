using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorObject : MonoBehaviour
{
    //attributes
    private string floorName;
    private int price;
    private float coordX;
    private float coordY;

    //folgende vars werden automatisch erstellt
    private GameObject FloorPrefab;
    private GameObject FloorGameObject;

    //contructor
    public FloorObject(string floorName, int price, float coordX, float coordY){
        this.floorName = floorName;
        this.price = price;
        this.coordX = coordX;
        this.coordY = coordY;

        FloorPrefab = Resources.Load("Prefabs/FloorPrefab") as GameObject;
        FloorGameObject = Instantiate(FloorPrefab, new Vector2(coordX,coordY), Quaternion.identity);
    }

    //methods
    public void Info(){
        Debug.Log("FloorObject: ["+coordX+","+coordY+"], "+floorName+", "+price);
    }

    //getters
    public string getFloorName(){
        return this.floorName;
    }
    public int getPrice(){
        return this.price;
    }
    public float getCoordX(){
        return this.coordX;
    }
    public float getCoordY(){
        return this.coordY;
    }
    public string getInfo(){
        string info = floorName+";"+price+";"+coordX+";"+coordY;
        return info;
    }

    //setters
    public void setFloorName(string floorName){
        this.floorName = floorName;
    }
    public void setPrice(int price){
        this.price = price;
    }
    public void setCoordX(float coordX){
        this.coordX = coordX;
    }
    public void setCoordY(float coordY){
        this.coordY = coordY;
    }
}
