using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardObject : MonoBehaviour
{   
    //attributes
    public string type { get; set; }
    public string objectName { get; set; }
    public int price { get; set; }
    public int rotation { get; set; }
    public float coordCorrectionX { get; set; }
    public float coordCorrectionY { get; set; }
    public float coordX { get; set; }
    public float coordY { get; set; }

    //folgende vars werden automatisch erstellt
    private GameObject StandartPrefab;
    private GameObject StandartGameObject;

    private List<Sprite> dekoSpriteList = new List<Sprite>();
    private Sprite standartSprite;

    //constructor
    public StandardObject(string type, string objectName, int price, int rotation, float coordCorrectionX, float coordCorrectionY, float coordX, float coordY){
        this.type = type;
        this.objectName = objectName;
        this.price = price;
        this.rotation = rotation;
        this.coordCorrectionX = coordCorrectionX;
        this.coordCorrectionY = coordCorrectionY;
        this.coordX = coordX;
        this.coordY = coordY;

        LoadAssets();
        setStandartSpriteFromList();
        
        //generate UI
        StandartGameObject = Instantiate(StandartPrefab, new Vector2(coordX,coordY), Quaternion.identity);
        StandartGameObject.transform.position = new Vector2(StandartGameObject.transform.position.x + coordCorrectionX, StandartGameObject.transform.position.y + coordCorrectionY);

        setStandartSprite();
    }

    //methods
    private void LoadAssets(){
        StandartPrefab = Resources.Load("Prefabs/StandardPrefab") as GameObject;

        //load images from folder
        object[] sprites = Resources.LoadAll("Textures/DekoFloor",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	dekoSpriteList.Add((Sprite)sprites[x]);
        }
    } 
    public void Rotate(){
        //get ende von name (a,b,c,d) dann nächster buchstabe
        string[] nameSlice = objectName.Split("_");
        if(nameSlice[nameSlice.Length-1].Equals("a")){
            this.objectName = objectName.Substring(0,objectName.Length-1)+"b";
        }else if(nameSlice[nameSlice.Length-1].Equals("b")){
            this.objectName = objectName.Substring(0,objectName.Length-1)+"c";
        }else if(nameSlice[nameSlice.Length-1].Equals("c")){
            this.objectName = objectName.Substring(0,objectName.Length-1)+"d";
        }else if(nameSlice[nameSlice.Length-1].Equals("d")){
            this.objectName = objectName.Substring(0,objectName.Length-1)+"a";
        }
        setStandartSpriteFromList();
        setStandartSprite();
    }
    public void Info(){
        Debug.Log("StandartObject-"+type+": ["+coordX+","+coordY+"], rotation:"+rotation+", objectName:"+objectName+", price:"+price);
    }

    //getters

    //setters
    private void setStandartSpriteFromList(){
        //suche sprite from lists
        Sprite sprite = null;
        if(type.Equals("Deko")){
            for(int a=0;a<dekoSpriteList.Count;a++){
                if(dekoSpriteList[a].name.Equals(objectName)){
                    sprite = dekoSpriteList[a];
                }
            }
        }
        this.standartSprite = sprite;
    }
    public void setStandartSprite(){
        StandartGameObject.GetComponent<SpriteRenderer>().sprite = standartSprite;
        StandartGameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
    }
}
