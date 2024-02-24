using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorObject : MonoBehaviour
{
    //attributes
    private string floorName;
    private int price;
    private int coordX;
    private int coordY;

    //contructor
    public FloorObject(string floorName, int price, int coordX, int coordY){
        this.floorName = floorName;
        this.price = price;
        this.coordX = coordX;
        this.coordY = coordY;
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
    public int getCoordX(){
        return this.coordX;
    }
    public int getCoordY(){
        return this.coordY;
    }

    //setters
    public void setFloorName(string floorName){
        this.floorName = floorName;
    }
    public void setPrice(int price){
        this.price = price;
    }
    public void setCoordX(int coordX){
        this.coordX = coordX;
    }
    public void setCoordY(int coordY){
        this.coordY = coordY;
    }
}
