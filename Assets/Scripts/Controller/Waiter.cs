using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

public class Waiter : MonoBehaviour
{
    //prioritäten laden

    //funktionen
    //essen von tresen zu einem npc bringen
    //leeren teller vom tisch zum tresen bringen

    //beinhaltet den waiter
    public GameObject waiterGO;

    //variable zu wie viel prozent der waiter geschirr abräumt
    public int ToDish { get; set; }
    public int toServe { get; set; }

    //aktuelle aufgabe des waiters
    public int objective; //0=none,1=go to tresen

    //variable um flüssiges movement des waiters zu erzeugen
    private float timeDelayMovement = 0.0f;    

    //sagt aus ob der waiter gerade in bewegung ist
    //wird in verbindung mit der "waittime" benutzt
    public bool isOnWalk { get; set; }

    //beinhaltet die tür position
    public int[] doorPos { get; set;}

    //beinhaltet den path zum zum stuhl
    public List<string> path { get; set; }

    //beinhaltet die abzulaufende temporäre liste
    public List<string> curPath = new List<string>();

    //aktuelle waiter position WÄHREND des laufens
    public Vector3 curDynWaiterPos;

    //aktuell zu belaufendes FloorObject
    public string objName;

    //bestimmt die LaufAnimation des waiter 
    public int walkAnim = 0;

    //gender des waiters
    private bool isBoy;
    public bool IsBoy 
    { 
        get {
                return isBoy;
            }
        set {
                isBoy = value;
                CreateWaiterSkin();
            }
    }

    //anzeigename des waiters
    private string name;
    public string Name 
    {
        get {
                return name;
            }
        set {
                waiterGO.transform.GetChild(9).gameObject.GetComponent<TMPro.TextMeshPro>().text = value;
                name = value;
            }
    }

    //rgba values für skin
    private float[] hairColor;
    public float[] HairColor 
    { 
        get { 
                return hairColor; 
            }
        set { 
                waiterGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().color = new Color(value[0],value[1],value[2],1);
                hairColor = value;
            }
    }

    private float[] skinColor;
    public float[] SkinColor 
    { 
        get { 
                return skinColor; 
            } 
        set {
                waiterGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().color = new Color(value[0],value[1],value[2],1);
                skinColor = value;
            }
    }

    private float[] tshirtColor;
    public float[] TshirtColor 
    { 
        get {
                return tshirtColor;
            } 
        set {
                waiterGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().color = new Color(value[0],value[1],value[2],1);
                tshirtColor = value;
            }
    }

    private float[] hoseColor;
    public float[] HoseColor 
    { 
        get {
                return hoseColor;
            } 
        set {
                waiterGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().color = new Color(value[0],value[1],value[2],1);
                hoseColor = value;
            }
    }

    //beinhalten die sprites für die waiterAnim
    public List<Sprite> Hat = new List<Sprite>();
    public List<Sprite> FaceBoy = new List<Sprite>();
    public List<Sprite> HairBoy = new List<Sprite>();
    public List<Sprite> HairOverlayBoy = new List<Sprite>();
    public List<Sprite> LegBoy = new List<Sprite>();
    public List<Sprite> LegOverlayBoy = new List<Sprite>();
    public List<Sprite> SkinBoy = new List<Sprite>();
    public List<Sprite> SkinOverlayBoy = new List<Sprite>();
    public List<Sprite> TshirtBoy = new List<Sprite>();
    public List<Sprite> TshirtOverlayBoy = new List<Sprite>();
    public List<Sprite> FaceGirl = new List<Sprite>();
    public List<Sprite> HairGirl = new List<Sprite>();
    public List<Sprite> HairOverlayGirl = new List<Sprite>();
    public List<Sprite> LegGirl = new List<Sprite>();
    public List<Sprite> LegOverlayGirl = new List<Sprite>();
    public List<Sprite> SkinGirl = new List<Sprite>();
    public List<Sprite> SkinOverlayGirl = new List<Sprite>();
    public List<Sprite> TshirtGirl = new List<Sprite>();
    public List<Sprite> TshirtOverlayGirl = new List<Sprite>();

