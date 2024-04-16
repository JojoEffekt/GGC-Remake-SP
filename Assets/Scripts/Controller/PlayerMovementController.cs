using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovementController : MonoBehaviour
{
    //Player GO Refezenz
    public static GameObject PlayerCharacter;


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


    //EXPERIMENTAL
    //ist für die laufgeschwindigkeit abhänig von der zeit zuständig
    public float timer;

    //aktuell zu belaufendes FloorObject
    public string objName;

    //aktuelle spieler position WÄHREND des laufens
    static Vector3 curDynPlayerPos;

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

            if(GameObject.Find(objName).gameObject.transform.position.x<PlayerCharacter.transform.position.x){
                if(GameObject.Find(objName).gameObject.transform.position.y<PlayerCharacter.transform.position.y){
                    Debug.Log("links unten");
                }else{
                    Debug.Log("links oben");
                }
            }else{
                if(GameObject.Find(objName).gameObject.transform.position.y<PlayerCharacter.transform.position.y){
                    Debug.Log("rechts unten");
                }else{
                    Debug.Log("rechts oben");
                }
            }
        //spieler kann nichtmehr laufen
        }else if(playerPath.Count==0&&isPlayerInMove==true){

            //beim beenden der aktuellen position wird die letzte position die neue player pos
            curPlayerPos = new int[]{Int32.Parse(objName.Split("-")[0]), Int32.Parse(objName.Split("-")[1])};
        
            //spieler ist am ende angekommen, sorge dafür, das dieses statement erst wieder nach neuer spielerbewegung abgerufen wird
            isPlayerInMove = false;
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
