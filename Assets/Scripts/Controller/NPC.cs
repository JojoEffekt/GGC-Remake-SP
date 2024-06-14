using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{   
    //sagt aus ob npc bedient werden kann oder nicht
    public bool isOnTable = false;

    //sagt aus ob der npc gerade in bewegung ist
    //wird in verbindung mit der "waittime" benutzt
    public bool isOnWalk { get; set; }

    //die zeit die der npc nocht wartet bevor er geht
    //wird beim hinsetzen an einem tisch zurückgesetzt
    //wird während laufen pausiert
    public int waittime { get; set; }

    //position an die der npc laufen soll zum essen
    public int[] eatingPosition { get; set; }

    //variable um jede sekunde ein event zu erzeugen
    private float timeDelay = 0.0f;

    //Constructor der für die initalisiereung verantwortlich ist
    public NPC()
    {
        //setzte die selbstzerstörung bei nicht handlungsfähig auf 30 sekunden
        waittime = 30;

        //npc ist gerade nicht in bewegung
        isOnWalk = false;

        //erstelle den npc
        CreateNPC();

        

        /*
        erstelle neue position
            gucke ob ein stuhl + tisch frei ist
                List<string> path = LabyrinthBuilder.LabyrinthManager(doorPos,posNebenStuhl)
                NPCMovement(path)
                    wenn am ziel, setzte npc auf stuhl und mache für kellner verfügbar
                    warte 30 sekunden ansonsten gehe
                    werde bedient
                    essen animation
                    stehe auf gehe zur tür
                        lösche npc
        */

        //sucht eine position wohin der spieler laufen kann
        //rufe jede sekunde auf wenn kein wert zurückgegeben wird
        //-> NPCManager.getNPCPosition();
    }

    //instantiate den npc anhand des prefabs und fügt einen zufälligen skin ein
    private void CreateNPC()
    {

    }

    //laufe zur neuen position anhand des erstellten path
    //gibt TRUE zurück wenn npc am ziel ankommen ist
    private bool NPCMovement(List<string> path)
    {
        return true;
    }

    //gibt die npc position auf dem grid wieder
    public int[] getPosition()
    {
        return new int[]{0,0};
    }
}

