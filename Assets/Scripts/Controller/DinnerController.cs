using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Debug.Log("on "+ovenToCookOnByOpenDinnerShopUI+": "+item.name);

        //spieler soll zum oven gehen
        if(SearchForPositionNearTheClickedOven()){
            //CONTINUE
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
            if(Gameobject.Find(nearbyObj)){

                //Floorobject existiert nicht
                if(!Gameobject.Find(nearbyObj+"-Child")){
                    
                    //starte playermovement
                    if(PlayerMovementController.MovePlayer(new int[]{Int32.Parse(nearbyObj.Split("-")[0]),Int32.Parse(nearbyObj.Split("-")[1])})){
        
                        //break loop und continue
                        return true;
                    }
                }
            }
        }
        //cooking abbrechen
        return false;
    }
}
