using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RebuildUIController : MonoBehaviour
{      
    public GameObject ButtonController;
    public GameObject MainController;
    public GameObject RebuildStore;

    public GameObject ItemController;

    public GameObject ItemPrefab;

    public GameObject switcher1;
    public GameObject switcher2;
    public GameObject switcher3;
    public GameObject ArrowLeft;
    public GameObject ArrowRight;

    public List<Object> ObjectList = new List<Object>();
    public List<Sprite> SpriteList = new List<Sprite>();
    public int shopSite = 0;
    public int objectFilter = 0;//0=chair,1=table,2=wall,3=door,4=walldeko,5=floor,6=deko,7=oven,8=counter,9=fridge,10=toaster,11=expansion
    public int switcher = 0;//switcher zwischen "Deko"==0, "Oven"==1, Expansion==2 

    public void OpenShop(){
        RebuildStore.SetActive(true);//aktiviert UI
        LoadItems();
        RenderShop();
    }

    public void CloseShop(){
        DeleteItems();
        switcher = 0;
        objectFilter = 0;
        shopSite = 0;
        RebuildStore.SetActive(false);
        ObjectList.Clear();
        SpriteList.Clear();

        MainController.GetComponent<MainController>().ActivateBTNs();//aktiviere alle main btns
        MainController.GetComponent<MainController>().ActivateUI();//aktiviere UI
    }

    public void RenderShop(){
        List<Object> builderList = new List<Object>();
        builderList = ObjectListBuilder();

        if(shopSite==0){
            ArrowLeft.SetActive(false);
        }else{
            ArrowLeft.SetActive(true);
        }

        if((shopSite+1)*5<builderList.Count){//guckt ob rechter pfeil geladen wird
            ArrowRight.SetActive(true);
        }else{
            ArrowRight.SetActive(false);
        }

        switcher1.SetActive(false);
        switcher2.SetActive(false);
        switcher3.SetActive(false);
        if(switcher==0){
            switcher1.SetActive(true);
        }else if(switcher==1){
            switcher2.SetActive(true);
        }else if(switcher==2){
            switcher3.SetActive(true);
        }



        //rendert die items
        for(int a=shopSite*5;a<(shopSite*5)+5;a++){
            if(builderList.Count>a){
                RenderItem(builderList[a], a-(shopSite*5));
            }
        }
    }

    public void RenderItem(Object item, int position){
        Instantiate(ItemPrefab, new Vector2((position*195)+570, 110), Quaternion.identity, ItemController.transform);

        for(int a=0;a<SpriteList.Count;a++){//lade bild
            if(item.spriteName.Equals(SpriteList[a].name)){
                ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<Image>().overrideSprite = SpriteList[a];
                //setzt die resolution auf die image größe
                ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2 (SpriteList[a].rect.width, SpriteList[a].rect.height);
            }
        }

        //lade größe vom bild
        if(item.sortiment==0){//chair
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<RectTransform>().localScale = new Vector2 (0.5f, 0.5f);
        }else if(item.sortiment==1){//table
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<RectTransform>().localScale = new Vector2 (0.4f, 0.4f);
        }else if(item.sortiment==2){//wall
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<RectTransform>().localScale = new Vector2 (0.2f, 0.2f);
        }else if(item.sortiment==3){//door
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<RectTransform>().localScale = new Vector2 (0.2f, 0.2f);
        }else if(item.sortiment==4){//walldeko
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<RectTransform>().localScale = new Vector2 (0.3f, 0.3f);
        }else if(item.sortiment==5){//floor
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<RectTransform>().localScale = new Vector2 (0.4f, 0.4f);
        }else if(item.sortiment==6){//floordeko
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<RectTransform>().localScale = new Vector2 (0.3f, 0.3f);
        }else if(item.sortiment==7){//oven
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<RectTransform>().localScale = new Vector2 (0.4f, 0.4f);
        }else if(item.sortiment==8){//counter
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<RectTransform>().localScale = new Vector2 (0.4f, 0.4f);
        }else if(item.sortiment==9){//fridge
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<RectTransform>().localScale = new Vector2 (0.25f, 0.25f);
        }else if(item.sortiment==10){//sluhsi
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<RectTransform>().localScale = new Vector2 (0.25f, 0.25f);
        }


        item.inBackup = PlayerController.getStorageItemCount(item.spriteName);//gucke ob bereits in backup vorhanden
        ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(5).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = ""+item.inBackup;
    
        if(PlayerController.playerLevel>=item.level){
            if(PlayerController.playerGold>=item.priceGold&&PlayerController.playerMoney>=item.priceMoney){
                ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<Button>().interactable = true;
            }else{
                ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<Button>().interactable = false;
            }
        }else{//load lock
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(7).gameObject.GetComponent<Image>().enabled = true;
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<Button>().interactable = false;
        }


        if(item.priceGold>item.priceMoney){//guck für was man das item kaufen kann
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(1).gameObject.GetComponent<Image>().enabled = true;
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(2).gameObject.GetComponent<Image>().enabled = false;
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(6).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = ""+item.priceGold;
        }else if(item.priceGold<item.priceMoney){
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(1).gameObject.GetComponent<Image>().enabled  = false;
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(2).gameObject.GetComponent<Image>().enabled  = true;
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(6).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = ""+item.priceMoney;
        }

        ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<Button>().onClick.AddListener(delegate{BuyItem(item);});
    }

    public void BuyItem(Object item){
        ButtonController.GetComponent<ButtonController>().MouseAction = 2;//create obj
    }

    public void RightSite(){
        shopSite = shopSite + 1;
        DeleteItems();
        RenderShop();
    }

    public void LeftSite(){
        shopSite = shopSite - 1;
        DeleteItems();
        RenderShop();
    }

    public void ChooseFilter(int number){
        objectFilter = number;
        shopSite = 0;
        DeleteItems();
        RenderShop();
    }

    public void SwitchShop(int num){//wechselt 
        if(num==0){
            objectFilter = 0;
        }else if(num==1){
            objectFilter = 7;
        }else if(num==2){    
            objectFilter = 11;
        }
        switcher = num;
        shopSite = 0;
        DeleteItems();
        RenderShop();
    }

    public List<Object> ObjectListBuilder(){
        List<Object> builderList = new List<Object>();
        foreach(var item in ObjectList){
            if(item.sortiment==objectFilter){
                builderList.Add(item);
            }
        }
        return builderList;
    }

    public void LoadItems(){
        ObjectList.Add(new Object("Chair_18_a", 0, 1, 0, true, 0, 0));
        ObjectList.Add(new Object("Chair_02_a", 100, 0, 0, true, 0, 1));
        ObjectList.Add(new Object("Chair_06_a", 100, 0, 0, true, 0, 2));
        ObjectList.Add(new Object("Chair_01_a", 100, 0, 0, true, 0, 3));
        ObjectList.Add(new Object("Chair_03_a", 100, 0, 0, true, 0, 4));
        ObjectList.Add(new Object("Chair_04_a", 100, 0, 0, true, 0, 5));
        ObjectList.Add(new Object("Chair_05_a", 100, 0, 0, true, 0, 6));
        ObjectList.Add(new Object("Chair_12_a", 1000, 0, 0, true, 0, 7));
        ObjectList.Add(new Object("Chair_13_a", 1000, 0, 0, true, 0, 8));
        ObjectList.Add(new Object("Chair_14_a", 1000, 0, 0, true, 0, 9));
        ObjectList.Add(new Object("Chair_15_a", 1000, 0, 0, true, 0, 10));
        ObjectList.Add(new Object("Chair_16_a", 1000, 0, 0, true, 0, 11));
        ObjectList.Add(new Object("Chair_17_a", 1500, 0, 0, true, 0, 12));
        ObjectList.Add(new Object("Chair_01_a_1", 0, 1, 0, true, 0, 13));
        ObjectList.Add(new Object("Chair_02_a_1", 0, 1, 0, true, 0, 14));
        ObjectList.Add(new Object("Chair_03_a_1", 0, 1, 0, true, 0, 15));
        ObjectList.Add(new Object("Chair_04_a_1", 0, 2, 0, true, 0, 16));
        ObjectList.Add(new Object("Chair_05_a_1", 0, 3, 0, true, 0, 17));
        ObjectList.Add(new Object("Chair_06_a_1", 0, 3, 0, true, 0, 18));
        ObjectList.Add(new Object("Chair_07_a_1", 0, 3, 0, true, 0, 19));
        ObjectList.Add(new Object("Chair_08_a_1", 0, 3, 0, true, 0, 20));
        ObjectList.Add(new Object("Chair_09_a_1", 0, 1, 0, true, 0, 21));
        ObjectList.Add(new Object("Chair_10_a_1", 500, 0, 0, true, 0, 22));
        ObjectList.Add(new Object("Chair_11_a_1", 0, 1, 0, true, 0, 23));

        ObjectList.Add(new Object("Counter_01_a", 100, 0, 0, true, 8, 23));
        ObjectList.Add(new Object("Counter_02_a", 1000, 0, 0, true, 8, 23));
        ObjectList.Add(new Object("Counter_03_a", 0, 1, 0, true, 8, 23));
        ObjectList.Add(new Object("Counter_04_a", 0, 1, 0, true, 8, 23));
        ObjectList.Add(new Object("Counter_05_a", 0, 1, 0, true, 8, 23));
        ObjectList.Add(new Object("Counter_06_a", 0, 1, 0, true, 8, 23));
        ObjectList.Add(new Object("Counter_01_a_1", 0, 1, 0, true, 8, 23));
        ObjectList.Add(new Object("Counter_02_a_1", 0, 1, 0, true, 8, 23));
        ObjectList.Add(new Object("Counter_03_a_1", 0, 1, 0, true, 8, 23));
        ObjectList.Add(new Object("Counter_04_a_1", 0, 1, 0, true, 8, 23));
        ObjectList.Add(new Object("Counter_05_a_1", 0, 1, 0, true, 8, 23));
        ObjectList.Add(new Object("Counter_06_a_1", 0, 1, 0, true, 8, 23));

        ObjectList.Add(new Object("Deko_01_a", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_02_a", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_03_a", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_04_a", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_05_a", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_06_a", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_07_a", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_08_a", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_09_a", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_10_a", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_11_a", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_12_a", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_13_a", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_14_a", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_15_a", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_01_a_1", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_02_a_1", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_03_a_1", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_04_a_1", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_05_a_1", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_06_a_1", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_07_a_1", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_08_a_1", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_09_a_1", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_10_a_1", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_11_a_1", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_12_a_1", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_13_a_1", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_14_a_1", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_15_a_1", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_16_a_1", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_17_a_1", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_18_a_1", 0, 1, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_19_a_1", 0, 1, 0, true, 6, 23));

        ObjectList.Add(new Object("Wall_Deko_01_a", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_02_a", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_03_a", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_04_a", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_05_a", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_06_a", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_07_a", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_08_a", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_09_a", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_10_a", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_11_a", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_12_a", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_13_a", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_14_a", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_15_a", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_01_a_1", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_02_a_1", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_03_a_1", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_04_a_1", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_05_a_1", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_06_a_1", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_07_a_1", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_08_a_1", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_09_a_1", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_10_a_1", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_11_a_1", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_12_a_1", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_13_a_1", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_14_a_1", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_15_a_1", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_16_a_1", 0, 1, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_17_a_1", 0, 1, 0, true, 4, 23));

        ObjectList.Add(new Object("Floor_01", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_02", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_03", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_04", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_05", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_06", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_07", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_08", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_09", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_10", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_11", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_01_1", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_02_1", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_03_1", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_04_1", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_05_1", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_06_1", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_07_1", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_08_1", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_09_1", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_10_1", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_11_1", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_12_1", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_13_1", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_14_1", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_15_1", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_16_1", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_17_1", 0, 1, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_18_1", 0, 1, 0, true, 5, 23));

        ObjectList.Add(new Object("Fridge_03_a", 0, 1, 0, true, 9, 23));
        ObjectList.Add(new Object("Fridge_01_a_1", 0, 1, 0, true, 9, 23));
        ObjectList.Add(new Object("Fridge_02_a_1", 0, 1, 0, true, 9, 23));
        ObjectList.Add(new Object("Fridge_04_a_1", 0, 1, 0, true, 9, 23));
        ObjectList.Add(new Object("Fridge_05_a_1", 0, 1, 0, true, 9, 23));
        ObjectList.Add(new Object("Fridge_06_a_1", 0, 1, 0, true, 9, 23));
        ObjectList.Add(new Object("Fridge_07_a_1", 0, 1, 0, true, 9, 23));
        ObjectList.Add(new Object("Fridge_08_a_1", 0, 1, 0, true, 9, 23));

        ObjectList.Add(new Object("Oven_01_a", 0, 1, 0, true, 7, 23));
        ObjectList.Add(new Object("Oven_07_a", 0, 1, 0, true, 7, 23));
        ObjectList.Add(new Object("Oven_01_a_1", 0, 1, 0, true, 7, 23));
        ObjectList.Add(new Object("Oven_02_a_1", 0, 1, 0, true, 7, 23));
        ObjectList.Add(new Object("Oven_03_a_1", 0, 1, 0, true, 7, 23));
        ObjectList.Add(new Object("Oven_04_a_1", 0, 1, 0, true, 7, 23));
        ObjectList.Add(new Object("Oven_05_a_1", 0, 1, 0, true, 7, 23));
        ObjectList.Add(new Object("Oven_06_a_1", 0, 1, 0, true, 7, 23));

        ObjectList.Add(new Object("Shlushi_a", 0, 1, 0, true, 10, 23));

        ObjectList.Add(new Object("Table_01_a", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_02_a", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_03_a", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_04_a", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_05_a", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_06_a", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_07_a", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_08_a", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_09_a", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_10_a", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_11_a", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_12_a", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_01_a_1", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_02_a_1", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_03_a_1", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_04_a_1", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_05_a_1", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_06_a_1", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_07_a_1", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_08_a_1", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_09_a_1", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_10_a_1", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_11_a_1", 0, 1, 0, true, 1, 23));

        ObjectList.Add(new Object("Wall_01_a", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_02_a", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_03_a", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_04_a", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_05_a", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_06_a", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_07_a", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_08_a", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_09_a", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_10_a", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_11_a", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_12_a", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_13_a", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_14_a", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_15_a", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_16_a", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_17_a", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_01_a_1", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_02_a_1", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_03_a_1", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_04_a_1", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_05_a_1", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_06_a_1", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_07_a_1", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_08_a_1", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_09_a_1", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_10_a_1", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_11_a_1", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_12_a_1", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_13_a_1", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_14_a_1", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_15_a_1", 0, 1, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_16_a_1", 0, 1, 0, true, 2, 23));

        ObjectList.Add(new Object("Door_01_a", 0, 1, 0, true, 3, 23));
        ObjectList.Add(new Object("Door_02_a", 0, 1, 0, true, 3, 23));
        ObjectList.Add(new Object("Door_01_a_1", 0, 1, 0, true, 3, 23));
        ObjectList.Add(new Object("Door_02_a_1", 0, 1, 0, true, 3, 23));
        ObjectList.Add(new Object("Door_03_a_1", 0, 1, 0, true, 3, 23));
        ObjectList.Add(new Object("Door_04_a_1", 0, 1, 0, true, 3, 23));
        ObjectList.Add(new Object("Door_05_a_1", 0, 1, 0, true, 3, 23));

        object[] sprites = Resources.LoadAll("Textures/UI/RebuildItems",typeof(Sprite));
        for(int x=0;x<sprites.Length;x++){
           	SpriteList.Add((Sprite)sprites[x]);
        }
    }

    public void DeleteItems(){
        for(int a=ItemController.transform.childCount-1;a>=0;a--){
            Destroy(ItemController.transform.GetChild(a).gameObject);
        }
    }
}

public class Object{
    public string spriteName { get; set; }
    public int priceMoney { get; set; }
    public int priceGold { get; set; }
    public int inBackup { get; set; }
    public bool isActive { get; set; }
    public int sortiment { get; set; }//0=chair,1=table,2=wall,3=door,4=walldeko,5=floor,6=deko,7=oven,8=counter,9=fridge,10=toaster,11=expansion
    public int level { get; set; }
    public Object(string spriteName, int priceMoney, int priceGold, int inBackup, bool isActive, int sortiment, int level){
        this.spriteName = spriteName;
        this.priceMoney = priceMoney;
        this.priceGold = priceGold;
        this.inBackup = inBackup;
        this.isActive = isActive;
        this.sortiment = sortiment;
        this.level = level;
    }
}
