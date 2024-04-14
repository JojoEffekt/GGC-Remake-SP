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



    public static List<string> ppath = new List<string>();
    public bool test = true;

    //switcher ob der player sich gerade bewegt
    public static bool isPlayerInMove = false;

    //wird aufgerufen wenn eine neue position angeklickt wurde
    public static void MovePlayer(int[] newPos){

        //wenn der spieler nicht in bewegung ist
        if(isPlayerInMove==false){

            //spieler wird bewegt, kann nicht neue position bekommen bis aktueller path walk abgeschlossen ist
            isPlayerInMove = true;

            //wenn keine playerPos vorhanden ist, gehe davon aus das der spieler an der tür steht
            if(curPlayerPos==null){
                //aktuelle eingangs position für bsp npc generierung
                doorPos = FindDoorPos();
                curPlayerPos = doorPos;
            }
    
            //erhält die zu laufende path liste anhand der start und end pos
            List<string> path = LabyrinthBuilder.LabyrinthManager(curPlayerPos, newPos);
            ppath = path;

            /*if(path.Count!=0){
                foreach(string item in path){
                    //StartCoroutine(Fade(item));
                    CoroutinePawn.Instance.StartCoroutine(Fade(item));
                }
            }else{
                Debug.Log("error!");
            }*/
        }
    }

    void Update(){
        if(ppath.Count!=0&&test==true){
            test=false;
            StartCoroutine(Fade(ppath));
        }
    }

    IEnumerator Fade(List<string> pos){
        while(pos.Count!=0){
            yield return new WaitForSeconds(1);
            string objName = pos[0].Split(":")[0]+"-"+pos[0].Split(":")[1];
            Debug.Log("Move To: "+objName);
            PlayerCharacter.transform.position  = new Vector2(GameObject.Find(objName).transform.position.x, GameObject.Find(objName).transform.position.y+3.5f);    
            pos.RemoveAt(0);
        }

        Debug.Log("Destination:");
    }

    //sucht den spieler und platziert ihn bei laden des spiels an der tür
    public static void LoadPlayer(){

        //sucht die spieler UI
        PlayerCharacter = GameObject.Find("Player");

        //platziert den spieler an der tür
        PlayerCharacter.transform.position = new Vector2(GameObject.Find(FindDoorPos()[0]+"-"+FindDoorPos()[1]).transform.position.x, GameObject.Find(FindDoorPos()[0]+"-"+FindDoorPos()[1]).transform.position.y+3.5f);
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
