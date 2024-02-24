using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallObject : MonoBehaviour
{
    //attributes
    private string wallChildName = null;
    private int wallChildLength = 0;
    private int coordX;
    private int coordY;

    //constructor
    public WallObject(string wallChildName, int wallChildLength, int coordX, int coordY){
        this.wallChildName = wallChildName;
        this.wallChildLength = wallChildLength;
        this.coordX = coordX;
        this.coordY = coordY;
    }

    //methods
    public void CreateObject(){
        //continue
    }

    //getters
    public string getWallChildName(){
        return this.wallChildName;
    }

    public int getWallChildLength(){
        return this.wallChildLength;
    }

    public int getCoordX(){
        return this.coordX;
    }

    public int getCoordY(){
        return this.coordY;
    }

    //setters
    public void setWallChildName(string name){
        this.wallChildName = name;
    }

    public void setWallChildLength(int length){
        this.wallChildLength = length;
    }

    public void setCoordX(int coordX){
        this.coordX = coordX;
    }

    public void setCoordY(int coordY){
        this.coordY = coordY;
    }
}
