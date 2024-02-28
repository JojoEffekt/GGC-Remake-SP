using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorObject : MonoBehaviour
{
    //attributes
    //muss gespeichert werden, save & load
    public string floorGameObjectName { get; set; }
    public string floorName { get; set; }
    public int floorPrice { get; set; }
    public float floorCoordX { get; set; }
    public float floorCoordY { get; set; }
    public string floorChildType { get; set; }
    public string floorChildName { get; set; }
    public int floorChildPrice { get; set; }
    public int floorChildRotation { get; set; }
    public float floorChildCoordCorrectionX { get; set; }
    public float floorChildCoordCorrectionY { get; set; }

    //folgende vars werden automatisch erstellt
    private GameObject FloorPrefab;
    private GameObject FloorGameObject;

    private List<Sprite> floorSpriteList = new List<Sprite>();
    private Sprite floorSprite;
    private Sprite floorChildSprite;

    //contructor
    public FloorObject(string floorGameObjectName, string floorName, int floorPrice, float floorCoordX, float floorCoordY, string floorChildType, string floorChildName, int floorChildPrice, int floorChildRotation, float floorChildCoordCorrectionX, float floorChildCoordCorrectionY){
        this.floorGameObjectName = floorGameObjectName;
        this.floorName = floorName;
        this.floorPrice = floorPrice;
        this.floorCoordX = floorCoordX;
        this.floorCoordY = floorCoordY;
        this.floorChildType = floorChildType;
        this.floorChildName = floorChildName;
        this.floorChildPrice = floorChildPrice;
        this.floorChildRotation = floorChildRotation;
        this.floorChildCoordCorrectionX = floorChildCoordCorrectionX;
        this.floorChildCoordCorrectionY = floorChildCoordCorrectionY;

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
        object[] sprites = Resources.LoadAll("Textures/Floor",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	floorSpriteList.Add((Sprite)sprites[x]);
        }
    }
    public void Info(){
        Debug.Log("FloorObject-"+floorName+": ["+floorCoordX+","+floorCoordY+"], "+floorPrice+", "+floorChildName);
    }

    //getters
    public string getInfo(){
        string info = floorGameObjectName+";"+floorName+";"+floorPrice+";"+floorCoordX+";"+floorCoordY+";"+floorChildType+";"+floorChildName+";"+floorChildPrice+";"+floorChildRotation+";"+floorChildCoordCorrectionX+";"+floorChildCoordCorrectionY;
        return info;
    }

    //setters
    //generate obj on this floor obj
    public StandardObject setChild(string floorChildType, string floorChildName, int floorChildPrice, int floorChildRotation, float floorChildCoordCorrectionX, float floorChildCoordCorrectionY){
        this.floorChildType = floorChildType;
        this.floorChildName = floorChildName;
        this.floorChildPrice = floorChildPrice;
        this.floorChildRotation = floorChildRotation;
        this.floorChildCoordCorrectionX = floorChildCoordCorrectionX;
        this.floorChildCoordCorrectionY = floorChildCoordCorrectionY;

        //return obj to save in list (get reference)
        return new StandardObject(floorChildType, floorChildName, floorChildPrice, floorChildRotation, floorChildCoordCorrectionX, floorChildCoordCorrectionY, floorCoordX, floorCoordY);
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
