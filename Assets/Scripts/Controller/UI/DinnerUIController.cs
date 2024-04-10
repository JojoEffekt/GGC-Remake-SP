using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DinnerUIController : MonoBehaviour
{
    //referenz für das MainController Script
    public GameObject MainController;

    //referenz auf das parent Object des Dinner Stores
    public GameObject DinnerStore;



    //referenz auf right btn
    public GameObject ArrowRight;

    //referenz auf left btn
    public GameObject ArrowLeft;


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


    //bestimmt die aktuelle shopseite
    public int shopSite = 0;


    //ist eine globale referenz damit das zu kochende item erhalten werden kann wenn er gekocht werden soll
    public static DinnerItem DinnerToCook;


    //öffnet den shop und läd die items
    public void OpenShop(){
        
        //überprfüe ob der shop nicht schon aktiviert ist, ansonsten lade shop
        if(DinnerStore.activeInHierarchy==false){

            DinnerStore.SetActive(true);//aktiviert UI

            LoadItems();
            BuildShopSite();
        }
    }

    //baut die shopseite
    public void BuildShopSite(){
        if(shopSite==0){
            ArrowLeft.SetActive(false);
        }else{
            ArrowLeft.SetActive(true);
        }


        List<DinnerItem> builderList = new List<DinnerItem>();
        //baut item liste die gerade gerendert werden können
        //sucht nach allen items die geladen werden können mit filter
        foreach(var item in DinnerItemList){
            if(item.type.Equals("background_dinner")){//filter hier ob event dinner geladen werden dürfen!!!!!!!!!
                builderList.Add(item);
            }
        }

        //guckt ob rechter pfeil geladen werden kann
        if((shopSite+1)*4<builderList.Count){
            ArrowRight.SetActive(true);
        }else{
            ArrowRight.SetActive(false);
        }

        //bestimmt die zu ladenen dinner
        for(int a=shopSite*4;a<(shopSite*4)+4;a++){
            if(builderList.Count>a){
                InstantiateItem(builderList[a], a-(shopSite*4));
            }
        }
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
        int counter = 8;//ist für child gameobject verantwortlich
        bool isCookable = true;//wird benutzt um zu schauen ob dinner cookable ist
    	for(int c=0;c<IngredientsSpriteList.Count;c++){
            foreach(var ingredient in item.infoIngredients){
                if(ingredient.Key.Equals(IngredientsSpriteList[c].name)){
                    //generiert das ingredient
                    prefab.transform.GetChild(0).gameObject.transform.GetChild(counter).gameObject.SetActive(true);
                    prefab.transform.GetChild(0).gameObject.transform.GetChild(counter).gameObject.GetComponent<Image>().overrideSprite = IngredientsSpriteList[c];


                    //wenn das item nicht vorhanden(im kühlschrank) dann graue aus
                    //kriege das ingredient andhand des spritenamen herraus und gucke ob es im kühlschrank genug vorhanden ist um das gericht zu kochen
                    if(PlayerController.getFoodItemCount(Transform(IngredientsSpriteList[c].name))<ingredient.Value){
                        prefab.transform.GetChild(0).gameObject.transform.GetChild(counter).gameObject.GetComponent<Image>().color = Color.grey;
                        
                        //zeigt cook zeichen an
                        isCookable = false;
                    }

                    //guckt ob das gericht überhaupt gekocht werden kann, dann lock ingredients egal ob vorhanden
                    if(item.level>PlayerController.playerLevel){
                        prefab.transform.GetChild(0).gameObject.transform.GetChild(counter).gameObject.GetComponent<Image>().color = Color.grey;
                    }


                    //generiert die stückanzahl
                    prefab.transform.GetChild(2).gameObject.transform.GetChild(counter-3).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = ""+ingredient.Value;
                    counter ++;
                }
            }
        }

        //delegiert das passende event für den jeweiligen btn
        if(item.level>PlayerController.playerLevel){
            //lock dinner wenn playerlevel nicht erreicht ist
            //buy btn
            prefab.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().overrideSprite = UISpriteList[9];
            prefab.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.grey;

            //lock btn
            prefab.transform.GetChild(0).gameObject.transform.GetChild(14).gameObject.SetActive(true);

        }else{
            //guckt ob das istem cookable ist, anhand ob alle igredients vorhanden sind
            if(isCookable){
                prefab.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().overrideSprite = UISpriteList[9];

                //delegate cook CONITNUE
                 prefab.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(delegate{CookDinner(item);});
            }else{
                //delegate open ingredient
                prefab.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(delegate{OpenIngredientShop();});
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
    public void LoadItems(){
        //                               name       spriteName        Dinnertype       type        level                                                                                                            infoIngredients                           Superzutat                                stars
        //DinnerItemList.Add(new DinnerItem("","","","",0,new Dictionary<string ,string>(){{"number",""},{"time",""},{"moneyPerItem",""},{"xp",""},{"moneyPerItemSI",""},{"xpSI",""}},new Dictionary<string ,int>(){{"",}},new Dictionary<string ,int>(){{"",}},new Dictionary<string ,int>(){{"",},{"",},{"",}}));
        DinnerItemList.Add(new DinnerItem("Garden Salad","Dinner_18_04","type_vegetable","background_dinner",1,new Dictionary<string ,string>(){{"number","10"},{"time","3"},{"moneyPerItem","24"},{"xp","5"},{"moneyPerItemSI","0"},{"xpSI","0"}},new Dictionary<string ,int>(){{"item_02",1},{"item_13",1},{"item_19",1}},new Dictionary<string ,int>(){{"0",0}},new Dictionary<string ,int>(){{"bronze_star_icon",0},{"silver_star_icon",0},{"gold_star_icon",0}}));
        DinnerItemList.Add(new DinnerItem("Tomatosoup","Dinner_02_04","type_soup","background_dinner",2,new Dictionary<string ,string>(){{"number","40"},{"time","10"},{"moneyPerItem","14"},{"xp","16"},{"moneyPerItemSI","0"},{"xpSI","0"}},new Dictionary<string ,int>(){{"item_02",1},{"item_13",1},{"item_26",1}},new Dictionary<string ,int>(){{"0",0}},new Dictionary<string ,int>(){{"bronze_star_icon",0},{"silver_star_icon",0},{"gold_star_icon",0}}));
        DinnerItemList.Add(new DinnerItem("Omelette","Dinner_55_04","type_burger","background_dinner",3,new Dictionary<string ,string>(){{"number","5"},{"time","1"},{"moneyPerItem","100"},{"xp","2"},{"moneyPerItemSI","0"},{"xpSI","0"}},new Dictionary<string ,int>(){{"item_23",1}},new Dictionary<string ,int>(){{"0",0}},new Dictionary<string ,int>(){{"bronze_star_icon",0},{"silver_star_icon",0},{"gold_star_icon",0}}));
        DinnerItemList.Add(new DinnerItem("Mousse au Chocolat","Dinner_20_04","type_candy","background_dinner",4,new Dictionary<string ,string>(){{"number","390"},{"time","180"},{"moneyPerItem","4"},{"xp","259"},{"moneyPerItemSI","0"},{"xpSI","0"}},new Dictionary<string ,int>(){{"item_23",1},{"item_29",1},{"item_26",1}},new Dictionary<string ,int>(){{"0",0}},new Dictionary<string ,int>(){{"bronze_star_icon",0},{"silver_star_icon",0},{"gold_star_icon",0}}));
        DinnerItemList.Add(new DinnerItem("Spaghetti Bolognese","Dinner_06_04","type_main","background_dinner",5,new Dictionary<string ,string>(){{"number","1040"},{"time","540"},{"moneyPerItem","1"},{"xp","624"},{"moneyPerItemSI","0"},{"xpSI","0"}},new Dictionary<string ,int>(){{"item_11",1},{"item_17",1},{"item_02",1}},new Dictionary<string ,int>(){{"item_35",1}},new Dictionary<string ,int>(){{"bronze_star_icon",0},{"silver_star_icon",0},{"gold_star_icon",0}}));
        DinnerItemList.Add(new DinnerItem("Hamburger","Dinner_17_04","type_burger","background_dinner",6,new Dictionary<string ,string>(){{"number","130"},{"time","60"},{"moneyPerItem","7"},{"xp","97"},{"moneyPerItemSI","0"},{"xpSI","0"}},new Dictionary<string ,int>(){{"item_17",1},{"item_02",1},{"item_22",1}},new Dictionary<string ,int>(){{"item_30",1}},new Dictionary<string ,int>(){{"bronze_star_icon",0},{"silver_star_icon",0},{"gold_star_icon",0}}));

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
    
    //kriege anhand des spritenamens das ingredient herraus
    public string Transform(string spriteName){
        string name = "";
        if(spriteName.Equals("item_19")){
            name = "Lettuce";
        }else if(spriteName.Equals("item_13")){
            name = "Onions";
        }else if(spriteName.Equals("item_02")){
            name = "Tomatoes";
        }else if(spriteName.Equals("item_11")){
            name = "Pasta";
        }else if(spriteName.Equals("item_29")){
            name = "Chocolate";
        }else if(spriteName.Equals("item_26")){
            name = "Cream";
        }else if(spriteName.Equals("item_23")){
            name = "Eggs";
        }else if(spriteName.Equals("item_17")){
            name = "Minced meat";
        }else if(spriteName.Equals("item_22")){
            name = "Flour";
        }else if(spriteName.Equals("item_32")){
            name = "Cheese";
        }else if(spriteName.Equals("item_10")){
            name = "Peas";
        }else if(spriteName.Equals("item_03")){
            name = "Corn";
        }else if(spriteName.Equals("item_09")){
            name = "Pineapple";
        }else if(spriteName.Equals("item_14")){
            name = "Oil";
        }else if(spriteName.Equals("item_34")){
            name = "Beans";
        }else if(spriteName.Equals("item_36")){
            name = "Bacon";
        }else if(spriteName.Equals("item_06")){
            name = "Rice";
        }else if(spriteName.Equals("item_37")){
            name = "Paprika";
        }else if(spriteName.Equals("item_50")){
            name = "Cocktail Cherry";
        }else if(spriteName.Equals("item_53")){
            name = "Butter";
        }else if(spriteName.Equals("item_08")){
            name = "Potatoes";
        }else if(spriteName.Equals("item_48")){
            name = "Duck";
        }else if(spriteName.Equals("item_45")){
            name = "Orange";
        }else if(spriteName.Equals("item_33")){
            name = "Beetroot";
        }else if(spriteName.Equals("item_25")){
            name = "Cucumbers";
        }else if(spriteName.Equals("item_18")){
            name = "Meat";
        }else if(spriteName.Equals("item_24")){
            name = "Eggplant";
        }else if(spriteName.Equals("item_04")){
            name = "Sugar";
        }else if(spriteName.Equals("item_07")){
            name = "Rhubarb";
        }else if(spriteName.Equals("item_21")){
            name = "Garlic";
        }else if(spriteName.Equals("item_31")){
            name = "Chicken";
        }else if(spriteName.Equals("item_49")){
            name = "Biscuits";
        }
        return name; 
    }

    public void DeleteItems(){
        for(int a=ItemController.transform.childCount-1;a>=0;a--){
            Destroy(ItemController.transform.GetChild(a).gameObject);
        }
    }

    public void RightSite(){
        shopSite = shopSite + 1;
        DeleteItems();
        BuildShopSite();
    }

    public void LeftSite(){
        shopSite = shopSite - 1;
        DeleteItems();
        BuildShopSite();
    }

    public void CloseShop(){
        DeleteItems();//löscht alle geladenen items
        DinnerStore.SetActive(false);

        MainController.GetComponent<MainController>().ActivateBTNs();//aktiviere alle main btns

        DinnerItemList.Clear();
        SpriteList.Clear();
        UISpriteList.Clear();
        IngredientsSpriteList.Clear();
        shopSite = 0;
    }

    //schließt den dinnershop und öffnet dafür den ingredientshop
    public void OpenIngredientShop(){
        CloseShop();
        MainController.GetComponent<MainController>().buttonPressed(1);
    }

    //wird im shop durch cook btn aufgerufen und übergibt das zu kochende dinner
    public void CookDinner(DinnerItem item){
        
        //macht das zu kochende item global verfügbar
        DinnerToCook = item;

        //Debug.Log("Cook: "+item.name);
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