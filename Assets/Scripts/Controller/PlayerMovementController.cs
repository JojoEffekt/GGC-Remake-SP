using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    //feld auf dem der spieler zuletzt war (dynamisch)
    public string lastWalkedObjName;

    //aktuelle spieler position WÄHREND des laufens
    static Vector3 curDynPlayerPos;

    //bestimmt die LaufAnimation des spielers 
    public int walkAnim = 0; //0=none,1=idle in cur state,2=right...

    //beinhalten die sprites für die playerAnim
    public static List<Sprite> Hat = new List<Sprite>();
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



    //speichert temp daten, wenn essen zum counter geliefert wird
    public static List<string> counterData = new List<string>();



    //wird aufgerufen wenn eine neue position angeklickt wurde
    public static bool MovePlayer(int[] newPos){

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

            //damit wird überprüft ob eine Bewegung des spieler durchgeführt wird
            if(playerPath.Count!=0){
                return true;
            }
        }
        //wenn der spieler nicht zu der gegebenen koordinate gehen kann
        return false;
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

                //speicher das zulest belafende obj
                lastWalkedObjName = playerPath[0];

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
        //spieler kann nichtmehr laufen und ist an position angekommen
        }else if(playerPath.Count==0&&isPlayerInMove==true){

            //beim beenden der aktuellen position wird die letzte position die neue player pos
            curPlayerPos = new int[]{Int32.Parse(objName.Split("-")[0]), Int32.Parse(objName.Split("-")[1])};
        
            //spieler ist am ende angekommen, sorge dafür, das dieses statement erst wieder nach neuer spielerbewegung abgerufen wird
            isPlayerInMove = false;

            //deactivate walkAnim
            walkAnim = 0;

            //ist für Dinnercontroller wichtig (dinner auf counter rendern)
            //wird benutzt um abzufragen ob darauf gewartet wird wenn der spieler zu einem
            //objekt gehen soll und danach aktion folgt
            if(DinnerController.isWaitForPlayerToStopCounter){
                DinnerController.isWaitForPlayerToStopCounter = false;

                //spieler ist am counter, aktiviere das render des dinners auf dem counter
                //darf erst passieren wenn, der spieler am counter ist, aber in background ist schon alles übernommen
                CounterController.AddDinnerOnCounter(counterData[0], counterData[1]);
            }
            //WICHTIG!!!
            /*
            MUSS! vor DinnerController.isWaitForPlayerToStop abgespielt werden, weil: 
                bei DinnerController.ReduceStepCount_UI() der "isWaitForPlayerToStopCounter" auf true
                gesetzt wird und somit "if(DinnerController.isWaitForPlayerToStopCounter){" direkt im
                anschluss aktiviert wird und das dinner direkt beim ankommen an den oven auf den counter 
                gerendert wird
            */

            //ist für Dinnercontroller wichtig (StepAnzahl FCED / dinner "anbauen")
            //wird benutzt um abzufragen ob darauf gewartet wird wenn der spieler zu einem
            //objekt gehen soll und danach aktion folgt
            if(DinnerController.isWaitForPlayerToStop){
                DinnerController.isWaitForPlayerToStop = false;

                //spieler ist am ziel
                DinnerController.ReduceStepCount_UI();
            }
        }
    }

    //steuert die lauf animation für den spieler
    public IEnumerator Anim(){

        //solange der spieler läuft
        while(walkAnim!=0){

            //links unten  (front_left)
            if(walkAnim==2){

                //überprüft das geschlecht und rendert je nach dem male, female (true=male)
                //male
                if(PlayerCharBuilder.player.gender==true){

                    //bestimmt die rendering position
                    PlayerCharacter.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.79f,5.38f,-0.04f);
                    PlayerCharacter.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0.178f,1.657f,-0.02f);
                    PlayerCharacter.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(0.263f,3.393f,-0.03f);
                    PlayerCharacter.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(0.263f,3.406f,-0.02f);
                    PlayerCharacter.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(-0.153f,2.719f,-0.01f);
                    PlayerCharacter.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(-0.22f,0.38f,-0.01f);
                    PlayerCharacter.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.023f,2.643f,-0.01f);
                    PlayerCharacter.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(0.185f,1.655f,-0.01f);
                    PlayerCharacter.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.023f,2.642f,0f);
                    PlayerCharacter.transform.GetChild(9).gameObject.transform.localPosition = new Vector3(-0.115f,0.672f,0f);

                    //animation
                    for(int a=1;a<7;a++){
                        PlayerCharacter.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Hat[1];
                        PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[80+a];
                        PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[80+a];
                        PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[80+a];
                        PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[80+a];
                        PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[80+a];
                        PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[80+a];
                        PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[80+a];
                        PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[80+a];
                        PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[80+a];

                        //zeitspann der einzelnen frames
                        if(walkAnim==2){
                            yield return new WaitForSeconds(0.15F);
                        }
                    }

                    //endzenario, wird gebraucht fals der spieler steht
                    PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[80];
                    PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[80];
                    PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[80];
                    PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[80];
                    PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[80];
                    PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[80];
                    PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[80];
                    PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[80];
                    PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[80];

                //female
                }else{
                    
                    //bestimmt die rendering position
                    PlayerCharacter.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.539f,5.457f,-0.03f);
                    PlayerCharacter.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0.1f,1.782f,-0.02f);
                    PlayerCharacter.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(0.4230f,3.345f,-0.02f);
                    PlayerCharacter.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(0.423f,3.345f,-0.01f);
                    PlayerCharacter.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(0.092f,2.804f,-0.01f);
                    PlayerCharacter.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(-0.21f,0.56f,-0.01f);
                    PlayerCharacter.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.081f,2.064f,-0.01f);
                    PlayerCharacter.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(0.102f,1.773f,-0.01f);
                    PlayerCharacter.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.083f,2.058f,0f);
                    PlayerCharacter.transform.GetChild(9).gameObject.transform.localPosition = new Vector3(-0.023f,1.05f,0f);

                    //animation
                    for(int a=1;a<7;a++){
                        PlayerCharacter.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Hat[1];
                        PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[80+a];
                        PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[80+a];
                        PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[80+a];
                        PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[80+a];
                        PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[80+a];
                        PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[80+a];
                        PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[80+a];
                        PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[80+a];
                        PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[80+a];

                        //zeitspann der einzelnen frames
                        if(walkAnim==2){
                            yield return new WaitForSeconds(0.15F);
                        }
                    }

                    //endzenario, wird gebraucht fals der spieler steht
                    PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[80];
                    PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[80];
                    PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[80];
                    PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[80];
                    PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[80];
                    PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[80];
                    PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[80];
                    PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[80];
                    PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[80];
                }

            //links oben  
            }else if(walkAnim==3){

                if(PlayerCharBuilder.player.gender==true){

                    PlayerCharacter.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.62f,5.16f,-0.07f);
                    PlayerCharacter.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0.178f,1.657f,-0.07f);
                    PlayerCharacter.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(0.263f,3.437f,-0.05f);
                    PlayerCharacter.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(0.263f,3.406f,-0.04f);//hair
                    PlayerCharacter.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(-0.153f,2.719f,-0.01f);
                    PlayerCharacter.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(-0.22f,0.38f,-0.05f);//leg o
                    PlayerCharacter.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.023f,2.643f,-0.03f);
                    PlayerCharacter.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(0.185f,1.655f,-0.06f);
                    PlayerCharacter.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.023f,2.642f,-0.02f);//skin
                    PlayerCharacter.transform.GetChild(9).gameObject.transform.localPosition = new Vector3(-0.115f,0.672f,-0.04f);//leg

                    for(int a=1;a<7;a++){
                        PlayerCharacter.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Hat[1];
                        PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[64+a];
                        PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[64+a];
                        PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[64+a];
                        PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[64+a];
                        PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[64+a];
                        PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[64+a];
                        PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[64+a];
                        PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[64+a];
                        PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[64+a];
                        if(walkAnim==3){
                            yield return new WaitForSeconds(0.15F);
                        }
                    }

                    PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[64];
                    PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[64];
                    PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[64];
                    PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[64];
                    PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[71];
                    PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[64];
                    PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[64];
                    PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[64];
                    PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[64];
                
                //female
                }else{

                    //bestimmt die rendering position
                    PlayerCharacter.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.539f,5.457f,-0.04f);
                    PlayerCharacter.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0.1f,1.782f,-0.04f);
                    PlayerCharacter.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(0.4230f,3.345f,-0.03f);
                    PlayerCharacter.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(0.423f,3.345f,-0.02f);
                    PlayerCharacter.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(0.092f,2.804f,-0.01f);
                    PlayerCharacter.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(-0.21f,0.56f,-0.02f);
                    PlayerCharacter.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.081f,2.064f,-0.01f);
                    PlayerCharacter.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(0.102f,1.773f,-0.03f);
                    PlayerCharacter.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.083f,2.058f,0f);
                    PlayerCharacter.transform.GetChild(9).gameObject.transform.localPosition = new Vector3(-0.023f,1.05f,-0.01f);

                    //animation
                    for(int a=1;a<7;a++){
                        PlayerCharacter.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Hat[1];
                        PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[64+a];
                        PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[64+a];
                        PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[64+a];
                        PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[64+a];
                        PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[64+a];
                        PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[64+a];
                        PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[64+a];
                        PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[64+a];
                        PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[64+a];

                        //zeitspann der einzelnen frames
                        if(walkAnim==3){
                            yield return new WaitForSeconds(0.15F);
                        }
                    }

                    //endzenario, wird gebraucht fals der spieler steht
                    PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[64];
                    PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[64];
                    PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[64];
                    PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[64];
                    PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[64];
                    PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[64];
                    PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[64];
                    PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[64];
                    PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[64];
                }
                
            //rechts unten
            }else if(walkAnim==4){

                if(PlayerCharBuilder.player.gender==true){

                    PlayerCharacter.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(-0.64f,5.28f,-0.07f);
                    PlayerCharacter.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.058f,1.617f,-0.03f);
                    PlayerCharacter.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.134f,3.335f,-0.03f);
                    PlayerCharacter.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(-0.137f,3.34f,-0.02f);
                    PlayerCharacter.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(0.258f,2.691f,-0.01f);
                    PlayerCharacter.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(0.3399f,0.3809f,-0.01f);
                    PlayerCharacter.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(0.143f,2.611f,-0.01f);
                    PlayerCharacter.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(-0.062f,1.616f,-0.02f);
                    PlayerCharacter.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(0.143f,2.602f,0f);
                    PlayerCharacter.transform.GetChild(9).gameObject.transform.localPosition = new Vector3(0.24f,0.63f,0f);
            
                    for(int a=1;a<7;a++){
                        PlayerCharacter.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Hat[0];
                        PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[88+a];
                        PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[88+a];
                        PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[88+a];
                        PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[88+a];
                        PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[88+a];
                        PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[88+a];
                        PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[88+a];
                        PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[88+a];
                        PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[88+a];
                        if(walkAnim==4){
                            yield return new WaitForSeconds(0.15F);
                        }
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
                
                //female
                }else{

                    //bestimmt die rendering position
                    PlayerCharacter.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(-0.207f,5.457f,-0.04f);
                    PlayerCharacter.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.267f,1.776f,-0.04f);
                    PlayerCharacter.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.57f,3.342f,-0.03f);
                    PlayerCharacter.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(-0.569f,3.342f,-0.02f);
                    PlayerCharacter.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(-0.248f,2.862f,-0.01f);
                    PlayerCharacter.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(0.059f,0.532f,-0.02f);
                    PlayerCharacter.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.081f,2.064f,-0.01f);
                    PlayerCharacter.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(-0.265f,1.767f,-0.03f);
                    PlayerCharacter.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.083f,2.058f,0f);
                    PlayerCharacter.transform.GetChild(9).gameObject.transform.localPosition = new Vector3(-0.139f,1.041f,-0.01f);

                    //animation
                    for(int a=1;a<7;a++){
                        PlayerCharacter.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Hat[1];
                        PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[88+a];
                        PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[88+a];
                        PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[88+a];
                        PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[88+a];
                        PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[88+a];
                        PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[88+a];
                        PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[88+a];
                        PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[88+a];
                        PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[88+a];

                        //zeitspann der einzelnen frames
                        if(walkAnim==4){
                            yield return new WaitForSeconds(0.15F);
                        }
                    }

                    //endzenario, wird gebraucht fals der spieler steht
                    PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[88];
                    PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[88];
                    PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[88];
                    PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[88];
                    PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[88];
                    PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[88];
                    PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[88];
                    PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[88];
                    PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[88];
                }

            //rechts oben
            }else if(walkAnim==5){

                if(PlayerCharBuilder.player.gender==true){

                    PlayerCharacter.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(-0.48f,5.4f,-0.07f);
                    PlayerCharacter.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.079f,1.747f,-0.06f);
                    PlayerCharacter.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.136f,3.497f,-0.06f);
                    PlayerCharacter.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(-0.136f,3.4659f,-0.05f);
                    PlayerCharacter.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(-0.282f,2.719f,-0.01f);
                    PlayerCharacter.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(0.336f,0.46f,-0.04f);
                    PlayerCharacter.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(0.131f,2.731f,-0.03f);
                    PlayerCharacter.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(-0.073f,1.745f,-0.05f);
                    PlayerCharacter.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(0.131f,2.731f,-0.02f);
                    PlayerCharacter.transform.GetChild(9).gameObject.transform.localPosition = new Vector3(0.234f,0.754f,-0.03f);

                    for(int a=1;a<7;a++){
                        PlayerCharacter.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Hat[0];
                        PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[72+a];
                        PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[72+a];
                        PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[72+a];
                        PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[72+a];
                        PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[72+a];
                        PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[72+a];
                        PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[72+a];
                        PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[72+a];
                        PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[72+a];
                        if(walkAnim==5){
                            yield return new WaitForSeconds(0.15F);
                        }
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
                
                //female
                }else{

                    //bestimmt die rendering position
                    PlayerCharacter.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(-0.31f,5.33f,-0.03f);
                    PlayerCharacter.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.151f,1.782f,-0.02f);
                    PlayerCharacter.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.445f,3.265f,-0.02f);
                    PlayerCharacter.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(-0.445f,3.265f,-0.01f);
                    PlayerCharacter.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(-0.163f,3.074f,-0.01f);
                    PlayerCharacter.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(0.153f,0.565f,-0.01f);
                    PlayerCharacter.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(0.042f,2.065f,-0.01f);
                    PlayerCharacter.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(-0.15f,1.773f,-0.01f);
                    PlayerCharacter.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(0.04f,2.06f,0f);
                    PlayerCharacter.transform.GetChild(9).gameObject.transform.localPosition = new Vector3(-0.023f,1.05f,0f);

                    //animation
                    for(int a=1;a<7;a++){
                        PlayerCharacter.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Hat[1];
                        PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[72+a];
                        PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[72+a];
                        PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[72+a];
                        PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[72+a];
                        PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[72+a];
                        PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[72+a];
                        PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[72+a];
                        PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[72+a];
                        PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[72+a];

                        //zeitspann der einzelnen frames
                        if(walkAnim==5){
                            yield return new WaitForSeconds(0.15F);
                        }
                    }

                    //endzenario, wird gebraucht fals der spieler steht
                    PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[72];
                    PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[72];
                    PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[72];
                    PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[72];
                    PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[72];
                    PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[72];
                    PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[72];
                    PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[72];
                    PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[72];
                }
            }

            //nur zum test
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

        //load gender player
        //male
        if(PlayerCharBuilder.player.gender==true){
            //bestimmt die rendering position
            PlayerCharacter.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.79f,5.38f,-0.02f);
            PlayerCharacter.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0.178f,1.657f,-0.02f);
            PlayerCharacter.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(0.263f,3.437f,-0.02f);
            PlayerCharacter.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(0.263f,3.406f,-0.01f);
            PlayerCharacter.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(-0.153f,2.719f,-0.01f);
            PlayerCharacter.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(-0.22f,0.38f,-0.01f);
            PlayerCharacter.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.023f,2.643f,-0.01f);
            PlayerCharacter.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(0.185f,1.655f,-0.01f);
            PlayerCharacter.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.023f,2.642f,0f);
            PlayerCharacter.transform.GetChild(9).gameObject.transform.localPosition = new Vector3(-0.115f,0.672f,0f);

            PlayerCharacter.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Hat[1];
            PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[80];
            PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[80];
            PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[80];
            PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[80];
            PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[80];
            PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[80];
            PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[80];
            PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[80];
            PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[80];
        
        //female
        }else{
            //bestimmt die rendering position
            PlayerCharacter.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.539f,5.457f,-0.03f);
            PlayerCharacter.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0.1f,1.782f,-0.02f);
            PlayerCharacter.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(0.4230f,3.345f,-0.02f);
            PlayerCharacter.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(0.423f,3.345f,-0.01f);
            PlayerCharacter.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(0.092f,2.804f,-0.01f);
            PlayerCharacter.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(-0.21f,0.56f,-0.01f);
            PlayerCharacter.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.081f,2.064f,-0.01f);
            PlayerCharacter.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(0.102f,1.773f,-0.01f);
            PlayerCharacter.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.083f,2.058f,0f);
            PlayerCharacter.transform.GetChild(9).gameObject.transform.localPosition = new Vector3(-0.023f,1.05f,0f);

            PlayerCharacter.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Hat[1];
            PlayerCharacter.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[80];
            PlayerCharacter.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[80];
            PlayerCharacter.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[80];
            PlayerCharacter.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[80];
            PlayerCharacter.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[80];
            PlayerCharacter.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[80];
            PlayerCharacter.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[80];
            PlayerCharacter.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[80];
            PlayerCharacter.transform.GetChild(9).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[80];
        }
    }

    //läd jedes sprite für die playerAnim (1728 stück)
    public static void LoadSprites(){

        //beinhaltet die geladenen sprites
        object[] spriteList = new object[]{};

        //beinhaltet temporär die geladene sprite liste
        spriteList = Resources.LoadAll("Textures/Player/Hat",typeof(Sprite));
        foreach(object obj in spriteList){
           	Hat.Add((Sprite)obj);
        }

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
