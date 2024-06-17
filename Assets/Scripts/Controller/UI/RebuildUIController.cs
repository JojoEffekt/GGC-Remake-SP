using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RebuildUIController : MonoBehaviour
{      
    public GameObject ButtonController;
    public GameObject MainController;
    public GameObject RebuildStore;
    public GameObject NPCController;

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

        //Sorgt dafür das ButtonController Weiß das der RebuildShop offen ist
        ButtonController.GetComponent<ButtonController>().isRebuildShopOpen = true;
        ButtonController.GetComponent<ButtonController>().MouseAction = 0;
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

        //Sorgt dafür das ButtonController Weiß das der RebuildShop geschlossen ist
        ButtonController.GetComponent<ButtonController>().isRebuildShopOpen = false;
        ButtonController.GetComponent<ButtonController>().MouseAction = 0;
        Destroy(ButtonController.GetComponent<ButtonController>().DynamicPrefab);//zerstört SettingsUI fals noch offen

        //BEIM SCHLIE?EN DES SHOPS SPEICHER WICHTIGE DATEN BEI DIE DAS CAFE NACH OBJECTEN GESCANNT WIRD
        //erfasse alle tables und chairs nachdem der shop geschlossen wird um npc sitzpositionen ermitteln zu können
        NPCController.GetComponent<NPCManager>().CollectAllChairsAndTablesInList();
        //läd das grid für npc/player movement
        LabyrinthBuilder.GenerateGrid();
    }

    public void RenderShop(){
        List<Object> builderList = new List<Object>();
        builderList = ObjectListBuilder();

        //guckt ob der linke pfeil gerendert werden kann
        if(shopSite==0){
            ArrowLeft.SetActive(false);
        }else{
            ArrowLeft.SetActive(true);
        }

        //guckt ob der pfeil nach rechts geladen werden kann, wenn mehr als 5 objecte im shop generiert werden
        if((shopSite+1)*5<builderList.Count){//guckt ob rechter pfeil geladen wird
            ArrowRight.SetActive(true);
        }else{
            ArrowRight.SetActive(false);
        }

        //läd die jeweiligen untergruppen des shops
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



        //rendert die items der jeweilgen shopseite die gerade aktiv ist
        for(int a=shopSite*5;a<(shopSite*5)+5;a++){
            if(builderList.Count>a){
                RenderItem(builderList[a], a-(shopSite*5));
            }
        }
    }

    public void RenderItem(Object item, int position){

        //erstellt prefab
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
        }else if(item.sortiment==11){//expansion
            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<RectTransform>().localScale = new Vector2 (0.25f, 0.25f);
        }

        //gucke ob bereits in backup vorhanden
        item.inBackup = PlayerController.getStorageItemCount(item.spriteName);
        ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(5).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = ""+item.inBackup;
        

        //gucke ob player das nötige level hat
        if(PlayerController.playerLevel>=item.level){

            //gucke ob player überhaupt noch bestimmte objecttypen platzieren darf (Oven, fridge etc)
            if(PlayerController.getObjectLimiterDictInfoForObject(item.spriteName.Split("_")[0])>0){
                
                //gucke ob player genug geld ODER in backup vorhanden ist
                if((PlayerController.playerGold>=item.priceGold&&PlayerController.playerMoney>=item.priceMoney)||item.inBackup>0){
                    
                    //es darf nur die kleinste Griderweiterung gekauft werden, heißt die größeren müssen gesperrt sein solange nicht die kleinste gekauft wurden
                    if(item.spriteName.Split("_")[0].Equals("Expansion")){
                        if(isLowestExpansionItem(item.spriteName)){

                            //kleinstes ExpansionItem, kann gekauft werden
                            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<Button>().interactable = true;
                        }else{
                            ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<Button>().interactable = false;
                        }
                    }else{

                        //andere objecte können gekauft werden
                        ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<Button>().interactable = true;
                    }   
                }else{
                    ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<Button>().interactable = false;
                }
            }else{
                ItemController.transform.GetChild(ItemController.transform.childCount-1).gameObject.transform.GetChild(3).gameObject.GetComponent<Button>().interactable = false;
            }
        }else{
            
            //load lock
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
        //hat spieler genug geld ODER in backup
        if((PlayerController.playerMoney>=item.priceMoney&&PlayerController.playerGold>=item.priceGold)||item.inBackup>0){

            //wenn das item eine tür ist, generiere die tür direkt ohne nutzung von ButtonController
            if(item.spriteName.Split("_")[1].Equals("Door")){
                string[] details = new string[]{};
                if(item.spriteName.Equals("Wall_Door_01_1_")){
                    //                     spritename   coordCorX coordCordY goldPrice moneyprice
                    details = new string[]{"Wall_Door_01_1_", "0,2", "-0,5", "0", "50"};
                }
                if(item.spriteName.Equals("Wall_Door_02_1_")){
                    details = new string[]{"Wall_Door_02_1_", "0,2", "-0,5", "0", "1100"};
                }
                if(item.spriteName.Equals("Wall_Door_03_1_")){
                    details = new string[]{"Wall_Door_03_1_", "0,5", "-0,3", "15", "0"};
                }
                if(item.spriteName.Equals("Wall_Door_04_1_")){
                    details = new string[]{"Wall_Door_04_1_", "0,2", "-1,0", "5", "0"};
                }
                if(item.spriteName.Equals("Wall_Door_05_1_")){
                    details = new string[]{"Wall_Door_05_1_", "0,2", "-0,65", "3", "0"};
                }
                if(item.spriteName.Equals("Wall_Door_06_1_")){
                    details = new string[]{"Wall_Door_06_1_", "0,2", "-0,9", "0", "20000"};
                }
                if(item.spriteName.Equals("Wall_Door_07_1_")){
                    details = new string[]{"Wall_Door_07_1_", "0,2", "-0,6", "0", "14000"};
                }

                //Generiert die neue Tür, gibt true wiereder wenn geklappt
                if(ObjectController.ChangeDoorOnWall(details[0], float.Parse(details[1]), float.Parse(details[2]), Int32.Parse(details[3]), Int32.Parse(details[4]))){
                    //tür wird Abgerechnet wenn tür erstellt
                    //wenn in backup, rechne erst das ab
                    if(item.inBackup>0){
                        PlayerController.RemoveStorageItem(item.spriteName);
                    }else{
                        PlayerController.playerMoney = PlayerController.playerMoney - item.priceMoney;
                        PlayerController.playerGold = PlayerController.playerGold - item.priceGold;
                    }
                }

                //updated die mainUI player stats
                PlayerController.ReloadPlayerStats();

                //nach jeder action muss gespeichert werden
                SaveAndLoadController.SavePlayerData();
            
            //gucke ob expansion gekauft wurde
            }else if(item.spriteName.Split("_")[0].Equals("Expansion")){

                //lösche expansion für gold und money
                string itemDoubleName = item.spriteName;
                ObjectList.Remove(item);
                //hole item referenz für gleichnamiges object und lösche
                for(int a=0;a<ObjectList.Count;a++){
                    if(ObjectList[a].spriteName.Equals(itemDoubleName)){
                        ObjectList.RemoveAt(a);
                    }
                }

                //rechne ab
                PlayerController.playerMoney = PlayerController.playerMoney - item.priceMoney;
                PlayerController.playerGold = PlayerController.playerGold - item.priceGold;

                //expandiere grid
                GridController.UpgradeGrid();

                //aktuallisiere stats
                PlayerController.ReloadPlayerStats();
            }else{
                //item kann nun über ButtonController platziert werden
                //object zur generierung freigeschaltet
                ButtonController.GetComponent<ButtonController>().MouseAction = 2;
                ButtonController.GetComponent<ButtonController>().ObjectToCreate = new string[]{item.spriteName, ""+item.priceGold, ""+item.priceMoney, ""+item.inBackup};
            }

            //muss nach jeder shop aktion neu ausgeführt werder um bsp zu gucken ob der player noch genug geld für objekte hat und entsprechend 
            //buy buttons ausblenden
            DeleteItems();
            RenderShop();
        }
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
        ObjectList.Add(new Object("Chair_12_a", 1500, 0, 0, true, 0, 7));
        ObjectList.Add(new Object("Chair_13_a", 1800, 0, 0, true, 0, 8));
        ObjectList.Add(new Object("Chair_14_a", 2000, 0, 0, true, 0, 9));
        ObjectList.Add(new Object("Chair_15_a", 1700, 0, 0, true, 0, 10));
        ObjectList.Add(new Object("Chair_16_a", 1600, 0, 0, true, 0, 11));
        ObjectList.Add(new Object("Chair_17_a", 9000, 0, 0, true, 0, 12));
        ObjectList.Add(new Object("Chair_01_a_1", 0, 1, 0, true, 0, 13));
        ObjectList.Add(new Object("Chair_02_a_1", 5000, 0, 0, true, 0, 14));
        ObjectList.Add(new Object("Chair_03_a_1", 0, 1, 0, true, 0, 15));
        ObjectList.Add(new Object("Chair_04_a_1", 0, 1, 0, true, 0, 16));
        ObjectList.Add(new Object("Chair_05_a_1", 4000, 0, 0, true, 0, 17));
        ObjectList.Add(new Object("Chair_06_a_1", 0, 999, 0, true, 0, 18));//CONTINUE
        ObjectList.Add(new Object("Chair_07_a_1", 0, 999, 0, true, 0, 19));//CONTINUE
        ObjectList.Add(new Object("Chair_08_a_1", 0, 999, 0, true, 0, 20));//CONTINUE
        ObjectList.Add(new Object("Chair_09_a_1", 2000, 0, 0, true, 0, 21));
        ObjectList.Add(new Object("Chair_10_a_1", 3000, 0, 0, true, 0, 22));
        ObjectList.Add(new Object("Chair_11_a_1", 0, 999, 0, true, 0, 23));//CONTINUE

        ObjectList.Add(new Object("Counter_01_a", 0, 999, 0, true, 8, 23));//CONTINUE
        ObjectList.Add(new Object("Counter_02_a", 500, 0, 0, true, 8, 23));
        ObjectList.Add(new Object("Counter_03_a", 0, 999, 0, true, 8, 23));//CONTINUE
        ObjectList.Add(new Object("Counter_04_a", 0, 999, 0, true, 8, 23));//CONTINUE
        ObjectList.Add(new Object("Counter_05_a", 1000, 0, 0, true, 8, 23));
        ObjectList.Add(new Object("Counter_06_a", 500, 0, 0, true, 8, 23));
        ObjectList.Add(new Object("Counter_01_a_1", 10000, 0, 0, true, 8, 23));
        ObjectList.Add(new Object("Counter_02_a_1", 0, 999, 0, true, 8, 23));//CONTINUE
        ObjectList.Add(new Object("Counter_03_a_1", 0, 999, 0, true, 8, 23));//CONTINUE
        ObjectList.Add(new Object("Counter_04_a_1", 0, 3, 0, true, 8, 23));
        ObjectList.Add(new Object("Counter_05_a_1", 0, 999, 0, true, 8, 23));//CONTINUE
        ObjectList.Add(new Object("Counter_06_a_1", 0, 999, 0, true, 8, 23));//CONTINUE

        ObjectList.Add(new Object("Deko_01_a", 700, 0, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_02_a", 0, 3, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_03_a", 500, 0, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_04_a", 8000, 0, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_05_a", 0, 2, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_06_a", 0, 999, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_07_a", 20000, 0, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_08_a", 1500, 0, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_09_a", 0, 3, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_10_a", 1750, 0, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_11_a", 2000, 0, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_12_a", 0, 5, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_13_a", 0, 5, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_14_a", 0, 3, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_15_a", 0, 3, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_01_a_1", 0, 5, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_02_a_1", 0, 5, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_03_a_1", 0, 4, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_04_a_1", 0, 3, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_05_a_1", 0, 3, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_06_a_1", 0, 999, 0, true, 6, 23));//CONTINUE
        ObjectList.Add(new Object("Deko_07_a_1", 0, 999, 0, true, 6, 23));//CONTINUE
        ObjectList.Add(new Object("Deko_08_a_1", 0, 999, 0, true, 6, 23));//CONTINUE
        ObjectList.Add(new Object("Deko_09_a_1", 0, 20, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_10_a_1", 0, 12, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_11_a_1", 0, 3, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_12_a_1", 0, 3, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_13_a_1", 0, 999, 0, true, 6, 23));//CONTINUE
        ObjectList.Add(new Object("Deko_14_a_1", 4000, 0, 0, true, 6, 23));
        ObjectList.Add(new Object("Deko_15_a_1", 0, 999, 0, true, 6, 23));//CONTINUE
        ObjectList.Add(new Object("Deko_16_a_1", 0, 999, 0, true, 6, 23));//CONTINUE
        ObjectList.Add(new Object("Deko_17_a_1", 0, 999, 0, true, 6, 23));//CONTINUE
        ObjectList.Add(new Object("Deko_18_a_1", 0, 999, 0, true, 6, 23));//CONTINUE
        ObjectList.Add(new Object("Deko_19_a_1", 0, 999, 0, true, 6, 23));//CONTINUE

        ObjectList.Add(new Object("Wall_Deko_01_a", 800, 0, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_02_a", 400, 0, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_03_a", 0, 4, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_04_a", 0, 5, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_05_a", 700, 0, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_06_a", 11000, 0, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_07_a", 0, 2, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_08_a", 0, 3, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_09_a", 6000, 0, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_10_a", 8000, 0, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_11_a", 12500, 0, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_12_a", 0, 4, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_13_a", 0, 4, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_14_a", 5000, 0, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_15_a", 10000, 0, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_01_a_1", 2500, 0, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_02_a_1", 1500, 0, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_03_a_1", 500, 0, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_04_a_1", 7000, 0, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_05_a_1", 15000, 0, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_06_a_1", 0, 3, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_07_a_1", 10000, 0, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_08_a_1", 0, 999, 0, true, 4, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_Deko_09_a_1", 0, 999, 0, true, 4, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_Deko_10_a_1", 0, 999, 0, true, 4, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_Deko_11_a_1", 0, 999, 0, true, 4, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_Deko_12_a_1", 0, 999, 0, true, 4, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_Deko_13_a_1", 0, 999, 0, true, 4, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_Deko_14_a_1", 0, 4, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_15_a_1", 0, 2, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_16_a_1", 0, 5, 0, true, 4, 23));
        ObjectList.Add(new Object("Wall_Deko_17_a_1", 0, 5, 0, true, 4, 23));

        ObjectList.Add(new Object("Floor_01", 20, 0, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_02", 20, 0, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_03", 20, 0, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_04", 100, 0, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_05", 20, 0, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_06", 100, 0, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_07", 100, 0, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_08", 20, 0, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_09", 20, 0, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_10", 20, 0, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_11", 100, 0, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_01_1", 1000, 0, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_02_1", 1500, 0, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_03_1", 2000, 0, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_04_1", 999, 0, 0, true, 5, 23));//CONTINUE
        ObjectList.Add(new Object("Floor_05_1", 999, 0, 0, true, 5, 23));//CONTINUE
        ObjectList.Add(new Object("Floor_06_1", 999, 0, 0, true, 5, 23));//CONTINUE
        ObjectList.Add(new Object("Floor_07_1", 999, 0, 0, true, 5, 23));//CONTINUE
        ObjectList.Add(new Object("Floor_08_1", 999, 0, 0, true, 5, 23));//CONTINUE
        ObjectList.Add(new Object("Floor_09_1", 999, 0, 0, true, 5, 23));//CONTINUE
        ObjectList.Add(new Object("Floor_10_1", 999, 0, 0, true, 5, 23));//CONTINUE
        ObjectList.Add(new Object("Floor_11_1", 100, 0, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_12_1", 999, 0, 0, true, 5, 23));//CONTINUE
        ObjectList.Add(new Object("Floor_13_1", 100, 0, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_14_1", 250, 0, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_15_1", 999, 0, 0, true, 5, 23));//CONTINUE
        ObjectList.Add(new Object("Floor_16_1", 1000, 0, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_17_1", 1000, 0, 0, true, 5, 23));
        ObjectList.Add(new Object("Floor_18_1", 1000, 0, 0, true, 5, 23));

        ObjectList.Add(new Object("Fridge_03_a", 0, 999, 0, true, 9, 23));//CONTINUE
        ObjectList.Add(new Object("Fridge_01_a_1", 0, 7, 0, true, 9, 23));
        ObjectList.Add(new Object("Fridge_02_a_1", 0, 8, 0, true, 9, 23));
        ObjectList.Add(new Object("Fridge_04_a_1", 0, 4, 0, true, 9, 23));
        ObjectList.Add(new Object("Fridge_05_a_1", 0, 999, 0, true, 9, 23));//CONTINUE
        ObjectList.Add(new Object("Fridge_06_a_1", 0, 6, 0, true, 9, 23));
        ObjectList.Add(new Object("Fridge_07_a_1", 0, 999, 0, true, 9, 23));//CONTINUE
        ObjectList.Add(new Object("Fridge_08_a_1", 0, 4, 0, true, 9, 23));

        ObjectList.Add(new Object("Oven_01_a", 1000, 0, 0, true, 7, 10));
        ObjectList.Add(new Object("Oven_07_a", 12000, 0, 0, true, 7, 10));
        ObjectList.Add(new Object("Oven_01_a_1", 0, 3, 0, true, 7, 10));
        ObjectList.Add(new Object("Oven_02_a_1", 15000, 0, 0, true, 7, 10));
        ObjectList.Add(new Object("Oven_03_a_1", 0, 7, 0, true, 7, 10));
        ObjectList.Add(new Object("Oven_04_a_1", 0, 999, 0, true, 7, 23));//CONTINUE
        ObjectList.Add(new Object("Oven_05_a_1", 0, 4, 0, true, 7, 23));
        ObjectList.Add(new Object("Oven_06_a_1", 0, 5, 0, true, 7, 23));

        ObjectList.Add(new Object("Shlushi_a", 500, 0, 0, true, 10, 23));

        ObjectList.Add(new Object("Table_01_a", 200, 0, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_02_a", 200, 0, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_03_a", 200, 0, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_04_a", 200, 0, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_05_a", 200, 0, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_06_a", 200, 0, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_07_a", 1500, 0, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_08_a", 1500, 0, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_09_a", 2000, 0, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_10_a", 7500, 0, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_11_a", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_12_a", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_01_a_1", 0, 1, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_02_a_1", 8000, 0, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_03_a_1", 0, 2, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_04_a_1", 0, 999, 0, true, 1, 23));//CONTINUE
        ObjectList.Add(new Object("Table_05_a_1", 0, 999, 0, true, 1, 23));//CONTINUE
        ObjectList.Add(new Object("Table_06_a_1", 0, 999, 0, true, 1, 23));//CONTINUE
        ObjectList.Add(new Object("Table_07_a_1", 0, 3, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_08_a_1", 0, 2, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_09_a_1", 0, 2, 0, true, 1, 23));
        ObjectList.Add(new Object("Table_10_a_1", 0, 999, 0, true, 1, 23));//CONTINUE
        ObjectList.Add(new Object("Table_11_a_1", 0, 1, 0, true, 1, 23));

        ObjectList.Add(new Object("Wall_01_a", 900, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_02_a", 900, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_03_a", 800, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_04_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_05_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_06_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_07_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_08_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_09_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_10_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_11_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_12_a", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_13_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_14_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_15_a", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_16_a", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_17_a", 20, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_01_a_1", 1500, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_02_a_1", 1000, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_03_a_1", 2500, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_04_a_1", 7500, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_05_a_1", 7000, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_06_a_1", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_07_a_1", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_08_a_1", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_09_a_1", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_10_a_1", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_11_a_1", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_12_a_1", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_13_a_1", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_14_a_1", 0, 999, 0, true, 2, 23));//CONTINUE
        ObjectList.Add(new Object("Wall_15_a_1", 6000, 0, 0, true, 2, 23));
        ObjectList.Add(new Object("Wall_16_a_1", 7000, 0, 0, true, 2, 23));

        ObjectList.Add(new Object("Wall_Door_01_1_", 50, 0, 0, true, 3, 23));
        ObjectList.Add(new Object("Wall_Door_02_1_", 1100, 0, 0, true, 3, 23));
        ObjectList.Add(new Object("Wall_Door_03_1_", 0, 15, 0, true, 3, 23));
        ObjectList.Add(new Object("Wall_Door_04_1_", 0, 5, 0, true, 3, 23));
        ObjectList.Add(new Object("Wall_Door_05_1_", 0, 3, 0, true, 3, 23));
        ObjectList.Add(new Object("Wall_Door_06_1_", 20000, 0, 0, true, 3, 23));
        ObjectList.Add(new Object("Wall_Door_07_1_", 14000, 0, 0, true, 3, 23));

        //instantziert nur, wenn das upgrade noch nicht gekauft wurde
        if(PlayerController.gridSize<8){
            ObjectList.Add(new Object("Expansion_8x8", 0, 5, 0, true, 11, 0));
            ObjectList.Add(new Object("Expansion_8x8", 1000, 0, 0, true, 11, 3));
        }
        if(PlayerController.gridSize<9){
            ObjectList.Add(new Object("Expansion_9x9", 0, 10, 0, true, 11, 0));
            ObjectList.Add(new Object("Expansion_9x9", 10000, 5, 0, true, 11, 10));
        }
        if(PlayerController.gridSize<10){
            ObjectList.Add(new Object("Expansion_10x10", 0, 20, 0, true, 11, 0));
            ObjectList.Add(new Object("Expansion_10x10", 30000, 0, 0, true, 11, 15));
        }
        if(PlayerController.gridSize<11){
            ObjectList.Add(new Object("Expansion_11x11", 0, 30, 0, true, 11, 0));
            ObjectList.Add(new Object("Expansion_11x11", 100000, 10, 0, true, 11, 20));
        }
        if(PlayerController.gridSize<12){
            ObjectList.Add(new Object("Expansion_12x12", 0, 40, 0, true, 11, 0));
            ObjectList.Add(new Object("Expansion_12x12", 200000, 0, 0, true, 11, 25));
        }
        if(PlayerController.gridSize<13){
            ObjectList.Add(new Object("Expansion_13x13", 0, 50, 0, true, 11, 0));
            ObjectList.Add(new Object("Expansion_13x13", 1000000, 20, 0, true, 11, 35));
        }
        if(PlayerController.gridSize<14){
            ObjectList.Add(new Object("Expansion_14x14", 0, 60, 0, true, 11, 0));
            ObjectList.Add(new Object("Expansion_14x14", 2000000, 0, 0, true, 11, 45));
        }
        if(PlayerController.gridSize<15){
            ObjectList.Add(new Object("Expansion_15x15", 0, 70, 0, true, 11, 0));
            ObjectList.Add(new Object("Expansion_15x15", 4000000, 30, 0, true, 11, 60));
        }
        if(PlayerController.gridSize<16){
            ObjectList.Add(new Object("Expansion_16x16", 0, 80, 0, true, 11, 0));
            ObjectList.Add(new Object("Expansion_16x16", 8000000, 30, 0, true, 11, 75));
        }

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

    //suche in der Objectliste nach einem expansionItem was kleiner als das übergebene ist, wenn ja -> nicht das kleinste
    public bool isLowestExpansionItem(string name){
        string[] splitNumber = name.Split("x");

        //splitte jeden item name und suche nach expansion, wenn gefunden suche nach der größer der expnasion
        foreach(Object item in ObjectList){
            if(item.spriteName.Split("_")[0].Equals("Expansion")){
                
                //gucke ob das gefundene item kleiner ist
                if(Int32.Parse(item.spriteName.Split("x")[2])<Int32.Parse(splitNumber[2])){
                    return false;
                }
            }
        }
        return true;
    }
}

public class Object{
    public string spriteName { get; set; }
    public int priceMoney { get; set; }
    public int priceGold { get; set; }
    public int inBackup { get; set; }
    public bool isActive { get; set; }
    public int sortiment { get; set; }//0=chair,1=table,2=wall,3=door,4=walldeko,5=floor,6=deko,7=oven,8=counter,9=fridge,10=slushi,11=expansion
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
