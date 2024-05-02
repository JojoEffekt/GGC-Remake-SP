using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerUIController : MonoBehaviour
{   
    //der eigene shop
    public GameObject PlayerUIShop;

    //referenz auf das MainController GO um in diesen GO das "MainController" script aufzurufen
    public GameObject MainController;


    //prefab für die cerschieden color varianten
    public GameObject ColorBTNPrefab;

    //parent object unter dem alle prefabs erzeugt werden
    public GameObject ColorBTNPrefabHandler;


    //enthält die einzelnen teile des beispielChars
    public GameObject GirlChar;

    //enthält die einzelnen teile des beispielChars
    public GameObject BoyChar;

    
    //beinhaltet alle farben für die hairColor
    public float[,] hairColor = new float[,]{{0.94f,0.87f,0.58f},{0.97f,0.81f,0.35f},{0.97f,0.44f,0.26f},{0.83f,0.56f,0.32f},{0.65f,0.37f,0.29f},{0.40f,0.27f,0.16f},{0.23f,0.14f,0.08f},{0.14f,0.05f,0.02f},{0.80f,0.70f,0.36f},{0.66f,0.54f,0.34f},{0.76f,0.45f,0.22f},{0.57f,0.39f,0.25f},{0.38f,0.27f,0.17f},{0.31f,0.22f,0.12f},{0.23f,0.16f,0.08f}};

    //beinhaltet alle farben für die skinColor
    public float[,] skinColor = new float[,]{{0.94f,0.77f,0.61f},{0.92f,0.68f,0.49f},{0.93f,0.61f,0.41f},{0.69f,0.39f,0.27f},{0.81f,0.53f,0.36f},{0.93f,0.73f,0.58f},{0.92f,0.68f,0.46f},{0.81f,0.58f,0.37f}};

    //beinhaltet alle farben für die tshirtColor
    public float[,] tshirtColor = new float[,]{{0.26f,0.32f,0.31f},{0.77f,0.78f,0.81f},{0.45f,0.70f,0.72f},{0.40f,0.70f,0.41f},{0.95f,0.87f,0.44f},{0.93f,0.72f,0.76f},{1f,0.49f,0.56f},{0.81f,0.55f,0.77f},{0.29f,0.52f,0.45f},{0.33f,0.54f,0.33f},{0.41f,0.64f,0.23f},{0.50f,0.77f,0.64f},{0.52f,0.69f,0.69f},{0.27f,0.69f,0.50f},{0.71f,0.69f,0.68f},{0.75f,0.52f,0.23f},{0.68f,0.36f,0.23f}};

    //beinhaltet alle farben für die hoseColor
    public float[,] hoseColor = new float[,]{{0.26f,0.32f,0.31f},{0.77f,0.78f,0.81f},{0.45f,0.70f,0.72f},{0.40f,0.70f,0.41f},{0.95f,0.87f,0.44f},{0.93f,0.72f,0.76f},{1f,0.49f,0.56f},{0.81f,0.55f,0.77f},{0.29f,0.52f,0.45f},{0.33f,0.54f,0.33f},{0.41f,0.64f,0.23f},{0.50f,0.77f,0.64f},{0.52f,0.69f,0.69f},{0.27f,0.69f,0.50f},{0.71f,0.69f,0.68f},{0.75f,0.52f,0.23f},{0.68f,0.36f,0.23f}};


    //temporäre variablen die die angeklickten werte enthalten
    private bool tempGender; //true=boy, false=girl
    private float[] tempHair;
    private float[] tempSkin;
    private float[] tempTshirt;
    private float[] tempHose;

    
    //öffnet den shop und läd die items
    public void OpenShop(){

        //aktiviert UI
        PlayerUIShop.SetActive(true);

        //rendert den spieler namen in die UI
        PlayerUIShop.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = PlayerController.playerName;
    }



    //für jede verschiedene skinFarbe, render ein gegebenes objekt
    public void RenderAllColorsForCurrentPiece(int num){

        //löscht erstmal alle prefabObjekte die noch vorhanden sein könnten
        DeleteAllPrefabs();

        //Gender Boy
        if(num==1){
            GirlChar.SetActive(false);
            BoyChar.SetActive(true);
            tempGender=true;
        }

        //Gender Girl
        if(num==2){
            GirlChar.SetActive(true);
            BoyChar.SetActive(false);
            tempGender=false;
        }

        //hairColor
        if(num==3){
            for(int a=0;a<hairColor.Length/3;a++){

                //erzeuge für jedes farbtrippel ein farbobject btn
                RenderBtn(a, new float[]{hairColor[a,0],hairColor[a,1],hairColor[a,2]}, num);
            }
        }

        //skinColor
        if(num==4){
            for(int a=0;a<skinColor.Length/3;a++){

                //erzeuge für jedes farbtrippel ein farbobject btn
                RenderBtn(a, new float[]{skinColor[a,0],skinColor[a,1],skinColor[a,2]}, num);
            }
        }

        //tshirtColor
        if(num==5){
            for(int a=0;a<tshirtColor.Length/3;a++){

                //erzeuge für jedes farbtrippel ein farbobject btn
                RenderBtn(a, new float[]{tshirtColor[a,0],tshirtColor[a,1],tshirtColor[a,2]}, num);
            }
        }

        //hoseColor
        if(num==6){
            for(int a=0;a<hoseColor.Length/3;a++){

                //erzeuge für jedes farbtrippel ein farbobject btn
                RenderBtn(a, new float[]{hoseColor[a,0],hoseColor[a,1],hoseColor[a,2]}, num);
            }
        }
    }

    //erzeugt die möglichen ColorBtns des jeweiligen körperteils
    private void RenderBtn(int BtnNum, float[] color, int type){

        //rendert color Object an die entsprechende stelle
        if(BtnNum>10){
            GameObject Btn = Instantiate(ColorBTNPrefab, new Vector2(1300, 740+((BtnNum-11)*-47)), Quaternion.identity, ColorBTNPrefabHandler.transform);
            
            //rendert farbe als object
            Btn.transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color(color[0],color[1],color[2],1);

            //delegiert den zu übergebenden wert bei kauf
            Btn.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(delegate{SelectBtn(color,type);});
        }else{
            GameObject Btn = Instantiate(ColorBTNPrefab, new Vector2(1250, 740+(BtnNum*-47)), Quaternion.identity, ColorBTNPrefabHandler.transform);
            
            //rendert farbe als object
            Btn.transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color(color[0],color[1],color[2],1);

            //delegiert den zu übergebenden wert bei kauf
            Btn.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(delegate{SelectBtn(color,type);});
        }
    }

    //wählt zufällige werte aus
    public void GenerateRndmChar(){

        //generiere eine zufällige zahl für jedes körperteil und deren anzahl an colors
        System.Random rnd = new System.Random();

        //erzeugt eine random zahl und übergibt von der ColorListe den farb wert und erzeugt die farbe für das körperteil
        int hairColorRndm = rnd.Next(0, hairColor.Length/3);
        SelectBtn(new float[]{hairColor[hairColorRndm,0],hairColor[hairColorRndm,1],hairColor[hairColorRndm,2]},3);

        int skinColorRndm = rnd.Next(0, skinColor.Length/3);
        SelectBtn(new float[]{skinColor[skinColorRndm,0],skinColor[skinColorRndm,1],skinColor[skinColorRndm,2]},4);

        int tshirtColorRndm = rnd.Next(0, tshirtColor.Length/3);
        SelectBtn(new float[]{tshirtColor[tshirtColorRndm,0],tshirtColor[tshirtColorRndm,1],tshirtColor[tshirtColorRndm,2]},5);

        int hoseColorRndm = rnd.Next(0, hoseColor.Length/3);
        SelectBtn(new float[]{hoseColor[hoseColorRndm,0],hoseColor[hoseColorRndm,1],hoseColor[hoseColorRndm,2]},6);
    }

    //die funktion die benutzt wird um die farbe eines körperteils zu ändern
    //ändert das pre layout
    private void SelectBtn(float[] color, int type){
        
        //hairColor
        if(type==3){
            GirlChar.transform.GetChild(2).gameObject.GetComponent<Image>().color = new Color(color[0],color[1],color[2],1f);
            BoyChar.transform.GetChild(6).gameObject.GetComponent<Image>().color = new Color(color[0],color[1],color[2],1f);
            tempHair=color;
        }

        //skinColor
        if(type==4){
            GirlChar.transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color(color[0],color[1],color[2],1f);
            BoyChar.transform.GetChild(2).gameObject.GetComponent<Image>().color = new Color(color[0],color[1],color[2],1f);
            tempSkin=color;
        }

        //tshirtColor
        if(type==5){
            GirlChar.transform.GetChild(7).gameObject.GetComponent<Image>().color = new Color(color[0],color[1],color[2],1f);
            BoyChar.transform.GetChild(4).gameObject.GetComponent<Image>().color = new Color(color[0],color[1],color[2],1f);
            tempTshirt=color;
        }

        //hoseColor
        if(type==6){
            GirlChar.transform.GetChild(6).gameObject.GetComponent<Image>().color = new Color(color[0],color[1],color[2],1f);
            BoyChar.transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color(color[0],color[1],color[2],1f);
            tempHose=color;
        }
    }

    //bestätige die auswahl
    //übergebe die neuen werte an den spieler char
    public void ConfirmNewDress(){

        //überprüfe ob der spieler genug gold hat
        if(PlayerController.playerGold>0){

            //bool um zu gucken ob überhaupt was angeklickt wurde (prevent unwanted spending)
            bool isDressChange = false;

            //rechne ab
            PlayerController.playerGold = PlayerController.playerGold - 1;

            //schließe shop
            CloseShop();

            //übergebe die neuen daten dem spielerCharbuilder
            PlayerCharBuilder.player.setGender(tempGender);

            if(tempHair!=null){
                PlayerCharBuilder.player.setHair(tempHair[0],tempHair[1],tempHair[2],1);
                isDressChange=true;
            }
            if(tempSkin!=null){
                PlayerCharBuilder.player.setSkin(tempSkin[0],tempSkin[1],tempSkin[2],1);
                isDressChange=true;
            }
            if(tempTshirt!=null){
                PlayerCharBuilder.player.setTshirt(tempTshirt[0],tempTshirt[1],tempTshirt[2],1);
                isDressChange=true;
            }
            if(tempHose!=null){
                PlayerCharBuilder.player.setLeg(tempHose[0],tempHose[1],tempHose[2],1);
                isDressChange=true;
            }

            //rechne ab
            if(isDressChange==true){
                PlayerController.playerGold = PlayerController.playerGold - 1;

                //nach jeder action muss neu gespeichert werden
                SaveAndLoadController.SavePlayerData();
            }
        }
    }

    //löscht alle objecte in den PrefabHandler
    private void DeleteAllPrefabs(){
        for(int a=ColorBTNPrefabHandler.transform.childCount-1;a>=0;a--){
            Destroy(ColorBTNPrefabHandler.transform.GetChild(a).gameObject);
        }
    }

    //schließt den shop wieder und aktiviert damit wieder die anderen UI elemente
    public void CloseShop(){

        //löscht erstmal alle prefabObjekte die noch vorhanden sein könnten
        DeleteAllPrefabs();

        //schließt die UI
        PlayerUIShop.SetActive(false);

        //aktiviert die anderen btns
        MainController.GetComponent<MainController>().ActivateBTNs();//aktiviere alle main btns
    }
}
