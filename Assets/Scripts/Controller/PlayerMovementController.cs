using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class PlayerMovementController : MonoBehaviour
{
    //Player GO Refezenz
    public static GameObject PlayerCharacter;

    //beinhaltet alle playerSprites
    public static List<Sprite> SpriteList = new List<Sprite>();


    //position auf die der spieler STEHT 
    public static int[] curPlayerPos { get; set; }

    //enthält die position zu der sich der spieler bewegen soll
    public static int[] tryNewPos { get; set; }

    //makiert die position wo die tür steht
    public static int[] doorPos { get; set; }


    //enthält den aktuell zu gehenden weg für den spieler
    public static List<string> playerPath = new List<string>();

    //switcher ob der player sich gerade bewegt
    public static bool isPlayerInMove = false;

    //ist für die laufgeschwindigkeit abhänig von der zeit zuständig
    public float timer;

    //aktuell zu belaufendes FloorObject
    public string objName;

    //aktuelle spieler position WÄHREND des laufens
    static Vector3 curDynPlayerPos;

    //bestimmt die LaufAnimation des spielers 
    public int walkAnim = 0; //0=none,1=idle in cur state,2=right...

    //beinhalten die sprites für die playerAnim
    public static List<Sprite> FaceBoy = new List<Sprite>();
    public static List<Sprite> HairBoy = new List<Sprite>();
    public static List<Sprite> HairOverlayBoy = new List<Sprite>();
    public static List<Sprite> LegBoy = new List<Sprite>();
    public static List<Sprite> LegOverlayBoy = new List<Sprite>();
    public static List<Sprite> SkinBoy = new List<Sprite>();
    public static List<Sprite> SkinOverlayBoy = new List<Sprite>();
    public static List<Sprite> TshirtBoy = new List<Sprite>();
    public static List<Sprite> TshirtOverlayBoy = new List<Sprite>();
    public static List<Sprite> FaceGirl = new List<Sprite>();
    public static List<Sprite> HairGirl = new List<Sprite>();
    public static List<Sprite> HairOverlayGirl = new List<Sprite>();
    public static List<Sprite> LegGirl = new List<Sprite>();
    public static List<Sprite> LegOverlayGirl = new List<Sprite>();
    public static List<Sprite> SkinGirl = new List<Sprite>();
    public static List<Sprite> SkinOverlayGirl = new List<Sprite>();
    public static List<Sprite> TshirtGirl = new List<Sprite>();
    public static List<Sprite> TshirtOverlayGirl = new List<Sprite>();

    //wird aufgerufen wenn eine neue position angeklickt wurde
    public static void MovePlayer(int[] newPos){

        //wenn der spieler nicht in bewegung ist
        if(playerPath.Count==0){

            //wenn keine playerPos vorhanden ist, gehe davon aus das der spieler an der tür steht
            if(curPlayerPos==null){
                //aktuelle eingangs position für bsp npc generierung
                doorPos = FindDoorPos();
                curPlayerPos = doorPos;
            }

            //erhält die zu laufende path liste anhand der start und end pos
            playerPath = LabyrinthBuilder.LabyrinthManager(curPlayerPos, newPos);

            curDynPlayerPos = PlayerCharacter.transform.position;
        }
    }

    void Update(){

        //sorgt für eine flüssige bewegung
        timer += Time.deltaTime * 1.5F;

        //solange der PlayerPath schritte beinhaltet die noch nicht gegangen wurden...
        if(playerPath.Count!=0){

            //spieler läuft
            isPlayerInMove = true;

            //sucht das aktuell zu belaufende floorObj in der Scene
            objName = playerPath[0].Split(":")[0]+"-"+playerPath[0].Split(":")[1];

            //bewegt den spieler "grob"
            PlayerCharacter.transform.position = Vector3.Lerp(curDynPlayerPos, GameObject.Find(objName).gameObject.transform.position, timer);

            //render den spieler richtig auf dem spielfeld, sowie anim.
            PlayerRender();
            
            //floorObj wurde belaufen, zerstöre das element und übergebe die DynPlayerPos
            if(PlayerCharacter.transform.position==GameObject.Find(objName).gameObject.transform.position){
                playerPath.RemoveAt(0);
                timer = 0;
                curDynPlayerPos = PlayerCharacter.transform.position;
            }

            //instanziiert einmalig die Coroutine solange bis der spieler fertig gelaufen ist
            if(walkAnim==0){
                walkAnim=1;
                StartCoroutine(Anim());
            }

            //bestimmt die PlayerAnim
            if(GameObject.Find(objName).gameObject.transform.position.x<PlayerCharacter.transform.position.x){
                if(GameObject.Find(objName).gameObject.transform.position.y<PlayerCharacter.transform.position.y){
                    //links unten
                    walkAnim=2;
                }else{
                    //links oben
                    walkAnim=3;
                }
            }else{
                if(GameObject.Find(objName).gameObject.transform.position.y<PlayerCharacter.transform.position.y){
                    //rechts unten
                    walkAnim=4;
                }else{
                    //rechts oben
                    walkAnim=5;
                }
            }
        //spieler kann nichtmehr laufen
        }else if(playerPath.Count==0&&isPlayerInMove==true){

            //beim beenden der aktuellen position wird die letzte position die neue player pos
            curPlayerPos = new int[]{Int32.Parse(objName.Split("-")[0]), Int32.Parse(objName.Split("-")[1])};
        
            //spieler ist am ende angekommen, sorge dafür, das dieses statement erst wieder nach neuer spielerbewegung abgerufen wird
            isPlayerInMove = false;

            //deactivate walkAnim
            walkAnim = 0;
        }
    }

    //steuert die lauf animation für den spieler
    public IEnumerator Anim(){
        while(walkAnim!=0){
            //links unten  (front_left)
            if(walkAnim==2){
                PlayerCharacter.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0.178f,1.657f,-0.02f);
                PlayerCharacter.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(0.263f,3.437f,-0.02f);
                PlayerCharacter.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(0.263f,3.406f,-0.01f);
                PlayerCharacter.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(-0.153f,2.719f,-0.01f);
                PlayerCharacter.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(-0.22f,0.38f,-0.01f);
                PlayerCharacter.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.023f,2.643f,-0.01f);
                PlayerCharacter.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(0.185f,1.655f,-0.01f);
                PlayerCharacter.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.023f,2.642f,0f);
                PlayerCharacter.transform.GetChild(9).gameObject.transform.localPosition = new Vector3(-0.115f,0.672f,0f);
                //if(true){//Player.isBoy==true
                    for(int a=1;a<7;a++){
                        PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[80+a];
                        PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[80+a];
                        PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[80+a];
                        PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[80+a];
                        PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[80+a];
                        PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[80+a];
                        PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[80+a];
                        PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[80+a];
                        PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[80+a];
                        yield return new WaitForSeconds(0.2F);
                    }
                    PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[80];
                    PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[80];
                    PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[80];
                    PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[80];
                    PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[80];
                    PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[80];
                    PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[80];
                    PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[80];
                    PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[80];
                //}

            //links oben  
            }else if(walkAnim==3){
                PlayerCharacter.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0.178f,1.657f,-0.05f);
                PlayerCharacter.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(0.263f,3.437f,-0.07f);
                PlayerCharacter.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(0.263f,3.406f,-0.06f);
                PlayerCharacter.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(-0.153f,2.719f,-0.01f);
                PlayerCharacter.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(-0.22f,0.38f,-0.01f);
                PlayerCharacter.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.023f,2.643f,-0.03f);
                PlayerCharacter.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(0.185f,1.655f,-0.04f);
                PlayerCharacter.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.023f,2.642f,-0.02f);
                PlayerCharacter.transform.GetChild(9).gameObject.transform.localPosition = new Vector3(-0.115f,0.672f,0f);
                //if(true){//Player.isBoy==true
                    for(int a=1;a<7;a++){
                        PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[64+a];
                        PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[64+a];
                        PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[64+a];
                        PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[64+a];
                        PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[64+a];
                        PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[64+a];
                        PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[64+a];
                        PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[64+a];
                        PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[64+a];
                        yield return new WaitForSeconds(0.2F);
                    }
                    PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[71];
                    PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[71];
                    PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[71];
                    PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[71];
                    PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[71];
                    PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[71];
                    PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[71];
                    PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[71];
                    PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[71];
                //}
                
            //rechts unten
            }else if(walkAnim==4){
                PlayerCharacter.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.058f,1.617f,-0.03f);
                PlayerCharacter.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.134f,3.314f,-0.03f);
                PlayerCharacter.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(-0.137f,3.34f,-0.02f);
                PlayerCharacter.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(0.258f,2.691f,-0.01f);
                PlayerCharacter.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(0.3399f,0.3809f,-0.01f);
                PlayerCharacter.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(0.143f,2.611f,-0.01f);
                PlayerCharacter.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(-0.062f,1.616f,-0.02f);
                PlayerCharacter.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(0.143f,2.602f,0f);
                PlayerCharacter.transform.GetChild(9).gameObject.transform.localPosition = new Vector3(0.24f,0.63f,0f);
               //if(true){//Player.isBoy==true
                    for(int a=1;a<7;a++){
                        PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[88+a];
                        PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[88+a];
                        PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[88+a];
                        PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[88+a];
                        PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[88+a];
                        PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[88+a];
                        PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[88+a];
                        PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[88+a];
                        PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[88+a];
                        yield return new WaitForSeconds(0.2F);
                    }
                    PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[88];
                    PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[88];
                    PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[88];
                    PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[88];
                    PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[88];
                    PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[88];
                    PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[88];
                    PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[88];
                    PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[88];
                //} 
            //rechts oben
            }else if(walkAnim==5){
                PlayerCharacter.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.079f,1.747f,-0.05f);
                PlayerCharacter.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.136f,3.497f,-0.07f);
                PlayerCharacter.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(-0.136f,3.4659f,-0.06f);
                PlayerCharacter.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(-0.282f,2.719f,-0.01f);
                PlayerCharacter.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(0.336f,0.46f,-0.04f);
                PlayerCharacter.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(0.131f,2.731f,-0.03f);
                PlayerCharacter.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(-0.073f,1.745f,-0.04f);
                PlayerCharacter.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(0.131f,2.731f,-0.02f);
                PlayerCharacter.transform.GetChild(9).gameObject.transform.localPosition = new Vector3(0.234f,0.754f,-0.03f);
                //if(true){//Player.isBoy==true
                    for(int a=1;a<7;a++){
                        PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[72+a];
                        PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[72+a];
                        PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[72+a];
                        PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[72+a];
                        PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[72+a];
                        PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[72+a];
                        PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[72+a];
                        PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[72+a];
                        PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[72+a];
                        yield return new WaitForSeconds(0.2F);
                    }
                    PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[72];
                    PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[72];
                    PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[72];
                    PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[72];
                    PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[72];
                    PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[72];
                    PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[72];
                    PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[72];
                    PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[72];
                //}
            }

            Debug.Log("idle: "+walkAnim);
            yield return new WaitForSeconds(0.1F);
        }
    }

    //rendert spieler richtig und animiert den laufschritt
    public void PlayerRender(){
        //set sorting Order auf die der anderen floorChild obj (floorx+floory+1=SO)
        for(int a=0;a<PlayerCharacter.transform.childCount;a++){
            PlayerCharacter.transform.GetChild(a).gameObject.GetComponent<SpriteRenderer>().sortingOrder = Int32.Parse(playerPath[0].Split(":")[0])+Int32.Parse(playerPath[0].Split(":")[1])+1;
        }
    }

    //sucht den spieler und platziert ihn bei laden des spiels an der tür
    public static void LoadPlayer(){

        LoadSprites();

        //sucht die spieler UI
        PlayerCharacter = GameObject.Find("Player");

        //platziert den spieler an der tür
        PlayerCharacter.transform.position = new Vector2(GameObject.Find(FindDoorPos()[0]+"-"+FindDoorPos()[1]).transform.position.x, GameObject.Find(FindDoorPos()[0]+"-"+FindDoorPos()[1]).transform.position.y);

        //rendert den spieler auf die richtige ebene
        for(int a=0;a<PlayerCharacter.transform.childCount;a++){
            PlayerCharacter.transform.GetChild(a).gameObject.GetComponent<SpriteRenderer>().sortingOrder = FindDoorPos()[0]+FindDoorPos()[1]+1;
            PlayerCharacter.transform.GetChild(a).gameObject.transform.position = new Vector3(PlayerCharacter.transform.GetChild(a).gameObject.transform.position.x, PlayerCharacter.transform.GetChild(a).gameObject.transform.position.y+3.5f, PlayerCharacter.transform.GetChild(a).gameObject.transform.position.z);     
        }
    }

    //läd jedes sprite für die playerAnim (1728 stück)
    public static void LoadSprites(){

        //beinhaltet die geladenen sprites
        object[] spriteList = new object[]{};

        //beinhaltet temporär die geladene sprite liste
        spriteList = Resources.LoadAll("Textures/Player/Boy/face_boy",typeof(Sprite));
        foreach(object obj in spriteList){
           	FaceBoy.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Boy/hair_boy",typeof(Sprite));
        foreach(object obj in spriteList){
           	HairBoy.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Boy/hair_overlay_boy",typeof(Sprite));
        foreach(object obj in spriteList){
           	HairOverlayBoy.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Boy/leg_boy",typeof(Sprite));
        foreach(object obj in spriteList){
           	LegBoy.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Boy/leg_overlay_boy",typeof(Sprite));
        foreach(object obj in spriteList){
           	LegOverlayBoy.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Boy/skin_boy",typeof(Sprite));
        foreach(object obj in spriteList){
           	SkinBoy.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Boy/skin_overlay_boy",typeof(Sprite));
        foreach(object obj in spriteList){
           	SkinOverlayBoy.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Boy/tshirt_boy",typeof(Sprite));
        foreach(object obj in spriteList){
           	TshirtBoy.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Boy/tshirt_overlay_boy",typeof(Sprite));
        foreach(object obj in spriteList){
           	TshirtOverlayBoy.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Girl/face_girl",typeof(Sprite));
        foreach(object obj in spriteList){
           	FaceGirl.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Girl/hair_girl",typeof(Sprite));
        foreach(object obj in spriteList){
           	HairGirl.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Girl/hair_overlay_girl",typeof(Sprite));
        foreach(object obj in spriteList){
           	HairOverlayGirl.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Girl/leg_girl",typeof(Sprite));
        foreach(object obj in spriteList){
           	LegGirl.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Girl/leg_overlay_girl",typeof(Sprite));
        foreach(object obj in spriteList){
           	LegOverlayGirl.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Girl/skin_girl",typeof(Sprite));
        foreach(object obj in spriteList){
           	SkinGirl.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Girl/skin_overlay_girl",typeof(Sprite));
        foreach(object obj in spriteList){
           	SkinOverlayGirl.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Girl/tshirt_girl",typeof(Sprite));
        foreach(object obj in spriteList){
           	TshirtGirl.Add((Sprite)obj);
        }

        spriteList = Resources.LoadAll("Textures/Player/Girl/tshirt_overlay_girl",typeof(Sprite));
        foreach(object obj in spriteList){
           	TshirtOverlayGirl.Add((Sprite)obj);
        }
    }

    //sucht die DoorPos anhander der Tür an der Wand
    public static int[] FindDoorPos(){
        int[] pos = new int[]{0,0}; 
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach(GameObject item in allObjects){
            string[] split = item.name.Split("-");
            if(split.Length==3&&split[2].Equals("Wall")){
                if(!Object.ReferenceEquals(item.GetComponent<SpriteRenderer>().sprite, null)){
                    string[] spriteName = item.GetComponent<SpriteRenderer>().sprite.name.Split("_");
                    if(spriteName.Length>1){
                        if(spriteName[1].Equals("Door")){
                            if(Int32.Parse(item.name.Split("-")[0])!=0){
                                pos[0] = Int32.Parse(item.name.Split("-")[0])-1;
                                pos[1] = Int32.Parse(item.name.Split("-")[1]);
                            }else{
                                pos[0] = Int32.Parse(item.name.Split("-")[0]);
                                pos[1] = Int32.Parse(item.name.Split("-")[1])-1;
                            }
                        }
                    }
                }
            }
        }
        return pos;
    }
}
