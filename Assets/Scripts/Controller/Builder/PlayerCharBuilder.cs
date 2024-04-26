using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCharBuilder : MonoBehaviour
{   
    //referenz auf den spieler
    public GameObject PlayerPrefab;

    //beinhaltet alle hüte
    public static List<Sprite> SpriteListHat = new List<Sprite>();

    public static Player player;

    //läd bilder und instanziiert den char
    //kriegt die daten aus dem PlayerController script das bei start vom save&load script die daten konvertiert
    public static void Intizialisierer(Dictionary<string,string> PlayerDict){
        LoadSprites();

        //läd daten als referenz
        player = new Player(bool.Parse(PlayerDict["Gender"]),PlayerDict["Hat"],PlayerDict["Face"],PlayerDict["Hair"],PlayerDict["HairOverlay"],PlayerDict["Leg"],PlayerDict["LegOverlay"],PlayerDict["Skin"],PlayerDict["SkinOverlay"],PlayerDict["Tshirt"],PlayerDict["TshirtOverlay"]);

        player.setFace(0.1f,1f,0.5f,1);
    }

    public static void LoadSprites(){
        object[] sprites = Resources.LoadAll("Textures/Player/Hat",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	SpriteListHat.Add((Sprite)sprites[x]);
        }
    }
}

//constructor für den skin
public class Player 
{
    public bool gender { get; set; }                //true = male, false = female
    public string hat { get; set; }                 //farbe für das jeweilige sprite
    public float[] face { get; set; }                //farbe für das jeweilige sprite
    public string hair { get; set; }                //farbe für das jeweilige sprite
    public string hair_overlay { get; set; }        //farbe für das jeweilige sprite
    public string leg { get; set; }                 //farbe für das jeweilige sprite
    public string leg_overlay { get; set; }         //farbe für das jeweilige sprite
    public string skin { get; set; }                //farbe für das jeweilige sprite
    public string skin_overlay { get; set; }        //farbe für das jeweilige sprite
    public string tshirt { get; set; }              //farbe für das jeweilige sprite
    public string tshirt_overlay { get; set; }      //farbe für das jeweilige sprite

    public Player(bool gender, string hat, string face, string hair, string hair_overlay, string leg, string leg_overlay, string skin, string skin_overlay, string tshirt, string tshirt_overlay){
        this.gender = gender;
        this.hat = hat;
        this.face = new float[]{float.Parse(face.Split("-")[0]),float.Parse(face.Split("-")[1]),float.Parse(face.Split("-")[2]),float.Parse(face.Split("-")[3])};
        this.hair = hair;
        this.hair_overlay = hair_overlay;
        this.leg = leg;
        this.leg_overlay = leg_overlay;
        this.skin = skin;
        this.skin_overlay = skin_overlay;
        this.tshirt = tshirt;
        this.tshirt_overlay = tshirt_overlay;

        Debug.Log("face: "+this.face[0]);
    }

    //setzte neuen farbwert für das gesicht
    public void setFace(float r, float g, float b, float a){
        Debug.Log("val: "+r);
    }

    //CONITNUE
    //...
    //update player char
    // mache hier funktionen hin um palyer farbe zu ändern (über shop)
}
