using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCharBuilder : MonoBehaviour
{   
    //referenz auf den spieler
    public GameObject PlayerPrefab;

    //beinhaltet alle hüte
    public List<Sprite> SpriteListHat = new List<Sprite>();

    //läd bilder und instanziiert den char
    public void Intizialisierer(){
        LoadSprites();

        //sitzen stehen laufen
        //links vorne rechts unten
    }

    public void LoadSprites(){
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
    public string face { get; set; }                //farbe für das jeweilige sprite
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
        this.face = face;
        this.hair = hair;
        this.hair_overlay = hair_overlay;
        this.leg = leg;
        this.leg_overlay = leg_overlay;
        this.skin = skin;
        this.skin_overlay = skin_overlay;
        this.tshirt = tshirt;
        this.tshirt_overlay = tshirt_overlay;
    }
}
