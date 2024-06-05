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

    //var um den delay für die überprüfung des dinnerstates abzufragen
    public float timeDelay = 0f;

    //bei der initialisierung des scripts wird die referenz geholt
    public void Awake(){
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


    public void Update(){
        
        //Überprüfe die zubereitungszeit der dinner und aktuallisiere die bilder
        //FCED die die stepstufe auf 100 haben sind im cooking modus und müssen aktuallisiert werden
        timeDelay = timeDelay + Time.deltaTime;
        if(timeDelay>=1.0f){
            timeDelay = 0.0f;

            //fetcht FCED oven data with dinner on oven
            List<string> ovenDinnerList = FloorChildExtraDataController.getFCEDFromTyp("Oven", "");

            //für jeden listeintrag, prüfe ob dinner bestimmte locale prozentuale zeiteinheit überschritten hat um neue UI zu laden
            for(int a=0;a<ovenDinnerList.Count;a++){
                string[] item = ovenDinnerList[a].Split(";");

                //nehme die startzeit des dinners und berechne die prozentuale zubereitungszeit
                DateTime startDate0 = DateTime.Parse(item[5]); // ab 0% 
                DateTime startDate1 = startDate0.AddMinutes(Int32.Parse(item[6])*0.33f); //ab 33%
                DateTime startDate2 = startDate0.AddMinutes(Int32.Parse(item[6])*0.66f); //ab 66% 
                DateTime endDate0 = startDate0.AddMinutes(Int32.Parse(item[6])); // ab 100% ready
                DateTime endDate1 = startDate0.AddMinutes(Int32.Parse(item[6])*2.0f); //ab 200% verdorrt

                //rechne die prozentuale zeit vom start bis zum ende aus und rendert die dinner
                //0-33%
                if(DateTime.Now>startDate0&&DateTime.Now<startDate1){
                    UpdateDinnerOnOven(item[1]+"-Child-Dinner",getDinnerName(item[3]),1);
                //33-66%
                }else if(DateTime.Now>startDate1&&DateTime.Now<startDate2){
                    UpdateDinnerOnOven(item[1]+"-Child-Dinner",getDinnerName(item[3]),2);
                //66-99%
                }else if(DateTime.Now>startDate2&&DateTime.Now<endDate0){
                    UpdateDinnerOnOven(item[1]+"-Child-Dinner",getDinnerName(item[3]),3);
                //100-200%
                }else if(DateTime.Now>endDate0&&DateTime.Now<endDate1){
                    UpdateDinnerOnOven(item[1]+"-Child-Dinner",getDinnerName(item[3]),4);
                //>200%
                }else if(DateTime.Now>endDate1){
                    UpdateDinnerOnOven(item[1]+"-Child-Dinner",getDinnerName(item[3]),5);
                }
            }
        }
    }

    //rendert die nächste dinnerstufe anhand der vergangenen zeit die das gericht auf dem oven ist
    public bool UpdateDinnerOnOven(string oven, string dinner, int step){
        //baut den dinnernamen und holt den string
        Debug.Log("update: "+oven+" zu dinner: "+dinner+" ["+step+"]");
        Sprite dinnerSprite = getSprite(dinner.Substring(0,11)+step);
        GameObject.Find(oven).transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = dinnerSprite;
        return true;
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
    //überprüft verschiendene szenarios des dinners sobald der spieler am oven steht
    public static bool ReduceStepCount_UI(){

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
                //Gucke nach allen Tresen die das gleiche essen beinhalten oder leer sind
                //gibt all tresennamen (objName) als string zurück
                string[] tresen = TresenController.getAllTresenForDinner(ovenFCED[3]);

                /*
                gucke ob ein tresen bereits das gleiche essen beinhaltet oder wenn NICHT ein tresen leer ist
                gucke ob spieler zum tresen hinlaufen kann
                -> lösche dinner von oven & FCED, rechne auf tresen FCED
                gehe zum tresen 
                platziere dinnerUI auf tresen 
                wenn tresen vorher leer war, schalte frei das mitarbeiter essen nehmen können
                */

                //lösche das dinner
                //DeleteDinnerPrefabOnOven(ovenFCED[1]+"-Child-Dinner");
                //TO TEST!!! muss dinner auf oven löschen und neues dinner kann darauf platziert wrerden sowie FCED eintrag löschung
                FloorChildExtraDataController.DeleteFCED(ovenFCED[1]);

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

                //zerstöre das letzte ingredient auf den oven
                DeleteIngredientOnDinner((ovenFCED[1]+"-Child-Dinner"), ovenFCED[3]);

                //spielt die Dinner animation ab
                //sperrt weitere handlungen da Animation pflicht ist!
                DinnerAnim.GetComponent<DinnerAnim>().Controller();
            
            //stepanzahl wird reduziert und weiter zubereitet
            }else{

                //FCED wird verändert (nur stepanzahl um -1)
                data = "Oven;"+ovenFCED[1]+";"+(stepAnzahl-1)+";"+ovenFCED[3]+";"+ovenFCED[4]+";"+ovenFCED[5]+";"+ovenFCED[6];

                Debug.Log("Reduce: "+(stepAnzahl-1)+" : "+data+" [DinnerController]");

                //verändere das ingredient auf dem oven
                UpdateDinnerOnOvenImg((ovenFCED[1]+"-Child-Dinner"), ovenFCED[3], stepAnzahl);

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

    //lösche das dinner auf den oven
    public static bool DeleteDinnerPrefabOnOven(string oven){
        GameObject objToDelete = GameObject.Find(oven);
        if(objToDelete!=null){
            DestroyImmediate(objToDelete);
            return true;
        }
        return false;
    }


    //erzeugt ein neues Dinner auf den replacten oven
    //wird bei ButtonController.MoveObjectOnFloor aufgerufen wenn ein floorObj replaced wird
    public static bool ReadjustAllDinnerPrefabsOnOven(){

        //hole das FCED vom replatzierten oven
        string[] OvenFCED = FloorChildExtraDataController.FindMovedOvenFCED().Split(";");

        //wenn das replacte object kein dinner enthält, breche ab
        if(OvenFCED[0].Equals("")){
            return false;
        }

        //da das alte dinner beim replacen gelöscht wird, erzeuge ein neues
        CreateDinnerPrefabOnOven((OvenFCED[1]+"-Child"), OvenFCED[3], Int32.Parse(OvenFCED[2]));

        return true;
    }

    //erzeugt das anzubereitende dinner auf dem oven
    public static void CreateDinnerPrefabOnOven(string oven, string dinner, int step){
        Debug.Log("CreateDinnerPrefabOnOven: "+oven+" "+dinner+" "+step);

        //suche die koordianten vom oven
        float[] coords = new float[]{GameObject.Find(oven).gameObject.transform.position.x, GameObject.Find(oven).gameObject.transform.position.y};
        //wandelt das dinner in den spritenamen das in den files liegt
        string dinnerName = getDinnerName(dinner);
        float[] dinnerCoords = getDinnerCoords(dinner);
        Sprite dinnerSprite = getSprite(dinnerName);

        //erzeuge das prefab
        GameObject prefab = Instantiate(DinnerOnOvenPrefab, new Vector3(coords[0]+dinnerCoords[0], coords[1]+dinnerCoords[1], 0), Quaternion.identity, DinnerOnOvenHandler.transform);
        prefab.name = oven+"-Dinner";
        prefab.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = GameObject.Find(oven).gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
        prefab.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = dinnerSprite;

        //rendert ingredient auf dinner 
        if(step!=100){
            prefab.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = getIngredientSprite(getIngredientsForDinner(dinner).Split(";")[step-1]);
            prefab.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sortingOrder = GameObject.Find(oven).gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
        }
    }

    //erzeugt die nächste prepare stufe für das dinner auf dem oven
    public static void UpdateDinnerOnOvenImg(string oven, string dinner, int step){
        
        //suche den das prefab auf  dem oven anhand des oven floorchild namen
        GameObject prefab = GameObject.Find(oven);

        //rendert ingredient auf dinner 
        prefab.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = getIngredientSprite(getIngredientsForDinner(dinner).Split(";")[step-2]);
    }

    //entfert das ingedient von der dinner zubereitung auf dem oven, wenn alle ingredients verbraucht wurden um das gericht zu kochen
    public static void DeleteIngredientOnDinner(string oven, string dinner){
        
        //suche den das prefab auf  dem oven anhand des oven floorchild namen
        GameObject prefab = GameObject.Find(oven);

        //rendert ingredient auf dinner 
        prefab.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = null;
    }

    //wandelt das dinner in das spritenamen item um
    public static string getDinnerName(string dinner){
        string item = "";

        if(dinner.Equals("Christmas Roast")){
            item = "Dinner_45_04";
        }
        if(dinner.Equals("Fruit Cake")){
            item = "Dinner_41_04";
        }
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
        if(dinner.Equals("Toast Hawai")){
            item = "Dinner_03_01";
        }
        if(dinner.Equals("Mexican Salad")){
            item = "Dinner_14_01";
        }
        if(dinner.Equals("Pasta Salad")){
            item = "Dinner_10_01";
        }
        if(dinner.Equals("Stuffed Peppers")){
            item = "Dinner_04_01";
        }
        if(dinner.Equals("Potato Soup")){
            item = "Dinner_01_01";
        }
        if(dinner.Equals("Black Forest Cake")){
            item = "Dinner_54_01";
        }
        if(dinner.Equals("Duck a l'Orange")){
            item = "Dinner_44_01";
        }
        if(dinner.Equals("Potato Salad")){
            item = "Dinner_08_01";
        }
        if(dinner.Equals("Labskaus")){
            item = "Dinner_07_01";
        }
        if(dinner.Equals("Minestrone")){
            item = "Dinner_13_01";
        }
        if(dinner.Equals("Rhubarb Compote")){
            item = "Dinner_05_01";
        }
        if(dinner.Equals("Nasi Goreng")){
            item = "Dinner_11_01";
        }
        if(dinner.Equals("Cheese Fondue")){
            item = "Dinner_21_01";
        }
        if(dinner.Equals("Baked Potatoes")){
            item = "Dinner_19_01";
        }
        if(dinner.Equals("Mac'n'Cheese")){
            item = "Dinner_15_01";
        }
        if(dinner.Equals("Pizza")){
            item = "Dinner_09_01";
        }
        if(dinner.Equals("White Chocolate Cheesecake")){
            item = "Dinner_25_01";
        }
        if(dinner.Equals("Tofuburger")){
            item = "Dinner_26_01";
        }
        if(dinner.Equals("Lasagne")){
            item = "Dinner_16_01";
        }
        if(dinner.Equals("Cheeseburger")){
            item = "Dinner_23_01";
        }
        if(dinner.Equals("Cherry Pie")){
            item = "Dinner_29_01";
        }
        if(dinner.Equals("Strawberry mousse")){
            item = "Dinner_28_01";
        }
        if(dinner.Equals("Jam Sponge Pudding")){
            item = "Dinner_39_01";
        }
        if(dinner.Equals("Sushi")){
            item = "Dinner_27_01";
        }
        if(dinner.Equals("Chocolate Fondue")){
            item = "Dinner_46_01";
        }
        if(dinner.Equals("Berry Compote")){
            item = "Dinner_32_01";
        }
        if(dinner.Equals("Chocolate Cake")){
            item = "Dinner_48_01";
        }
        if(dinner.Equals("Strawberry Cake")){
            item = "Dinner_29_01";
        }
        if(dinner.Equals("Pizza Tonno")){
            item = "Dinner_34_01";
        }
        if(dinner.Equals("Chicken Dish")){
            item = "Dinner_50_01";
        }
        if(dinner.Equals("Cheese Cake New York")){
            item = "Dinner_37_01";
        }
        if(dinner.Equals("Roast Chicken")){
            item = "Dinner_31_01";
        }
        if(dinner.Equals("Pea Soup")){
            item = "Dinner_36_01";
        }
        if(dinner.Equals("Fish'n'Chips")){
            item = "Dinner_42_01";
        }
        if(dinner.Equals("Roasted Pork")){
            item = "Dinner_33_01";
        }
        if(dinner.Equals("Sandwich")){
            item = "Dinner_30_01";
        }
        if(dinner.Equals("Waffle With Warm Cherries")){
            item = "Dinner_40_01";
        }
        if(dinner.Equals("White Chocolate Mousse")){
            item = "Dinner_24_01";
        }
        if(dinner.Equals("Chickenburger")){
            item = "Dinner_51_01";
        }
        if(dinner.Equals("Chilli con Carne")){
            item = "Dinner_49_01";
        }
        if(dinner.Equals("Cherry Compote")){
            item = "Dinner_52_01";
        }
        if(dinner.Equals("Babka")){
            item = "Dinner_38_01";
        }
        if(dinner.Equals("Pizza Hawaii")){
            item = "Dinner_35_01";
        }
        if(dinner.Equals("Chocolate Cheesecake")){
            item = "Dinner_47_01";
        }

        return item;
    }

    //gibt die coords des jeweiligen dinner auf den oven wieder
    public static float[] getDinnerCoords(string dinner){
        float[] coords = null;

        if(dinner.Equals("Christmas Roast")){
            coords = new float[]{0.0f, 1.4f};
        }
        if(dinner.Equals("Fruit Cake")){
            coords = new float[]{0.0f, 1.4f};
        }
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
            coords = new float[]{0.0f, 1.755f};
        }
        if(dinner.Equals("Cheese Plate")){
            coords = new float[]{-0.074f, 1.147f};
        }
        if(dinner.Equals("Hamburger")){
            coords = new float[]{0.228f, 1.368f};
        }
        if(dinner.Equals("Mixed Salad")){
            coords = new float[]{0.0f, 1.48f};
        }
        if(dinner.Equals("Toast Hawai")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Mexican Salad")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Pasta Salad")){
            coords = new float[]{-0.1f, 1.5f};
        }
        if(dinner.Equals("Stuffed Peppers")){
            coords = new float[]{0.01f, 1.15f};
        }
        if(dinner.Equals("Potato Soup")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Black Forest Cake")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Duck a l'Orange")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Potato Salad")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Labskaus")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Minestrone")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Rhubarb Compote")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Nasi Goreng")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Cheese Fondue")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Baked Potatoes")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Mac'n'Cheese")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Pizza")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("White Chocolate Cheesecake")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Tofuburger")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Lasagne")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Cheeseburger")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Cherry Pie")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Strawberry mousse")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Jam Sponge Pudding")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Sushi")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Chocolate Fondue")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Berry Compote")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Chocolate Cake")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Strawberry Cake")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Pizza Tonno")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Chicken Dish")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Cheese Cake New York")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Roast Chicken")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Pea Soup")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Fish'n'Chips")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Roasted Pork")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Sandwich")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Waffle With Warm Cherries")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("White Chocolate Mousse")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Chickenburger")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Chilli con Carne")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Cherry Compote")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Babka")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Pizza Hawaii")){
            coords = new float[]{0.3f, 1.3f};
        }
        if(dinner.Equals("Chocolate Cheesecake")){
            coords = new float[]{0.3f, 1.3f};
        }

        return coords;
    }

    //gibt die liste an zutaten für das dinner wieder um
    //beim zubereiten die richtiger zutat anzuzeigen
    public static string getIngredientsForDinner(string dinner){
        string list = "";

        if(dinner.Equals("Christmas Roast")){
            list = "item_40;item_40;item_39;item_39;item_39;item_39";
        }
        if(dinner.Equals("Fruit Cake")){
            list = "item_54;item_54;item_54;item_43;item_43;item_43";
        }
        if(dinner.Equals("Garden Salad")){
            list = "item_02;item_13;item_19";
        }
        if(dinner.Equals("Tomatosoup")){
            list = "item_02;item_13;item_26";
        }
        if(dinner.Equals("Omelette")){
            list = "item_23";
        }
        if(dinner.Equals("Mousse au Chocolat")){
            list = "item_23;item_29;item_26";
        }
        if(dinner.Equals("Spaghetti Bolognese")){
            list = "item_11;item_17;item_02";
        }
        if(dinner.Equals("Cheese Plate")){
            list = "item_32;item_32;item_32";
        }
        if(dinner.Equals("Hamburger")){
            list = "item_17;item_02;item_22";
        }
        if(dinner.Equals("Mixed Salad")){
            list = "item_02;item_03;item_10;item_19";
        }
        if(dinner.Equals("Toast Hawai")){
            list = "item_32;item_09;item_22";
        }
        if(dinner.Equals("Mexican Salad")){
            list = "item_34;item_03;item_13;item_36";
        }
        if(dinner.Equals("Pasta Salad")){
            list = "item_02;item_11;item_23;item_14";
        }
        if(dinner.Equals("Stuffed Peppers")){
            list = "item_23;item_06;item_37";
        }
        if(dinner.Equals("Potato Soup")){
            list = "item_02;item_13;item_08";
        }
        if(dinner.Equals("Black Forest Cake")){
            list = "item_29;item_26;item_22;item_50";
        }
        if(dinner.Equals("Duck a l'Orange")){
            list = "item_45;item_48;item_53;item_13";
        }
        if(dinner.Equals("Potato Salad")){
            list = "item_08;item_14;item_25";
        }
        if(dinner.Equals("Labskaus")){
            list = "item_18;item_25;item_33;item_23";
        }
        if(dinner.Equals("Minestrone")){
            list = "item_11;item_02;item_24";
        }
        if(dinner.Equals("Rhubarb Compote")){
            list = "item_07;item_04";
        }
        if(dinner.Equals("Nasi Goreng")){
            list = "item_31;item_09;item_06;item_13;item_23";
        }
        if(dinner.Equals("Cheese Fondue")){
            list = "item_32;item_21;item_22";
        }
        if(dinner.Equals("Baked Potatoes")){
            list = "item_08;item_13;item_36";
        }
        if(dinner.Equals("Mac'n'Cheese")){
            list = "item_11;item_32;item_26";
        }
        if(dinner.Equals("Pizza")){
            list = "item_32;item_02";
        }
        if(dinner.Equals("White Chocolate Cheesecake")){
            list = "item_26;item_46;item_49;item_44;item_38";
        }
        if(dinner.Equals("Tofuburger")){
            list = "item_19;item_02;item_41;item_29";
        }
        if(dinner.Equals("Lasagne")){
            list = "item_32;item_17;item_02;item_11";
        }
        if(dinner.Equals("Cheeseburger")){
            list = "item_32;item_22;item_17;item_02";
        }
        if(dinner.Equals("Cherry Pie")){
            list = "item_26;item_22;item_04;item_51";
        }
        if(dinner.Equals("Strawberry mousse")){
            list = "item_04;item_26;item_23;item_42";
        }
        if(dinner.Equals("Jam Sponge Pudding")){
            list = "item_46;item_42;item_04";
        }
        if(dinner.Equals("Sushi")){
            list = "item_06;item_47;item_25";
        }
        if(dinner.Equals("Chocolate Fondue")){
            list = "item_29;item_26;item_42";
        }
        if(dinner.Equals("Berry Compote")){
            list = "item_04;item_51;item_42;item_07";
        }
        if(dinner.Equals("Chocolate Cake")){
            list = "item_22;item_29;item_26;item_23";
        }
        if(dinner.Equals("Strawberry Cake")){
            list = "item_42;item_22;item_26";
        }
        if(dinner.Equals("Pizza Tonno")){
            list = "item_32;item_22;item_13;item_02;item_47";
        }
        if(dinner.Equals("Chicken Dish")){
            list = "item_36;item_31;item_13;item_11";
        }
        if(dinner.Equals("Cheese Cake New York")){
            list = "item_44;item_49;item_04;item_26";
        }
        if(dinner.Equals("Roast Chicken")){
            list = "item_31;item_53;item_53;item_13;item_13;item_13";
        }
        if(dinner.Equals("Pea Soup")){
            list = "item_13;item_36;item_10;item_10;item_10;item_21";
        }
        if(dinner.Equals("Fish'n'Chips")){
            list = "item_47;item_08;item_22;item_14;item_14;item_14;item_14;item_14;item_10";
        }
        if(dinner.Equals("Roasted Pork")){
            list = "item_18;item_18;item_52;item_52;item_52;item_52;item_13;item_21;item_21";
        }
        if(dinner.Equals("Sandwich")){
            list = "item_36;item_32;item_22;item_02;item_53";
        }
        if(dinner.Equals("Waffle With Warm Cherries")){
            list = "item_22;item_46;item_51;item_51;item_23;item_26";
        }
        if(dinner.Equals("White Chocolate Mousse")){
            list = "item_38;item_26;item_23";
        }
        if(dinner.Equals("Chickenburger")){
            list = "item_31;item_25;item_22;item_02";
        }
        if(dinner.Equals("Chilli con Carne")){
            list = "item_36;item_34;item_17;item_13;item_02";
        }
        if(dinner.Equals("Cherry Compote")){
            list = "item_04;item_51";
        }
        if(dinner.Equals("Babka")){
            list = "item_29;item_22;item_04;item_53";
        }
        if(dinner.Equals("Pizza Hawaii")){
            list = "item_36;item_32;item_22;item_09;item_02";
        }
        if(dinner.Equals("Chocolate Cheesecake")){
            list = "item_29;item_46;item_49;item_44";
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
