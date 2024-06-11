using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class CounterController : MonoBehaviour
{   
    //enthält das dinnerPrefab was auf dem oven erzeugt wird
    public static GameObject DinnerOnCounterPrefab;

    //dieses object enthält die Dinners auf den verschiedenen öfen
    public static GameObject DinnerOnCounterHandler;

    public void Awake()
    {
        //läd das dinnerPrefab
        DinnerOnCounterPrefab = (GameObject)Resources.Load("Prefabs/DinnerOnCounterPrefab", typeof(GameObject));

        //sucht den ovenObjectHandler
        DinnerOnCounterHandler = GameObject.Find("DinnerOnOvens");
    }

    //lösche das dinner auf den counter
    public static bool DeleteDinnerPrefabOnCounter(string counter)
    {
        Debug.Log("suche:"+counter);
        GameObject objToDelete = GameObject.Find(counter);
        if(objToDelete!=null)
        {
            DestroyImmediate(objToDelete);
            return true;
        }
        return false;
    }

    //rechnet die neue zahl für den jeweiligen counter zusammen wie viele gericht darauf stehen
    public static bool ChangeFCEDDataForDinnerOnCounter(string counter, string dinner, int foodToAdd)
    {
        //addiere die foods zusammen
        int foodCount = FloorChildExtraDataController.getCounter(counter).foodCount + foodToAdd;
        //ändere das FCED für den counter
        FloorChildExtraDataController.ChangeFCEDData("Counter;"+counter+";False;"+dinner+";"+foodCount);

        return true;
    }

    //erzeugt ein neues Dinner auf den replacten counter
    //wird bei ButtonController.MoveObjectOnFloor aufgerufen wenn ein floorObj replaced wird
    public static bool ReadjustAllDinnerPrefabsOnCounter()
    {

        //hole das FCED vom replatzierten oven
        string[] CounterFCED = FloorChildExtraDataController.FindMovedCounterFCED().Split(";");

        //wenn das replacte object kein dinner enthält, breche ab
        if(CounterFCED[0].Equals("")){
            return false;
        }

        //da das alte dinner beim replacen gelöscht wird, erzeuge ein neues
        AddDinnerOnCounter(CounterFCED[1], CounterFCED[3]);

        return true;
    }

    //platziere dinner auf counter und rechne FCED ab
    public static bool AddDinnerOnCounter(string counter, string dinner)
    {   
        //erzeuge ein prefab auf dem counter
        CreateDinnerPrefabOnCounter(counter+"-Child", dinner);

        return true;
    }

    //erzeugt das dinner auf dem counter (wird auf bei neustart des spiels aufgerufen)
    public static bool CreateDinnerPrefabOnCounter(string counter, string dinner)
    {   
        //suche erst nach counter object ob es schon existiert, wenn ja, erzeuge kein weiteres (nur ein object auf einem counter)
        if(GameObject.Find(counter+"-Counter")!=null)
        {
            return false;
        }

        //suche die koordianten vom counter
        float[] coords = new float[]{GameObject.Find(counter).gameObject.transform.position.x, GameObject.Find(counter).gameObject.transform.position.y};
        //wandelt das dinner in den spritenamen das in den files liegt
        string dinnerName = DinnerController.getDinnerName(dinner);
        float[] dinnerCoords = DinnerController.getDinnerCoords(dinner);
        Sprite dinnerSprite = DinnerController.getSprite(dinnerName.Substring(0,11)+"4");

        //erzeuge das prefab
        GameObject prefab = Instantiate(DinnerOnCounterPrefab, new Vector3(coords[0]+dinnerCoords[0], coords[1]+dinnerCoords[1], 0), Quaternion.identity, DinnerOnCounterHandler.transform);
        prefab.name = counter+"-Counter";
        prefab.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = GameObject.Find(counter).gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
        prefab.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = dinnerSprite;

        return true;
    }

    //sucht alle tresen mit dem gleichen gericht bzw freie tresen herraus
    public static string getCounterForDinner(string dinner)
    {
        //enthält die sortierten counter für das dinner zum ablagern
        List<string> sortCounters = SortCounterList(dinner);

        //suche nach einem begehbaren counter
        return getCounter(sortCounters);
    }

    //holt sich die daten aus FCED und gibt eine liste mit allen counters wieder
    public static List<string> SortCounterList(string dinner)
    {
        //liste die die übergebenen counter enthält, wird im verlauf nach prioität sortiert
        List<string> counters = new List<string>();

        //RÜCKGABEWERT 
        //enthält die sortierten counter für das dinner zum ablagern
        List<string> sortCounters = new List<string>();

        //hole alle dinnner um herrauszufinden welcher für den spieler der beste und erreichbar ist
        counters = FloorChildExtraDataController.getFCEDFromTyp("Counter", dinner);

        //sortiere counter nach priorität 1 (gleiches dinner)
        for(int a=0;a<counters.Count;a++)
        {
            string[] items = counters[a].Split(";");
            
            //counter enthält dinner! -> prio 1
            if(items[3].Equals(dinner))
            {
                sortCounters.Add(counters[a]);
            }
        }

        //sortiere counter nach priorität 2 (leer)
        for(int b=0;b<counters.Count;b++)
        {
            string[] items = counters[b].Split(";");
            
            //counter ist leer! -> prio 2
            if(items[3].Length==0&&bool.Parse(items[2])==true)
            {
                sortCounters.Add(counters[b]);
            }
        }

        return sortCounters;
    }

    //sucht in der übergebenen liste nach einem begehbaren counter
    public static string getCounter(List<string> sortCounters)
    {
        //für jeden eintrag in der liste 
        for(int a=0;a<sortCounters.Count;a++)
        {   
            //guck ob position begehbar ist
            if(isPositionValid(sortCounters[a].Split(";")[1]))
            {   
                //gibt den begehbaren counter zurück
                return sortCounters[a];
            }
        }
        return null;
    }


    //schaut ob die position begehbar ist, gibt true zurück
    public static bool isPositionValid(string objectToMoveOn)
    {
        //sucht nach einer freien position oben, rechts, unten, links vom oven
        int x = Int32.Parse(objectToMoveOn.Split("-")[0]);
        int y = Int32.Parse(objectToMoveOn.Split("-")[1]);
        string oben = ""+(x-1)+"-"+y;
        string rechts = ""+x+"-"+(y-1);
        string unten = ""+(x+1)+"-"+y;
        string links = ""+x+"-"+(y+1);
        string[] suroundingPositions = new string[]{oben, rechts, unten, links};

        //gucke ob umliegende floors existieren
        foreach(string nearbyObj in suroundingPositions)
        {
            //GameObj existiert
            if(GameObject.Find(nearbyObj))
            {
                //Floorobject existiert nicht
                if(!GameObject.Find(nearbyObj+"-Child"))
                {
                    //starte playermovement und gucke ob der spieler dahin laufen kann
                    if(PlayerMovementController.MovePlayer(new int[]{Int32.Parse(nearbyObj.Split("-")[0]),Int32.Parse(nearbyObj.Split("-")[1])}))
                    {
                        //spieler kann zum tresen gehen gehen
                        return true;
                    }
                }
            }
        }
        //abbrechen, falls ungültig
        return false;
    }
} 
