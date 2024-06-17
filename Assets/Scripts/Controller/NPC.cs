using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{   
    //beinhaltet den npc
    private GameObject npcGO;

    

    //sagt aus ob npc bedient werden kann oder nicht
    public bool isOnTable = false;

    //sagt aus ob der npc gerade in bewegung ist
    //wird in verbindung mit der "waittime" benutzt
    public bool isOnWalk { get; set; }



    //die zeit die der npc nocht wartet bevor er geht
    //wird beim hinsetzen an einem tisch zurückgesetzt
    //wird während laufen pausiert
    public int waittime { get; set; }



    //position von der der npc loslaufen und hinlaufen soll
    public int[] startPos { get; set; }
    public int[] endPos { get; set; }

    //der GO name des stuhls/table zum essen
    public int[] chairPos { get; set; }
    public int[] tablePos { get; set; }

    //beinhaltet den path zum zum stuhl
    public List<string> npcPath { get; set; }



    //Constructor der für die initalisiereung verantwortlich ist
    public NPC(GameObject prefab)
    {
        //setzte die selbstzerstörung bei nicht handlungsfähig auf 30 sekunden
        waittime = 30;

        //npc ist gerade nicht in bewegung
        isOnWalk = false;

        //erstelle den npc
        CreateNPC(prefab);
    }

    //instantiate den npc anhand des prefabs und fügt einen zufälligen skin ein
    private void CreateNPC(GameObject prefab)
    {
        npcGO = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    //laufe zur neuen position anhand des erstellten path
    //gibt TRUE zurück wenn npc am ziel ankommen ist
    public bool NPCMovement()
    {
        Debug.Log("npc laufe los");
        Debug.Log("laufen?: "+isOnWalk);
        Debug.Log("start position: "+startPos[0]+":"+startPos[1]);
        Debug.Log("end position: "+endPos[0]+":"+endPos[1]);
        Debug.Log("chair position: "+chairPos[0]+":"+chairPos[1]);
        Debug.Log("table position: "+tablePos[0]+":"+tablePos[1]);
        /*for(int a=0;a<npcPath.Count;a++){
            Debug.Log(npcPath[a]);
        }*/
        return true;

        //CONTINUE
        //;;;;
    }
}
