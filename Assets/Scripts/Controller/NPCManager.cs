using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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

                //speichert den neuen npc in list
                npcList.Add(npc);
            }



            //finde eine aktuell mögliche position an die sich alle wartenden npcs hinsetzten könnten
            SearchForNPCSitPosition();

            //prüfe auf zerstörbare npcs
            CheckForDestroyableNPCs();
        }
    }
    
    //sucht im gesamten spielfeld nach freie plätze für alle npc wo der npc sich an einem tisch setzten kann
    private void SearchForNPCSitPosition()
    {   
        //für jeden npc in list
        for(int a=0;a<npcList.Count;a++)
        {   
            //hat npc keine position zum essen, dann suche eine
            if(npcList[a].eatingPosition==null)
            {
               // Debug.Log("suche platz für: "+a);
                getPositionNextToChair();
            }
        }
    }

    //prüft alle npcs in npcList
    //guckt ob der cooldown der npcs abgelaufen ist oder verringere ihn
    private void CheckForDestroyableNPCs()
    {
        //für jeden npc aus der npcList von hinten
        for(int a=npcList.Count-1;a>=0;a--)
        {   	
            //ist cooldown abgelaufen
            if(npcList[a].waittime<=0)
            {   
                //cooldown ist abgelaufen, zerstöre npc
                npcList.Remove(npcList[a]);
            }
            //cooldown verringern wenn npc auf der stelle steht/sitzt
            else if(!npcList[a].isOnWalk)
            {
                //verringere die waittime
                npcList[a].waittime = npcList[a].waittime - 1;
            }
        }
    }



    //suche eine mögliche sitzposition für ein npc
    //gibt koordinate neben stuhl wieder, wenn angekommen
    //setzte npc auf stuhl etc.
    public int[] getPositionNextToChair()
    {
        //enthält alle objecte auf dem spielfeld
        List<GameObject> objectList = new List<GameObject>();

        //suche alle objecte in auf dem spielfeld
        for(int a=0;a<PlayerController.gridSize-1;a++)
        {
            for(int b=0;b<PlayerController.gridSize-1;b++)
            {   
                //suche alle existierende childs
                if(GameObject.Find(a+"-"+b+"-Child"))
                {   
                    //füge sie der liste hinzu
                    objectList.Add(GameObject.Find(a+"-"+b+"-Child"));
                }
            }
        }

        //für jedes object in liste
        for(int a=0;a<objectList.Count;a++)
        {
            //gucke ob das object ein tisch ist
            if(objectList[a].GetComponent<SpriteRenderer>().sprite.name.Split("_")[0].Equals("Table"))
            {
                //alle nebenstehende positionen bilden
                int x = Int32.Parse(objectList[a].name.Split("-")[0]);
                int y = Int32.Parse(objectList[a].name.Split("-")[1]);
                string objectNameRechtsOben = x+"-"+(y-1)+"-Child";  //rechts oben   sprite: d
                string objectNameRechtsUnten = (x+1)+"-"+y+"-Child"; //rechts unten  sprite: c
                string objectNameLinksOben = (x-1)+"-"+y+"-Child";   //links oben    sprite: a
                string objectNameLinksUnten = x+"-"+(y+1)+"-Child";  //links unten   sprite: b

                //gucke ob ein umliegendes gameobject um den tisch ein stuhl mit  richter blickrichtung ist
                for(int b=0;b<objectList.Count;b++)
                {
                    //gucke ob nebenstehendes object existiert
                    if(objectList[b].name.Equals(objectNameRechtsOben))
                    {
                        //gucke ob object chair ist und richtige  blickrichtung hat
                        if(objectList[b].GetComponent<SpriteRenderer>().sprite.name.Split("_")[0].Equals("Chair")&&objectList[b].GetComponent<SpriteRenderer>().sprite.name.Split("_").Last().Equals("d"))
                        {
                            //prüfe ob neben dem stuhl eine position zum laufen frei ist
                            if(SearchForPositionNearTheObject(objectList[b].name))
                            {
                                Debug.Log("passende sitzposition gefunden: "+objectList[a].name+":"+objectList[b].name);
                            }
                        }
                    }
                    if(objectList[b].name.Equals(objectNameRechtsUnten))
                    {
                        if(objectList[b].GetComponent<SpriteRenderer>().sprite.name.Split("_")[0].Equals("Chair")&&objectList[b].GetComponent<SpriteRenderer>().sprite.name.Split("_").Last().Equals("c"))
                        {
                            //prüfe ob neben dem stuhl eine position zum laufen frei ist
                            if(SearchForPositionNearTheObject(objectList[b].name))
                            {
                                Debug.Log("passende sitzposition gefunden: "+objectList[a].name+":"+objectList[b].name);
                            }
                        }
                    }
                    if(objectList[b].name.Equals(objectNameLinksOben))
                    {
                        if(objectList[b].GetComponent<SpriteRenderer>().sprite.name.Split("_")[0].Equals("Chair")&&objectList[b].GetComponent<SpriteRenderer>().sprite.name.Split("_").Last().Equals("a"))
                        {
                            //prüfe ob neben dem stuhl eine position zum laufen frei ist
                            if(SearchForPositionNearTheObject(objectList[b].name))
                            {
                                Debug.Log("passende sitzposition gefunden: "+objectList[a].name+":"+objectList[b].name);
                            }
                        }
                    }
                    if(objectList[b].name.Equals(objectNameLinksUnten))
                    {
                        if(objectList[b].GetComponent<SpriteRenderer>().sprite.name.Split("_")[0].Equals("Chair")&&objectList[b].GetComponent<SpriteRenderer>().sprite.name.Split("_").Last().Equals("b"))
                        {
                            //prüfe ob neben dem stuhl eine position zum laufen frei ist
                            if(SearchForPositionNearTheObject(objectList[b].name))
                            {
                                Debug.Log("passende sitzposition gefunden: "+objectList[a].name+":"+objectList[b].name);
                            }
                        }
                    }
                }
            }
        }
        return null;
    }

    //sucht nach einer nebenstehenden positionen des chairs
    //gibt false wieder wenn nichts gefunden
    public bool SearchForPositionNearTheObject(string ObjectToMoveOn)
    {
        //sucht nach einer freien position oben, rechts, unten, links vom oven
        int x = Int32.Parse(ObjectToMoveOn.Split("-")[0]);
        int y = Int32.Parse(ObjectToMoveOn.Split("-")[1]);
        string oben = ""+(x-1)+"-"+y;
        string rechts = ""+x+"-"+(y-1);
        string unten = ""+(x+1)+"-"+y;
        string links = ""+x+"-"+(y+1);
        string[] suroundingPositions = new string[]{oben, rechts, unten, links};

        //gucke ob umliegende floors existieren
        foreach(string nearbyObj in suroundingPositions)
        {
            //FloorObj existiert (größe des grids beachten)
            if(GameObject.Find(nearbyObj))
            {
                //Floorobject existiert nicht
                if(!GameObject.Find(nearbyObj+"-Child"))
                {
                    //gucke ob npc zur position laufen kann
                    int[] doorPos = PlayerMovementController.FindDoorPos();
                    List<string> playerPath = LabyrinthBuilder.LabyrinthManager(doorPos, new int[]{Int32.Parse(nearbyObj.Split("-")[0]),Int32.Parse(nearbyObj.Split("-")[1])});
                    if(playerPath.Count!=0)
                    {
                        //npc kann zum chair gehen
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