    //Constructor der für die initalisiereung verantwortlich ist
    public Waiter(GameObject prefab, int[] doorPos)
    {
        //npc ist gerade nicht in bewegung
        isOnWalk = false;

        //übergebe die position auf die die tür steht
        this.doorPos = doorPos;

        //erstelle den waiter
        CreateWaiter(prefab);

        //da der waiter beim initialisieren an der tür stht, ist die erste aufgabe zu einem tresen zu gehen
        //1 = gehe zum tresen
        objective = 1;

        //CONTINUE 
        //suche path von aktuelle posi zu besten tresen
        //laufe dorthin
    }

    //erstellt das gameObject für den waiter
    private void CreateWaiter(GameObject prefab)
    {
        //sucht die türposition auf die der waiter zum start gerendert wird
        GameObject doorGO = GameObject.Find(""+doorPos[0]+"-"+doorPos[1]);

        //vector3 für NPCMovement muss doorPos haben, da sonst beim initialisieren der ausgangswert (0,0,0) 
        curDynWaiterPos = new Vector3(doorGO.transform.position.x, doorGO.transform.position.y, 0);

        //erstellt waiter unter NPCHandlerGO
        GameObject NPCHandler = GameObject.Find("WaiterHandler");
        waiterGO = Instantiate(prefab, new Vector3(doorGO.transform.position.x, doorGO.transform.position.y, 0), Quaternion.identity, NPCHandler.transform);

        //rendere die teile auf die richtige stufenebene sodass der waiter über dem background ist
        for(int a=0;a<waiterGO.transform.childCount-1;a++)
        {
            waiterGO.transform.GetChild(a).gameObject.GetComponent<SpriteRenderer>().sortingOrder = doorPos[0]+doorPos[1]+1;
        }
        //sorting order für name
        waiterGO.transform.GetChild(9).gameObject.GetComponent<TMPro.TextMeshPro>().sortingOrder = 100;

        //erstelle die UI des waiters (gender, name & color)
        CreateWaiterSkin();
    }

    //gibt die grafik des waiters zurück
    public string Info()
    {
        return name+":"+isBoy+":"+ToDish+":"+toServe+":"+hairColor[0]+"-"+hairColor[1]+"-"+hairColor[2]+":"+skinColor[0]+"-"+skinColor[1]+"-"+skinColor[2]+":"+tshirtColor[0]+"-"+tshirtColor[1]+"-"+tshirtColor[2]+":"+hoseColor[0]+"-"+hoseColor[1]+"-"+hoseColor[2];
    }

    //lösche das waiterPrefab wenn der waiter zerstört wird durch build store öffnen
    public void DeleteWaiter()
    {
        Debug.Log("Lösche waiter: "+waiterGO.name);
        Destroy(waiterGO);
    }

