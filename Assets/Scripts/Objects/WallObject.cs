using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallObject : MonoBehaviour
{
    //attributes
    //muss gespeichert werden save&load
    public string wallGameObjectName { get; set; }

    private string wallSpriteName;
    public string WallSpriteName { 
        get { return this.wallSpriteName; } 
        set { wallSpriteName = value; setWallSpriteFromList(); } 
    }

    public int rotation { get; set; }

    private string wallChildName;
    public string WallChildName {
        get { return this.wallChildName; } 
        set { wallChildName = value; setDekoSpriteFromList(); }
    }

    public int wallChildLength { get; set; }//0=NoChild 1=SingleChild 2=SecDoubleChild 3=MainDoubleChild
    public float wallChildCoordCorrectionX { get; set; }
    public float wallChildCoordCorrectionY { get; set; }
    public int coordX { get; set; }
    public int coordY { get; set; }

    //folgende vars werden automatisch erstellt
    private GameObject WallPrefab;
    private GameObject WallGameObject;

    private List<Sprite> spriteWallListRight = new List<Sprite>();
    private List<Sprite> spriteWallListLeft = new List<Sprite>();
    private List<Sprite> spriteDekoListRight = new List<Sprite>();
    private List<Sprite> spriteDekoListLeft = new List<Sprite>();
    private Sprite wallSprite;
    private Sprite dekoSprite;

    //constructor
    public WallObject(string wallGameObjectName, string wallSpriteName, int rotation, string wallChildName, int wallChildLength, float wallChildCoordCorrectionX, float wallChildCoordCorrectionY, int coordX, int coordY){
        this.wallGameObjectName = wallGameObjectName;
        this.wallSpriteName = wallSpriteName;
        this.rotation = rotation;
        this.wallChildName = wallChildName;
        this.wallChildLength = wallChildLength;
        this.wallChildCoordCorrectionX = wallChildCoordCorrectionX;
        this.wallChildCoordCorrectionY = wallChildCoordCorrectionY;
        this.coordX = coordX;
        this.coordY = coordY;

        LoadAssets();

        InstantiateWall();
        setWallSpriteFromList();
        setDekoSpriteFromList();
    }
    public WallObject(){
    }

    //methods
    public void LoadAssets(){
        //load prefab from folder
        WallPrefab = Resources.Load("Prefabs/WallPrefab") as GameObject;

        //load images from folder
        object[] sprites = Resources.LoadAll("Textures/Wall/WallRight",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	spriteWallListRight.Add((Sprite)sprites[x]);
        }
        sprites = Resources.LoadAll("Textures/Wall/WallLeft",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	spriteWallListLeft.Add((Sprite)sprites[x]);
        }

        sprites = Resources.LoadAll("Textures/Wall/WallDekoRight",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	spriteDekoListRight.Add((Sprite)sprites[x]);
        }
        sprites = Resources.LoadAll("Textures/Wall/WallDekoLeft",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	spriteDekoListLeft.Add((Sprite)sprites[x]);
        }
    }

    //generiert WallObject
    private void InstantiateWall(){
        if(rotation==0){
            WallGameObject = Instantiate(WallPrefab, new Vector2((coordX*2)-1,(1-coordX)), Quaternion.identity);
            WallGameObject.GetComponent<SpriteRenderer>().sortingOrder = coordX-3;
        }else if(rotation==1){
            WallGameObject = Instantiate(WallPrefab, new Vector2(-1.23f+((coordY-1)*-2),(1-coordY)), Quaternion.identity);
            WallGameObject.GetComponent<SpriteRenderer>().sortingOrder = coordY-3;
        }
        WallGameObject.name = wallGameObjectName;
        GameObject child = WallGameObject.transform.GetChild(0).gameObject;
        child.name = wallGameObjectName;
    }

    //rendert wallSprite
    private void RenderWallSprite(){
        WallGameObject.GetComponent<SpriteRenderer>().sprite = wallSprite;
    }
    
    //rendert DekoSprite und passt position an
    private void RenderDekoSprite(){
        GameObject child = WallGameObject.transform.GetChild(0).gameObject;
        child.GetComponent<SpriteRenderer>().sprite = dekoSprite;
        if(rotation==0){
            child.transform.position = new Vector2(WallGameObject.transform.position.x-wallChildCoordCorrectionX,WallGameObject.transform.position.y+wallChildCoordCorrectionY);
            child.GetComponent<SpriteRenderer>().sortingOrder = coordX-2;
        }else if(rotation==1){
            child.transform.position = new Vector2(WallGameObject.transform.position.x+wallChildCoordCorrectionX,WallGameObject.transform.position.y+wallChildCoordCorrectionY);
            child.GetComponent<SpriteRenderer>().sortingOrder = coordY-2;    
        }
    }

    public void DeleteChild(){//reset all child data
        this.wallChildName = null;
        this.wallChildLength = 0;
        this.wallChildCoordCorrectionX = 0.0f;
        this.wallChildCoordCorrectionY = 0.0f;
        this.dekoSprite = null;
        setDekoSpriteFromList();//reload data
    }


    //getters
    public string getInfo(){
        string info = wallGameObjectName+";"+wallSpriteName+";"+rotation+";"+wallChildName+";"+wallChildLength+";"+wallChildCoordCorrectionX+";"+wallChildCoordCorrectionY+";"+coordX+";"+coordY;
        return info;
    }



    //setters
    //sucht und speichert sprite from spriteWallList
    private void setWallSpriteFromList(){
        Sprite sprite = null;
        if(rotation==0){
            for(int a=0;a<spriteWallListRight.Count;a++){
                if(spriteWallListRight[a].name.Equals(wallSpriteName+"b")){
                    sprite = spriteWallListRight[a];
                }
            }
        }else if(rotation==1){
            for(int a=0;a<spriteWallListLeft.Count;a++){
                if(spriteWallListLeft[a].name.Equals(wallSpriteName+"a")){
                    sprite = spriteWallListLeft[a];
                }
            }
        }
        this.wallSprite = sprite;

        RenderWallSprite();
    }

    //sucht und speichert sprite from spriteDekoList
    private void setDekoSpriteFromList(){
        Sprite sprite = null;
        if(rotation==0){
            for(int a=0;a<spriteDekoListRight.Count;a++){
                if(spriteDekoListRight[a].name.Equals(wallChildName+"b")){
                    sprite = spriteDekoListRight[a];
                }
            }
        }else if(rotation==1){
            for(int a=0;a<spriteDekoListLeft.Count;a++){
                if(spriteDekoListLeft[a].name.Equals(wallChildName+"a")){
                    sprite = spriteDekoListLeft[a];
                }
            }
        }
        this.dekoSprite = sprite;

        RenderDekoSprite();
    }
}