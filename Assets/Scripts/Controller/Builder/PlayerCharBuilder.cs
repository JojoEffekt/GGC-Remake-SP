using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCharBuilder : MonoBehaviour
{   
    //referenz auf den spieler
    public GameObject PlayerPrefab;

    public static Player player;

    //läd bilder und instanziiert den char
    //kriegt die daten aus dem PlayerController script das bei start vom save&load script die daten konvertiert
    public static void Intizialisierer(Dictionary<string,string> PlayerDict){

        //läd daten als referenz
        player = new Player(bool.Parse(PlayerDict["Gender"]),PlayerDict["Hat"],PlayerDict["Face"],PlayerDict["Hair"],PlayerDict["HairOverlay"],PlayerDict["Leg"],PlayerDict["LegOverlay"],PlayerDict["Skin"],PlayerDict["SkinOverlay"],PlayerDict["Tshirt"],PlayerDict["TshirtOverlay"]);
    }
}

//constructor für den skin
public class Player 
{
    public bool gender { get; set; }                 //true = male, false = female
    public float[] hat { get; set; }                 //farbe für das jeweilige sprite
    public float[] face { get; set; }                //farbe für das jeweilige sprite
    public float[] hair { get; set; }                //farbe für das jeweilige sprite
    public float[] hair_overlay { get; set; }        //farbe für das jeweilige sprite
    public float[] leg { get; set; }                 //farbe für das jeweilige sprite
    public float[] leg_overlay { get; set; }         //farbe für das jeweilige sprite
    public float[] skin { get; set; }                //farbe für das jeweilige sprite
    public float[] skin_overlay { get; set; }        //farbe für das jeweilige sprite
    public float[] tshirt { get; set; }              //farbe für das jeweilige sprite
    public float[] tshirt_overlay { get; set; }      //farbe für das jeweilige sprite

    public Player(bool gender, string hat, string face, string hair, string hair_overlay, string leg, string leg_overlay, string skin, string skin_overlay, string tshirt, string tshirt_overlay){
        this.gender = gender;
        this.hat = new float[]{1f,1f,1f,1f};
        this.face = new float[]{float.Parse(face.Split("-")[0]),float.Parse(face.Split("-")[1]),float.Parse(face.Split("-")[2]),float.Parse(face.Split("-")[3])};
        this.hair = new float[]{float.Parse(hair.Split("-")[0]),float.Parse(hair.Split("-")[1]),float.Parse(hair.Split("-")[2]),float.Parse(hair.Split("-")[3])};
        this.hair_overlay = new float[]{float.Parse(hair_overlay.Split("-")[0]),float.Parse(hair_overlay.Split("-")[1]),float.Parse(hair_overlay.Split("-")[2]),float.Parse(hair_overlay.Split("-")[3])};
        this.leg = new float[]{float.Parse(leg.Split("-")[0]),float.Parse(leg.Split("-")[1]),float.Parse(leg.Split("-")[2]),float.Parse(leg.Split("-")[3])};
        this.leg_overlay = new float[]{float.Parse(leg_overlay.Split("-")[0]),float.Parse(leg_overlay.Split("-")[1]),float.Parse(leg_overlay.Split("-")[2]),float.Parse(leg_overlay.Split("-")[3])};
        this.skin = new float[]{float.Parse(skin.Split("-")[0]),float.Parse(skin.Split("-")[1]),float.Parse(skin.Split("-")[2]),float.Parse(skin.Split("-")[3])};
        this.skin_overlay = new float[]{float.Parse(skin_overlay.Split("-")[0]),float.Parse(skin_overlay.Split("-")[1]),float.Parse(skin_overlay.Split("-")[2]),float.Parse(skin_overlay.Split("-")[3])};
        this.tshirt = new float[]{float.Parse(tshirt.Split("-")[0]),float.Parse(tshirt.Split("-")[1]),float.Parse(tshirt.Split("-")[2]),float.Parse(tshirt.Split("-")[3])};
        this.tshirt_overlay = new float[]{float.Parse(tshirt_overlay.Split("-")[0]),float.Parse(tshirt_overlay.Split("-")[1]),float.Parse(tshirt_overlay.Split("-")[2]),float.Parse(tshirt_overlay.Split("-")[3])};

        UpdatePlayerColor();
    }

