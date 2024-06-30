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

    //enthält alle chairs und tables auf dem spielfeld
    List<GameObject> objectList = new List<GameObject>();

    //beinhaltet das prefab der npcs
    private GameObject prefab;

    //enthält das gameobject mit folgenden script
    public GameObject ButtonController;


    public int tempCounter = 1;

    void Start()
    {
        //läd das prefab für den npc
        prefab = Resources.Load("Prefabs/NPCPrefab") as GameObject;

        //lösche alle npc details falls beim letzten schließen noch was übrig geblieben ist
        DeleteAllNPCS();
    }
    
    private void Update()
    {
        //führe folgenden code jede sekunde aus
        //prüfe ob rebuildShop geschlossen ist
        timeDelay = timeDelay + Time.deltaTime;
        if(timeDelay>=1.0f&&!ButtonController.GetComponent<ButtonController>().isRebuildShopOpen)
        {
            timeDelay = 0.0f;

            //erstelle aus der beliebtheit des cafes eine chance auf die generierung eines neuen npcs
            //wahrscheinlichkeit auf generierung erhöht sich bei höherer beliebtheit
            System.Random rndm = new System.Random();
            int rndmNum = rndm.Next(0,150);
            if(tempCafeFavNumber>=rndmNum)
            {
                //generiere einen neuen npc
                NPC npc = new NPC(prefab, tempCounter, PlayerMovementController.FindDoorPos());
                tempCounter = tempCounter+1;

                //speichert den neuen npc in list
                npcList.Add(npc);
            }



            //finde eine aktuell mögliche position an die sich alle wartenden npcs hinsetzten könnten
            SearchForNPCSitPosition();
            //Debug.Log("existierende npcs: "+npcList.Count);

            //prüfe auf zerstörbare npcs
            CheckForDestroyableNPCs();
        }

        //für jeden npc muss geprüft werden ob er läuft, wenn ja sorge für flüssige animation
        for(int a=0;a<npcList.Count;a++)
        {
            //npc kann laufen
            if(npcList[a].isOnWalk)
            {
                npcList[a].UpdateAnim();
            }
        }
    }
    
    //sucht im gesamten spielfeld nach freie plätze für alle npc wo der npc sich an einem tisch setzten kann
    private void SearchForNPCSitPosition()
    {
        //für jeden npc in list
        for(int a=0;a<npcList.Count;a++)
        {   
            //hat npc keine position zum essen, dann suche eine
            if(npcList[a].endPos==null)
            {
                //weg gefunden, daten übergeben, starte movement
                //change FCED für chair und table
                if(getPositionNextToChair(npcList[a]))
                {
                    //verändere das FCED für table/chair auf besetzt
                    FloorChildExtraDataController.ChangeFCEDData("Table;"+(npcList[a].tablePos[0]+"-"+npcList[a].tablePos[1])+";False");
                    FloorChildExtraDataController.ChangeFCEDData("Chair;"+(npcList[a].chairPos[0]+"-"+npcList[a].chairPos[1])+";False");

                    //npc läuft los, ture=weg von tür zu path wird erstellt
                    npcList[a].NPCMovement(true);

                    //CONTINUE
                    /*
                    verändere bool wenn spieler sitzt
                    
                    */

                    //KEINE AHNUNG OB NÖTIG
                    //nach jeder action muss neu gespeichert werden
                    SaveAndLoadController.SavePlayerData();
                }
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
            //cooldown verringern wenn npc auf der stelle steht/sitzt
            if(!npcList[a].isOnWalk)
            {
                //verringere die waittime
                npcList[a].waittime = npcList[a].waittime - 1;
            }

            //ist cooldown abgelaufen
            if(npcList[a].waittime<=0)
            {   
                //gucke ob der npc nocht an der tür steht
                if(npcList[a].state==0)
                {
                    //lösche den npc vom NPCHandler
                    npcList[a].DeleteNPC();

                    //zerstöre npc
                    npcList.RemoveAt(a);
                }
                //gucke ob der npc schon auf einem stuhl saß,
                //wenn ja, erst npc löschen wenn npc an der tür angekommen ist
                else if(npcList[a].state==1)
                {
                    //lösche FCED für chair und table
                    DeleteFCEDForNPC(npcList[a]);

                    //lasse npc erst zurück laufen und dann zerstören
                    npcList[a].NPCMovement(false);

                    //erhöhe um 1 damit diese funktion nicht nochmal aufgerufen wird
                    npcList[a].state = npcList[a].state + 1;
                }
                //npc ist wieder an der tür, soll gelöscht werden
                else if(npcList[a].state==3)
                {
                    //lösche den npc vom NPCHandler
                    npcList[a].DeleteNPC();
    
                    //zerstöre npc
                    npcList.RemoveAt(a);
                }
            }
        }
    }



    //suche eine mögliche sitzposition für ein npc
    //gibt koordinate neben stuhl wieder, wenn angekommen
    //setzte npc auf stuhl etc.
    public bool getPositionNextToChair(NPC npc)
    {
        //für jedes object in liste
        for(int a=0;a<objectList.Count;a++)
        {
            //gucke ob das object ein tisch ist
            if(objectList[a].GetComponent<SpriteRenderer>().sprite.name.Split("_")[0].Equals("Table"))
            {
                //prüfe ob dieser table im FCED nicht von einem npc besetzt ist
                if(Convert.ToBoolean(FloorChildExtraDataController.getTable(objectList[a].name.Split("-")[0]+"-"+objectList[a].name.Split("-")[1]).getData().Split(";")[2]))
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
                                //prüfe ob dieser chair im FCED nicht von einem npc besetzt ist 
                                if(Convert.ToBoolean(FloorChildExtraDataController.getChair(objectList[b].name.Split("-")[0]+"-"+objectList[b].name.Split("-")[1]).getData().Split(";")[2]))
                                {
                                    //prüfe ob neben dem stuhl eine position zum laufen frei ist
                                    if(SearchForPositionNearTheObject(objectList[b].name, npc))
                                    {
                                        //weg gefunden, übergebe die chair position dem npc
                                        npc.chairPos = new int[]{x,(y-1)};
                                        npc.tablePos = new int[]{Int32.Parse(objectList[a].name.Split("-")[0]), Int32.Parse(objectList[a].name.Split("-")[1])};
                                        //Debug.Log("passende sitzposition gefunden: "+objectList[a].name+":"+objectList[b].name);
                                        return true;
                                    }
                                }
                            }
                        }
                        if(objectList[b].name.Equals(objectNameRechtsUnten))
                        {
                            if(objectList[b].GetComponent<SpriteRenderer>().sprite.name.Split("_")[0].Equals("Chair")&&objectList[b].GetComponent<SpriteRenderer>().sprite.name.Split("_").Last().Equals("c"))
                            {
                                if(Convert.ToBoolean(FloorChildExtraDataController.getChair(objectList[b].name.Split("-")[0]+"-"+objectList[b].name.Split("-")[1]).getData().Split(";")[2]))
                                {
                                    //prüfe ob neben dem stuhl eine position zum laufen frei ist
                                    if(SearchForPositionNearTheObject(objectList[b].name, npc))
                                    {
                                        npc.chairPos = new int[]{(x+1),y};
                                        npc.tablePos = new int[]{Int32.Parse(objectList[a].name.Split("-")[0]), Int32.Parse(objectList[a].name.Split("-")[1])};
                                        //Debug.Log("passende sitzposition gefunden: "+objectList[a].name+":"+objectList[b].name);
                                        return true;
                                    }
                                }
                            }
                        }
                        if(objectList[b].name.Equals(objectNameLinksOben))
                        {
                            if(objectList[b].GetComponent<SpriteRenderer>().sprite.name.Split("_")[0].Equals("Chair")&&objectList[b].GetComponent<SpriteRenderer>().sprite.name.Split("_").Last().Equals("a"))
                            {
                                if(Convert.ToBoolean(FloorChildExtraDataController.getChair(objectList[b].name.Split("-")[0]+"-"+objectList[b].name.Split("-")[1]).getData().Split(";")[2]))
                                {
                                    //prüfe ob neben dem stuhl eine position zum laufen frei ist
                                    if(SearchForPositionNearTheObject(objectList[b].name, npc))
                                    {
                                        npc.chairPos = new int[]{(x-1),y};
                                        npc.tablePos = new int[]{Int32.Parse(objectList[a].name.Split("-")[0]), Int32.Parse(objectList[a].name.Split("-")[1])};
                                        //Debug.Log("passende sitzposition gefunden: "+objectList[a].name+":"+objectList[b].name);
                                        return true;
                                    }
                                }
                            }
                        }
                        if(objectList[b].name.Equals(objectNameLinksUnten))
                        {
                            if(objectList[b].GetComponent<SpriteRenderer>().sprite.name.Split("_")[0].Equals("Chair")&&objectList[b].GetComponent<SpriteRenderer>().sprite.name.Split("_").Last().Equals("b"))
                            {
                                if(Convert.ToBoolean(FloorChildExtraDataController.getChair(objectList[b].name.Split("-")[0]+"-"+objectList[b].name.Split("-")[1]).getData().Split(";")[2]))
                                {
                                    //prüfe ob neben dem stuhl eine position zum laufen frei ist
                                    if(SearchForPositionNearTheObject(objectList[b].name, npc))
                                    {
                                        npc.chairPos = new int[]{x,(y+1)};
                                        npc.tablePos = new int[]{Int32.Parse(objectList[a].name.Split("-")[0]), Int32.Parse(objectList[a].name.Split("-")[1])};
                                        //Debug.Log("passende sitzposition gefunden: "+objectList[a].name+":"+objectList[b].name);
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    //lösche die FCED für table und chair auf den ein npc saß
    public void DeleteFCEDForNPC(NPC npc)
    {   
        FloorChildExtraDataController.ChangeFCEDData("Table;"+(npc.tablePos[0]+"-"+npc.tablePos[1])+";True");
        FloorChildExtraDataController.ChangeFCEDData("Chair;"+(npc.chairPos[0]+"-"+npc.chairPos[1])+";True");
    }

    //lösche alle NPCs aus der scene
    //FCED und Liste reseten
    public void DeleteAllNPCS()
    {
        //lösche alle npcs
        GameObject NPCHandler = GameObject.Find("NPCHandler");
        for(int a=NPCHandler.transform.childCount-1;a>=0;a--){
            Destroy(NPCHandler.transform.GetChild(a).gameObject);
        }

        //setzt alle tables und chairs auf nicht besetzt
        //die FCED wird zurückgesetzt sodas die objekte wieder von npc benutzbar sind
        foreach(var item in objectList)
        {
            //gucke ob das object table oder chair ist
            if(item.GetComponent<SpriteRenderer>().sprite.name.Split("_")[0].Equals("Table"))
            {
                FloorChildExtraDataController.ChangeFCEDData("Table;"+(item.name.Split("-")[0]+"-"+item.name.Split("-")[1])+";True");
            }
            else
            {
                FloorChildExtraDataController.ChangeFCEDData("Chair;"+(item.name.Split("-")[0]+"-"+item.name.Split("-")[1])+";True");
            }
        }

        //cleare die npc liste 
        npcList.Clear();

        //KEINE AHNUNG OB NÖTIG
        //nach jeder action muss neu gespeichert werden
        SaveAndLoadController.SavePlayerData();
    }

    //sucht nach einer nebenstehenden positionen des chairs
    //gibt false wieder wenn nichts gefunden
    public bool SearchForPositionNearTheObject(string ObjectToMoveOn, NPC npc)
    {
        //sucht nach einer freien position oben, rechts, unten, links vom oven
        int x = Int32.Parse(ObjectToMoveOn.Split("-")[0]);
        int y = Int32.Parse(ObjectToMoveOn.Split("-")[1]);
        string oben = ""+(x-1)+"-"+y; //oben links
        string rechts = ""+x+"-"+(y-1); //oben rechts
        string unten = ""+(x+1)+"-"+y; //unten rechts
        string links = ""+x+"-"+(y+1); //unten links
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
                    //gucke ob npc zur position laufen kann (übergibt die endPos)
                    List<string> npcPath = LabyrinthBuilder.getNPCPath(new int[]{Int32.Parse(nearbyObj.Split("-")[0]),Int32.Parse(nearbyObj.Split("-")[1])});
                    //gucke ob route gefunden wurde
                    if(npcPath.Count!=0)
                    {   
                        //weg gefunden, übergebe datenm zum laufen
                        npc.startPos = new int[]{Int32.Parse(npcPath[0].Split(":")[0]),Int32.Parse(npcPath[0].Split(":")[1])};
                        npc.endPos = new int[]{Int32.Parse(npcPath[npcPath.Count-1].Split(":")[0]),Int32.Parse(npcPath[npcPath.Count-1].Split(":")[1])};
                        npc.npcPath = npcPath;

                        //npc kann zum chair gehen
                        return true;
                    }
                }
            }
        }
        return false;
    }

    //aktuallisiert die list der chair und table objekte in dem cafe
    //wird beim starten und verändern des cafes aufgerufen 
    public void CollectAllChairsAndTablesInList()
    {
        //lösche erstmal alle vorhandenen items aus der liste beim aktuallisieren
        objectList.Clear();

        //suche alle objecte die chair oder table sind, auf dem spielfeld
        for(int a=0;a<PlayerController.gridSize-1;a++)
        {
            for(int b=0;b<PlayerController.gridSize-1;b++)
            {   
                //suche alle existierende childs
                GameObject obj = GameObject.Find(a+"-"+b+"-Child");
                if(obj!=null)
                {   
                    //füge sie der liste hinzu wenn es chair oder tabel ist
                    if(obj.GetComponent<SpriteRenderer>().sprite.name.Split("_")[0].Equals("Chair"))
                    {
                        objectList.Add(obj);
                    }
                    else if(obj.GetComponent<SpriteRenderer>().sprite.name.Split("_")[0].Equals("Table"))
                    {
                        objectList.Add(obj);
                    }
                }
            }
        }
    }
}
