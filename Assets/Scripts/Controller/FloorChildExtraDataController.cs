using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

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
                    TableList.Add(new Table(listItem[0], listItem[1], Convert.ToBoolean(listItem[2]), Convert.ToBoolean(listItem[3])));
                }else{
                    TableList.Add(new Table(listItem[0], listItem[1], true, false));
                }
            }else if(type.Equals("Counter")){
                if(listItem.Length>2){
                    CounterList.Add(new Counter(listItem[0], listItem[1], Convert.ToBoolean(listItem[2]), listItem[3], Int32.Parse(listItem[4])));   
                }else{
                    CounterList.Add(new Counter(listItem[0], listItem[1], true, null, 0));
                }
            }else if(type.Equals("Oven")){
                if(listItem.Length>2){
                    OvenList.Add(new Oven(listItem[0], listItem[1], Int32.Parse(listItem[2]), listItem[3], Int32.Parse(listItem[4]), listItem[5],  Int32.Parse(listItem[6])));   
                }else{
                    OvenList.Add(new Oven(listItem[0], listItem[1], 0, null, 0, null, 0));
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

                //löscht, falls vorhanden, das dinner auf dem counter
                CounterController.DeleteDinnerPrefabOnCounter(floorGOName+"-Child-Counter");
            }
        }
        for(int d=0;d<OvenList.Count;d++){
            if(OvenList[d].gameObjectName.Equals(floorGOName)){
                OvenList.RemoveAt(d);

                //löscht, falls vorhanden, das dinner auf dem oven
                DinnerController.DeleteDinnerPrefabOnOven(floorGOName+"-Child-Dinner");
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
            table.setData(Convert.ToBoolean(listItem[2]), Convert.ToBoolean(listItem[3]));
        }else if(type.Equals("Counter")){
            Counter counter = getCounter(listItem[1]);
            counter.setData(Convert.ToBoolean(listItem[2]), listItem[3], Int32.Parse(listItem[4]));
        }else if(type.Equals("Oven")){
            Oven oven = getOven(listItem[1]);
            oven.setData(Int32.Parse(listItem[2]), listItem[3], Int32.Parse(listItem[4]), listItem[5], Int32.Parse(listItem[6]));
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


    //gibt die stepAnzahl für ein Oven FCED wieder
    public static int getOvenStep(string gameObject){

        //holt das FCED vom oven aus der gespeicherten .txt datei
        string item = LoadOvenFCED(gameObject);

        //gucke ob kein leerer string übergeben wurde
        if(item.Length==0){
            //gibt -1 zurück um zu wissen das irgendwas nicht stimmt
            return -1;
        }

        //überprüft die stepanzahl die auf dem oven "liegt"
        if(Int32.Parse(item.Split(";")[2])!=0){
            //wenn string empty("") return 0 damit man weiß das der oven frei ist
            //gibt die stepAnzahl zurück
            return Int32.Parse(item.Split(";")[2]);
        }

        //string ist empty bzw. ==0 also gib 0 zurück
        return 0;
    }

    //läd die FCED und sucht nach object
    public static string LoadOvenFCED(string gameObject){

        string item = "";

        //läd die FCED
        try{
            if(File.Exists(Application.dataPath+"/Data/"+SaveAndLoadController.wallDataFilePath)==true&&File.Exists(Application.dataPath+"/Data/"+SaveAndLoadController.floorDataFilePath)==true&&File.Exists(Application.dataPath+"/Data/"+SaveAndLoadController.floorChildExtraDataFilePath)==true){
                string[] lines = ReadStream(SaveAndLoadController.floorChildExtraDataFilePath);

                for(int a=0;a<lines.Length-1;a++){
                    string[] lineItem = lines[a].Split(";");

                    //ist item oven?
                    if(lineItem[0].Equals("Oven")){

                        //gucke ob es das gesuchte gameObject ist
                        if(lineItem[1].Equals(gameObject.Split("-")[0]+"-"+gameObject.Split("-")[1])){
                            item = lines[a];
                        }
                    }
                }
            }
        }catch{
            return item;
        }

        return item;
    }

    //suche das replatierte ovenObj und gibt den zugehörigen ovenFCED zurück
    public static string FindMovedOvenFCED(){

        //für jedes oven FCED suche ob das dinner existiert
        try{
            if(File.Exists(Application.dataPath+"/Data/"+SaveAndLoadController.wallDataFilePath)==true&&File.Exists(Application.dataPath+"/Data/"+SaveAndLoadController.floorDataFilePath)==true&&File.Exists(Application.dataPath+"/Data/"+SaveAndLoadController.floorChildExtraDataFilePath)==true){
                string[] lines = ReadStream(SaveAndLoadController.floorChildExtraDataFilePath);

                for(int a=0;a<lines.Length-1;a++){
                    string[] lineItem = lines[a].Split(";");

                    //ist item oven und hat ein dinner?
                    if(lineItem[0].Equals("Oven")&&(!lineItem[2].Equals("0"))){

                        //gucke ob es das gesuchte ovenObj noch das  dinner besitzt
                        if(GameObject.Find(lineItem[1]+"-Child-Dinner")==null){
                            return lines[a];
                        }
                    }
                }
            }
        }catch{
            return "";
        }

        return "";
    }

    //suche das replatzierte ovenObj und gibt den zugehörigen counterFCED zurück
    public static string FindMovedCounterFCED(){

        //für jedes oven FCED suche ob das dinner existiert
        try{
            if(File.Exists(Application.dataPath+"/Data/"+SaveAndLoadController.wallDataFilePath)==true&&File.Exists(Application.dataPath+"/Data/"+SaveAndLoadController.floorDataFilePath)==true&&File.Exists(Application.dataPath+"/Data/"+SaveAndLoadController.floorChildExtraDataFilePath)==true){
                string[] lines = ReadStream(SaveAndLoadController.floorChildExtraDataFilePath);

                for(int a=0;a<lines.Length-1;a++){
                    string[] lineItem = lines[a].Split(";");

                    //ist item counter und hat ein dinner?
                    if(lineItem[0].Equals("Counter")&&(!lineItem[3].Equals(""))){

                        //gucke ob es das gesuchte counterObj noch das dinner besitzt
                        if(GameObject.Find(lineItem[1]+"-Child-Counter")==null){
                            return lines[a];
                        }
                    }
                }
            }
        }catch{
            return "";
        }

        return "";
    }
    
    //sucht ein bestimmten object
    public static string getObjectFCED(string name){
        //für jedes oven FCED suche ob der table existiert
        try{
            if(File.Exists(Application.dataPath+"/Data/"+SaveAndLoadController.wallDataFilePath)==true&&File.Exists(Application.dataPath+"/Data/"+SaveAndLoadController.floorDataFilePath)==true&&File.Exists(Application.dataPath+"/Data/"+SaveAndLoadController.floorChildExtraDataFilePath)==true){
                string[] lines = ReadStream(SaveAndLoadController.floorChildExtraDataFilePath);

                for(int a=0;a<lines.Length-1;a++){
                    string[] lineItem = lines[a].Split(";");

                    //ist chair ?
                    if(lineItem[0].Equals("Chair")&&(lineItem[1].Equals(name))){
                        return lines[a];
                    }
                    //ist table?
                    if(lineItem[0].Equals("Table")&&(lineItem[1].Equals(name))){
                        return lines[a];
                    }
                }
            }
        }catch{
            return "";
        }

        return "";
    }

    //sucht alle FCED für bestimmte typ und gibt als list wieder
    public static List<string> getFCEDFromTyp(string type, string param1){

        //liste die die types enthält
        List<string> typeFCED = new List<string>();

        //für jedes oven FCED suche ob das dinner existiert
        try{
            if(File.Exists(Application.dataPath+"/Data/"+SaveAndLoadController.wallDataFilePath)==true&&File.Exists(Application.dataPath+"/Data/"+SaveAndLoadController.floorDataFilePath)==true&&File.Exists(Application.dataPath+"/Data/"+SaveAndLoadController.floorChildExtraDataFilePath)==true){
                string[] lines = ReadStream(SaveAndLoadController.floorChildExtraDataFilePath);

                for(int a=0;a<lines.Length-1;a++){
                    string[] lineItem = lines[a].Split(";");

                    //guckt nach type oven
                    if(lineItem[0].Equals(type)&&type.Equals("Oven")){
                        //gucke ob das gericht in cooking state ist
                        if(lineItem[5].Length!=0){
                            //speichert die gleichen types in liste
                            typeFCED.Add(lines[a]);
                        }
                    }
                    //guckt nach type counter
                    if(lineItem[0].Equals(type)&&type.Equals("Counter")){
                        //gucke ob tresen frei ist oder auf dem tresen das gewünschte gericht drauf liegt
                        if(Convert.ToBoolean(lineItem[2])==true||lineItem[3].Equals(param1)){
                            //speichert die gleichen types in liste
                            typeFCED.Add(lines[a]);
                        }
                    }
                }
            }
        }catch{
        }

        return typeFCED; 
    }

    //gibt alle counter mit zugehöriger data zurück
    public static List<string> getFCEDFromAllCounter(){

        List<string> data = new List<string>();

        try{
            if(File.Exists(Application.dataPath+"/Data/"+SaveAndLoadController.floorChildExtraDataFilePath)==true){
                string[] lines = ReadStream(SaveAndLoadController.floorChildExtraDataFilePath);

                for(int a=0;a<lines.Length-1;a++){
                    string[] lineItem = lines[a].Split(";");

                    //guckt nach type counter
                    if(lineItem[0].Equals("Counter")){
                        //speichert die gleichen types in liste
                        data.Add(lines[a]);
                    }
                }
            }
        }catch{
        }

        return data;    
    }

    //ließt den inhalt der datei aus und gibt sie zurück
    private static string[] ReadStream(string pathFile){
        StreamReader source = new StreamReader(Application.dataPath + "/Data/" + pathFile);
        string fileContents = source.ReadToEnd();
        source.Close();
        string[] lines = fileContents.Split("\n"[0]);
        return lines;
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
    public bool isEmpty { get; set; } // ist npc am tisch?
    public bool isFood { get; set; }  // ist essen auf tisch?

    public Table(string type, string gameObjectName, bool isEmpty, bool isFood){
        this.type = type;
        this.gameObjectName = gameObjectName;
        this.isEmpty = isEmpty;
        this.isFood = isFood;
    }

    public string getData(){
        return type+";"+gameObjectName+";"+isEmpty+";"+isFood;
    }

    public void setData(bool isEmpty, bool isFood){
        this.isEmpty = isEmpty;
        this.isFood = isFood;
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
}

public class Oven{
    public string type { get; set; }
    public string gameObjectName { get; set; }
    public int foodStep { get; set; }
    public string foodSprite { get; set; }
    public int foodCount { get; set; }
    public string dateStart { get; set; }
    public int time { get; set; }
    public Oven(string type, string gameObjectName, int foodStep, string foodSprite, int foodCount, string dateStart, int time){
        this.type = type;
        this.gameObjectName = gameObjectName;
        this.foodStep = foodStep;//100 = onProcess, 0 = empty oven
        this.foodSprite = foodSprite;
        this.foodCount = foodCount;
        this.dateStart = dateStart;
        this.time = time;
    }
    public string getData(){
        return type+";"+gameObjectName+";"+foodStep+";"+foodSprite+";"+foodCount+";"+dateStart+";"+time;
    }
    public void setData(int foodStep, string foodSprite, int foodCount, string dateStart, int time){
        this.foodStep = foodStep;
        this.foodSprite = foodSprite;
        this.foodCount = foodCount;
        this.dateStart = dateStart;
        this.time = time;
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
    public string getData(){
        return type+";"+gameObjectName+";"+cocktailSprite+";"+cocktailCount;
    }
    public void setData(string cocktailSprite, int cocktailCount){
        this.cocktailSprite = cocktailSprite;
        this.cocktailCount = cocktailCount;
    }
}