    //update gender
    public void setGender(bool val){
        this.gender = val;
        UpdatePlayerColor();
    }

    //setzte neuen farbwert für das gesicht
    public void setFace(float r, float g, float b, float a){
        this.face = new float[]{r,g,b,a};
        UpdatePlayerColor();
    }

    public void setHair(float r, float g, float b, float a){
        this.hair = new float[]{r,g,b,a};
        UpdatePlayerColor();
    }

    public void setHairOverlay(float r, float g, float b, float a){
        this.hair_overlay = new float[]{r,g,b,a};
        UpdatePlayerColor();
    }

    public void setLeg(float r, float g, float b, float a){
        this.leg = new float[]{r,g,b,a};
        UpdatePlayerColor();
    }

    public void setLegOverlay(float r, float g, float b, float a){
        this.leg_overlay = new float[]{r,g,b,a};
        UpdatePlayerColor();
    }

    public void setSkin(float r, float g, float b, float a){
        this.skin = new float[]{r,g,b,a};
        UpdatePlayerColor();
    }

    public void setSkinOverlay(float r, float g, float b, float a){
        this.skin_overlay = new float[]{r,g,b,a};
        UpdatePlayerColor();
    }

    public void setTshirt(float r, float g, float b, float a){
        this.tshirt = new float[]{r,g,b,a};
        UpdatePlayerColor();
    }

    public void setTshirtOverlay(float r, float g, float b, float a){
        this.tshirt_overlay = new float[]{r,g,b,a};
        UpdatePlayerColor();
    }

    public string getPlayerInfo(){
        string info = "Gender:"+gender+";Hat:"+hat[0]+"-"+hat[1]+"-"+hat[2]+"-"+hat[3]+";Face:"+face[0]+"-"+face[1]+"-"+face[2]+"-"+face[3]+"-"+";Hair:"+hair[0]+"-"+hair[1]+"-"+hair[2]+"-"+hair[3]+"-"+";HairOverlay:"+hair_overlay[0]+"-"+hair_overlay[1]+"-"+hair_overlay[2]+"-"+hair_overlay[3]+"-"+";Leg:"+leg[0]+"-"+leg[1]+"-"+leg[2]+"-"+leg[3]+"-"+";LegOverlay:"+leg_overlay[0]+"-"+leg_overlay[1]+"-"+leg_overlay[2]+"-"+leg_overlay[3]+"-"+";Skin:"+skin[0]+"-"+skin[1]+"-"+skin[2]+"-"+skin[3]+"-"+";SkinOverlay:"+skin_overlay[0]+"-"+skin_overlay[1]+"-"+skin_overlay[2]+"-"+skin_overlay[3]+"-"+";Tshirt:"+tshirt[0]+"-"+tshirt[1]+"-"+tshirt[2]+"-"+tshirt[3]+"-"+";TshirtOverlay:"+tshirt_overlay[0]+"-"+tshirt_overlay[1]+"-"+tshirt_overlay[2]+"-"+tshirt_overlay[3];
        return info;
    }

    //aktualisiere player ui
    public void UpdatePlayerColor(){

        //sucht die spieler UI
        GameObject PlayerCharacter = GameObject.Find("Player");

        //CONTINUE 
        //RELOAD PLAYER GENDER ETC

        //color
        PlayerCharacter.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(hat[0],hat[1],hat[2],hat[3]);
        PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = new Color(tshirt_overlay[0],tshirt_overlay[1],tshirt_overlay[2],tshirt_overlay[3]); 
        PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().color = new Color(hair_overlay[0],hair_overlay[1],hair_overlay[2],hair_overlay[3]); 
        PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().color = new Color(hair[0],hair[1],hair[2],hair[3]); 
        PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().color = new Color(face[0],face[1],face[2],face[3]); 
        PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().color = new Color(leg_overlay[0],leg_overlay[1],leg_overlay[2],leg_overlay[3]); 
        PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().color = new Color(skin_overlay[0],skin_overlay[1],skin_overlay[2],skin_overlay[3]); 
        PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().color = new Color(tshirt[0],tshirt[1],tshirt[2],tshirt[3]); 
        PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().color = new Color(skin[0],skin[1],skin[2],skin[3]); 
        PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().color = new Color(leg[0],leg[1],leg[2],leg[3]);
    }
}
