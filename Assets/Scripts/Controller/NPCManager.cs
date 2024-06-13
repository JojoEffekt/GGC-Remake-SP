using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// managed npcs die im cafe bedient werden
public class NPCManager : MonoBehaviour
{   
    //TEMP speicher die beliebtheit des cafes MUSS AUSGELAGRRT werdden in player wscript
    public int tempCafeFavNumber = 50;

    //variable um jede sekunde ein event zu erzeugen
    private float timeDelay = 0.0f;
    

    //liste die alle aktiven npcs beinhaltet
    public List<NPC> npcList = new List<NPC>();

    /*
    liste die die aktuellen npcs beinhaltet

    list die die aktuell bedienbaren npc beinhaltet sowie die zeit wie lange sie noch bedienbar sind

    berechne das grid (get)

    berechne alle möglichen sitzpositionen (get)

    erzeugt neuen npc
    npc sucht nach tisch mit stuhl (leer) -> npc geht dahin
        warte 30 sec -> gehe aus cafe
    */
    
    private void Update()
    {
        //führe folgenden code jede sekunde aus
        timeDelay = timeDelay + Time.deltaTime;
        if(timeDelay>=1.0f)
        {
            timeDelay = 0.0f;

            //erstelle aus der beliebtheit des cafes eine chance auf die generierung eines neuen npcs
            //wahrscheinlichkeit auf generierung erhöht sich bei höherer beliebtheit
            System.Random rndm = new System.Random();
            int rndmNum = rndm.Next(0,150);
            if(tempCafeFavNumber>=rndmNum)
            {
                //generiere einen neuen npc
                NPC npc = new NPC();

                //speichert alle neuen npcs
                npcList.Add(npc);
            }



            //finde eine aktuell mögliche position an die sich ein npc hinsetzten könnte
            getNPCPosition();

            //prüfe auf zerstörbare npcs
            CheckForDestroyableNPCs();
        }
    }
    
    //sucht im gesamten spielfeld nach einem freien platz für ein npc wo der spieler sich
    //an einem tisch setzten kann, gibt eine position neben einen stuhl wieder
    private int[] getNPCPosition(){
        return new int[]{0,0};
    }

    //prüft alle npcs in npcList
    //guckt ob der cooldown der npcs abgelaufen ist oder verringere ihn
    private void CheckForDestroyableNPCs()
    {
        for(int a=npcList.Count-1;a>=0;a--)
        {   	
            //cooldown abgelaufen
            if(npcList[a].waittime<=0)
            {   
                //cooldown ist abgelaufen, zerstöre npc
                npcList.Remove(npcList[a]);
            }
            //cooldown verringern wenn npc auf der stelle steht/sitzt
            else if(!npcList[a].isOnWalk)
            {
                npcList[a].waittime = npcList[a].waittime - 1;
            }
        }
    }
}
