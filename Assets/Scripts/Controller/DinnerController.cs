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
    public static bool CookNewDinnerOnOven(DinnerItem item){

        //abbruch wenn der spieler nicht zum oven gehen kann
        if(!SearchForPositionNearTheClickedOven()){
            return false;
        }
        
        //Debug.Log("der "+item.name+" kann auf "+ovenToCookOnByOpenDinnerShopUI+" gekocht werden");

        //entfernt die items für das dinner aus dem fridge
        if(!RemoveIngredients(item)){
            return false;
        }

        
        
        //erzeuge die DinnerUI auf dem oven,
        /*
        set FCED
        dinner prefab auf oven erzeugen
        die einzelnen schritte des jeweiligen gerichtes abgehen (für jedes item einmal klicken)
        -> jeden klick auf prefab registrieren und eins weiter springen

        wenn fertig, dinner erzeugen und dauerhaft zeit gegenrechnen

        */
        //baut den FCED string und übergibt ihn zum speichern
        //CONTINUE stepanzahl muss aus den gesamten ingredients errechnet werden
        string data = "Oven;"+ovenToCookOnByOpenDinnerShopUI.Split("-")[0]+"-"+ovenToCookOnByOpenDinnerShopUI.Split("-")[1]+";"+1+";"+item.name+";"+item.info["number"]+";heute;morgen";
        Debug.Log("cook: "+item.name+" : "+data);

        //verändere das FCED von dem angeklickten oven, wenn ein neues dinner erstellt wird
        FloorChildExtraDataController.ChangeFCEDData(data);



        //nach jeder action muss neu gespeichert werden
        SaveAndLoadController.SavePlayerData();

        return true;
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

    //entfernt die ingredients aus dem kühlschrank
    public static bool RemoveIngredients(DinnerItem item){

        //für jedes item für das dinner entferne es aus dem kühlschrank
        foreach(var ingredient in item.infoIngredients){
            
            //Debug.Log(ingredient.Key+":"+item.infoIngredients[ingredient.Key]);

            //entfernt die entsprechende anzahl des jeweiligen items
            for(int a=0;a<item.infoIngredients[ingredient.Key];a++){
                PlayerController.RemoveFoodItem(IngredientDict(ingredient.Key));
            }
        }
        return true;
    }

    //wandelt "item_23" in ein ingredient um und gibt es zurück
    public static string IngredientDict(string item){
        string ingredient="";

        if(item.Equals("item_19")){
            ingredient="Lettuce";
        }
        if(item.Equals("item_13")){
            ingredient="Onions";
        }
        if(item.Equals("item_02")){
            ingredient="Tomatoes";
        }
        if(item.Equals("item_11")){
            ingredient="Pasta";
        }
        if(item.Equals("item_29")){
            ingredient="Chocolate";
        }
        if(item.Equals("item_26")){
            ingredient="Cream";
        }
        if(item.Equals("item_23")){
            ingredient="Eggs";
        }
        if(item.Equals("item_17")){
            ingredient="Minced";
        }
        if(item.Equals("item_22")){
            ingredient="Flour";
        }
        if(item.Equals("item_32")){
            ingredient="Cheese";
        }
        if(item.Equals("item_10")){
            ingredient="Peas";
        }
        if(item.Equals("item_03")){
            ingredient="Corn";
        }
        if(item.Equals("item_09")){
            ingredient="Pineapple";
        }
        if(item.Equals("item_14")){
            ingredient="Oil";
        }
        if(item.Equals("item_34")){
            ingredient="Beans";
        }
        if(item.Equals("item_36")){
            ingredient="Bacon";
        }
        if(item.Equals("item_16")){
            ingredient="Rice";
        }
        if(item.Equals("item_37")){
            ingredient="Paprika";
        }
        if(item.Equals("item_50")){
            ingredient="Cocktail Cherry";
        }
        if(item.Equals("item_53")){
            ingredient="Butter";
        }
        if(item.Equals("item_08")){
            ingredient="Potatoes";
        }
        if(item.Equals("item_48")){
            ingredient="Duck";
        }
        if(item.Equals("item_45")){
            ingredient="Oranges";
        }
        if(item.Equals("item_33")){
            ingredient="Beetroot";
        }
        if(item.Equals("item_25")){
            ingredient="Cucumbers";
        }
        if(item.Equals("item_18")){
            ingredient="Meat";
        }
        if(item.Equals("item_24")){
            ingredient="Eggplant";
        }
        if(item.Equals("item_04")){
            ingredient="Sugar";
        }
        if(item.Equals("item_07")){
            ingredient="Rhubarb";
        }
        if(item.Equals("item_21")){
            ingredient="Garlic";
        }
        if(item.Equals("item_31")){
            ingredient="Chicken";
        }
        if(item.Equals("item_49")){
            ingredient="Biscuits";
        }
        return ingredient;
    }
}
