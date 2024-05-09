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
        Debug.Log("der "+item.name+" kann auf "+ovenToCookOnByOpenDinnerShopUI+" gekocht werden");

        foreach(var ingredient in item.infoIngredients){
            Debug.Log(ingredient.Key+":"+item.infoIngredients[ingredient.Key]);

            //CONTINUE item_01 to lettuce umwandeln und nach anzahler der stücke entfernen
            PlayerController.RemoveFoodItem(ingredient.Key);
            /*

            IngredientList.Add(new IngredientItem("Lettuce", "item_19", 30, 0, 0, true, 2, 0));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Onions", "item_13", 60, 0, 0, true, 2, 1));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Tomatoes", "item_02", 90, 0, 0, true, 2, 2));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Pasta", "item_11", 250, 0, 0, true, 4, 3));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Chocolate", "item_29", 300, 0, 0, true, 4, 4));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Cream", "item_26", 300, 0, 0, true, 1, 5));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Eggs", "item_23", 400, 0, 0, true, 1, 6));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Minced meat", "item_17", 400, 0, 0, true, 1, 7));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Flour", "item_22", 200, 0, 0, true, 4, 8));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Cheese", "item_32", 320, 0, 0, true, 1, 9));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Peas", "item_10", 150, 0, 0, true, 2, 10));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Corn", "item_03", 150, 0, 0, true, 2, 11));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Pineapple", "item_09", 300, 0, 0, true, 3, 12));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Oil", "item_14", 300, 0, 0, true, 4, 13));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Beans", "item_34", 100, 0, 0, true, 2, 14));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Bacon", "item_36", 300, 0, 0, true, 1, 15));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Rice", "item_16", 350, 0, 0, true, 2, 16));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Paprika", "item_37", 450, 0, 0, true, 2, 17));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Cocktail Cherry", "item_50", 0, 0, 0, true, 5, 18));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Butter", "item_53", 425, 0, 0, true, 4, 19));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Potatoes", "item_08", 550, 0, 0, true, 2, 20));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Duck", "item_48", 1500, 0, 0, true, 1, 21));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Oranges", "item_45", 0, 1, 0, true, 5, 22));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Beetroot", "item_33", 400, 0, 0, true, 2, 23));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Cucumbers", "item_25", 400, 0, 0, true, 2, 24));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Meat", "item_18", 1500, 0, 0, true, 1, 25));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Eggplant", "item_24", 100, 0, 0, true, 3, 12));
        IngredientList.Add(new IngredientItem("Sugar", "item_04", 1000, 0, 0, true, 4, 13));
        IngredientList.Add(new IngredientItem("Rhubarb", "item_07", 1500, 0, 0, true, 2, 28));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Garlic", "item_21", 750, 0, 0, true, 2, 29));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Chicken", "item_31", 404, 0, 0, true, 1, 30));//CONTINUE Level
        IngredientList.Add(new IngredientItem("Biscuits", "item_49", 404, 0, 0, true, 4, 31));//CONTINUE Level

            */
        }


        //wenn angekommen, rechne gericht zutaten aus dem fridge ab
        
        
        //erzeuge die DinnerUI auf dem oven, 
        //beginne mit zubereitungsschritten
        //FCED change erzeugen(wie gemacht wird, ist in buttonController)
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
}
