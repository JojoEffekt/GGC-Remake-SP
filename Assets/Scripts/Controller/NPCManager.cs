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
                //gucke ob npcMovementAnim gestartet wurde
                if(npcList[a].walkAnim==0)
                {
                    npcList[a].walkAnim=1;
                    StartCoroutine(Anim(npcList[a]));
                }

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
    	
        System.Random rndm = new System.Random();
        //randomize list um zufälligere positionen für die npcs zuzuweisen
        for(int a=objectList.Count-1;a>0;a--)
        {
            int k = rndm.Next(a + 1);
            GameObject value = objectList[k];
            objectList[k] = objectList[a];
            objectList[a] = value;
        }
    }

    //steuert die lauf animation für den npc
    public IEnumerator Anim(NPC npc){
        GameObject npcGO = npc.npcGO;
        int walkAnim = npc.walkAnim;
        List<Sprite> Hat = npc.Hat;
        List<Sprite> FaceBoy = npc.FaceBoy;
        List<Sprite> HairBoy = npc.HairBoy;
        List<Sprite> HairOverlayBoy = npc.HairOverlayBoy;
        List<Sprite> LegBoy = npc.LegBoy;
        List<Sprite> LegOverlayBoy = npc.LegOverlayBoy;
        List<Sprite> SkinBoy = npc.SkinBoy;
        List<Sprite> SkinOverlayBoy = npc.SkinOverlayBoy;
        List<Sprite> TshirtBoy = npc.TshirtBoy;
        List<Sprite> TshirtOverlayBoy = npc.TshirtOverlayBoy;
        List<Sprite> FaceGirl = npc.FaceGirl;
        List<Sprite> HairGirl = npc.HairGirl;
        List<Sprite> HairOverlayGirl = npc.HairOverlayGirl;
        List<Sprite> LegGirl = npc.LegGirl;
        List<Sprite> LegOverlayGirl = npc.LegOverlayGirl;
        List<Sprite> SkinGirl = npc.SkinGirl;
        List<Sprite> SkinOverlayGirl = npc.SkinOverlayGirl;
        List<Sprite> TshirtGirl = npc.TshirtGirl;
        List<Sprite> TshirtOverlayGirl = npc.TshirtOverlayGirl;
        bool isBoy = npc.isBoy;

        //solange der npc läuft
        while(walkAnim!=0){
            walkAnim = npc.walkAnim;

            //links unten  (front_left)
            if(walkAnim==2){

                //überprüft das geschlecht und rendert je nach dem male, female (true=male)
                //male
                if(isBoy==true){
                    
                    if(npcGO!=null){
                        //bestimmt die rendering position
                        npcGO.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.178f,1.657f,-0.02f);
                        npcGO.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(0.263f,3.437f,-0.02f);
                        npcGO.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(0.263f,3.406f,-0.01f);
                        npcGO.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.153f,2.719f,-0.01f);
                        npcGO.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.22f,0.38f,-0.01f);
                        npcGO.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(-0.023f,2.643f,-0.01f);
                        npcGO.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(0.185f,1.655f,-0.01f);
                        npcGO.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.023f,2.642f,0f);
                        npcGO.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.115f,0.672f,0f);
                    }

                    //animation
                    for(int a=1;a<7;a++){
                        if(npcGO!=null){
                            npcGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[80+a];
                            npcGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[80+a];
                            npcGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[80+a];
                            npcGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[80+a];
                            npcGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[80+a];
                            npcGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[80+a];
                            npcGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[80+a];
                            npcGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[80+a];
                            npcGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[80+a];

                            //zeitspann der einzelnen frames
                            if(walkAnim==2){
                                yield return new WaitForSeconds(0.15F);
                            }
                        }
                    }

                    if(npcGO!=null){
                        //endzenario, wird gebraucht fals der ^npc steht
                        npcGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[80];
                        npcGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[80];
                        npcGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[80];
                        npcGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[80];
                        npcGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[80];
                        npcGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[80];
                        npcGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[80];
                        npcGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[80];
                        npcGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[80];    
                    }

                //female
                }else{
                    
                    //bestimmt die rendering position
                    if(npcGO!=null){
                        npcGO.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.1f,1.782f,-0.02f);
                        npcGO.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(0.4230f,3.345f,-0.02f);
                        npcGO.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(0.423f,3.345f,-0.01f);
                        npcGO.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(0.092f,2.804f,-0.01f);
                        npcGO.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.21f,0.56f,-0.01f);
                        npcGO.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(-0.081f,2.064f,-0.01f);
                        npcGO.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(0.102f,1.773f,-0.01f);
                        npcGO.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.083f,2.058f,0f);
                        npcGO.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.023f,1.05f,0f);
                    }

                    //animation
                    for(int a=1;a<7;a++){
                        if(npcGO!=null){
                            npcGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[80+a];
                            npcGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[80+a];
                            npcGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[80+a];
                            npcGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[80+a];
                            npcGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[80+a];
                            npcGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[80+a];
                            npcGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[80+a];
                            npcGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[80+a];
                            npcGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[80+a];

                            //zeitspann der einzelnen frames
                            if(walkAnim==2){
                                yield return new WaitForSeconds(0.15F);
                            }
                        }
                    }

                    if(npcGO!=null){
                        //endzenario, wird gebraucht fals der spieler steht
                        npcGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[80];
                        npcGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[80];
                        npcGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[80];
                        npcGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[80];
                        npcGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[80];
                        npcGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[80];
                        npcGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[80];
                        npcGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[80];
                        npcGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[80];
                    }
                }

            //links oben  
            }else if(walkAnim==3){

                if(isBoy==true){

                    if(npcGO!=null){
                        npcGO.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.178f,1.657f,-0.05f);
                        npcGO.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(0.263f,3.437f,-0.07f);
                        npcGO.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(0.263f,3.406f,-0.06f);
                        npcGO.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.153f,2.719f,-0.01f);
                        npcGO.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.22f,0.38f,-0.01f);
                        npcGO.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(-0.023f,2.643f,-0.03f);
                        npcGO.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(0.185f,1.655f,-0.04f);
                        npcGO.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.023f,2.642f,-0.02f);
                        npcGO.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.115f,0.672f,0f);
                    }                        

                    for(int a=1;a<7;a++){
                        if(npcGO!=null){
                            npcGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[64+a];
                            npcGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[64+a];
                            npcGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[64+a];
                            npcGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[64+a];
                            npcGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[64+a];
                            npcGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[64+a];
                            npcGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[64+a];
                            npcGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[64+a];
                            npcGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[64+a];
                            if(walkAnim==3){
                                yield return new WaitForSeconds(0.15F);
                            }
                        }
                    }

                    if(npcGO!=null){
                        npcGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[71];
                        npcGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[71];
                        npcGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[71];
                        npcGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[71];
                        npcGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[71];
                        npcGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[71];
                        npcGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[71];
                        npcGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[71];
                        npcGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[71];
                    }
        
                //female
                }else{
                    
                    if(npcGO!=null){
                        //bestimmt die rendering position
                        npcGO.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.1f,1.782f,-0.04f);
                        npcGO.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(0.4230f,3.345f,-0.03f);
                        npcGO.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(0.423f,3.345f,-0.02f);
                        npcGO.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(0.092f,2.804f,-0.01f);
                        npcGO.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.21f,0.56f,-0.02f);
                        npcGO.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(-0.081f,2.064f,-0.01f);
                        npcGO.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(0.102f,1.773f,-0.03f);
                        npcGO.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.083f,2.058f,0f);
                        npcGO.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.023f,1.05f,-0.01f);
                    }

                    //animation
                    for(int a=1;a<7;a++){
                        if(npcGO!=null){
                            npcGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[64+a];
                            npcGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[64+a];
                            npcGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[64+a];
                            npcGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[64+a];
                            npcGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[64+a];
                            npcGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[64+a];
                            npcGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[64+a];
                            npcGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[64+a];
                            npcGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[64+a];

                            //zeitspann der einzelnen frames
                            if(walkAnim==3){
                                yield return new WaitForSeconds(0.15F);
                            }
                        }
                    }

                    if(npcGO!=null){
                        //endzenario, wird gebraucht fals der spieler steht
                        npcGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[64];
                        npcGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[64];
                        npcGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[64];
                        npcGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[64];
                        npcGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[64];
                        npcGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[64];
                        npcGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[64];
                        npcGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[64];
                        npcGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[64];
                    }
                }
                
            //rechts unten
            }else if(walkAnim==4){

                if(isBoy==true){
                    
                    if(npcGO!=null){
                        npcGO.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(-0.058f,1.617f,-0.03f);
                        npcGO.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(-0.134f,3.314f,-0.03f);
                        npcGO.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(-0.137f,3.34f,-0.02f);
                        npcGO.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(0.258f,2.691f,-0.01f);
                        npcGO.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0.3399f,0.3809f,-0.01f);
                        npcGO.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(0.143f,2.611f,-0.01f);
                        npcGO.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(-0.062f,1.616f,-0.02f);
                        npcGO.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(0.143f,2.602f,0f);
                        npcGO.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(0.24f,0.63f,0f);
                    }
            
                    for(int a=1;a<7;a++){
                        if(npcGO!=null){
                            npcGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[88+a];
                            npcGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[88+a];
                            npcGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[88+a];
                            npcGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[88+a];
                            npcGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[88+a];
                            npcGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[88+a];
                            npcGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[88+a];
                            npcGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[88+a];
                            npcGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[88+a];
                            if(walkAnim==4){
                                yield return new WaitForSeconds(0.15F);
                            }
                        }
                    }
                    if(npcGO!=null){
                        npcGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[88];
                        npcGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[88];
                        npcGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[88];
                        npcGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[88];
                        npcGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[88];
                        npcGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[88];
                        npcGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[88];
                        npcGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[88];
                        npcGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[88];
                    }
                
                //female
                }else{
                    
                    if(npcGO!=null){
                        //bestimmt die rendering position
                        npcGO.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(-0.267f,1.776f,-0.02f);
                        npcGO.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(-0.57f,3.342f,-0.02f);
                        npcGO.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(-0.569f,3.342f,-0.01f);
                        npcGO.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.248f,2.862f,-0.01f);
                        npcGO.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0.059f,0.532f,-0.01f);
                        npcGO.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(-0.081f,2.064f,-0.01f);
                        npcGO.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(-0.265f,1.767f,-0.01f);
                        npcGO.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.083f,2.058f,0f);
                        npcGO.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.139f,1.041f,0f);
                    }

                    //animation
                    for(int a=1;a<7;a++){
                        if(npcGO!=null){
                            npcGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[88+a];
                            npcGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[88+a];
                            npcGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[88+a];
                            npcGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[88+a];
                            npcGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[88+a];
                            npcGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[88+a];
                            npcGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[88+a];
                            npcGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[88+a];
                            npcGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[88+a];

                            //zeitspann der einzelnen frames
                            if(walkAnim==4){
                                yield return new WaitForSeconds(0.15F);
                            }
                        }
                    }
                    if(npcGO!=null){
                        //endzenario, wird gebraucht fals der spieler steht
                        npcGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[88];
                        npcGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[88];
                        npcGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[88];
                        npcGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[88];
                        npcGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[88];
                        npcGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[88];
                        npcGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[88];
                        npcGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[88];
                        npcGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[88];
                    }
                }

            //rechts oben
            }else if(walkAnim==5){

                if(isBoy==true){

                    if(npcGO!=null){
                        npcGO.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(-0.079f,1.747f,-0.05f);
                        npcGO.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(-0.136f,3.497f,-0.07f);
                        npcGO.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(-0.136f,3.4659f,-0.06f);
                        npcGO.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.282f,2.719f,-0.01f);
                        npcGO.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0.336f,0.46f,-0.04f);
                        npcGO.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(0.131f,2.731f,-0.03f);
                        npcGO.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(-0.073f,1.745f,-0.04f);
                        npcGO.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(0.131f,2.731f,-0.02f);
                        npcGO.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(0.234f,0.754f,-0.03f);
                    }

                    for(int a=1;a<7;a++){
                        if(npcGO!=null){
                            npcGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[72+a];
                            npcGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[72+a];
                            npcGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[72+a];
                            npcGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[72+a];
                            npcGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[72+a];
                            npcGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[72+a];
                            npcGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[72+a];
                            npcGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[72+a];
                            npcGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[72+a];
                            if(walkAnim==5){
                                yield return new WaitForSeconds(0.15F);
                            }
                        }
                    }

                    if(npcGO!=null){
                        npcGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[72];
                        npcGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[72];
                        npcGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[72];
                        npcGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[72];
                        npcGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[72];
                        npcGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[72];
                        npcGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[72];
                        npcGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[72];
                        npcGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[72];
                    }
                
                //female
                }else{

                    if(npcGO!=null){
                        //bestimmt die rendering position
                        npcGO.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(-0.151f,1.782f,-0.02f);
                        npcGO.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(-0.445f,3.265f,-0.02f);
                        npcGO.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(-0.445f,3.265f,-0.01f);
                        npcGO.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.163f,3.074f,-0.01f);
                        npcGO.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0.153f,0.565f,-0.01f);
                        npcGO.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(0.042f,2.065f,-0.01f);
                        npcGO.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(-0.15f,1.773f,-0.01f);
                        npcGO.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(0.04f,2.06f,0f);
                        npcGO.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.023f,1.05f,0f);
                    }

                    //animation
                    for(int a=1;a<7;a++){
                        if(npcGO!=null){
                            npcGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[72+a];
                            npcGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[72+a];
                            npcGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[72+a];
                            npcGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[72+a];
                            npcGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[72+a];
                            npcGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[72+a];
                            npcGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[72+a];
                            npcGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[72+a];
                            npcGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[72+a];

                            //zeitspann der einzelnen frames
                            if(walkAnim==5){
                                yield return new WaitForSeconds(0.15F);
                            }
                        }
                    }

                    if(npcGO!=null){
                        //endzenario, wird gebraucht fals der npc steht
                        npcGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[72];
                        npcGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[72];
                        npcGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[72];
                        npcGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[72];
                        npcGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[72];
                        npcGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[72];
                        npcGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[72];
                        npcGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[72];
                        npcGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[72];
                    }
                }
            }

            //nur zum test
            yield return new WaitForSeconds(0.1F);
        }
    }
}
