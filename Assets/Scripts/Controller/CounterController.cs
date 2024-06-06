using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class CounterController : MonoBehaviour
{
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
            if(items[3].Equals(dinner)){
                sortCounters.Add(counters[a]);
            }
        }

        //sortiere counter nach priorität 2 (leer)
        for(int b=0;b<counters.Count;b++)
        {
            string[] items = counters[b].Split(";");
            
            //counter ist leer! -> prio 2
            if(items[3].Length==0&&bool.Parse(items[2])==true){
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
