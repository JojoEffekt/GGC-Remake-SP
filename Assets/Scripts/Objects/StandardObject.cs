using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardObject : MonoBehaviour
{   
    //attributes
    public string type { get; set; }
    public string gameObjectName { get; set; }
    public string objectName { get; set; }
    public int price { get; set; }
    public int rotation { get; set; }
    public float coordCorrectionXA { get; set; }
    public float coordCorrectionYA { get; set; }
    public float coordCorrectionXB { get; set; }
    public float coordCorrectionYB { get; set; }
    public float coordX { get; set; }
    public float coordY { get; set; }

    //folgende vars werden automatisch erstellt
    private GameObject StandartPrefab;
    private GameObject StandartGameObject;

    private List<Sprite> dekoSpriteList = new List<Sprite>();
    private List<Sprite> fridgeSpriteList = new List<Sprite>();
    private Sprite standartSprite;

    //constructor
    public StandardObject(string type, string gameObjectName, string objectName, int price, float coordCorrectionXA, float coordCorrectionYA, float coordCorrectionXB, float coordCorrectionYB, float coordX, float coordY){
        this.type = type;
        this.gameObjectName = gameObjectName;
        this.objectName = objectName;
        this.price = price;
        this.coordCorrectionXA = coordCorrectionXA;
        this.coordCorrectionYA = coordCorrectionYA;
        this.coordCorrectionXB = coordCorrectionXB;
        this.coordCorrectionYB = coordCorrectionYB;
        this.coordX = coordX;
        this.coordY = coordY;

        LoadAssets();
        
        //generate UI
        StandartGameObject = Instantiate(StandartPrefab, new Vector2(this.coordX, this.coordY), Quaternion.identity);
        StandartGameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(-coordY-2.75f);
        StandartGameObject.name = this.gameObjectName;

        setStandartSpriteFromList();
        setStandartSprite();
        RenderType();
    }

    //methods
    private void LoadAssets(){
        StandartPrefab = Resources.Load("Prefabs/StandardPrefab") as GameObject;

        //load images from folder
        object[] sprites = Resources.LoadAll("Textures/DekoFloor",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	dekoSpriteList.Add((Sprite)sprites[x]);
        }

        sprites = Resources.LoadAll("Textures/FridgeFloor",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	fridgeSpriteList.Add((Sprite)sprites[x]);
        }
    } 

    public string Rotate(){
        //get ende von name (a,b,c,d) dann nächster buchstabe
        //set next name, danach generiere das bild
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

        return objectName;
    }

    private void RenderType(){
        //wenn obj == fridge: erzeuge Klickable UI to open FridgeShop
        if(type.Equals("Fridge")){
            this.StandartGameObject.AddComponent(typeof(PolygonCollider2D));
        }
        //continue
    }



    //getters
    public string getInfo(){
        string info = type+";"+gameObjectName+";"+objectName+";"+price+";"+coordCorrectionXA+";"+coordCorrectionYA+";"+coordCorrectionXB+";"+coordCorrectionYB;
        return info;
    }



    //setters
    //sucht passendes sprite
    private void setStandartSpriteFromList(){
        Sprite sprite = null;
        if(type.Equals("Deko")){
            for(int a=0;a<dekoSpriteList.Count;a++){
                if(dekoSpriteList[a].name.Equals(objectName)){
                    sprite = dekoSpriteList[a];
                }
            }
        }else if(type.Equals("Fridge")){
            for(int b=0;b<fridgeSpriteList.Count;b++){
                if(fridgeSpriteList[b].name.Equals(objectName)){
                    sprite = fridgeSpriteList[b];
                }
            }
        }//continue...

        this.standartSprite = sprite;
    }
    //rendert passendes sprite
    public void setStandartSprite(){
        StandartGameObject.GetComponent<SpriteRenderer>().sprite = standartSprite;
        //reset coords
        StandartGameObject.transform.position = new Vector2(coordX, coordY);

        //render coordcorrection for rotated sprite 
        string[] nameSlice = objectName.Split("_");
        if(nameSlice[nameSlice.Length-1].Equals("a")||nameSlice[nameSlice.Length-1].Equals("c")){
            StandartGameObject.transform.position = new Vector2(StandartGameObject.transform.position.x + coordCorrectionXA, StandartGameObject.transform.position.y + coordCorrectionYA);
        }else if(nameSlice[nameSlice.Length-1].Equals("b")||nameSlice[nameSlice.Length-1].Equals("d")){
            StandartGameObject.transform.position = new Vector2(StandartGameObject.transform.position.x + coordCorrectionXB, StandartGameObject.transform.position.y + coordCorrectionYB);
        }
    }
}
