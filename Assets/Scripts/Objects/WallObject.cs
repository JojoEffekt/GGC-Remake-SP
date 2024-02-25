using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallObject : MonoBehaviour
{
    //attributes
    private string wallName;
    private int rotation;
    private string wallChildName;
    private int wallChildLength;
    private float wallChildCoordCorrection;
    private int coordX;
    private int coordY;

    private GameObject WallPrefab;
    private GameObject WallGameObject;

    private List<Sprite> spriteWallListRight = new List<Sprite>();
    private List<Sprite> spriteWallListLeft = new List<Sprite>();
    private List<Sprite> spriteDekoListRight = new List<Sprite>();
    private List<Sprite> spriteDekoListLeft = new List<Sprite>();
    private Sprite wallSprite;
    private Sprite dekoSprite;

    //constructor
    public WallObject(string wallName, int rotation, string wallChildName, int wallChildLength, float wallChildCoordCorrection, int coordX, int coordY){
        this.wallName = wallName;
        this.rotation = rotation;
        this.wallChildName = wallChildName;
        this.wallChildLength = wallChildLength;
        this.wallChildCoordCorrection = wallChildCoordCorrection;
        this.coordX = coordX;
        this.coordY = coordY;

        LoadAssets();
        setWallSpriteFromList();
        setDekoSpriteFromList();
        
        //generate UI
        if(rotation==0){
            WallGameObject = Instantiate(WallPrefab, new Vector2((coordX*2)-1,(1-coordX)), Quaternion.identity);
        }else if(rotation==1){
            WallGameObject = Instantiate(WallPrefab, new Vector2(-1.23f+((coordY-1)*-2),(1-coordY)), Quaternion.identity);
        }

        setWallSprite();
        setDekoSprite();
    }

    //methods
    public void LoadAssets(){
        //load prefab from folder
        WallPrefab = Resources.Load("Prefabs/WallPrefab") as GameObject;

        //load images from folder
        object[] sprites = Resources.LoadAll("Wall/WallRight",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	spriteWallListRight.Add((Sprite)sprites[x]);
        }
        sprites = Resources.LoadAll("Wall/WallLeft",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	spriteWallListLeft.Add((Sprite)sprites[x]);
        }

        sprites = Resources.LoadAll("Wall/WallDekoRight",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	spriteDekoListRight.Add((Sprite)sprites[x]);
        }
        sprites = Resources.LoadAll("Wall/WallDekoLeft",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	spriteDekoListLeft.Add((Sprite)sprites[x]);
        }
    }
    public void Info(){
        Debug.Log("WallObject-"+wallSprite.name+": ["+coordX+","+coordY+"], rotation:"+rotation+", wallChildName:"+wallChildName+", wallChildLength:"+wallChildLength);
    }

    //getters
    public string getWallName(){
        return this.wallName;
    }
    public int getRotation(){
        return this.rotation;
    }
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
    public void setWallName(string wallName){
        this.wallName = wallName;

        //render new wallName
        setWallSpriteFromList();
        setWallSprite();
    }
    public void setRotation(int rotation){
        this.rotation = rotation;
    }
    public void setWallChildName(string name){
        this.wallChildName = name;

        //render new dekoName
        setDekoSpriteFromList();
        setDekoSprite();
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
    public void setWallChildNameEmpty(){
        //continue
        //remove deko, add to inventory 
    }

    private void setWallSpriteFromList(){
        //suche sprite from lists
        Sprite sprite = null;
        if(rotation==0){
            for(int a=0;a<spriteWallListRight.Count;a++){
                if(spriteWallListRight[a].name.Equals(wallName+"b")){
                    sprite = spriteWallListRight[a];
                }
            }
        }else if(rotation==1){
            for(int a=0;a<spriteWallListLeft.Count;a++){
                if(spriteWallListLeft[a].name.Equals(wallName+"a")){
                    sprite = spriteWallListLeft[a];
                }
            }
        }
        this.wallSprite = sprite;
    }
    private void setWallSprite(){
        WallGameObject.GetComponent<SpriteRenderer>().sprite = wallSprite;
    }
    private void setDekoSpriteFromList(){
        //suche sprite from lists
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
    }
    private void setDekoSprite(){
        WallGameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = dekoSprite;
        WallGameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 1;
        if(rotation==0){
            WallGameObject.transform.GetChild(0).transform.position = new Vector2(WallGameObject.transform.position.x-wallChildCoordCorrection,WallGameObject.transform.position.y+0.85f);
        }else if(rotation==1){
            WallGameObject.transform.GetChild(0).transform.position = new Vector2(WallGameObject.transform.position.x+wallChildCoordCorrection,WallGameObject.transform.position.y+0.85f);
        }
    }
}