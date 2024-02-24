using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandartObject : MonoBehaviour
{   
    //attributes
    private string type;
    private string objectName;
    private int rotation;
    private int price;
    private int coordX;
    private int coordY;

    //constructor
    public StandartObject(string type, string objectName, int rotation, int price, int coordX, int coordY){
        this.type = type;
        this.objectName = objectName;
        this.rotation = rotation;
        this.price = price;
        this.coordX = coordX;
        this.coordY = coordY;

        GenerateObject();
    }

    //methods
    public void Info(){
        Debug.Log("StandartObject-"+type+": ["+coordX+","+coordY+"], rotation:"+rotation+", objectName:"+objectName+", price:"+price);
    }

    //methods
    private void GenerateObject(){
        //continue
    }

    //getters
    public string getType(){
        return this.type;
    }
    public string getObjectName(){
        return this.objectName;
    }
    public int getRotation(){
        return this.rotation;
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
    public void setType(string type){
        this.type = type;
    }
    public void setObjectName(string objectName){
        this.objectName = objectName;
    }
    public void setRotation(int rotation){
        this.rotation = rotation;
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
