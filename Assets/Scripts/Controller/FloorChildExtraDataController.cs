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

    public static List<List<string>> FloorChildExtraDataList = new List<List<string>>();//speichert nur die ParentNamen(FloorChild) des jeweiligen Objects

    public static void InstantiateFCED(string data){//erstellt ein Object und speichert dieses
        string[] listItem = data.Split(";");//type;GON;...;...
        string type = listItem[0];

        FloorChildExtraDataList.Add(new List<string>());
        FloorChildExtraDataList[FloorChildExtraDataList.Count-1].Add(listItem[1]);//speichert den namen(0) und type(1) in der referenzliste
        FloorChildExtraDataList[FloorChildExtraDataList.Count-1].Add(listItem[0]);

        if(type.Equals("Chair")){
            ChairList.Add(new Chair(listItem[0], listItem[1], Convert.ToBoolean(listItem[2])));
        }else if(type.Equals("Table")){
            TableList.Add(new Table(listItem[0], listItem[1], Convert.ToBoolean(listItem[2])));
        }else if(type.Equals("Counter")){
            CounterList.Add(new Counter(listItem[0], listItem[1], Convert.ToBoolean(listItem[2]), listItem[3], Int32.Parse(listItem[4])));
        }else if(type.Equals("Oven")){
            OvenList.Add(new Oven(listItem[0], listItem[1], Int32.Parse(listItem[2]), listItem[3], Int32.Parse(listItem[4]), listItem[5], listItem[6]));
        }else if(type.Equals("Slushi")){
            SlushiList.Add(new Slushi(listItem[0], listItem[1], listItem[2], Int32.Parse(listItem[1])));
        }
    }

    public string getTypeInfo(string ChildGOName){
        string type = getTypeFromChildGOName(ChildGOName);

        if(type.Equals("Chair")){
            Chair chair = getChair(ChildGOName);
            return ""+chair.type+";"+chair.gameObjectName+";"+chair.isEmpty;
        }else if(type.Equals("Table")){
            Table table = getTable(ChildGOName);
            return ""+table.type+";"+table.gameObjectName+";"+table.isEmpty;
        }else if(type.Equals("Counter")){
            Counter counter = getCounter(ChildGOName);
            return ""+counter.type+";"+counter.gameObjectName+";"+counter.isEmpty+";"+counter.foodSprite+";"+counter.foodCount;
        }else if(type.Equals("Oven")){
            Oven oven = getOven(ChildGOName);
            return ""+oven.type+";"+oven.gameObjectName+";"+oven.foodStep+";"+oven.foodSprite+";"+oven.foodCount+";"+oven.dateStart+";"+oven.dateEnd;
        }else if(type.Equals("Slushi")){
            Slushi slushi = getSlushi(ChildGOName);
            return ""+slushi.type+";"+slushi.gameObjectName+";"+slushi.cocktailSprite+";"+slushi.cocktailCount;
        }
        return null;
    }


    //getter
    public static string getTypeFromChildGOName(string objName){
        for(int a=0;a<FloorChildExtraDataList.Count;a++){
            if(FloorChildExtraDataList[a][0].Equals(objName)){
                return FloorChildExtraDataList[a][1];
            }
        }
        return null;
    }

    public static Chair getChair(string objName){
        for(int a=0;a<ChairList.Count;a++){
            if(ChairList[a].gameObjectName.Equals(objName)){
                return ChairList[a];
            }
        }
        return null;
    }

    public static Table getTable(string objName){
        for(int a=0;a<TableList.Count;a++){
            if(TableList[a].gameObjectName.Equals(objName)){
                return TableList[a];
            }
        }
        return null;
    }

    public static Counter getCounter(string objName){
        for(int a=0;a<CounterList.Count;a++){
            if(CounterList[a].gameObjectName.Equals(objName)){
                return CounterList[a];
            }
        }
        return null;
    }

    public static Oven getOven(string objName){
        for(int a=0;a<OvenList.Count;a++){
            if(OvenList[a].gameObjectName.Equals(objName)){
                return OvenList[a];
            }
        }
        return null;
    }

    public static Slushi getSlushi(string objName){
        for(int a=0;a<SlushiList.Count;a++){
            if(SlushiList[a].gameObjectName.Equals(objName)){
                return SlushiList[a];
            }
        }
        return null;
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
}
