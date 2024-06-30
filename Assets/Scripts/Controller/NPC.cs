using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPC : MonoBehaviour
{   
    //beinhaltet den npc
    public GameObject npcGO;



    //variable um flüssiges movement des npcs zu erzeugen
    private float timeDelayMovement = 0.0f;    

    //sagt aus was der npc gerade macht
    public int state = 0; //0=nichts, 1=kann bediennt werden, 2=geht gerade zur tür, 3=steht wieder an der tür weil nicht bedient

    //sagt aus ob der npc gerade in bewegung ist
    //wird in verbindung mit der "waittime" benutzt
    public bool isOnWalk { get; set; }



    //die zeit die der npc nocht wartet bevor er geht
    //wird beim hinsetzen an einem tisch zurückgesetzt
    //wird während laufen pausiert
    public int waittime { get; set; }

    
    //beinhaltet die tür position
    public int[] doorPos { get; set;}

    //position von der der npc loslaufen und hinlaufen soll
    public int[] startPos { get; set; }
    public int[] endPos { get; set; }

    //der GO name des stuhls/table zum essen
    public int[] chairPos { get; set; }
    public int[] tablePos { get; set; }

    //beinhaltet den path zum zum stuhl
    public List<string> npcPath { get; set; }

    //beinhaltet die abzulaufende liste, temp
    public List<string> curPath = new List<string>();

    //aktuelle spieler position WÄHREND des laufens
    public Vector3 curDynPlayerPos;

    //aktuell zu belaufendes FloorObject
    public string objName;



    //gender des npcs
    private bool isBoy;

    //beinhalten die sprites für die playerAnim
    private List<Sprite> Hat = new List<Sprite>();
    private List<Sprite> FaceBoy = new List<Sprite>();
    private List<Sprite> HairBoy = new List<Sprite>();
    private List<Sprite> HairOverlayBoy = new List<Sprite>();
    private List<Sprite> LegBoy = new List<Sprite>();
    private List<Sprite> LegOverlayBoy = new List<Sprite>();
    private List<Sprite> SkinBoy = new List<Sprite>();
    private List<Sprite> SkinOverlayBoy = new List<Sprite>();
    private List<Sprite> TshirtBoy = new List<Sprite>();
    private List<Sprite> TshirtOverlayBoy = new List<Sprite>();
    private List<Sprite> FaceGirl = new List<Sprite>();
    private List<Sprite> HairGirl = new List<Sprite>();
    private List<Sprite> HairOverlayGirl = new List<Sprite>();
    private List<Sprite> LegGirl = new List<Sprite>();
    private List<Sprite> LegOverlayGirl = new List<Sprite>();
    private List<Sprite> SkinGirl = new List<Sprite>();
    private List<Sprite> SkinOverlayGirl = new List<Sprite>();
    private List<Sprite> TshirtGirl = new List<Sprite>();
    private List<Sprite> TshirtOverlayGirl = new List<Sprite>();

    private int temp;

    //Constructor der für die initalisiereung verantwortlich ist
    public NPC(GameObject prefab, int temp, int[] doorPos)
    {
        //setzte die selbstzerstörung bei nicht handlungsfähig auf 30 sekunden
        waittime = 30;

        //npc ist gerade nicht in bewegung
        isOnWalk = false;

        //übergebe die position auf die die tür steht
        this.doorPos = doorPos;

        this.temp = temp;

        //erstelle den npc
        CreateNPC(prefab);
    }

    //wird vom npcController aufgerufen wenn npc am laufen ist
    public void UpdateAnim()
    {
        //sorgt für eine flüssige bewegung
        timeDelayMovement += Time.deltaTime * 1.5f;
        
        if(curPath.Count!=0)
        {
            //sucht das aktuell zu belaufende floorObj in der Scene
            GameObject objName = GameObject.Find(curPath[0].Split(":")[0]+"-"+curPath[0].Split(":")[1]).gameObject;

            //bewegt den spieler "grob"
            npcGO.transform.position = Vector3.Lerp(curDynPlayerPos, objName.transform.position, timeDelayMovement);


            //render den spieler richtig auf dem spielfeld, sowie anim.
            for(int a=0;a<npcGO.transform.childCount;a++){
                npcGO.transform.GetChild(a).gameObject.GetComponent<SpriteRenderer>().sortingOrder = Int32.Parse(curPath[0].Split(":")[0])+Int32.Parse(curPath[0].Split(":")[1])+1;
            }

            //floorObj wurde belaufen, zerstöre das element und übergebe die DynPlayerPos
            if(npcGO.transform.position==objName.transform.position)
            {
                //lösche letzten npcPath eintrag sodas der npc zur nächsten pos geht
                curPath.RemoveAt(0);
                timeDelayMovement = 0;
                curDynPlayerPos = npcGO.transform.position;

                //wenn der npc fertig mit laufen ist
                if(curPath.Count==0)
                {
                    //npc läuft nicht mehr, cooldown bis er geht
                    isOnWalk = false;

                    //npc steht immernoch an der tür
                    if(state==0)
                    {
                        //npc kann bedient werden
                        state = 1;
                    }
                    //gucke ob das schon der rückweg des npcs ist
                    else if(state==2)
                    {
                        //npc steht wieder an der tür
                        //kann gelöscht werden
                        state = 3;
                    }
                }
            }
        }
    }

    //lösche das NPCPrefab wenn der npc zerstört wird
    public void DeleteNPC()
    {
        Debug.Log("Lösche: "+npcGO.name);
        Destroy(npcGO);
    }

    //laufe zur neuen position anhand des erstellten path
    //gibt TRUE zurück wenn npc am ziel ankommen ist
    public bool NPCMovement(bool val)
    { 
        //kopiere die übergebene liste damit aus dieser werte entfernen können ohne die originale zu zerstören
        //CONTINUE gucke welche liste hier erstellt werden soll / BZW INVERTIERE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        curPath.Clear();
        //path von platz zur tür
        if(!val)
        {
            for(int a=npcPath.Count-1;a>=0;a--)
            {
                curPath.Add(npcPath[a]);
            }
        }
        //path von tür zu platz
        else
        {
            for(int a=0;a<npcPath.Count;a++)
            {
                curPath.Add(npcPath[a]);
            }
        }

        //npc setzt sich in bewegung
        isOnWalk = true;

        return true;
    }

    //instantiate den npc anhand des prefabs und fügt einen zufälligen skin ein
    private void CreateNPC(GameObject prefab)
    {
        //sucht die türposition auf die der npc zum start gerendert wird
        GameObject doorGO = GameObject.Find(""+doorPos[0]+"-"+doorPos[1]);

        //vector3 für NPCMovement muss doorPos haben, da sonst beim initialisieren der ausgangswert (0,0,0) 
        curDynPlayerPos = new Vector3(doorGO.transform.position.x, doorGO.transform.position.y, 0);

        //erstellt npc unter NPCHandler go
        GameObject NPCHandler = GameObject.Find("NPCHandler");
        npcGO = Instantiate(prefab, new Vector3(doorGO.transform.position.x, doorGO.transform.position.y, 0), Quaternion.identity, NPCHandler.transform);
        
        //npc namen setzten
        npcGO.name = ""+temp;

        //rendere die teile auf die richtige stufenebene sodass sich der npc über dem background ist
        for(int a=0;a<npcGO.transform.childCount;a++)
        {
            npcGO.transform.GetChild(a).gameObject.GetComponent<SpriteRenderer>().sortingOrder = doorPos[0]+doorPos[1]+1;
        }

        //erstelle die UI des npcs (gender & color)
        CreateNPCSkin();
    }

    //erstelle den skin sowie gender etc
    private void CreateNPCSkin()
    {
        System.Random rndm = new System.Random();
        int rndmGender = rndm.Next(0,100);
        
        //bestimmt ob mänl. oder weibl.
        if(rndmGender%2==1)
        {
            isBoy = true;
        }
        else
        {
            isBoy = false;
        }

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

        //load gender npc
        //male
        if(isBoy==true)
        {
            //bestimmt die rendering position
            npcGO.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.178f,1.657f,-0.02f);
            npcGO.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.22f,0.38f,-0.01f);
            npcGO.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.115f,0.672f,0f);
            npcGO.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(0.185f,1.655f,-0.01f);
            npcGO.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(0.263f,3.406f,-0.01f);
            npcGO.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(0.263f,3.437f,-0.02f);
            npcGO.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.153f,2.719f,-0.01f);
            npcGO.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(-0.023f,2.643f,-0.01f);
            npcGO.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.023f,2.642f,0f);

            npcGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[80];
            npcGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[80];
            npcGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[80];
            npcGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[80];
            npcGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[80];
            npcGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[80];
            npcGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[80];
            npcGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[80];
            npcGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[80];
        
        //female
        }
        else
        {
            //bestimmt die rendering position
            npcGO.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.1f,1.782f,-0.02f);
            npcGO.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.21f,0.56f,-0.01f);
            npcGO.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.023f,1.05f,0f);
            npcGO.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(0.102f,1.773f,-0.01f);
            npcGO.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(0.411f,3.342f,-0.01f);
            npcGO.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(0.4110f,3.342f,-0.02f);
            npcGO.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(0.092f,2.804f,-0.01f);
            npcGO.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(-0.081f,2.064f,-0.01f);
            npcGO.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.083f,2.058f,0f);

            npcGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[80];
            npcGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[80];
            npcGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[80];
            npcGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[80];
            npcGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[80];
            npcGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[80];
            npcGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[80];
            npcGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[80];
            npcGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[80];
            
        }

        //beinhaltet alle farben für die hairColor
        float[,] hairColor = new float[,]{{0.94f,0.87f,0.58f},{0.97f,0.81f,0.35f},{0.97f,0.44f,0.26f},{0.83f,0.56f,0.32f},{0.65f,0.37f,0.29f},{0.40f,0.27f,0.16f},{0.23f,0.14f,0.08f},{0.14f,0.05f,0.02f},{0.80f,0.70f,0.36f},{0.66f,0.54f,0.34f},{0.76f,0.45f,0.22f},{0.57f,0.39f,0.25f},{0.38f,0.27f,0.17f},{0.31f,0.22f,0.12f},{0.23f,0.16f,0.08f}};
        //beinhaltet alle farben für die skinColor
        float[,] skinColor = new float[,]{{0.94f,0.77f,0.61f},{0.92f,0.68f,0.49f},{0.93f,0.61f,0.41f},{0.69f,0.39f,0.27f},{0.81f,0.53f,0.36f},{0.93f,0.73f,0.58f},{0.92f,0.68f,0.46f},{0.81f,0.58f,0.37f}};
        //beinhaltet alle farben für die tshirtColor
        float[,] tshirtColor = new float[,]{{0.26f,0.32f,0.31f},{0.77f,0.78f,0.81f},{0.45f,0.70f,0.72f},{0.40f,0.70f,0.41f},{0.95f,0.87f,0.44f},{0.93f,0.72f,0.76f},{1f,0.49f,0.56f},{0.81f,0.55f,0.77f},{0.29f,0.52f,0.45f},{0.33f,0.54f,0.33f},{0.41f,0.64f,0.23f},{0.50f,0.77f,0.64f},{0.52f,0.69f,0.69f},{0.27f,0.69f,0.50f},{0.71f,0.69f,0.68f},{0.75f,0.52f,0.23f},{0.68f,0.36f,0.23f}};
        //beinhaltet alle farben für die hoseColor
        float[,] hoseColor = new float[,]{{0.26f,0.32f,0.31f},{0.77f,0.78f,0.81f},{0.45f,0.70f,0.72f},{0.40f,0.70f,0.41f},{0.95f,0.87f,0.44f},{0.93f,0.72f,0.76f},{1f,0.49f,0.56f},{0.81f,0.55f,0.77f},{0.29f,0.52f,0.45f},{0.33f,0.54f,0.33f},{0.41f,0.64f,0.23f},{0.50f,0.77f,0.64f},{0.52f,0.69f,0.69f},{0.27f,0.69f,0.50f},{0.71f,0.69f,0.68f},{0.75f,0.52f,0.23f},{0.68f,0.36f,0.23f}};
    
        //generiere eine zufällige zahl die in der range jeder colorList liegt um dem npc ein zufälliges aussehen zu geben
        int rndmHair = rndm.Next(0,hairColor.Length/3);
        int rndmSkin = rndm.Next(0,skinColor.Length/3);
        int rndmTshirt = rndm.Next(0,tshirtColor.Length/3);
        int rndmHose = rndm.Next(0,hoseColor.Length/3);

        //setzt die farbe für die körperteile 
        npcGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().color = new Color(hairColor[rndmHair,0],hairColor[rndmHair,1],hairColor[rndmHair,2],1); 
        npcGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().color = new Color(tshirtColor[rndmTshirt,0],tshirtColor[rndmTshirt,1],tshirtColor[rndmTshirt,2],1); 
        npcGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().color = new Color(skinColor[rndmSkin,0],skinColor[rndmSkin,1],skinColor[rndmSkin,2],1); 
        npcGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().color = new Color(hoseColor[rndmHose,0],hoseColor[rndmHose,1],hoseColor[rndmHose,2],1);
    }
}