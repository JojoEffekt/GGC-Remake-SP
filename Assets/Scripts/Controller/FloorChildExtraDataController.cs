using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FloorChildExtraDataController : MonoBehaviour
{
    public static List<Chair> ChairList = new List<Chair>();
    public static List<Table> TableList = new List<Table>();
    public static List<Counter> CounterList = new List<Counter>();
    public static List<Oven> OvenList = new List<Oven>();
    public static List<Slushi> SlushiList = new List<Slushi>();

    //methodes
    public static void InstantiateFCED(string data){//erstellt ein Object und speichert dieses
        string[] listItem = data.Split(";");//type;GON;...;...
        string type = listItem[0];

        if(ObjectAlreadyExist(listItem[1])==false){//wenn das object noch nicht existiert
            if(type.Equals("Chair")){
                if(listItem.Length>2){
                    ChairList.Add(new Chair(listItem[0], listItem[1], Convert.ToBoolean(listItem[2])));
                }else{
                    ChairList.Add(new Chair(listItem[0], listItem[1], true));
                }
            }else if(type.Equals("Table")){
                if(listItem.Length>2){
                    TableList.Add(new Table(listItem[0], listItem[1], Convert.ToBoolean(listItem[2])));
                }else{
                    TableList.Add(new Table(listItem[0], listItem[1], true));
                }
            }else if(type.Equals("Counter")){
                if(listItem.Length>2){
                    CounterList.Add(new Counter(listItem[0], listItem[1], Convert.ToBoolean(listItem[2]), listItem[3], Int32.Parse(listItem[4])));   
                }else{
                    CounterList.Add(new Counter(listItem[0], listItem[1], true, null, 0));
                }
            }else if(type.Equals("Oven")){
                if(listItem.Length>2){
                    OvenList.Add(new Oven(listItem[0], listItem[1], Int32.Parse(listItem[2]), listItem[3], Int32.Parse(listItem[4]), listItem[5], listItem[6]));   
                }else{
                    OvenList.Add(new Oven(listItem[0], listItem[1], 0, null, 0, null, null));
                }
            }else if(type.Equals("Slushi")){
                if(listItem.Length>2){
                    SlushiList.Add(new Slushi(listItem[0], listItem[1], listItem[2], Int32.Parse(listItem[3])));  
                }else{ 
                    SlushiList.Add(new Slushi(listItem[0], listItem[1], null, 0));  
                }
            }
        }
    }

    public static bool ObjectAlreadyExist(string objName){
        if(getChair(objName)!=null){
            return true;
        }
        if(getTable(objName)!=null){
            return true;
        }
        if(getCounter(objName)!=null){
            return true;
        }
        if(getOven(objName)!=null){
            return true;
        }
        if(getSlushi(objName)!=null){
            return true;
        }
        return false;
    }

    public static void DeleteFCED(string floorChildGOName){
        string floorGOName = getFloorGONameFromFloorChildGOName(floorChildGOName);

        for(int a=0;a<ChairList.Count;a++){
            if(ChairList[a].gameObjectName.Equals(floorGOName)){
                ChairList.RemoveAt(a);
            }
        }
        for(int b=0;b<TableList.Count;b++){
            if(TableList[b].gameObjectName.Equals(floorGOName)){
                TableList.RemoveAt(b);
            }
        }
        for(int c=0;c<CounterList.Count;c++){
            if(CounterList[c].gameObjectName.Equals(floorGOName)){
                CounterList.RemoveAt(c);
            }
        }
        for(int d=0;d<OvenList.Count;d++){
            if(OvenList[d].gameObjectName.Equals(floorGOName)){
                OvenList.RemoveAt(d);
            }
        }
        for(int e=0;e<SlushiList.Count;e++){
            if(SlushiList[e].gameObjectName.Equals(floorGOName)){
                SlushiList.RemoveAt(e);
            }
        }
    }

    public static void CloneFCED(string oldFloorChildGOName, string newFloorGOName){
        //sucht anhand des GONamen das Object und erzeugt ein neues Objekt mit übegebenden werten
        oldFloorChildGOName = getFloorGONameFromFloorChildGOName(oldFloorChildGOName);

        string data = "";
        if(getChair(oldFloorChildGOName)!=null){
            data = getChair(oldFloorChildGOName).getData();
        }
        if(getTable(oldFloorChildGOName)!=null){
            data = getTable(oldFloorChildGOName).getData();
        }
        if(getCounter(oldFloorChildGOName)!=null){
            data = getCounter(oldFloorChildGOName).getData();
        }
        if(getOven(oldFloorChildGOName)!=null){
            data = getOven(oldFloorChildGOName).getData();
        }
        if(getSlushi(oldFloorChildGOName)!=null){
            data = getSlushi(oldFloorChildGOName).getData();
        }
        
        //data muss an 2 stelle gesplittet werden und das element muss mit dem "newFloorGOName" ausgetauscht werden
        string[] items = data.Split(";");
        data = "";
        for(int a=0;a<items.Length;a++){
            if(a==1){
                data += newFloorGOName+";";
            }else{
                data += items[a]+";";
            }
        }
        data = data.Remove(data.Length-1, 1);//löscht das überflüssige ";" am ende

        if(data!=""){
            InstantiateFCED(data);
        }
    }

    public static void ChangeFCEDData(string data){
        string[] listItem = data.Split(";");//type;GON;...;...
        string type = listItem[0];

        if(type.Equals("Chair")){
            Chair chair = getChair(listItem[1]);
            chair.setData(Convert.ToBoolean(listItem[2]));
        }else if(type.Equals("Table")){
            Table table = getTable(listItem[1]);
            table.setData(Convert.ToBoolean(listItem[2]));
        }else if(type.Equals("Counter")){
            Counter counter = getCounter(listItem[1]);
            counter.setData(Convert.ToBoolean(listItem[2]), listItem[3], Int32.Parse(listItem[4]));
        }else if(type.Equals("Oven")){
            Oven oven = getOven(listItem[1]);
            oven.setData(Int32.Parse(listItem[2]), listItem[3], Int32.Parse(listItem[4]), listItem[5], listItem[6]);
        }else if(type.Equals("Slushi")){
            Slushi slushi = getSlushi(listItem[1]);
            slushi.setData(listItem[2], Int32.Parse(listItem[2]));
        }
    }



    //getter    
    public static Chair getChair(string objName){for(int a=0;a<ChairList.Count;a++){if(ChairList[a].gameObjectName.Equals(objName)){return ChairList[a];}}return null;}
    public static Table getTable(string objName){for(int a=0;a<TableList.Count;a++){if(TableList[a].gameObjectName.Equals(objName)){return TableList[a];}}return null;}
    public static Counter getCounter(string objName){for(int a=0;a<CounterList.Count;a++){if(CounterList[a].gameObjectName.Equals(objName)){return CounterList[a];}}return null;}
    public static Oven getOven(string objName){for(int a=0;a<OvenList.Count;a++){if(OvenList[a].gameObjectName.Equals(objName)){return OvenList[a];}}return null;}
    public static Slushi getSlushi(string objName){for(int a=0;a<SlushiList.Count;a++){if(SlushiList[a].gameObjectName.Equals(objName)){return SlushiList[a];}}return null;}

    public static void getInfo(){
        for(int a=0;a<ChairList.Count;a++){
            Debug.Log(ChairList[a].getData());
        }
        for(int b=0;b<TableList.Count;b++){
            Debug.Log(TableList[b].getData());
        }
        for(int c=0;c<CounterList.Count;c++){
            Debug.Log(CounterList[c].getData());
        }
        for(int d=0;d<OvenList.Count;d++){
            Debug.Log(OvenList[d].getData());
        }
        for(int e=0;e<SlushiList.Count;e++){
            Debug.Log(SlushiList[e].getData());
        }
        Debug.Log("");
    }

    public static string getFloorGONameFromFloorChildGOName(string floorChildGOName){
        string[] name = floorChildGOName.Split("-");
        return name[0]+"-"+name[1];
    }
}

