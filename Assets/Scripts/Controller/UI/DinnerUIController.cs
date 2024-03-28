using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinnerUIController : MonoBehaviour
{
    public List<DinnerItem> DinnerItemList = new List<DinnerItem>();

    public void Start(){
        LoadDinnerItems();
        Debug.Log(DinnerItemList[0].name);
    }

    //läd die gleichbleibenden DinnerItems, danach können die counter der jeweiligen gerichte geladen werden
    public void LoadDinnerItems(){
        DinnerItemList.Add(new DinnerItem("test","normal","winter",0,new Dictionary<string ,string>(){{"df","dfgg"}},new Dictionary<string ,int>(){{"df",3}},new Dictionary<string ,int>(){{"df",23}},new Dictionary<string ,int>(){{"df",23}}));
        //CONTINUE
    }
}

//Klasse der einzelnen Gerichte
public class DinnerItem {

    public string name { get; set; }                                //Gerichte Bezeichnung
    public string typeDinner { get; set; }                          //Main,Vegetable,Burger,Soup,Fruit,Candy   Einordung des Gerichts
    public string type { get; set; }                                //normal,winter,gold                       Zuordnung des Gerichts
    public int counter { get; set; }                                //(gekochte male als zahl)
    public int level { get; set; }                                  //ab welchen level das gericht freigeschalten wird
    public Dictionary<string, string> info { get; set; }            //(kochdauer,portionen,money pro essen, xp bei fertigung, money pro essen bei superzutat, xp bei fertigung bei superzutat)
    public Dictionary<string, int> infoIngredients { get; set; }    //(ZutatenSpriteName:Anzahl,...)           Benötigte zutaten sum kochen
    public Dictionary<string, int> specialIngredient { get; set; }  //Superzutat
    public Dictionary<string, int> star { get; set; }               //(bronze ab:...,silber..,...)

    //instanziiert das object
    public DinnerItem(string name, string typeDinner, string type, int level, Dictionary<string, string> info, Dictionary<string, int> infoIngredients, Dictionary<string, int> specialIngredient, Dictionary<string, int> star){                    
        this.name = name;
        this.typeDinner = typeDinner;
        this.type = type;
        this.level = level;
        this.info = info;
        this.infoIngredients = infoIngredients;
        this.specialIngredient = specialIngredient;
        this.star = star;
    }
}