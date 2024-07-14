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


            /*
            idle position am tresen
            suche je nach priorität (50%,50%) oder (20%,80%) mit -> random(): essen liefern oder abräumen
                -> finde gegebenes ziel, wenn keins verfügbar, nehme anderes ziel
            suche essen, nimm essen, rechne essen ab
            
            hat waiter eine aufgabe?
            ja -> erledige aufgabe
            nein -> steht waiter am tresen? 
                nein -> gehe zum tresen mit essen darauf, sonst zu einem ohne essen
                ja -> suche nach aufgabe

            aufgabe: essem liefern, essen abräumen  
            */

            //suche jede sekunde nach allen tresen + inhalt dieser
            counterDataList = FloorChildExtraDataController.getFCEDFromAllCounter();
            
            //sort counter list nach essen (essen pos 0 in array)
            for(int a=0;a<counterDataList.Count;a++)
            {
                string[] splitData = counterDataList[a].Split(";");

                //tresen hat essen
                //vertausche die werte, sodas bei iteration der werten mit essen in liste ganz "oben" stehen
                if(splitData[2].Equals("False")){
                    string switchData = counterDataList[0];
                    counterDataList[0] = counterDataList[a];
                    counterDataList[a] = switchData;
                }
            }



            //für jeden waiter
            for(int a=0;a<waiterList.Count;a++)
            {
                //ist ziel zum tresen zu gehen? (objective==1)
                if(waiterList[a].objective==1)
                {
                    //CONTINUE suche path zum tresen
                    if(SearchForPath(waiterList[a]))
                    {
                        //wenn weg gefunden
                        Debug.Log("weg für "+waiterList[a].Name+" gefunden");
                    }
                }
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

//        Debug.Log("speicher dtata "+data);
        return data;
    }

    //suche nach path von waiter startPos zu übergebene pos
    public bool SearchForPath(Waiter waiter)
    {
        //für jeden counter prüfe ob weg von waiter zu einem tresen möglich ist
        for(int a=0;a<counterDataList.Count;a++)
        {
            //alle nebenstehende positionen bilden
            string[] splitCounter = counterDataList[a].Split(";");
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
                        List<string> npcPath = LabyrinthBuilder.getWaiterPath(waiter.startPos ,new int[]{Int32.Parse(nearbyObj.Split("-")[0]),Int32.Parse(nearbyObj.Split("-")[1])});
                        //gucke ob route gefunden wurde
                        if(npcPath.Count!=0)
                        {   
                            //CONTINUE
                            //waiter hat weg zum gehen
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }
}