public class Chair{
    public string type { get; set; }
    public string gameObjectName { get; set; }
    public bool isEmpty { get; set; }
    public Chair(string type, string gameObjectName, bool isEmpty){
        this.type = type;
        this.gameObjectName = gameObjectName;
        this.isEmpty = isEmpty;
    }
    public string getData(){
        return type+";"+gameObjectName+";"+isEmpty;
    }
    public void setData(bool isEmpty){
        this.isEmpty = isEmpty;
    }
}

public class Table{
    public string type { get; set; }
    public string gameObjectName { get; set; }
    public bool isEmpty { get; set; }
    public Table(string type, string gameObjectName, bool isEmpty){
        this.type = type;
        this.gameObjectName = gameObjectName;
        this.isEmpty = isEmpty;
    }
    public string getData(){
        return type+";"+gameObjectName+";"+isEmpty;
    }
    public void setData(bool isEmpty){
        this.isEmpty = isEmpty;
    }
}

public class Counter{
    public string type { get; set; }
    public string gameObjectName { get; set; }
    public bool isEmpty { get; set; }
    public string foodSprite { get; set; }
    public int foodCount { get; set; }
    public Counter(string type, string gameObjectName, bool isEmpty, string foodSprite, int foodCount){
        this.type = type;
        this.gameObjectName = gameObjectName;
        this.isEmpty = isEmpty;
        this.foodSprite = foodSprite;
        this.foodCount = foodCount;
    }
    public string getData(){
        return type+";"+gameObjectName+";"+isEmpty+";"+foodSprite+";"+foodCount;
    }
    public void setData(bool isEmpty, string foodSprite, int foodCount){
        this.isEmpty = isEmpty;
        this.foodSprite = foodSprite;
        this.foodCount = foodCount;
    }