    //erstelle den skin sowie gender etc
    private void CreateWaiterSkin()
    {
        //probiere waiter stats zu laden
        //wenn error, lade standardskin


        //beinhaltet die geladenen sprites
        object[] spriteList = new object[]{};

        //beinhaltet temporär die geladene sprite liste
        spriteList = Resources.LoadAll("Textures/Player/Hat",typeof(Sprite));
        foreach(object obj in spriteList)
        {
           	Hat.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Boy/face_boy",typeof(Sprite));
        foreach(object obj in spriteList)
        {
           	FaceBoy.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Boy/hair_boy",typeof(Sprite));
        foreach(object obj in spriteList)
        {
           	HairBoy.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Boy/hair_overlay_boy",typeof(Sprite));
        foreach(object obj in spriteList)
        {
           	HairOverlayBoy.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Boy/leg_boy",typeof(Sprite));
        foreach(object obj in spriteList)
        {
           	LegBoy.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Boy/leg_overlay_boy",typeof(Sprite));
        foreach(object obj in spriteList)
        {
           	LegOverlayBoy.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Boy/skin_boy",typeof(Sprite));
        foreach(object obj in spriteList)
        {
           	SkinBoy.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Boy/skin_overlay_boy",typeof(Sprite));
        foreach(object obj in spriteList)
        {
           	SkinOverlayBoy.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Boy/tshirt_boy",typeof(Sprite));
        foreach(object obj in spriteList)
        {
           	TshirtBoy.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Boy/tshirt_overlay_boy",typeof(Sprite));
        foreach(object obj in spriteList)
        {
           	TshirtOverlayBoy.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Girl/face_girl",typeof(Sprite));
        foreach(object obj in spriteList)
        {
           	FaceGirl.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Girl/hair_girl",typeof(Sprite));
        foreach(object obj in spriteList)
        {
           	HairGirl.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Girl/hair_overlay_girl",typeof(Sprite));
        foreach(object obj in spriteList)
        {
           	HairOverlayGirl.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Girl/leg_girl",typeof(Sprite));
        foreach(object obj in spriteList)
        {
           	LegGirl.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Girl/leg_overlay_girl",typeof(Sprite));
        foreach(object obj in spriteList)
        {
           	LegOverlayGirl.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Girl/skin_girl",typeof(Sprite));
        foreach(object obj in spriteList)
        {
           	SkinGirl.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Girl/skin_overlay_girl",typeof(Sprite));
        foreach(object obj in spriteList)
        {
           	SkinOverlayGirl.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Girl/tshirt_girl",typeof(Sprite));
        foreach(object obj in spriteList)
        {
           	TshirtGirl.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Girl/tshirt_overlay_girl",typeof(Sprite));
        foreach(object obj in spriteList)
        {
           	TshirtOverlayGirl.Add((Sprite)obj);
        }

        /*
        0 = tshirt overlay
        1 = leg overlay
        2 = leg
        3 = tshirt
        4 = hair
        5 = hair overlay
        6 = face
        7 = skin overlay
        8 = skin
        */

        //load gender waiter
        //male
        if(isBoy==true)
        {
            //bestimmt die rendering position
            waiterGO.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.178f,1.657f,-0.02f);
            waiterGO.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.22f,0.38f,-0.01f);
            waiterGO.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.115f,0.672f,0f);
            waiterGO.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(0.185f,1.655f,-0.01f);
            waiterGO.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(0.263f,3.406f,-0.01f);
            waiterGO.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(0.263f,3.437f,-0.02f);
            waiterGO.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.153f,2.719f,-0.01f);
            waiterGO.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(-0.023f,2.643f,-0.01f);
            waiterGO.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.023f,2.642f,0f);

            waiterGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[80];
            waiterGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[80];
            waiterGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[80];
            waiterGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[80];
            waiterGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[80];
            waiterGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[80];
            waiterGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[80];
            waiterGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[80];
            waiterGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[80];
        
        //female
        }
        else
        {
            //bestimmt die rendering position
            waiterGO.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.1f,1.782f,-0.02f);
            waiterGO.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.21f,0.56f,-0.01f);
            waiterGO.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.023f,1.05f,0f);
            waiterGO.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(0.102f,1.773f,-0.01f);
            waiterGO.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(0.411f,3.342f,-0.01f);
            waiterGO.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(0.4110f,3.342f,-0.02f);
            waiterGO.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(0.092f,2.804f,-0.01f);
            waiterGO.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(-0.081f,2.064f,-0.01f);
            waiterGO.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.083f,2.058f,0f);

            waiterGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[80];
            waiterGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[80];
            waiterGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[80];
            waiterGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[80];
            waiterGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[80];
            waiterGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[80];
            waiterGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[80];
            waiterGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[80];
            waiterGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[80];
            
        }
    }
}
