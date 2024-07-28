using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaiterManager : MonoBehaviour
{
    //enthält das buttoncontroller script
    public ButtonController ButtonController;

    //beinhaltet das waiter prefab
    private GameObject prefab;

    //beinhaltet alle waiter
    public List<Waiter> waiterList = new List<Waiter>();

    //wird benutzt um sekündliche abfragen zu starten
    public float timeDelay;

    //liste mit allen countern und deren inhalt, ird sekündlich aktualisiert
    public List<string> counterDataList = new List<string>();
    public List<string> counterEmptyDataList = new List<string>();
    
    //lasse jeden waiter rein laden und eine function aufrufen
    //bei die der waiter initialisiert wird

    void Start()
    {
        ButtonController = GameObject.Find("ButtonController").GetComponent<ButtonController>();
    }

    void Update()
    {
        timeDelay = timeDelay + Time.deltaTime;
        if(timeDelay>=1.0f&&!ButtonController.isRebuildShopOpen)
        {
            timeDelay = 0.0f;

            //suche jede sekunde nach allen tresen + inhalt dieser
            List<string> cdList = FloorChildExtraDataController.getFCEDFromAllCounter();
            counterDataList.Clear();
            counterEmptyDataList.Clear();
            
            //sort counter list nach essen (essen pos 0 in array)
            for(int a=0;a<cdList.Count;a++)
            {
                string[] splitData = cdList[a].Split(";");

                //tresen hat essen
                if(splitData[2].Equals("False"))
                {
                    counterDataList.Add(cdList[a]);
                }
                //tresen hat kein essen
                else
                {
                    counterEmptyDataList.Add(cdList[a]);
                }
            }



            //für jeden waiter, gucke nach seiner aktuellen aufgabe
            for(int a=0;a<waiterList.Count;a++)
            {
                //will zum tresen hat aber noch keinen path? (objective==1)
                if(waiterList[a].objective==1)
                {
                    //suche einen weg zum tresen
                    if(CounterForWaiter(waiterList[a]))
                    {
                        //weg wurde gefunden und übergeben, starte die laufanimation
                        waiterList[a].NPCMovement();

                        //von 1 -> 2, hat path zum thresen bekommen und geht jetzt dahin
                        waiterList[a].objective = 2;
                    }
                }
                //3==waiter beim tresen, suche aus prozentualer chance die nächste aktion aus
                else if(waiterList[a].objective==3)
                {
                    System.Random rndm = new System.Random();
                    int nextTask = rndm.Next(0,100);

                    //waiter probiert die task serve auszuführen
                    if(nextTask>=waiterList[a].ToDish)
                    {
                        //CONTINUE!!!!
                        /*
                        prüfe ob essen auf tresen steht, prüfe FCED von tresen (letztes element)
                        kriege essen raus anhand was auf tresen steht

                        Anonsten andeere aufgabe
                        */

                        //waiter holt sich die endposition eines npc der sitzt (endposition ist hierbei die position neben dem stuhl)
                        //dieser npc hat kein essen
                        int[] endPos = GetDeliveryPosition();

                        //endposition hat einen wert (kann also ausgeführt werden)
                        if((endPos[0]+endPos[1])!=-1)
                        {
                            Debug.Log($"[SERVE] {waiterList[a].Name} route von: {waiterList[a].curPos[0]}:{waiterList[a].curPos[1]} bis {endPos[0]}:{endPos[1]}");

                            //suche nach einem path für den waiter zum laufen bis zum npc
                            List<string> waiterPath = LabyrinthBuilder.getWaiterPath(waiterList[a].curPos, endPos);

                            //gucke ob route gefunden wurde
                            //starte das laufend des npcs
                            if(waiterPath.Count!=0)
                            {   
                                //übergebe den zu speichernden path den waiter
                                waiterList[a].path = waiterPath;

                                //weg wurde gefunden und übergeben, starte die laufanimation
                                waiterList[a].NPCMovement();

                                //von 3 -> 4, hat path zum thresen bekommen und geht jetzt dahin (4=gericht servieren)
                                waiterList[a].objective = 4;

                                //CONITNUE
                                /*
                                rechne essen ab
                                render essen beim waiter
                                stoppe countdown zeit vom npc
                                */
                            }
                            else
                            {
                                //Debug.Log($"kein path");
                                //andere aufgabe machen
                            }
                        }
                        else
                        {
                            //andere aufgabe machen
                            //Debug.Log($"[SERVE] {waiterList[a].Name} kein weg gefunden! [{endPos[0]}:{endPos[1]}]");
                        }
                        


                        /*guck ob waiter essen vom tresen nehmen kann
                        suche an dem der waiter gerade stehen muss
                        ja -> suche nach einem erreichbaren player, nimm essen, rechne essen ab
                              gehe zum player, stoppe player waittime, lege essen auf tisch, ping player, gehe zurück zum tresen(am besten mit essen)
                        nein -> taske dish task
                        */
                    }
                    else
                    {
                        //serve dish
//                        Debug.Log(waiterList[a].Name+"task dish");
                        //suche nach tisch mit essen drauf und ohne player daneben, gehe zum tish, nimm essen, change tischfced, mach tishc leer
                        //gehe mit essen zum counter, reset 
                    }
                }
            }
        }


        //für jeden waiter muss geprüft werden ob er läuft, wenn ja, sorge für flüssige animation
        for(int a=0;a<waiterList.Count;a++)
        {
            //waiter kann laufen
            if(waiterList[a].isOnWalk)
            {
                //gucke ob waiterMovementAnim gestartet wurde
                if(waiterList[a].walkAnim==0)
                {
                    waiterList[a].walkAnim = 1;

                    Debug.Log("rtratat!");
                    //für die animation während der bewegung
                    StartCoroutine(Anim(waiterList[a],0));
                }

                //für die bewegung über das spielfelds
                waiterList[a].UpdateAnim();
            }
        }
    }

    //wird von save and load aufgerufen
    //erstelle und speicher den waiter in einer liste
    public bool InitialisiereWaiter(string waiterInfo)
    {
        //läd das prefab für den waiter
        prefab = Resources.Load("Prefabs/WaiterPrefab") as GameObject;

        //prüfe ob waiterdaten vorhanden sind
        if(waiterInfo.Equals(""))
        {
            return false;
        }
        else
        {
            string[] waiterData = waiterInfo.Split(";");
            for(int a=0;a<waiterData.Length;a++)
            {
                //enthält die einzelnen elemnte eines waiters
                string[] splitWaiterData = waiterData[a].Split(":");

                //erzeuge den waiter und speicher ihn in liste
                Waiter waiter = new Waiter(prefab, PlayerMovementController.FindDoorPos());
                waiterList.Add(waiter);

                /*
                ; für neuen waiter, : für neuen parameter
                name,gender,hair,skin,tshirt,leg
                */

                //läd waiter spezifische daten aus dem speicher
                waiterList[a].Name = splitWaiterData[0];
                waiterList[a].IsBoy = Convert.ToBoolean(splitWaiterData[1]);
                waiterList[a].ToDish = Int32.Parse(splitWaiterData[2]);
                waiterList[a].toServe = Int32.Parse(splitWaiterData[3]);
                waiterList[a].HairColor = new float[]{float.Parse(splitWaiterData[4].Split("-")[0]), float.Parse(splitWaiterData[4].Split("-")[1]), float.Parse(splitWaiterData[4].Split("-")[2])};
                waiterList[a].SkinColor = new float[]{float.Parse(splitWaiterData[5].Split("-")[0]), float.Parse(splitWaiterData[5].Split("-")[1]), float.Parse(splitWaiterData[5].Split("-")[2])};
                waiterList[a].TshirtColor = new float[]{float.Parse(splitWaiterData[6].Split("-")[0]), float.Parse(splitWaiterData[6].Split("-")[1]), float.Parse(splitWaiterData[6].Split("-")[2])};
                waiterList[a].HoseColor = new float[]{float.Parse(splitWaiterData[7].Split("-")[0]), float.Parse(splitWaiterData[7].Split("-")[1]), float.Parse(splitWaiterData[7].Split("-")[2])};
                //namwwaiter:True:40:60:0,65-0,37-1:0,5-0,5-1:1-1-0,3:1-1-1;waiter2:False:50:50:0,65-0,37-1:0,5-0,5-1:1-1-0,3:1-1-1
            }
            return true;
        }       
        return false;
    }

    //funktion zur erstellen eines neuen waiters
    //wird aufgerufen wenn ein neuer waiter gekauft wird
    public void CreateNewWaiter()
    {
        //erzeuge den waiter und speicher ihn in liste
        Waiter waiter = new Waiter(prefab, PlayerMovementController.FindDoorPos());
        waiterList.Add(waiter);

        //läd standardklamotten
        waiterList[waiterList.Count-1].Name = "waiter";
        waiterList[waiterList.Count-1].IsBoy = true;
        waiterList[waiterList.Count-1].ToDish = 50;
        waiterList[waiterList.Count-1].toServe = 50;
        waiterList[waiterList.Count-1].HairColor = new float[]{1f,1f,1f};
        waiterList[waiterList.Count-1].SkinColor = new float[]{1f,1f,1f};
        waiterList[waiterList.Count-1].TshirtColor = new float[]{1f,1f,1f};
        waiterList[waiterList.Count-1].HoseColor = new float[]{1f,1f,1f};
    }

    //baut den string, der die waiter infos enthält zum speichern der daten
    public string WaiterDataToSave()
    {
        string data = "";
        for(int a=0;a<waiterList.Count;a++)
        {
            if(a==0)
            {
                data = waiterList[a].Info();
            }
            else
            {
                data = data+";"+waiterList[a].Info();
            }
        }
        return data;
    }

    //suche alle möglichen positionen  an die der waiter essen liefern kann
    public int[] GetDeliveryPosition()
    {
        int[] position = new int[]{0,-1};

        NPCManager npcManager = GameObject.Find("NPCController").GetComponent<NPCManager>();
        for(int a=0;a<npcManager.npcList.Count;a++)
        {
            //für jeden npc gucken ob state 2 ist(state 2 == npc sitzt auf platz)
            if(npcManager.npcList[a].state==2)
            {
                //ist der tisch leer (ohne essen drauf)
                if(FloorChildExtraDataController.getObjectFCED(npcManager.npcList[a].tablePos[0]+"-"+npcManager.npcList[a].tablePos[1]).Split(";")[3].Equals("False"))
                { 
                    position = new int[]{npcManager.npcList[a].endPos[0], npcManager.npcList[a].endPos[1]};
                    return position;
                   // Debug.Log(npcManager.npcList[a].npcGO.name+": kein essen auf tisch ["+npcManager.npcList[a].tablePos[0]+":"+npcManager.npcList[a].tablePos[1]+"] chair: ["+npcManager.npcList[a].chairPos[0]+":"+npcManager.npcList[a].chairPos[1]+"]");
                }else
                {
                    Debug.Log($"cant get position for [essen auf tisch]: {npcManager.npcList[a].npcGO.name}");
                    //Debug.Log(npcManager.npcList[a].npcGO.name+": essen! auf tisch ["+npcManager.npcList[a].tablePos[0]+":"+npcManager.npcList[a].tablePos[1]+"] chair: ["+npcManager.npcList[a].chairPos[0]+":"+npcManager.npcList[a].chairPos[1]+"]");
                }
            }
            else
            {
                //Debug.Log($"{npcManager.npcList[a].npcGO.name} ist unterwegs");
            }
        }
        return position;
    }

    //wähl randomisierte counter für die waiter aus
    public bool CounterForWaiter(Waiter waiter){

        //zufällige zahl aus der range des waiters
        System.Random rndm = new System.Random();
        int rndmDL = rndm.Next(0,counterDataList.Count);

        for(int a=0;a<counterDataList.Count;a++)
        {
            if(rndmDL+a>=counterDataList.Count)
            {
                //Debug.Log("DL zu hoch "+(counterDataList.Count-1)+"max  rndm: "+rndmDL+"->"+((rndmDL+a)%counterDataList.Count)+":");
                if(SearchForPath(waiter, counterDataList[(rndmDL+a)%counterDataList.Count]))
                {
                    return true;
                }
            }
            else
            {
                //Debug.Log("DL "+(counterDataList.Count-1)+"max  rndm: "+rndmDL+"->"+(rndmDL+a)+":");
                if(SearchForPath(waiter, counterDataList[rndmDL+a]))
                {
                    return true;
                }
            }
        }

        //wenn kein counter mit essen zum hinlaufen gefunden wurde(weil nicht erreichbar) such in leere counter
        int rndmEDL = rndm.Next(0,counterEmptyDataList.Count);

        for(int a=0;a<counterEmptyDataList.Count;a++)
        {
            if(rndmEDL+a>=counterEmptyDataList.Count)
            {
                //Debug.Log("EDL zu hoch "+(counterEmptyDataList.Count-1)+"max  rndm: "+rndmEDL+"->"+((rndmEDL+a)%counterEmptyDataList.Count)+":");
                if(SearchForPath(waiter, counterEmptyDataList[(rndmEDL+a)%counterEmptyDataList.Count]))
                {
                    return true;
                }
            }
            else
            {
                //Debug.Log("EDL "+(counterEmptyDataList.Count-1)+"max  rndm: "+rndmEDL+"->"+(rndmEDL+a)+":");
                if(SearchForPath(waiter, counterEmptyDataList[rndmEDL+a]))
                {
                    return true;
                }
            }
        }
        return false;
    }

    //suche nach path von waiter startPos zu übergebene pos
    public bool SearchForPath(Waiter waiter, string counter)
    {
        //alle nebenstehende positionen bilden
        string[] splitCounter = counter.Split(";");
        int x = Int32.Parse(splitCounter[1].Split("-")[0]);
        int y = Int32.Parse(splitCounter[1].Split("-")[1]);
        string oben = x+"-"+(y-1);  //rechts oben   sprite: d
        string rechts = (x+1)+"-"+y; //rechts unten  sprite: c
        string unten = (x-1)+"-"+y;   //links oben    sprite: a
        string links = x+"-"+(y+1);  //links unten   sprite: b
        string[] suroundingPositions = new string[]{oben, rechts, unten, links};

        //gucke ob umliegende floors existieren
        foreach(string nearbyObj in suroundingPositions)
        {
            //FloorObj existiert (größe des grids beachten)
            if(GameObject.Find(nearbyObj))
            {
                //die fläche ist frei
                if(!GameObject.Find(nearbyObj+"-Child"))
                {
                    //gucke ob npc zur position laufen kann (übergibt die endPos)
                    List<string> waiterPath = LabyrinthBuilder.getWaiterPath(waiter.startPos ,new int[]{Int32.Parse(nearbyObj.Split("-")[0]),Int32.Parse(nearbyObj.Split("-")[1])});
                       
                    //gucke ob route gefunden wurde
                    if(waiterPath.Count!=0)
                    {   
                        //übergebe den zu speichernden path den waiter
                        waiter.path = waiterPath;
    
                        return true;
                    }
                }
            }
        }
        return false;
    }

    //lösche alle waiters aus der scene
    public void DeleteAllWaiters()
    {
        //lösche alle waiter
        GameObject WaiterHandler = GameObject.Find("WaiterHandler");
        for(int a=WaiterHandler.transform.childCount-1;a>=0;a--){
            Destroy(WaiterHandler.transform.GetChild(a).gameObject);
        }

        //cleare die waiter liste 
        waiterList.Clear();
    }





    //steuert die lauf animation für den npc
    public IEnumerator Anim(Waiter waiter, int function)
    {
        GameObject waiterGO = waiter.waiterGO;
        int walkAnim = waiter.walkAnim;
        List<Sprite> Hat = waiter.Hat;
        List<Sprite> FaceBoy = waiter.FaceBoy;
        List<Sprite> HairBoy = waiter.HairBoy;
        List<Sprite> HairOverlayBoy = waiter.HairOverlayBoy;
        List<Sprite> LegBoy = waiter.LegBoy;
        List<Sprite> LegOverlayBoy = waiter.LegOverlayBoy;
        List<Sprite> SkinBoy = waiter.SkinBoy;
        List<Sprite> SkinOverlayBoy = waiter.SkinOverlayBoy;
        List<Sprite> TshirtBoy = waiter.TshirtBoy;
        List<Sprite> TshirtOverlayBoy = waiter.TshirtOverlayBoy;
        List<Sprite> FaceGirl = waiter.FaceGirl;
        List<Sprite> HairGirl = waiter.HairGirl;
        List<Sprite> HairOverlayGirl = waiter.HairOverlayGirl;
        List<Sprite> LegGirl = waiter.LegGirl;
        List<Sprite> LegOverlayGirl = waiter.LegOverlayGirl;
        List<Sprite> SkinGirl = waiter.SkinGirl;
        List<Sprite> SkinOverlayGirl = waiter.SkinOverlayGirl;
        List<Sprite> TshirtGirl = waiter.TshirtGirl;
        List<Sprite> TshirtOverlayGirl = waiter.TshirtOverlayGirl;
        bool isBoy = waiter.IsBoy;

        //solange der npc läuft
        //guckt ob function dieses aufrufes es war die laufanim des npc zu steuern
        while(walkAnim!=0&&function==0){
            walkAnim = waiter.walkAnim;

            //links unten  (front_left)
            if(walkAnim==2){

                //überprüft das geschlecht und rendert je nach dem male, female (true=male)
                //male
                if(isBoy==true){
                    
                    if(waiterGO!=null){
                        //bestimmt die rendering position
                        waiterGO.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.178f,1.657f,-0.02f);
                        waiterGO.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(0.263f,3.393f,-0.03f);
                        waiterGO.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(0.263f,3.406f,-0.02f);
                        waiterGO.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.153f,2.719f,-0.01f);
                        waiterGO.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.22f,0.38f,-0.01f);
                        waiterGO.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(-0.023f,2.643f,-0.01f);
                        waiterGO.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(0.185f,1.655f,-0.01f);
                        waiterGO.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.023f,2.642f,0f);
                        waiterGO.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.115f,0.672f,0f);
                    }

                    //animation
                    for(int a=1;a<7;a++){
                        if(waiterGO!=null){
                            waiterGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[80+a];
                            waiterGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[80+a];
                            waiterGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[80+a];
                            waiterGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[80+a];
                            waiterGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[80+a];
                            waiterGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[80+a];
                            waiterGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[80+a];
                            waiterGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[80+a];
                            waiterGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[80+a];

                            //zeitspann der einzelnen frames
                            if(walkAnim==2){
                                yield return new WaitForSeconds(0.15F);
                            }
                        }
                    }

                    if(waiterGO!=null){
                        //endzenario, wird gebraucht fals der ^npc steht
                        waiterGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[80];
                        waiterGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[80];
                        waiterGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[80];
                        waiterGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[80];
                        waiterGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[80];
                        waiterGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[80];
                        waiterGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[80];
                        waiterGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[80];
                        waiterGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[80];    
                    }

                //female
                }else{
                    
                    //bestimmt die rendering position
                    if(waiterGO!=null){
                        waiterGO.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.1f,1.782f,-0.05f);
                        waiterGO.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(0.407f,3.335f,-0.02f);
                        waiterGO.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(0.407f,3.335f,-0.01f);
                        waiterGO.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(0.092f,2.804f,-0.01f);
                        waiterGO.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.21f,0.56f,-0.03f);
                        waiterGO.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(-0.081f,2.064f,-0.01f);
                        waiterGO.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(0.102f,1.773f,-0.04f);
                        waiterGO.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.083f,2.058f,0f);
                        waiterGO.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.023f,1.05f,-0.02f);
                    }

                    //animation
                    for(int a=1;a<7;a++){
                        if(waiterGO!=null){
                            waiterGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[80+a];
                            waiterGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[80+a];
                            waiterGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[80+a];
                            waiterGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[80+a];
                            waiterGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[80+a];
                            waiterGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[80+a];
                            waiterGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[80+a];
                            waiterGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[80+a];
                            waiterGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[80+a];

                            //zeitspann der einzelnen frames
                            if(walkAnim==2){
                                yield return new WaitForSeconds(0.15F);
                            }
                        }
                    }

                    if(waiterGO!=null){
                        //endzenario, wird gebraucht fals der spieler steht
                        waiterGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[80];
                        waiterGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[80];
                        waiterGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[80];
                        waiterGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[80];
                        waiterGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[80];
                        waiterGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[80];
                        waiterGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[80];
                        waiterGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[80];
                        waiterGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[80];
                    }
                }

            //links oben  
            }else if(walkAnim==3){

                if(isBoy==true){

                    if(waiterGO!=null){
                        waiterGO.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.178f,1.657f,-0.07f);
                        waiterGO.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(0.263f,3.437f,-0.05f);
                        waiterGO.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(0.263f,3.406f,-0.04f);
                        waiterGO.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.153f,2.719f,-0.01f);
                        waiterGO.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.22f,0.38f,-0.05f);
                        waiterGO.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(-0.023f,2.643f,-0.03f);
                        waiterGO.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(0.185f,1.655f,-0.06f);
                        waiterGO.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.023f,2.642f,-0.02f);
                        waiterGO.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.115f,0.672f,-0.04f);
                    }                        

                    for(int a=1;a<7;a++){
                        if(waiterGO!=null){
                            waiterGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[64+a];
                            waiterGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[64+a];
                            waiterGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[64+a];
                            waiterGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[64+a];
                            waiterGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[64+a];
                            waiterGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[64+a];
                            waiterGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[64+a];
                            waiterGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[64+a];
                            waiterGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[64+a];
                            if(walkAnim==3){
                                yield return new WaitForSeconds(0.15F);
                            }
                        }
                    }

                    if(waiterGO!=null){
                        waiterGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[64];
                        waiterGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[64];
                        waiterGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[64];
                        waiterGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[64];
                        waiterGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[71];
                        waiterGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[64];
                        waiterGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[64];
                        waiterGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[64];
                        waiterGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[64];
                    }
        
                //female
                }else{
                    
                    if(waiterGO!=null){
                        //bestimmt die rendering position
                        waiterGO.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.1f,1.782f,-0.04f);
                        waiterGO.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(0.4230f,3.345f,-0.03f);
                        waiterGO.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(0.423f,3.345f,-0.02f);
                        waiterGO.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(0.092f,2.804f,-0.01f);
                        waiterGO.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.21f,0.56f,-0.02f);
                        waiterGO.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(-0.081f,2.064f,-0.01f);
                        waiterGO.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(0.102f,1.773f,-0.03f);
                        waiterGO.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.083f,2.058f,0f);
                        waiterGO.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.023f,1.05f,-0.01f);
                    }

                    //animation
                    for(int a=1;a<7;a++){
                        if(waiterGO!=null){
                            waiterGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[64+a];
                            waiterGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[64+a];
                            waiterGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[64+a];
                            waiterGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[64+a];
                            waiterGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[64+a];
                            waiterGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[64+a];
                            waiterGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[64+a];
                            waiterGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[64+a];
                            waiterGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[64+a];

                            //zeitspann der einzelnen frames
                            if(walkAnim==3){
                                yield return new WaitForSeconds(0.15F);
                            }
                        }
                    }

                    if(waiterGO!=null){
                        //endzenario, wird gebraucht fals der spieler steht
                        waiterGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[64];
                        waiterGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[64];
                        waiterGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[64];
                        waiterGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[64];
                        waiterGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[64];
                        waiterGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[64];
                        waiterGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[64];
                        waiterGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[64];
                        waiterGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[64];
                    }
                }
                
            //rechts unten
            }else if(walkAnim==4){

                if(isBoy==true){
                    
                    if(waiterGO!=null){
                        waiterGO.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(-0.058f,1.617f,-0.03f);
                        waiterGO.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(-0.134f,3.335f,-0.03f);
                        waiterGO.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(-0.137f,3.34f,-0.02f);
                        waiterGO.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(0.258f,2.691f,-0.01f);
                        waiterGO.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0.3399f,0.3809f,-0.01f);
                        waiterGO.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(0.143f,2.611f,-0.01f);
                        waiterGO.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(-0.062f,1.616f,-0.02f);
                        waiterGO.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(0.143f,2.602f,0f);
                        waiterGO.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(0.24f,0.63f,0f);
                    }
            
                    for(int a=1;a<7;a++){
                        if(waiterGO!=null){
                            waiterGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[88+a];
                            waiterGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[88+a];
                            waiterGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[88+a];
                            waiterGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[88+a];
                            waiterGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[88+a];
                            waiterGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[88+a];
                            waiterGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[88+a];
                            waiterGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[88+a];
                            waiterGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[88+a];
                            if(walkAnim==4){
                                yield return new WaitForSeconds(0.15F);
                            }
                        }
                    }
                    if(waiterGO!=null){
                        waiterGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[88];
                        waiterGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[88];
                        waiterGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[88];
                        waiterGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[88];
                        waiterGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[88];
                        waiterGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[88];
                        waiterGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[88];
                        waiterGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[88];
                        waiterGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[88];
                    }
                
                //female
                }else{
                    
                    if(waiterGO!=null){
                        //bestimmt die rendering position
                        waiterGO.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(-0.267f,1.776f,-0.04f);
                        waiterGO.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(-0.57f,3.342f,-0.03f);
                        waiterGO.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(-0.569f,3.342f,-0.02f);
                        waiterGO.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.248f,2.862f,-0.01f);
                        waiterGO.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0.059f,0.532f,-0.02f);
                        waiterGO.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(-0.081f,2.064f,-0.01f);
                        waiterGO.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(-0.265f,1.767f,-0.03f);
                        waiterGO.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(-0.083f,2.058f,0f);
                        waiterGO.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.139f,1.041f,-0.01f);
                    }

                    //animation
                    for(int a=1;a<7;a++){
                        if(waiterGO!=null){
                            waiterGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[88+a];
                            waiterGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[88+a];
                            waiterGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[88+a];
                            waiterGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[88+a];
                            waiterGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[88+a];
                            waiterGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[88+a];
                            waiterGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[88+a];
                            waiterGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[88+a];
                            waiterGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[88+a];

                            //zeitspann der einzelnen frames
                            if(walkAnim==4){
                                yield return new WaitForSeconds(0.15F);
                            }
                        }
                    }
                    if(waiterGO!=null){
                        //endzenario, wird gebraucht fals der spieler steht
                        waiterGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[88];
                        waiterGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[88];
                        waiterGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[88];
                        waiterGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[88];
                        waiterGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[88];
                        waiterGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[88];
                        waiterGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[88];
                        waiterGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[88];
                        waiterGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[88];
                    }
                }

            //rechts oben
            }else if(walkAnim==5){

                if(isBoy==true){

                    if(waiterGO!=null){
                        waiterGO.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(-0.079f,1.747f,-0.06f);
                        waiterGO.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(-0.136f,3.497f,-0.06f);
                        waiterGO.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(-0.136f,3.4659f,-0.05f);
                        waiterGO.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.282f,2.719f,-0.01f);
                        waiterGO.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0.336f,0.46f,-0.04f);
                        waiterGO.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(0.131f,2.731f,-0.03f);
                        waiterGO.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(-0.073f,1.745f,-0.05f);
                        waiterGO.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(0.131f,2.731f,-0.02f);
                        waiterGO.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(0.234f,0.754f,-0.03f);
                    }

                    for(int a=1;a<7;a++){
                        if(waiterGO!=null){
                            waiterGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[72+a];
                            waiterGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[72+a];
                            waiterGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[72+a];
                            waiterGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[72+a];
                            waiterGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[72+a];
                            waiterGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[72+a];
                            waiterGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[72+a];
                            waiterGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[72+a];
                            waiterGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[72+a];
                            if(walkAnim==5){
                                yield return new WaitForSeconds(0.15F);
                            }
                        }
                    }

                    if(waiterGO!=null){
                        waiterGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayBoy[72];
                        waiterGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayBoy[72];
                        waiterGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairBoy[72];
                        waiterGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceBoy[72];
                        waiterGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayBoy[72];
                        waiterGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayBoy[72];
                        waiterGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtBoy[72];
                        waiterGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinBoy[72];
                        waiterGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegBoy[72];
                    }
                
                //female
                }else{

                    if(waiterGO!=null){
                        //bestimmt die rendering position
                        waiterGO.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(-0.151f,1.782f,-0.05f);
                        waiterGO.transform.GetChild(5).gameObject.transform.localPosition = new Vector3(-0.445f,3.265f,-0.07f);
                        waiterGO.transform.GetChild(4).gameObject.transform.localPosition = new Vector3(-0.445f,3.265f,-0.06f);
                        waiterGO.transform.GetChild(6).gameObject.transform.localPosition = new Vector3(-0.163f,3.074f,-0.01f);
                        waiterGO.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0.153f,0.565f,-0.03f);
                        waiterGO.transform.GetChild(7).gameObject.transform.localPosition = new Vector3(0.042f,2.065f,-0.01f);
                        waiterGO.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(-0.15f,1.773f,-0.04f);
                        waiterGO.transform.GetChild(8).gameObject.transform.localPosition = new Vector3(0.04f,2.06f,0f);
                        waiterGO.transform.GetChild(2).gameObject.transform.localPosition = new Vector3(-0.023f,1.05f,-0.02f);
                    }

                    //animation
                    for(int a=1;a<7;a++){
                        if(waiterGO!=null){
                            waiterGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[72+a];
                            waiterGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[72+a];
                            waiterGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[72+a];
                            waiterGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[72+a];
                            waiterGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[72+a];
                            waiterGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[72+a];
                            waiterGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[72+a];
                            waiterGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[72+a];
                            waiterGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[72+a];

                            //zeitspann der einzelnen frames
                            if(walkAnim==5){
                                yield return new WaitForSeconds(0.15F);
                            }
                        }
                    }

                    if(waiterGO!=null){
                        //endzenario, wird gebraucht fals der npc steht
                        waiterGO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtOverlayGirl[72];
                        waiterGO.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = HairOverlayGirl[72];
                        waiterGO.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = HairGirl[72];
                        waiterGO.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = FaceGirl[72];
                        waiterGO.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = LegOverlayGirl[72];
                        waiterGO.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = SkinOverlayGirl[72];
                        waiterGO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = TshirtGirl[72];
                        waiterGO.transform.GetChild(8).gameObject.GetComponent<SpriteRenderer>().sprite = SkinGirl[72];
                        waiterGO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = LegGirl[72];
                    }
                }
            }

            //nur zum test
            yield return new WaitForSeconds(0.1F);
        }
        Debug.Log("ende!!!");
    }
}
