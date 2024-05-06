using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinnerController : MonoBehaviour
{
    //managed die gerichte die gerade auf den öfen gekocht werden
    //und sorgt dafür das die gericht zubereitet, abgeholt und serviert werden können


    //speicher den objectnamen von den angeklickten oven auf den gekocht werden soll
    public static string ovenToCookOnByOpenDinnerShopUI;

    public static void CookNewDinnerOnOven(DinnerItem item){
        Debug.Log("on "+ovenToCookOnByOpenDinnerShopUI+": "+item.name);

        //spieler soll zum oven gehen
        SearchForPositionNearTheClickedOven();

        //wenn angekommen, rechne gericht zutaten aus dem fridge ab
        //erzeuge die DinnerUI auf dem oven, 
        //beginne mit zubereitungsschritten
        //FCED change erzeugen(wie gemacht wird, ist in buttonController)
    }

    public static void SearchForPositionNearTheClickedOven(){
        //sucht nach einer freien position oben, rechts, unten, links vom oven
        
        //nimmt die erste gefundene position
        
        //PlayerMovementController.MovePlayer(new int[]{x,y});
    }
}
