using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovementController : MonoBehaviour
{
    //Player GO Refezenz
    public static GameObject PlayerCharacter;


    //aktuelle spieler position
    public static int[] curPlayerPos { get; set; }

    //enthält die position zu der sich der spieler bewegen soll
    public static int[] tryNewPos { get; set; }

    //makiert die position wo die tür steht
    public static int[] doorPos { get; set; }


    //enthält den aktuell zu gehenden weg für den spieler
    public static List<string> playerPath = new List<string>();

    //switcher ob der player sich gerade bewegt
    public static bool isPlayerInMove = false;

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
        }
    }

    void Update(){
        if(playerPath.Count!=0&&isPlayerInMove==false){
            isPlayerInMove = true;
            StartCoroutine(MovePlayerToPos());
        }
    }

    //bewegt den spieler zur gewünschten stelle
    IEnumerator MovePlayerToPos(){

        string objName = "";

        //für jede positiuon in der liste
        while(playerPath.Count!=0){

            //die schrittdauer
            yield return new WaitForSeconds(0.5f);
            objName = playerPath[0].Split(":")[0]+"-"+playerPath[0].Split(":")[1];
            Debug.Log("Move To: "+objName+" : "+playerPath.Count);

            //Spieler muss an sein umfeld angepasst werden, (SortingOrder)
            //CONTINUE

            //platziert den spieler auf die neue position
            PlayerCharacter.transform.position  = new Vector2(GameObject.Find(objName).transform.position.x, GameObject.Find(objName).transform.position.y+3.5f);    
            //set sorting Order auf die der anderen floorChild obj (floorx+floory+1=SO)
            for(int a=0;a<PlayerCharacter.transform.childCount;a++){
                PlayerCharacter.transform.GetChild(a).gameObject.GetComponent<SpriteRenderer>().sortingOrder = Int32.Parse(playerPath[0].Split(":")[0])+Int32.Parse(playerPath[0].Split(":")[1])+1;
            }

            //löscht das gegangene element aus der liste
            playerPath.RemoveAt(0);
        }

        //beim beenden der aktuellen position wird die letzte position die neue player pos
        curPlayerPos = new int[]{Int32.Parse(objName.Split("-")[0]), Int32.Parse(objName.Split("-")[1])};
        Debug.Log("Destination: "+curPlayerPos[0]+":"+curPlayerPos[1]);

        //spieler steht still, coroutine freigegeben
        isPlayerInMove = false;
    }

    //sucht den spieler und platziert ihn bei laden des spiels an der tür
    public static void LoadPlayer(){

        //sucht die spieler UI
        PlayerCharacter = GameObject.Find("Player");

        //platziert den spieler an der tür
        PlayerCharacter.transform.position = new Vector2(GameObject.Find(FindDoorPos()[0]+"-"+FindDoorPos()[1]).transform.position.x, GameObject.Find(FindDoorPos()[0]+"-"+FindDoorPos()[1]).transform.position.y+3.5f);

        //rendert den spieler auf die richtige ebene
        for(int a=0;a<PlayerCharacter.transform.childCount;a++){
            PlayerCharacter.transform.GetChild(a).gameObject.GetComponent<SpriteRenderer>().sortingOrder = FindDoorPos()[0]+FindDoorPos()[1]+1;
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
