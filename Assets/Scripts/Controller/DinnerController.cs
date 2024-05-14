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

    //switcher ob spieler gerade in bewegung ist und darauf gewartet wird das
    //der spieler am ende ankommt
    public static bool isWaitForPlayerToStop = false;

    //beinhaltet die schritte die zum erstellen eines dinners gebraucht werden
    //wird aufgerufen wenn im DinnerUIShop auf CookBtn gedrückt wird
    public static bool CookNewDinnerOnOven(DinnerItem item){

        //abbruch wenn der spieler nicht zum oven gehen kann
        if(!SearchForPositionNearTheClickOnObject(ovenToCookOnByOpenDinnerShopUI)){
            return false;
        }
        
        //zählt die anzahl der ingredients
        int IngredientsCount = CountIngredients(item);
        Debug.Log("Anzahl der items zum abrechnen: "+IngredientsCount);

        //entfernt die items für das dinner aus dem fridge
        if(!RemoveIngredients(item)){
            return false;
        }

        //baut den FCED string und übergibt ihn zum speichern
        //CONTINUE stepanzahl muss aus den gesamten ingredients errechnet werden
        string data = "Oven;"+ovenToCookOnByOpenDinnerShopUI.Split("-")[0]+"-"+ovenToCookOnByOpenDinnerShopUI.Split("-")[1]+";"+IngredientsCount+";"+item.name+";"+item.info["number"]+";heute;morgen";
        Debug.Log("cook: "+item.name+" : "+data);

        //verändere das FCED von dem angeklickten oven, wenn ein neues dinner erstellt wird
        FloorChildExtraDataController.ChangeFCEDData(data);


        //nach jeder action muss neu gespeichert werden
        SaveAndLoadController.SavePlayerData();

        return true;
    }

    //sucht nach einer nebenstehenden positionen des oven
    //gibt false wieder wenn nichts gefunden
    public static bool SearchForPositionNearTheClickOnObject(string ObjectToMoveOn){

        //sucht nach einer freien position oben, rechts, unten, links vom oven
        int x = Int32.Parse(ObjectToMoveOn.Split("-")[0]);
        int y = Int32.Parse(ObjectToMoveOn.Split("-")[1]);
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

    //zählt die anzahl der ingredients die für das item benötigt werden um später als
    //stepAnzahl benutzt werden
    public static int CountIngredients(DinnerItem item){
        int count = 0;

        foreach(var ingredient in item.infoIngredients){

            for(int a=0;a<item.infoIngredients[ingredient.Key];a++){

                count = count + 1;
            }
        }
        return count;
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



    //lässt den spieler zu einer position schicken und erstellt eine warteanfrage
    //indem darauf gewartet wird, das der spieler an der position angekommen ist
    //und daraufhin in PlayerMovementController weitergemacht wird
    public static bool ReduceStepCount_SendPlayer(string objectName, int stepCount){

        //gucke ob spieler zum oven laufen kann
        if(!SearchForPositionNearTheClickOnObject(objectName)){
            return false;
        }else{
            //spieler läuft los
            isWaitForPlayerToStop = true;
        }
        
        return true;
    }

    //dinner auf oven wird fortgesetzt
    public static bool ReduceStepCount_UI(){
        //CONTINUE
        //...
        //wenn spieler am oven ist, (muss überprüfen wann genau spieler am oven angekommen ist)
        //-> step-1
        //-> ui updaten 
        //change FCED
    }
}
