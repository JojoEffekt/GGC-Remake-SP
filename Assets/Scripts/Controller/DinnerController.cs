using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DinnerController : MonoBehaviour
{
    //managed die gerichte die gerade auf den öfen gekocht werden
    //und sorgt dafür das die gericht zubereitet, abgeholt und serviert werden können


    //speicher den objectnamen von den angeklickten oven auf den gekocht werden soll
    //wird bei anklicken auf einen oven gespeichert
    public static string ovenToCookOnByOpenDinnerShopUI;

    //beinhaltet die schritte die zum erstellen eines dinners gebraucht werden
    //wird aufgerufen wenn im DinnerUIShop auf CookBtn gedrückt wird
    public static void CookNewDinnerOnOven(DinnerItem item){

        //spieler soll zum oven gehen
        if(SearchForPositionNearTheClickedOven()){
            Debug.Log("der "+item.name+" kann auf "+ovenToCookOnByOpenDinnerShopUI+" gekocht werden");
        }

        //wenn angekommen, rechne gericht zutaten aus dem fridge ab
        //erzeuge die DinnerUI auf dem oven, 
        //beginne mit zubereitungsschritten
        //FCED change erzeugen(wie gemacht wird, ist in buttonController)
    }

    //sucht nach einer nebenstehenden positionen des oven
    //gibt false wieder wenn nichts gefunden
    public static bool SearchForPositionNearTheClickedOven(){

        //sucht nach einer freien position oben, rechts, unten, links vom oven
        int x = Int32.Parse(ovenToCookOnByOpenDinnerShopUI.Split("-")[0]);
        int y = Int32.Parse(ovenToCookOnByOpenDinnerShopUI.Split("-")[1]);
        string oben = ""+(x-1)+"-"+y;
        string rechts = ""+x+"-"+(y-1);
        string unten = ""+(x+1)+"-"+y;
        string links = ""+x+"-"+(y+1);
        string[] suroundingPositions = new string[]{oben, rechts, unten, links};

        //gucke ob umliegende floors existieren
        foreach(string nearbyObj in suroundingPositions){

            //GameObj existiert
            if(GameObject.Find(nearbyObj)){

                //Floorobject existiert nicht
                if(!GameObject.Find(nearbyObj+"-Child")){
                    
                    //starte playermovement und gucke ob der spieler dahin laufen kann
                    if(PlayerMovementController.MovePlayer(new int[]{Int32.Parse(nearbyObj.Split("-")[0]),Int32.Parse(nearbyObj.Split("-")[1])})){
                        
                        //spieler kann zum oven gehen
                        return true;
                    }
                }
            }
        }
        //cooking abbrechen, fals oven versperrt
        return false;
    }
}
