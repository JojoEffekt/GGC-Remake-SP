using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class IngredientsUIController : MonoBehaviour
{
    public GameObject MainController;
    public GameObject IngredientsStore;

    public GameObject ItemController;

    public GameObject ItemPrefab;

    public GameObject PlayerFridgeUI;
    public GameObject PlayerMoneyUI;
    public GameObject PlayerGoldUI;
    public GameObject ArrowLeft;
    public GameObject ArrowRight;
    public GameObject BackgroundIngredients;
    public GameObject BackgroundFridge;
    public GameObject ShopName;

    public List<IngredientItem> IngredientList = new List<IngredientItem>();
    public List<Sprite> SpriteList = new List<Sprite>();
    public int shopSite = 0;
    public int filter = 0;
    public bool isFridgeOpen = false;//switcher zwischen Fridge und Ingredients


    public void OpenShop(){
        IngredientsStore.SetActive(true);//aktiviert UI

        LoadItems();
        BuildShopSite();
    }

    public void CloseShop(){
        DeleteItems();//löscht alle geladenen items
        IngredientsStore.SetActive(false);

        MainController.GetComponent<MainController>().ActivateBTNs();//aktiviere alle main btns

        IngredientList.Clear();
        SpriteList.Clear();
        shopSite = 0;
        filter = 0;
        isFridgeOpen = false;
    }

    public void LoadItems(){
        //string name, string spriteName, int priceMoney, int priceGold, int inFridge, bool isActive, int sortiment, int level
        IngredientList.Add(new IngredientItem("Lettuce", "item_19", 30, 0, 0, true, 2, 0));
        IngredientList.Add(new IngredientItem("Onions", "item_13", 60, 0, 0, true, 2, 1));
        IngredientList.Add(new IngredientItem("Tomatoes", "item_02", 90, 0, 0, true, 2, 2));
        IngredientList.Add(new IngredientItem("Pasta", "item_11", 250, 0, 0, true, 4, 3));
        IngredientList.Add(new IngredientItem("Chocolate", "item_29", 300, 0, 0, true, 4, 4));
        IngredientList.Add(new IngredientItem("Cream", "item_26", 300, 0, 0, true, 1, 5));
        IngredientList.Add(new IngredientItem("Eggs", "item_23", 400, 0, 0, true, 1, 6));
        IngredientList.Add(new IngredientItem("Minced meat", "item_17", 400, 0, 0, true, 1, 7));
        IngredientList.Add(new IngredientItem("Flour", "item_22", 200, 0, 0, true, 4, 8));
        IngredientList.Add(new IngredientItem("Cheese", "item_32", 320, 0, 0, true, 1, 9));
        IngredientList.Add(new IngredientItem("Peas", "item_10", 150, 0, 0, true, 2, 10));
        IngredientList.Add(new IngredientItem("Corn", "item_03", 150, 0, 0, true, 2, 11));
        IngredientList.Add(new IngredientItem("Pineapple", "item_09", 300, 0, 0, true, 3, 12));
        IngredientList.Add(new IngredientItem("Oil", "item_14", 300, 0, 0, true, 4, 13));
        IngredientList.Add(new IngredientItem("Beans", "item_34", 100, 0, 0, true, 2, 14));
        IngredientList.Add(new IngredientItem("Bacon", "item_36", 300, 0, 0, true, 1, 15));
        IngredientList.Add(new IngredientItem("Rice", "item_16", 350, 0, 0, true, 2, 16));
        IngredientList.Add(new IngredientItem("Paprika", "item_37", 450, 0, 0, true, 2, 17));
        IngredientList.Add(new IngredientItem("Cocktail Cherry", "item_50", 0, 0, 0, true, 5, 18));
        IngredientList.Add(new IngredientItem("Butter", "item_53", 425, 0, 0, true, 4, 19));
        IngredientList.Add(new IngredientItem("Potatoes", "item_08", 550, 0, 0, true, 2, 20));
        IngredientList.Add(new IngredientItem("Duck", "item_48", 1500, 0, 0, true, 1, 21));
        IngredientList.Add(new IngredientItem("Oranges", "item_45", 0, 1, 0, true, 5, 22));
        IngredientList.Add(new IngredientItem("Beetroot", "item_33", 400, 0, 0, true, 2, 23));
        IngredientList.Add(new IngredientItem("Cucumbers", "item_25", 400, 0, 0, true, 2, 24));
        IngredientList.Add(new IngredientItem("Meat", "item_18", 1500, 0, 0, true, 1, 25));
        IngredientList.Add(new IngredientItem("Eggplant", "item_24", 100, 0, 0, true, 3, 26));
        IngredientList.Add(new IngredientItem("Sugar", "item_04", 1000, 0, 0, true, 4, 27));
        IngredientList.Add(new IngredientItem("Rhubarb", "item_07", 1500, 0, 0, true, 2, 28));
        IngredientList.Add(new IngredientItem("Garlic", "item_21", 750, 0, 0, true, 2, 29));
        IngredientList.Add(new IngredientItem("Chicken", "item_31", 404, 0, 0, true, 1, 30));
        IngredientList.Add(new IngredientItem("Biscuits", "item_49", 404, 0, 0, true, 4, 31));

        object[] sprites = Resources.LoadAll("Textures/UI/Ingredients",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	SpriteList.Add((Sprite)sprites[x]);
        }
    }

    public void BuildShopSite(){
        if(shopSite==0){
            ArrowLeft.SetActive(false);
        }else{
            ArrowLeft.SetActive(true);
        }

        List<IngredientItem> builderList = new List<IngredientItem>();
        //baut item liste die gerade gerendert werden können
        if(isFridgeOpen==false){//Fridge items oder Ingredients shop?

            //load shopbg
            BackgroundIngredients.SetActive(true);
            BackgroundFridge.SetActive(false);
            ShopName.GetComponent<TMPro.TextMeshProUGUI>().text = "Ingredients Store";

            if(filter==0){//sucht nach allen items ohne filter
                foreach(var item in IngredientList){
                    if(item.isActive){
                        builderList.Add(item);
                    }
                }
            }else{//sucht nach allen items die geladen werden können mit filter
                foreach(var item in IngredientList){
                    if(item.isActive&&item.sortiment==filter){
                        builderList.Add(item);
                    }
                }
            }
        }else{//alle items im fridge laden

            //load SpecialfridgeUI
            BackgroundIngredients.SetActive(false);
            BackgroundFridge.SetActive(true);
            ShopName.GetComponent<TMPro.TextMeshProUGUI>().text = "Fridge";

            if(filter==0){//sucht nach allen items im fridge ohne filter
                foreach(var itemIL in IngredientList){
                    foreach(var itemFI in PlayerController.FoodItemDict){
                        if(itemIL.name.Equals(itemFI.Key)){
                            if(itemIL.isActive){
                                builderList.Add(itemIL);
                            }
                        }
                    }
                }
            }else{//sucht nach allen items im fridge die geladen werden können mit filter
                foreach(var itemIL in IngredientList){
                    foreach(var itemFI in PlayerController.FoodItemDict){
                        if(itemIL.name.Equals(itemFI.Key)){
                            if(itemIL.isActive&&itemIL.sortiment==filter){
                                builderList.Add(itemIL);
                            }       
                        }
                    }
                }
            }
        }

        if((shopSite+1)*8<builderList.Count){//guckt ob rechter pfeil geladen wird
            ArrowRight.SetActive(true);
        }else{
            ArrowRight.SetActive(false);
        }

        for(int a=shopSite*8;a<(shopSite*8)+8;a++){
            if(builderList.Count>a){
                RenderItem(builderList[a], a-(shopSite*8));
            }
        }

        PlayerFridgeUI.GetComponent<TMPro.TextMeshProUGUI>().text = ""+PlayerController.FridgeStoragePlace;
        PlayerMoneyUI.GetComponent<TMPro.TextMeshProUGUI>().text = ""+PlayerController.playerMoney;
        PlayerGoldUI.GetComponent<TMPro.TextMeshProUGUI>().text = ""+PlayerController.playerGold;
    }

    public void RenderItem(IngredientItem item, int position){
        if(position<=3){
            Instantiate(ItemPrefab, new Vector2((position*200)+970, 530), Quaternion.identity, ItemController.transform);
        }else{
            Instantiate(ItemPrefab, new Vector2(((position-4)*200)+970, 260), Quaternion.identity, ItemController.transform);
        }   
        
        for(int a=0;a<SpriteList.Count;a++){//lade bild
            if(item.spriteName.Equals(SpriteList[a].name)){
                ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(8).gameObject.GetComponent<Image>().overrideSprite = SpriteList[a];
            }
        }
        ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = item.name;

        //lad fridgeCount
        item.inFridge = PlayerController.getFoodItemCount(item.name);
        ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = ""+item.inFridge;

        if(item.inFridge==0){//guck ob items bereits im fridge
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.GetComponent<Button>().interactable = false;
        }else{
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.GetComponent<Button>().interactable = true;
        }

        //guck ob player das nötige level hat, genug geld und ob platz im kühlschrank ist und er
        if(PlayerController.playerLevel>=item.level){
            if(PlayerController.playerGold>=item.priceGold&&PlayerController.playerMoney>=item.priceMoney){
                if(PlayerController.FridgeStoragePlace<=49){
                    ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(4).gameObject.GetComponent<Button>().interactable = true;
                }
            }else{
                ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(4).gameObject.GetComponent<Button>().interactable = false;
            }
        }else{//load lock
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(9).gameObject.GetComponent<Image>().enabled = true;
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(4).gameObject.GetComponent<Button>().interactable = false;
        }

        if(item.priceGold>item.priceMoney){//guck für was man das item kaufen kann
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(6).gameObject.GetComponent<Image>().enabled = false;
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(7).gameObject.GetComponent<Image>().enabled = true;
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(3).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = ""+item.priceGold;
        }else if(item.priceGold<item.priceMoney){
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(6).gameObject.GetComponent<Image>().enabled  = true;
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(7).gameObject.GetComponent<Image>().enabled  = false;
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(3).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = ""+item.priceMoney;
        }else{
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(6).gameObject.GetComponent<Image>().enabled  = false;
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(7).gameObject.GetComponent<Image>().enabled  = false;
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(4).gameObject.GetComponent<Button>().interactable = false;
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(3).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "";
        }

        ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(4).gameObject.GetComponent<Button>().onClick.AddListener(delegate{BuyItem(item);});
        ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.GetComponent<Button>().onClick.AddListener(delegate{SellItem(item);});
    }

    public void BuyItem(IngredientItem item){
        //abrechnen
        PlayerController.playerMoney = PlayerController.playerMoney - item.priceMoney;
        PlayerController.playerGold = PlayerController.playerGold - item.priceGold;

        //item PlayerController hinzufügen
        PlayerController.AddFoodItem(item.name);
    	
        //reload ob items immernoch kaufbar sind
        DeleteItems();
        BuildShopSite();

        //saveData
        SaveAndLoadController.SavePlayerData();
    }

    public void SellItem(IngredientItem item){
        //abrechnen
        PlayerController.playerMoney = PlayerController.playerMoney + (item.priceMoney/5);
        if(item.priceGold!=0){
            PlayerController.playerMoney = PlayerController.playerMoney + (item.priceGold*1000);
        }

        //item PlayerController hinzufügen
        PlayerController.RemoveFoodItem(item.name);
    	
        //reload ob items immernoch kaufbar sind
        DeleteItems();
        BuildShopSite();

        //saveData
        SaveAndLoadController.SavePlayerData();
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

    public void ChooseFilter(int number){
        filter = number;
        shopSite = 0;
        DeleteItems();
        BuildShopSite();
    }

    public void IsFridgeOpen(bool switcher){
        isFridgeOpen = switcher;
        filter = 0;
        shopSite = 0;
        DeleteItems();
        BuildShopSite();
    }
}

public class IngredientItem {
    public string name { get; set; }
    public string spriteName { get; set; }
    public int priceMoney { get; set; }
    public int priceGold { get; set; }
    public int inFridge { get; set; }
    public bool isActive { get; set; }
    public int sortiment { get; set; }//0=all, 1=animal 2=vegetables 3=fruit 4=other 5=special
    public int level { get; set; }

    public IngredientItem(string name, string spriteName, int priceMoney, int priceGold, int inFridge, bool isActive, int sortiment, int level){
        this.name = name;
        this.spriteName = spriteName;
        this.priceMoney = priceMoney;
        this.priceGold = priceGold;
        this.inFridge = inFridge;
        this.isActive = isActive;
        this.sortiment = sortiment;
        this.level = level;
    }
}
