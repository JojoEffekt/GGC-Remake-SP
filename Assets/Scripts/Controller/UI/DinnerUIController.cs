using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DinnerUIController : MonoBehaviour
{
    //referenz für die instanziierung des Dinnerprefabs für das Dinnerbuch
    public GameObject DinnerPrefab;

    //referenz auf das Gameobject unter dem die dynamischen DinnerItems generiert werden
    public GameObject ItemController;

    //enthält die aktuell verfügbaren Dinner (nicht die  zeitlich nicht verfügbaren)
    public List<DinnerItem> DinnerItemList = new List<DinnerItem>(); 

    //enthält dinner bilder
    public List<Sprite> SpriteList = new List<Sprite>();

    //enthält Typedinner und background bilder
    public List<Sprite> UISpriteList = new List<Sprite>();

    //enthält Ingredients
    public List<Sprite> IngredientsSpriteList = new List<Sprite>();

    public void Start(){
        LoadDinnerItems();

        InstantiateItem(DinnerItemList[0], 0);
        InstantiateItem(DinnerItemList[1], 1);
        InstantiateItem(DinnerItemList[2], 2);
        InstantiateItem(DinnerItemList[3], 3);
    }

    //Instanzierert das item anhand der nummer in dem DinnerBuch
    public void InstantiateItem(DinnerItem item, int position){

        //Generiert das DinnerItem an einer der 4 möglichen stellen der BuchSeite
        if(position<2){                           //pos ist broke, idk, idc
            Instantiate(DinnerPrefab, new Vector2((position*500)+700, 720), Quaternion.identity, ItemController.transform);
        }else{
            Instantiate(DinnerPrefab, new Vector2(((position-2)*500)+700, 370), Quaternion.identity, ItemController.transform);
        } 

        //referenz auf das aktuelle prefab
        GameObject prefab = ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject;

        //lade item bild
        for(int a=0;a<SpriteList.Count;a++){
            if(item.spriteName.Equals(SpriteList[a].name)){
                //generiert das bild
                prefab.transform.GetChild(0).gameObject.transform.GetChild(6).gameObject.GetComponent<Image>().overrideSprite = SpriteList[a];

                //setzt die resolution auf die image größe
                prefab.transform.GetChild(0).gameObject.transform.GetChild(6).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2 (SpriteList[a].rect.width, SpriteList[a].rect.height);
            }
        }

        //lade den item type
    	for(int b=0;b<UISpriteList.Count;b++){
            if(item.typeDinner.Equals(UISpriteList[b].name)){
                //generiert den DinneType
                prefab.transform.GetChild(0).gameObject.transform.GetChild(7).gameObject.GetComponent<Image>().overrideSprite = UISpriteList[b];
            }
        }

        //ladet die ingredient list
        //bestimmt den slot des jeweiligen items
        int counter = 8;
    	for(int c=0;c<IngredientsSpriteList.Count;c++){
            foreach(var ingredient in item.infoIngredients){
                if(ingredient.Key.Equals(IngredientsSpriteList[c].name)){
                    //generiert das ingredient
                    prefab.transform.GetChild(0).gameObject.transform.GetChild(counter).gameObject.SetActive(true);
                    prefab.transform.GetChild(0).gameObject.transform.GetChild(counter).gameObject.GetComponent<Image>().overrideSprite = IngredientsSpriteList[c];

                    //generiert die stückanzahl
                    prefab.transform.GetChild(2).gameObject.transform.GetChild(counter-3).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = ""+ingredient.Value;
                    counter ++;
                }
            }
        }

        //generiert den Dinner namen
        prefab.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = item.name;

        //generiert die postionen anzahl
        prefab.transform.GetChild(2).gameObject.transform.GetChild(1).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "portions: " + item.info["number"];

        //generiert die dauer des gerichts
        //baut die minutenanzahl
        string minute = ""+Int32.Parse(item.info["time"])%60;
        if(minute.Length==0){
            minute = "00";
        }else if(minute.Length==1){
            minute = "0"+minute;
        }
        //baut die stundenanzahl
        string stunde = ""+Int32.Parse(item.info["time"])/60;
        if(stunde.Length==0){
            stunde = "00";
        }else if(stunde.Length==1){
            stunde = "0"+stunde;
        }
        string dauer = stunde + ":" + minute;
        prefab.transform.GetChild(2).gameObject.transform.GetChild(2).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "ready in: " + dauer + ":00";

        //money
        prefab.transform.GetChild(2).gameObject.transform.GetChild(3).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = item.info["moneyPerItem"];

        //xp
        prefab.transform.GetChild(2).gameObject.transform.GetChild(4).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = item.info["xp"];
    }

    //läd die gleichbleibenden DinnerItems, danach können die counter der jeweiligen gerichte geladen werden
    public void LoadDinnerItems(){
        //                               name       spriteName    type       Dinnertype    level                                                                                                            infoIngredients                           Superzutat                                stars
        //DinnerItemList.Add(new DinnerItem("","","","",0,new Dictionary<string ,string>(){{"number",""},{"time",""},{"moneyPerItem",""},{"xp",""},{"moneyPerItemSI",""},{"xpSI",""}},new Dictionary<string ,int>(){{"",}},new Dictionary<string ,int>(){{"",}},new Dictionary<string ,int>(){{"",},{"",},{"",}}));
        DinnerItemList.Add(new DinnerItem("Garden Salad","Dinner_18_04","type_vegetable","background_dinner",0,new Dictionary<string ,string>(){{"number","10"},{"time","3"},{"moneyPerItem","24"},{"xp","5"},{"moneyPerItemSI","0"},{"xpSI","0"}},new Dictionary<string ,int>(){{"item_02",1},{"item_13",1},{"item_19",1}},new Dictionary<string ,int>(){{"0",0}},new Dictionary<string ,int>(){{"bronze_star_icon",0},{"silver_star_icon",0},{"gold_star_icon",0}}));
        DinnerItemList.Add(new DinnerItem("Tomatosoup","Dinner_02_04","type_soup","background_dinner",0,new Dictionary<string ,string>(){{"number","40"},{"time","10"},{"moneyPerItem","14"},{"xp","16"},{"moneyPerItemSI","0"},{"xpSI","0"}},new Dictionary<string ,int>(){{"item_02",1},{"item_13",1},{"item_26",1}},new Dictionary<string ,int>(){{"0",0}},new Dictionary<string ,int>(){{"bronze_star_icon",0},{"silver_star_icon",0},{"gold_star_icon",0}}));
        DinnerItemList.Add(new DinnerItem("Omelette","Dinner_55_04","type_burger","background_dinner",0,new Dictionary<string ,string>(){{"number","5"},{"time","1"},{"moneyPerItem","100"},{"xp","2"},{"moneyPerItemSI","0"},{"xpSI","0"}},new Dictionary<string ,int>(){{"item_23",1}},new Dictionary<string ,int>(){{"0",0}},new Dictionary<string ,int>(){{"bronze_star_icon",0},{"silver_star_icon",0},{"gold_star_icon",0}}));
        DinnerItemList.Add(new DinnerItem("Mousse au Chocolat","Dinner_20_04","type_candy","background_dinner",0,new Dictionary<string ,string>(){{"number","390"},{"time","180"},{"moneyPerItem","4"},{"xp","259"},{"moneyPerItemSI","0"},{"xpSI","0"}},new Dictionary<string ,int>(){{"item_23",1},{"item_29",1},{"item_26",1}},new Dictionary<string ,int>(){{"0",0}},new Dictionary<string ,int>(){{"bronze_star_icon",0},{"silver_star_icon",0},{"gold_star_icon",0}}));

        //läd die referenzbilder
        object[] sprites = Resources.LoadAll("Textures/UI/Dinner_",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	SpriteList.Add((Sprite)sprites[x]);
        }

        //läd die referenzbilder aller DinnerUI bilder
        object[] spritesUI = Resources.LoadAll("Textures/UI/Dinner",typeof(Sprite));
        for(int x=0;x<spritesUI.Length;x++){
           	UISpriteList.Add((Sprite)spritesUI[x]);
        }

        //läd die referenzbilder von den Ingredients
        object[] spritesIngredients = Resources.LoadAll("Textures/UI/Ingredients",typeof(Sprite));
        for(int x=0;x<spritesIngredients.Length;x++){
           	IngredientsSpriteList.Add((Sprite)spritesIngredients[x]);
        }
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