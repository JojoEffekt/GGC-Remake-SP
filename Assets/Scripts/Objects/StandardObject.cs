using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    private List<Sprite> chairSpriteList = new List<Sprite>();
    private List<Sprite> tableSpriteList = new List<Sprite>();
    private List<Sprite> counterSpriteList = new List<Sprite>();
    private List<Sprite> ovenSpriteList = new List<Sprite>();
    private List<Sprite> slushiSpriteList = new List<Sprite>();
    private Sprite standartSprite;

    private Chair chair;
    private Table table;
    private Counter counter;
    private Oven oven;
    private Slushi slushi;

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
        InstantiateTypeData();
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
        sprites = Resources.LoadAll("Textures/ChairFloor",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	chairSpriteList.Add((Sprite)sprites[x]);
        }
        sprites = Resources.LoadAll("Textures/TableFloor",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	tableSpriteList.Add((Sprite)sprites[x]);
        }
        sprites = Resources.LoadAll("Textures/CounterFloor",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	counterSpriteList.Add((Sprite)sprites[x]);
        }
        sprites = Resources.LoadAll("Textures/OvenFloor",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	ovenSpriteList.Add((Sprite)sprites[x]);
        }
        sprites = Resources.LoadAll("Textures/SlushiFloor",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	slushiSpriteList.Add((Sprite)sprites[x]);
        }
    }
    
    private void InstantiateTypeData(){//beim erstmaligen laden aufgerufen, kann überschrieben werden
        if(type.Equals("Chair")){
            chair = new Chair(false);
        }else if(type.Equals("Table")){
            table = new Table(false);
        }else if(type.Equals("Counter")){
            counter = new Counter(false, null, 0);
        }else if(type.Equals("Oven")){
            oven = new Oven(0, null, 0, null, null);
        }else if(type.Equals("Slushi")){
            slushi = new Slushi(null, 0);
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
        setStandartSpriteFromList();//hole bild aus liste
        setStandartSprite();//lade bild

        return objectName;
    }

    private void RenderType(){//gibt sprite ein collider
        this.StandartGameObject.AddComponent(typeof(PolygonCollider2D));
    }

    public void UpdateTypeData(string data){//wird zum updaten aufgerufen
        string[] listItem = data.Split(";");
        if(type.Equals("Chair")){
            chair.isEmpty = bool.Parse(listItem[2]);
        }else if(type.Equals("Table")){
            table.isEmpty = bool.Parse(listItem[2]);
        }else if(type.Equals("Counter")){
            counter.isEmpty = bool.Parse(listItem[2]);
            counter.foodSprite = listItem[3];
            counter.foodCount = Int32.Parse(listItem[4]);
        }else if(type.Equals("Oven")){
            oven.foodStep = Int32.Parse(listItem[2]);
            oven.foodSprite = listItem[3];
            oven.foodCount = Int32.Parse(listItem[4]);
            oven.dateStart = listItem[5];
            oven.dateEnd = listItem[6];
        }else if(type.Equals("Slushi")){
            slushi.cocktailSprite = listItem[2];
            slushi.cocktailCount = Int32.Parse(listItem[3]);
        }
    }



    //getters
    public string getInfo(){
        string info = type+";"+gameObjectName+";"+objectName+";"+price+";"+coordCorrectionXA+";"+coordCorrectionYA+";"+coordCorrectionXB+";"+coordCorrectionYB;
        return info;
    }

    public string getTypeInfo(){
        string info = "";
        if(type.Equals("Chair")){
            info = ""+chair.isEmpty;
        }else if(type.Equals("Table")){
            info = ""+table.isEmpty;
        }else if(type.Equals("Counter")){
            info = counter.isEmpty+";"+counter.foodSprite+";"+counter.foodCount;
        }else if(type.Equals("Oven")){
            info = oven.foodStep+";"+oven.foodSprite+";"+oven.foodCount+";"+oven.dateStart+";"+oven.dateEnd;
        }else if(type.Equals("Slushi")){
            info = slushi.cocktailSprite+";"+slushi.cocktailCount;
        }
        info = type+";"+gameObjectName+";"+info;
        return info;
    }



    //setters
    private void setStandartSpriteFromList(){//sucht passendes sprite
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
        }else if(type.Equals("Chair")){
            for(int b=0;b<chairSpriteList.Count;b++){
                if(chairSpriteList[b].name.Equals(objectName)){
                    sprite = chairSpriteList[b];
                }
            }
        }else if(type.Equals("Table")){
            for(int b=0;b<tableSpriteList.Count;b++){
                if(tableSpriteList[b].name.Equals(objectName)){
                    sprite = tableSpriteList[b];
                }
            }
        }else if(type.Equals("Counter")){
            for(int b=0;b<counterSpriteList.Count;b++){
                if(counterSpriteList[b].name.Equals(objectName)){
                    sprite = counterSpriteList[b];
                }
            }
        }else if(type.Equals("Oven")){
            for(int b=0;b<ovenSpriteList.Count;b++){
                if(ovenSpriteList[b].name.Equals(objectName)){
                    sprite = ovenSpriteList[b];
                }
            }
        }else if(type.Equals("Slushi")){
            for(int b=0;b<slushiSpriteList.Count;b++){
                if(slushiSpriteList[b].name.Equals(objectName)){
                    sprite = slushiSpriteList[b];
                }
            }
        }

        this.standartSprite = sprite;
    }

    public void setStandartSprite(){//rendert passendes sprite
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

public class Chair{
    public bool isEmpty { get; set; }
    public Chair(bool isEmpty){
        this.isEmpty = isEmpty;
    }
}

public class Table{
    public bool isEmpty { get; set; }
    public Table(bool isEmpty){
        this.isEmpty = isEmpty;
    }
}

public class Counter{
    public bool isEmpty { get; set; }
    public string foodSprite { get; set; }
    public int foodCount { get; set; }
    public Counter(bool isEmpty, string foodSprite, int foodCount){
        this.isEmpty = isEmpty;
        this.foodSprite = foodSprite;
        this.foodCount = foodCount;
    }
}

public class Oven{
    public int foodStep { get; set; }
    public string foodSprite { get; set; }
    public int foodCount { get; set; }
    public string dateStart { get; set; }
    public string dateEnd { get; set; }
    public Oven(int foodStep, string foodSprite, int foodCount, string dateStart, string dateEnd){
        this.foodStep = foodStep;//0 = empty, 1 = in Proccess, 2 = ready, 3 = dirty
        this.foodSprite = foodSprite;
        this.foodCount = foodCount;
        this.dateStart = dateStart;
        this.dateEnd = dateEnd;
    }
}

public class Slushi{
    public string cocktailSprite { get; set; }
    public int cocktailCount { get; set; }
    public Slushi(string cocktailSprite, int cocktailCount){
        this.cocktailSprite = cocktailSprite;
        this.cocktailCount = cocktailCount;
    }
}