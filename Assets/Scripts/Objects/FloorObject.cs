using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorObject : MonoBehaviour
{
    //attributes
    //muss gespeichert werden, save & load
    private string floorGameObjectName;
    private string floorName;
    private int floorPrice;
    private float floorCoordX;
    private float floorCoordY;
    private string floorChildType;
    private string floorChildName;
    private int floorChildPrice;
    private int floorChildRotation;
    private float floorChildCoordX;
    private float floorChildCoordY;
    //continue: hier muss child object (StandartObject) gespeichert werden

    //folgende vars werden automatisch erstellt
    private GameObject FloorPrefab;
    private GameObject FloorGameObject;

    private List<Sprite> floorSpriteList = new List<Sprite>();
    private Sprite floorSprite;
    private Sprite floorChildSprite;

    //contructor
    public FloorObject(string floorGameObjectName, string floorName, int floorPrice, float floorCoordX, float floorCoordY, string floorChildType, string floorChildName, int floorChildPrice, int floorChildRotation, float floorChildCoordX, float floorChildCoordY){
        this.floorGameObjectName = floorGameObjectName;
        this.floorName = floorName;
        this.floorPrice = floorPrice;
        this.floorCoordX = floorCoordX;
        this.floorCoordY = floorCoordY;
        this.floorChildType = floorChildType;
        this.floorChildName = floorChildName;
        this.floorChildPrice = floorChildPrice;
        this.floorChildRotation = floorChildRotation;
        this.floorChildCoordX = floorChildCoordX;
        this.floorChildCoordY = floorChildCoordY;

        LoadAssets();
        setFloorSpriteFromList();
        
        //generate UI
        FloorGameObject = Instantiate(FloorPrefab, new Vector2(floorCoordX,floorCoordY), Quaternion.identity);
        FloorGameObject.name = floorGameObjectName; 

        setFloorSprite();
    }

    //methods
    private void LoadAssets(){
        //load prefab from folder
        FloorPrefab = Resources.Load("Prefabs/FloorPrefab") as GameObject;

        //load images from folder
        object[] sprites = Resources.LoadAll("Floor",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	floorSpriteList.Add((Sprite)sprites[x]);
        }
    }
    public void Info(){
        Debug.Log("FloorObject-"+floorName+": ["+floorCoordX+","+floorCoordY+"], "+floorPrice+", "+floorChildName);
    }

    //getters
    public string getFloorGameObjectName(){
        return this.floorGameObjectName;
    }
    public string getFloorName(){
        return this.floorName;
    }
    public int getPrice(){
        return this.floorPrice;
    }
    public float getCoordX(){
        return this.floorCoordX;
    }
    public float getCoordY(){
        return this.floorCoordY;
    }
    public string getFloorChildType(){
        return this.floorChildType;
    }
    public string getFloorChildName(){
        return this.floorChildName;
    }
    public int getFloorChildPrice(){
        return this.floorChildPrice;
    }
    public int getFloorChildRotation(){
        return this.floorChildRotation;
    }
    public float getFloorChildCoordX(){
        return this.floorChildCoordX;
    }
    public float getFloorChildCoordY(){
        return this.floorChildCoordY;
    }
    public string getInfo(){
        string info = floorGameObjectName+";"+floorName+";"+floorPrice+";"+floorCoordX+";"+floorCoordY+";"+floorChildType+";"+floorChildName+";"+floorChildPrice+";"+floorChildRotation+";"+floorChildCoordX+";"+floorChildCoordY;
        return info;
    }

    //setters
    public void setFloorName(string floorName){
        this.floorName = floorName;
    }
    public void setPrice(int floorPrice){
        this.floorPrice = floorPrice;
    }
    public void setCoordX(float floorCoordX){
        this.floorCoordX = floorCoordX;
    }
    public void setCoordY(float floorCoordY){
        this.floorCoordY = floorCoordY;
    }
    public void setFloorChildType(string floorChildType){
        this.floorChildType = floorChildType;
    }
    public void setFloorChildName(string floorChildName){
        this.floorChildName = floorChildName;
    }
    public void setFloorChildPrice(int floorChildPrice){
        this.floorChildPrice = floorChildPrice;
    }
    public void setFloorChildRotation(int floorChildRotation){
        this.floorChildRotation = floorChildRotation;
    }
    public void setFloorChildCoordX(float floorChildCoordX){
        this.floorChildCoordX = floorChildCoordX;
    }
    public void setFloorChildCoordY(float floorChildCoordY){
        this.floorChildCoordY = floorChildCoordY;
    }
    public void setChild(string floorChildType, string floorChildName, int floorChildPrice, int floorChildRotation, float floorChildCoordX, float floorChildCoordY){
        this.floorChildType = floorChildType;
        this.floorChildName = floorChildName;
        this.floorChildPrice = floorChildPrice;
        this.floorChildRotation = floorChildRotation;
        this.floorChildCoordX = floorChildCoordX;
        this.floorChildCoordY = floorChildCoordY;
    }

    private void setFloorSpriteFromList(){
        //suche sprite from lists
        Sprite sprite = null;
        for(int a=0;a<floorSpriteList.Count;a++){
            if(floorSpriteList[a].name.Equals(floorName)){
                sprite = floorSpriteList[a];
            }
        }
        this.floorSprite = sprite;
    }
    private void setFloorSprite(){
        FloorGameObject.GetComponent<SpriteRenderer>().sprite = floorSprite;
        FloorGameObject.GetComponent<SpriteRenderer>().sortingOrder = -1;
    }
}
