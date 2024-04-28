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
    

    public float[,] skinColor = new float[,]{{0.94f,0.77f,0.61f},{0.92f,0.68f,0.49f}};

    //wird durch anklicken des jeweiligen "körperteils" verändert (0=null,1=boy,2=girl,3=hair,4=skin,..)
    public int currentPlayerPiece = 0;
    
    //öffnet den shop und läd die items
    public void OpenShop(){
        PlayerUIShop.SetActive(true);
    }



    //für jede verschiedene skinFarbe, render ein gegebenes objekt
    public void RenderAllColorsForCurrentPiece(int num){

        //skinColor
        if(num==4){
            for(int a=0;a<skinColor.Length/3;a++){

                //erzeuge für jedes farbtrippel ein farbobject btn
                RenderBtn(a, new float[]{skinColor[a,0],skinColor[a,1],skinColor[a,2]});
            }
        }
    }

    private void RenderBtn(int BtnNum, float[] color){
        GameObject Btn = Instantiate(ColorBTNPrefab, new Vector2(1250, 740+(BtnNum*-47)), Quaternion.identity, ColorBTNPrefabHandler.transform);
        Btn.transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color(color[0],color[1],color[2],1);
    }

    //schließt den shop wieder und aktiviert damit wieder die anderen UI elemente
    public void CloseShop(){

        //schließt die UI
        PlayerUIShop.SetActive(false);

        //aktiviert die anderen btns
        MainController.GetComponent<MainController>().ActivateBTNs();//aktiviere alle main btns
    }
}
