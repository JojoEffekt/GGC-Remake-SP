using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinnerUIController : MonoBehaviour
{
    public List<DinnerItem> DinnerItemList = new List<DinnerItem>();

    public void Start(){
        LoadDinnerItems();
        Debug.Log(DinnerItemList[0].name);
        Debug.Log(DinnerItemList[2].name);
    }

    //läd die gleichbleibenden DinnerItems, danach können die counter der jeweiligen gerichte geladen werden
    public void LoadDinnerItems(){
        //                               name       spriteName    type        Dinnertype level                                                                                                                                                          infoIngredients                                                                                 Superzutat                                stars
        //DinnerItemList.Add(new DinnerItem("","","","",0,new Dictionary<string ,string>(){{"number",""},{"time",""},{"moneyPerItem",""},{"xp",""},{"moneyPerItemSI",""},{"xpSI",""}},new Dictionary<string ,int>(){{"",}},new Dictionary<string ,int>(){{"",}},new Dictionary<string ,int>(){{"",},{"",},{"",}}));
        DinnerItemList.Add(new DinnerItem("garden salad","0","type_vegetable","background_dinner",0,new Dictionary<string ,string>(){{"number","10"},{"time","3"},{"moneyPerItem","24"},{"xp","5"},{"moneyPerItemSI","0"},{"xpSI","0"}},new Dictionary<string ,int>(){{"item_02",1},{"item_13",1},{"item_19",1}},new Dictionary<string ,int>(){{"0",0}},new Dictionary<string ,int>(){{"bronze_star_icon",0},{"silver_star_icon",0},{"gold_star_icon",0}}));
        DinnerItemList.Add(new DinnerItem("tomatosoup","0","type_soup","background_dinner",0,new Dictionary<string ,string>(){{"number","40"},{"time","10"},{"moneyPerItem","14"},{"xp","16"},{"moneyPerItemSI","0"},{"xpSI","0"}},new Dictionary<string ,int>(){{"item_02",1},{"item_13",1},{"item_26",1}},new Dictionary<string ,int>(){{"0",0}},new Dictionary<string ,int>(){{"bronze_star_icon",0},{"silver_star_icon",0},{"gold_star_icon",0}}));
        DinnerItemList.Add(new DinnerItem("omelette","0","type_burger","background_dinner",0,new Dictionary<string ,string>(){{"number","5"},{"time","1"},{"moneyPerItem","100"},{"xp","2"},{"moneyPerItemSI","0"},{"xpSI","0"}},new Dictionary<string ,int>(){{"item_23",1}},new Dictionary<string ,int>(){{"0",0}},new Dictionary<string ,int>(){{"bronze_star_icon",0},{"silver_star_icon",0},{"gold_star_icon",0}}));
        DinnerItemList.Add(new DinnerItem("mousse au chocolat","0","type_candy","background_dinner",0,new Dictionary<string ,string>(){{"number","390"},{"time","180"},{"moneyPerItem","4"},{"xp","259"},{"moneyPerItemSI","0"},{"xpSI","0"}},new Dictionary<string ,int>(){{"item_23",1},{"item_29",1},{"item_26",1}},new Dictionary<string ,int>(){{"0",0}},new Dictionary<string ,int>(){{"bronze_star_icon",0},{"silver_star_icon",0},{"gold_star_icon",0}}));
    }
}

//Klasse der einzelnen Gerichte
public class DinnerItem {

    public string name { get; set; }                                //Gerichte Bezeichnung
    public string spriteName { get; set; }                          //gerichte sprite name
    public string typeDinner { get; set; }                          //Main,Vegetable,Burger,Soup,Fruit,Candy   Einordung des Gerichts
    public string type { get; set; }                                //normal,winter,gold                       Zuordnung des Gerichts
    public int counter { get; set; }                                //(gekochte male als zahl)
    public int level { get; set; }                                  //ab welchen level das gericht freigeschalten wird
    public Dictionary<string, string> info { get; set; }            //(kochdauer,portionen,money pro essen, xp bei fertigung, money pro essen bei superzutat, xp bei fertigung bei superzutat)
    public Dictionary<string, int> infoIngredients { get; set; }    //(ZutatenSpriteName:Anzahl,...)           Benötigte zutaten sum kochen
    public Dictionary<string, int> specialIngredient { get; set; }  //Superzutat
    public Dictionary<string, int> star { get; set; }               //(bronze ab:...,silber..,...)

    //instanziiert das object
    public DinnerItem(string name, string spriteName, string typeDinner, string type, int level, Dictionary<string, string> info, Dictionary<string, int> infoIngredients, Dictionary<string, int> specialIngredient, Dictionary<string, int> star){                    
        this.name = name;
        this.spriteName = spriteName;
        this.typeDinner = typeDinner;
        this.type = type;
        this.level = level;
        this.info = info;
        this.infoIngredients = infoIngredients;
        this.specialIngredient = specialIngredient;
        this.star = star;
    }
}