    //methode zum Instanziierung der sprites bei Essenslieferung etc

    //methode zum aktualisieren bei -1 food

    //methode zum laden der counterinfo(hoverover)
}

public class Oven{
    public string type { get; set; }
    public string gameObjectName { get; set; }
    public int foodStep { get; set; }
    public string foodSprite { get; set; }
    public int foodCount { get; set; }
    public string dateStart { get; set; }
    public string dateEnd { get; set; }
    public Oven(string type, string gameObjectName, int foodStep, string foodSprite, int foodCount, string dateStart, string dateEnd){
        this.type = type;
        this.gameObjectName = gameObjectName;
        this.foodStep = foodStep;//0 = empty, 1 = in Proccess, 2 = ready, 3 = dirty
        this.foodSprite = foodSprite;
        this.foodCount = foodCount;
        this.dateStart = dateStart;
        this.dateEnd = dateEnd;
    }
    public string getData(){
        return type+";"+gameObjectName+";"+foodStep+";"+foodSprite+";"+foodCount+";"+dateStart+";"+dateEnd;
    }
    public void setData(int foodStep, string foodSprite, int foodCount, string dateStart, string dateEnd){
        this.foodStep = foodStep;
        this.foodSprite = foodSprite;
        this.foodCount = foodCount;
        this.dateStart = dateStart;
        this.dateEnd = dateEnd;
    }

    //methode zum Instanziierung der sprites bei Zutaten hinzufügen

    //methode zum Instanziierung der sprites bei Kochen

    //methode zum Instanziierung der sprites bei Gericht fertig
}

public class Slushi{
    public string type { get; set; }
    public string gameObjectName { get; set; }
    public string cocktailSprite { get; set; }
    public int cocktailCount { get; set; }
    public Slushi(string type, string gameObjectName, string cocktailSprite, int cocktailCount){
        this.type = type;
        this.gameObjectName = gameObjectName;
        this.cocktailSprite = cocktailSprite;
        this.cocktailCount = cocktailCount;
    }
    public string getData(){
        return type+";"+gameObjectName+";"+cocktailSprite+";"+cocktailCount;
    }
    public void setData(string cocktailSprite, int cocktailCount){
        this.cocktailSprite = cocktailSprite;
        this.cocktailCount = cocktailCount;
    }

    //methode zum Instanziierung der sprites bei Slushi vorhanden etc

    //methode zum aktualisieren bei -1 slushi

    //methode zum laden der slushiinfo(hoverover)
}
