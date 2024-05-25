using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class DinnerController : MonoBehaviour
{
    //managed die gerichte die gerade auf den öfen gekocht werden
    //und sorgt dafür das die gericht zubereitet, abgeholt und serviert werden können

    //gameobject dass das DinnerAnim script enthält
    public static GameObject DinnerAnim;

    //enthält das dinnerPrefab was auf dem oven erzeugt wird
    public static GameObject DinnerOnOvenPrefab;

    //dieses object enthält die Dinners auf den verschiedenen öfen
    public static GameObject DinnerOnOvenHandler;

    //speicher den objectnamen von den angeklickten oven auf den gekocht werden soll
    //wird bei anklicken auf einen oven gespeichert, wenn der oven frei ist
    public static string CookDinner_ovenClickedOn;

    //speicher den objectnamen von den angeklickten oven auf den das gericht fertig gestellt werden soll
    //wird bei anklicken auf einen oven gespeichert, wenn es ein diner FCED hat
    public static string ReduceCount_ovenClickedOn;

    //switcher ob spieler gerade in bewegung ist und darauf gewartet wird das
    //der spieler am ende ankommt
    public static bool isWaitForPlayerToStop = false;

    //beinhaltet die dinnersprites
    public static List<Sprite> sprites = new List<Sprite>();

    //beinhaltet die ingredientsprites
    public static List<Sprite> ingredientsSprites = new List<Sprite>();

    //bei der initialisierung des scripts wird die referenz geholt
    public void Start(){
        //läd ein script
        DinnerAnim = GameObject.Find("DinnerAnim");
    
        //läd das dinnerPrefab
        DinnerOnOvenPrefab = (GameObject)Resources.Load("Prefabs/DinnerOnOvenPrefab", typeof(GameObject));

        //sucht den ovenObjectHandler
        DinnerOnOvenHandler = GameObject.Find("DinnerOnOvens");

        //läd die dinnersprites
        object[] spriteList = Resources.LoadAll("Textures/Dinner", typeof(Sprite));
        foreach(object obj in spriteList){
            sprites.Add((Sprite)obj);
        }    

        //läd die dinnersprites
        object[] IngSpriteList = Resources.LoadAll("Textures/UI/Ingredients", typeof(Sprite));
        foreach(object obj in IngSpriteList){
            ingredientsSprites.Add((Sprite)obj);
        }    
    }

    //beinhaltet die schritte die zum erstellen eines dinners gebraucht werden
    //wird aufgerufen wenn im DinnerUIShop auf CookBtn gedrückt wird
    public static bool CookNewDinnerOnOven(DinnerItem item){

        //abbruch wenn der spieler nicht zum oven gehen kann
        if(!SearchForPositionNearTheClickOnObject(CookDinner_ovenClickedOn)){
            return false;
        }
        
        //zählt die anzahl der ingredients
        int IngredientsCount = CountIngredients(item);

        //entfernt die items für das dinner aus dem fridge
        if(!RemoveIngredients(item)){
            return false;
        }

        //baut den FCED string und übergibt ihn zum speichern
        string data = "Oven;"+CookDinner_ovenClickedOn.Split("-")[0]+"-"+CookDinner_ovenClickedOn.Split("-")[1]+";"+IngredientsCount+";"+item.name+";"+item.info["number"]+";"+null+";"+item.info["time"];
        Debug.Log("cook: "+item.name+" : "+data);

        //verändere das FCED von dem angeklickten oven, wenn ein neues dinner erstellt wird
        FloorChildExtraDataController.ChangeFCEDData(data);


        //erzeuge ein prefab auf dem oven
        CreateDinnerPrefabOnOven(CookDinner_ovenClickedOn, item.name, IngredientsCount);


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

        //Debug.Log("Spieler geht zum oven [DinnerController]");
        
        return true;
    }

    //dinner auf oven wird fortgesetzt
    public static bool ReduceStepCount_UI(){
        //CONTINUE
        //-> ui updaten 
        /*
        prefab ui updaten indem das dinnerobject aus FCED ausgelesen wird und die nächste cookingphase gesucht wird
        (die ingredients müssen nacherinander erscheinen bzw generiert werden)
        //kleine loopzeit für die bearbeitung
        */

        //baut den FCED string und übergibt ihn zum speichern
        int stepAnzahl = FloorChildExtraDataController.getOvenStep(ReduceCount_ovenClickedOn);

        //holt den die daten für den aktuellen zu behandelten oven
        string[] ovenFCED = FloorChildExtraDataController.LoadOvenFCED(ReduceCount_ovenClickedOn).Split(";");

        //wird zum bauen der neuen FCED für den oven verwendet
        string data = "";

        //wenn die stepanzahl bei 100 ist => Dinner ist fertig, starte servierung auf tresen ansonsten fertige dinner weiter an
        if(stepAnzahl==100&&ovenFCED.Length>1){

            //datum wann das gericht "angebaut" wurde
            DateTime startDate = DateTime.Parse(ovenFCED[5]);

            //berechne den endzeitpunkt an dem das gericht fertig ist
            DateTime endDate = startDate.AddMinutes(Int32.Parse(ovenFCED[6]));

            //essen ist fertig
            if(DateTime.Now>endDate){

                Debug.Log("essen fertig!, serviere essen");

                //spielt die Dinner animation ab
                //sperrt weitere handlungen da Animation pflicht ist!
                DinnerAnim.GetComponent<DinnerAnim>().Controller();

                //CONTINUE
                //serviere zum tresen
            }

        }else if(ovenFCED.Length>1){
            //wenn stepanzahl in diesem prozess auf 0 geht, dann ist das gericht fertig zubereitet und es wird auf 100 gesetzt 
            if((stepAnzahl-1)==0){

                //stepanzahl = 100 steht für essen in cooking status
                stepAnzahl = 100;
                
                //Gericht fängt an zu kochen, starte mit der aktuellen zeit
                DateTime startDate = DateTime.Now;

                //FCED wird verändert und gericht auf oven fängt an zu kochen
                data = "Oven;"+ovenFCED[1]+";"+stepAnzahl+";"+ovenFCED[3]+";"+ovenFCED[4]+";"+startDate+";"+ovenFCED[6];

                Debug.Log("essen fängt an zu kochen! [DinnerController]");

                //spielt die Dinner animation ab
                //sperrt weitere handlungen da Animation pflicht ist!
                DinnerAnim.GetComponent<DinnerAnim>().Controller();
            
            //stepanzahl wird reduziert und weiter zubereitet
            }else{

                //FCED wird verändert (nur stepanzahl um -1)
                data = "Oven;"+ovenFCED[1]+";"+(stepAnzahl-1)+";"+ovenFCED[3]+";"+ovenFCED[4]+";"+ovenFCED[5]+";"+ovenFCED[6];

                Debug.Log("Reduce: "+(stepAnzahl-1)+" : "+data+" [DinnerController]");

                //spielt die Dinner animation ab
                //sperrt weitere handlungen da Animation pflicht ist!
                DinnerAnim.GetComponent<DinnerAnim>().Controller();
            }
        }

        //verändere das FCED von dem aktuellen oven
        FloorChildExtraDataController.ChangeFCEDData(data); 

        //nach jeder action muss neu gespeichert werden
        SaveAndLoadController.SavePlayerData();

        return true;
    }

    //erzeugt das anzubereitende dinner auf dem oven
    public static void CreateDinnerPrefabOnOven(string oven, string dinner, int step){

        //suche die koordianten vom oven
        float[] coords = new float[]{GameObject.Find(oven).gameObject.transform.position.x, GameObject.Find(oven).gameObject.transform.position.y};
        //wandelt das dinner in den spritenamen das in den files liegt
        string dinnerName = getDinnerName(dinner);
        float[] dinnerCoords = getDinnerCoords(dinner);
        Sprite dinnerSprite = getSprite(dinnerName);

        //erzeuge das prefab
        GameObject prefab = Instantiate(DinnerOnOvenPrefab, new Vector3(coords[0]+dinnerCoords[0], coords[1]+dinnerCoords[1], 0), Quaternion.identity, DinnerOnOvenHandler.transform);
        prefab.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = GameObject.Find(oven).gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
        prefab.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = dinnerSprite;

        //rendert ingredient auf dinner 
        prefab.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = getIngredientSprite(getIngredientsForDinner(dinner).Split(";")[step-1]);
        prefab.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sortingOrder = GameObject.Find(oven).gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }

    //erzeugt die nächste prepare stufe für das dinner auf dem oven
    public static void UpdateDinnerOnOvenImg(){
        
    }

    //wandelt das dinner in das spritenamen item um
    public static string getDinnerName(string dinner){
        string item = "";

        if(dinner.Equals("Garden Salad")){
            item = "Dinner_18_01";
        }
        if(dinner.Equals("Tomatosoup")){
            item = "Dinner_02_01";
        }
        if(dinner.Equals("Omelette")){
            item = "Dinner_55_01";
        }
        if(dinner.Equals("Mousse au Chocolat")){
            item = "Dinner_20_01";
        }
        if(dinner.Equals("Spaghetti Bolognese")){
            item = "Dinner_06_01";
        }
        if(dinner.Equals("Cheese Plate")){
            item = "Dinner_22_01";
        }
        if(dinner.Equals("Hamburger")){
            item = "Dinner_17_01";
        }
        if(dinner.Equals("Mixed Salad")){
            item = "Dinner_12_01";
        }

        return item;
    }

    //gibt die coords des jeweiligen dinner auf den oven wieder
    public static float[] getDinnerCoords(string dinner){
        float[] coords = null;

        if(dinner.Equals("Garden Salad")){
            coords = new float[]{0.0f, 1.4f};
        }
        if(dinner.Equals("Tomatosoup")){
            coords = new float[]{-0.02f, 1.45f};
        }
        if(dinner.Equals("Omelette")){
            coords = new float[]{0.3f, 1.4f};
        }
        if(dinner.Equals("Mousse au Chocolat")){
            coords = new float[]{-0.03f, 1.875f};
        }
        if(dinner.Equals("Spaghetti Bolognese")){
            coords = new float[]{0.0f, 1.0f};
        }
        if(dinner.Equals("Cheese Plate")){
            coords = new float[]{0.0f, 1.0f};
        }
        if(dinner.Equals("Hamburger")){
            coords = new float[]{0.0f, 1.0f};
        }
        if(dinner.Equals("Mixed Salad")){
            coords = new float[]{0.0f, 1.0f};
        }

        return coords;
    }

    //gibt die liste an zutaten für das dinner wieder um
    //beim zubereiten die richtiger zutat anzuzeigen
    public static string getIngredientsForDinner(string dinner){
        string list = "";

        if(dinner.Equals("Garden Salad")){
            list = "item_02;item_13;item_19";
        }
        if(dinner.Equals("Tomatosoup")){
            list = "";
        }
        if(dinner.Equals("Omelette")){
            list = "";
        }
        if(dinner.Equals("Mousse au Chocolat")){
            list = "";
        }
        if(dinner.Equals("Spaghetti Bolognese")){
            list = "";
        }
        if(dinner.Equals("Cheese Plate")){
            list = "";
        }
        if(dinner.Equals("Hamburger")){
            list = "";
        }
        if(dinner.Equals("Mixed Salad")){
            list = "";
        }

        return list;
    }

    //sucht das passende sprite anhand des namen für das dinner
    public static Sprite getSprite(string item){
        Sprite sprite = null;
        foreach(Sprite obj in sprites){
            if(obj.name.Equals(item)){
                sprite = obj;
            }
        }
        return sprite;
    }

    //sucht das passende sprite anhand des namen für das ingredient
    public static Sprite getIngredientSprite(string item){
        Sprite sprite = null;
        foreach(Sprite obj in ingredientsSprites){
            if(obj.name.Equals(item)){
                sprite = obj;
            }
        }
        return sprite;
    }
}
