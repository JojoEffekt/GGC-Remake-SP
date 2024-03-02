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
    public string floorChildGameObjectName { get; set; }
    public string floorChildName { get; set; }
    public int floorChildPrice { get; set; }
    public float floorChildCoordCorrectionXA { get; set; }
    public float floorChildCoordCorrectionYA { get; set; }
    public float floorChildCoordCorrectionXB { get; set; }
    public float floorChildCoordCorrectionYB { get; set; }

    //folgende vars werden automatisch erstellt
    private GameObject FloorPrefab;
    private GameObject FloorGameObject;

    private List<Sprite> floorSpriteList = new List<Sprite>();
    private Sprite floorSprite;
    private Sprite floorChildSprite;

    //contructor
    public FloorObject(string floorGameObjectName, string floorName, int floorPrice, float floorCoordX, float floorCoordY, string floorChildType, string floorChildGameObjectName, string floorChildName, int floorChildPrice, float floorChildCoordCorrectionXA, float floorChildCoordCorrectionYA, float floorChildCoordCorrectionXB, float floorChildCoordCorrectionYB){
        this.floorGameObjectName = floorGameObjectName;
        this.floorName = floorName;
        this.floorPrice = floorPrice;
        this.floorCoordX = floorCoordX;
        this.floorCoordY = floorCoordY;
        this.floorChildType = floorChildType;
        this.floorChildGameObjectName = floorChildGameObjectName;
        this.floorChildName = floorChildName;
        this.floorChildPrice = floorChildPrice;
        this.floorChildCoordCorrectionXA = floorChildCoordCorrectionXA;
        this.floorChildCoordCorrectionYA = floorChildCoordCorrectionYA;
        this.floorChildCoordCorrectionXB = floorChildCoordCorrectionXB;
        this.floorChildCoordCorrectionYB = floorChildCoordCorrectionYB;

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
        string info = floorGameObjectName+";"+floorName+";"+floorPrice+";"+floorCoordX+";"+floorCoordY+";"+floorChildType+";"+floorChildGameObjectName+";"+floorChildName+";"+floorChildPrice+";"+floorChildCoordCorrectionXA+";"+floorChildCoordCorrectionYA+";"+floorChildCoordCorrectionXB+";"+floorChildCoordCorrectionYB;
        return info;
    }

    //setters
    //generate obj on this floor obj
    public StandardObject setChild(string floorChildType, string floorChildGameObjectName, string floorChildName, int floorChildPrice, float floorChildCoordCorrectionXA, float floorChildCoordCorrectionYA, float floorChildCoordCorrectionXB, float floorChildCoordCorrectionYB){
        this.floorChildType = floorChildType;
        this.floorChildGameObjectName = floorGameObjectName+"-Child";
        this.floorChildName = floorChildName;
        this.floorChildPrice = floorChildPrice;
        this.floorChildCoordCorrectionXA = floorChildCoordCorrectionXA;
        this.floorChildCoordCorrectionYA = floorChildCoordCorrectionYA;
        this.floorChildCoordCorrectionXB = floorChildCoordCorrectionXB;
        this.floorChildCoordCorrectionYB = floorChildCoordCorrectionYB;

        //return obj to save in list (get reference)
        return new StandardObject(floorChildType, this.floorChildGameObjectName, floorChildName, floorChildPrice, floorChildCoordCorrectionXA, floorChildCoordCorrectionYA, floorChildCoordCorrectionXB, floorChildCoordCorrectionYB, floorCoordX, floorCoordY);
    }

    public void RotateChild(StandardObject child){
        this.floorChildName = child.Rotate();
    }

    public void DeleteChild(){
        this.floorChildType = null;
        this.floorChildGameObjectName = null;
        this.floorChildName = null;
        this.floorChildPrice = 0;
        this.floorChildCoordCorrectionXA = 0.0f;
        this.floorChildCoordCorrectionYA = 0.0f;
        this.floorChildCoordCorrectionXB = 0.0f;
        this.floorChildCoordCorrectionYB = 0.0f;
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
        FloorGameObject.GetComponent<SpriteRenderer>().sortingOrder = -2;
    }
}
