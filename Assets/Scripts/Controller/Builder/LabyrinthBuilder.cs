using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LabyrinthBuilder : MonoBehaviour
{
    /*

    coord[x,y]=:

          0,0
           /\
          /\/\
       Y /\/\/\ X
        / .... \
    y,0/        \0,x
    
    */

    //beinhaltet die aktuellen mapdaten wo sich der spieler bewegen kann
    public static int[,] gridMap { get; set; }


    //enhält alle möglichen positionen auf die sich npcs von der tür 
    //aus, bewegen können
    public static List<string> npcPath = new List<string>();

    //alle möglichen positionen für den aktuellen waiter
    //TEMP MAYBE ZU RESCCOURENAUFWENDIG (da theoretisch pro waiter pro sekunde)
    public static List<string> waiterPath = new List<string>();



    //managed die die eingegebenen coords und sucht ein path
    public static List<string> LabyrinthManager(int[] startPos, int[] endPos){

        //sucht Potenzielle wege anhand von start- endPos
        List<string> unsortedPath = getPathData(startPos, endPos);

        //sucht den kürzesten pfad
        List<string> shortestPath = getShortestPath(unsortedPath, endPos);

        //wenn ein weg gefunden wurde, lösche die stelle an der der spieler steht
        /*if(shortestPath.Count!=0){
            shortestPath.RemoveAt(0);
        }*/

        //gibt den weg zurück
        return shortestPath;
    }



    //erstellt das grid indem der player laufen darf
    //muss nach jeder veränderung des Spielfeldes aufgerufen werden
    public static void GenerateGrid(){

        //erstellt die grid größe
        int gridSize = PlayerController.gridSize;
        gridMap = new int[gridSize,gridSize]; 

        //sucht alle FloorChildObjecte in der scene und markiert diese in der gridMap
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach(GameObject item in allObjects){
            string[] split = item.name.Split("-");
            if(split.Length==3&&split[2].Equals("Child")){

                //1=Object, cant walk there, makiert die position wo der spieler nicht laufen darf
                gridMap[Int32.Parse(split[0]),Int32.Parse(split[1])] = 1;
            }
        }

        //erstellt ein vereinfachtes grid extra nur für die npcs
        NPCPathManager();
    }

    //sucht den path anhand eines start und end punktes, returnt leere list bei fehler
    public static List<string> getPathData(int[] startPos, int[] endPos){

        //spielfeldgröße
        int gridSize = PlayerController.gridSize;

        //zählt bei wievielen schritten
        int step = 0;

        //beinhaltet die möglichen position mit den jeweiligen stepcounter (x,y,step)
        List<string> positions = new List<string>();

        //guckt ob die startposition möglich ist
        if(gridMap[startPos[0],startPos[1]]==1){
            return positions;
        }
        //guckt ob die endposition möglich ist
        if(gridMap[endPos[0],endPos[1]]==1){
            return positions;
        }


        //startet den zyklus mit der startpos
        positions.Add(startPos[0]+":"+(startPos[1])+":"+step);

        //die anzahl den flächen des aktuellen spielfelds(x^2 ist max mögliche anzahl an zu gehenden felder)
        for(int a=0;a<gridSize*gridSize;a++){

            //gehe alle positions in positionlist durch
            for(int b=0;b<positions.Count;b++){

                //sucht nur die nachbarn von positions mit der aktuellen stepPos zahl 
                if(Int32.Parse(positions[b].Split(":")[2])==step){

                    //suche nachbar über aktuelle pos
                    if(FindNeighbour(Int32.Parse(positions[b].Split(":")[0]),Int32.Parse(positions[b].Split(":")[1])-1,positions)){
                        positions.Add(Int32.Parse(positions[b].Split(":")[0])+":"+(Int32.Parse(positions[b].Split(":")[1])-1)+":"+(step+1));
                    }
                    //suche nachbar rechts von aktuelle pos
                    if(FindNeighbour(Int32.Parse(positions[b].Split(":")[0])-1,Int32.Parse(positions[b].Split(":")[1]),positions)){
                        positions.Add((Int32.Parse(positions[b].Split(":")[0])-1)+":"+Int32.Parse(positions[b].Split(":")[1])+":"+(step+1));
                    }
                    //suche nachbar unter aktuelle pos
                    if(FindNeighbour(Int32.Parse(positions[b].Split(":")[0]),Int32.Parse(positions[b].Split(":")[1])+1,positions)){
                        positions.Add(Int32.Parse(positions[b].Split(":")[0])+":"+(Int32.Parse(positions[b].Split(":")[1])+1)+":"+(step+1));
                    }
                    //suche nachbar links von aktuelle pos
                    if(FindNeighbour(Int32.Parse(positions[b].Split(":")[0])+1,Int32.Parse(positions[b].Split(":")[1]),positions)){
                        positions.Add((Int32.Parse(positions[b].Split(":")[0])+1)+":"+Int32.Parse(positions[b].Split(":")[1])+":"+(step+1));
                    }
                }
            }

            //gucke ob zielPos erreicht wurde
            foreach(var item in positions){
                if(Int32.Parse(item.Split(":")[0])==endPos[0]&&Int32.Parse(item.Split(":")[1])==endPos[1]){

                    //möglicher weg gefunden, gib zurück!
                    return positions;
                }
            }

            //stepPos muss um 1 erhöht werden, da alle nachbarn von den position mit der aktuellen stzepAnzahl gefunden wurden
            step = step + 1;
        }

        //kein weg wurde gefunden, gib leere liste zurück
        positions.Clear();
        return positions;
    }

    //guckt ob das gegebene feld frei ist und noch nicht in der positions liste vorhanden ist
    public static bool FindNeighbour(int x, int y, List<string> list){

        //guckt ob feld existiert
        if(x>=0&&x<PlayerController.gridSize&&y>=0&&y<PlayerController.gridSize){

            //guckt ob feld frei ist
            if(gridMap[x,y]==0){

                //guck ob der neighbour schon in der positions list ist
                foreach(var item in list){
                    if(Int32.Parse(item.Split(":")[0])==x&&Int32.Parse(item.Split(":")[1])==y){
                        return false;
                    }
                }

                //ist begehbar und nicht in der liste vorhanden
                return true;
            }
        }
        return false;
    }

    //sucht den kürzesten weg anhand von unsortierten daten
    public static List<string> getShortestPath(List<string> unsortedPath, int[] endPos){

        //beinhaltet die fertigen daten
        List<string> shortestPath = new List<string>();

        //guckt ob überhaupt daten vorhanden sind
        if(unsortedPath.Count!=0){

            //beginnt beim ende und hört beim anfang auf
            int[] curPos = endPos;
            int step = Int32.Parse(unsortedPath[unsortedPath.Count-1].Split(":")[2]);

            //speichert die endposition
            shortestPath.Add(endPos[0]+":"+endPos[1]+":"+step);

            //sucht ein feld umliegend von der curPos mit step = step-1 als bei der curPos, speicher diesen wert und beginne von neuem bis step 0 gefunden wurde(anfang)
            for(int a=0;a<unsortedPath.Count;a++){
                for(int b=0;b<unsortedPath.Count;b++){
                    if(Int32.Parse(unsortedPath[b].Split(":")[0])==curPos[0]&&Int32.Parse(unsortedPath[b].Split(":")[1])-1==curPos[1]&&Int32.Parse(unsortedPath[b].Split(":")[2])==(step-1)){
                        shortestPath.Add(unsortedPath[b]);
                        step = step - 1;
                        curPos[0] = Int32.Parse(unsortedPath[b].Split(":")[0]);
                        curPos[1] = Int32.Parse(unsortedPath[b].Split(":")[1]);
                    }
                    if(Int32.Parse(unsortedPath[b].Split(":")[0])-1==curPos[0]&&Int32.Parse(unsortedPath[b].Split(":")[1])==curPos[1]&&Int32.Parse(unsortedPath[b].Split(":")[2])==(step-1)){
                        shortestPath.Add(unsortedPath[b]);
                        step = step - 1;
                        curPos[0] = Int32.Parse(unsortedPath[b].Split(":")[0]);
                        curPos[1] = Int32.Parse(unsortedPath[b].Split(":")[1]);
                    }
                    if(Int32.Parse(unsortedPath[b].Split(":")[0])==curPos[0]&&Int32.Parse(unsortedPath[b].Split(":")[1])+1==curPos[1]&&Int32.Parse(unsortedPath[b].Split(":")[2])==(step-1)){
                        shortestPath.Add(unsortedPath[b]);
                        step = step - 1;
                        curPos[0] = Int32.Parse(unsortedPath[b].Split(":")[0]);
                        curPos[1] = Int32.Parse(unsortedPath[b].Split(":")[1]);
                    }
                    if(Int32.Parse(unsortedPath[b].Split(":")[0])+1==curPos[0]&&Int32.Parse(unsortedPath[b].Split(":")[1])==curPos[1]&&Int32.Parse(unsortedPath[b].Split(":")[2])==(step-1)){
                        shortestPath.Add(unsortedPath[b]);
                        step = step - 1;
                        curPos[0] = Int32.Parse(unsortedPath[b].Split(":")[0]);
                        curPos[1] = Int32.Parse(unsortedPath[b].Split(":")[1]);
                    }
                }
            }
        }

        //gibt die list in gedrheter ordnung zurück
        shortestPath.Reverse();
        return shortestPath;
    }





    //path for npc (better performance)
    //sucht den path anhand eines start und end punktes, returnt leere list bei fehler
    public static void NPCPathManager(){

        //leere die liste, damit sie neu erstellt werden kann
        npcPath.Clear();

        //sucht die tür pos um sie als erste begehbare koordinate hinzuzufügen
        int[] doorPos = PlayerMovementController.FindDoorPos();

        //startposition ist die tür
        int step = 0;
        npcPath.Add(doorPos[0]+":"+(doorPos[1])+":"+step);

        //erstellt dei neue liste
        buildNPCPath(step);
    }

    //baut die npcPath liste mit allen möglichen werten auf die der npc gehen kann
    public static void buildNPCPath(int step){

        //alle werte
        for(int a=0;a<npcPath.Count;a++){

            //werte der aktuellen position
            int itemX = Int32.Parse(npcPath[a].Split(":")[0]);
            int itemY = Int32.Parse(npcPath[a].Split(":")[1]);
            int itemStep = Int32.Parse(npcPath[a].Split(":")[2]);

            //suche alle felder mit dem aktuellen step
            if(itemStep==step){
                //suche von item rechts oben
                if(itemY-1>=0){
                    //position ist begehbar
                    if(gridMap[itemX, itemY-1]==0){
                        //gucke noch ob position nicht schon vorhanden ist in "position"
                        if(!InList(itemX, itemY-1)){
                            //füge den nachbarn hinzu mit einem höheren step der in der nächsten iteration gecheckt werden kann
                            npcPath.Add(itemX+":"+(itemY-1)+":"+(step+1));
                        }
                    }
                }
                //rechts unten
                if(itemX+1<=PlayerController.gridSize-1){
                    if(gridMap[itemX+1,itemY]==0){
                        if(!InList(itemX+1, itemY)){
                            npcPath.Add((itemX+1)+":"+itemY+":"+(step+1));
                        }
                    }
                }
                //links unten
                if(itemY+1<=PlayerController.gridSize-1){
                    if(gridMap[itemX,itemY+1]==0){
                        if(!InList(itemX, itemY+1)){
                            npcPath.Add(itemX+":"+(itemY+1)+":"+(step+1));
                        }
                    }
                }
                //links oben
                if(itemX-1>=0){
                    if(gridMap[itemX-1,itemY]==0){
                        if(!InList(itemX-1, itemY)){
                            npcPath.Add((itemX-1)+":"+itemY+":"+(step+1));
                        }
                    }
                }
            }
        }

        //wenn ein weiteres element hinzugefügt wurde rufe sich selbs nochmal auf
        if(Int32.Parse(npcPath[npcPath.Count-1].Split(":")[2])==(step+1)){
            //rufe npcPath wieder auf und erhöhe step +1
            //wodurch alle positions auf nachbarn geprüft werden, die gerade hinzugefügt wurden
            buildNPCPath(step+1);
        }
    }

    //guckt ob eine bestimmte koordinate in der npcPath liste ist
    public static bool InList(int x, int y){
        for(int a=0;a<npcPath.Count;a++){
            int oldX = Int32.Parse(npcPath[a].Split(":")[0]);
            int oldY = Int32.Parse(npcPath[a].Split(":")[1]);
            if(oldX==x&&oldY==y){
                return true;
            }
        }
        return false;
    }

    //guckt ob eine bestimmte koordinate in der waiterPath liste ist
    public static bool InWaiterList(int x, int y){
        for(int a=0;a<waiterPath.Count;a++){
            int oldX = Int32.Parse(waiterPath[a].Split(":")[0]);
            int oldY = Int32.Parse(waiterPath[a].Split(":")[1]);
            if(oldX==x&&oldY==y){
                return true;
            }
        }
        return false;
    }
    


    //holt den path für eine npc route
    public static List<string> getNPCPath(int[] endPos){

        //enthält die kopierte route
        List<string> route = new List<string>();
        for(int a=0;a<npcPath.Count;a++){
            route.Add(npcPath[a]);
        }

        //finale liste des npcs
        List<string> finaleRoute = new List<string>();

        //gucke ob endposition in npcPath vorhanden ist und benutzte sie als start
        //enthält die endkoordinate und diee position in der liste
        string[] value = npcDestinationInPath(endPos);
        if(value!=null){ 
            
            int x = Int32.Parse(value[0].Split(":")[0]);
            int y = Int32.Parse(value[0].Split(":")[1]);
            int step = Int32.Parse(value[0].Split(":")[2]);

            //endposition wurde in liste gefunden, lösche alle werte die einen höheren step haben
            //beginne ab der nachfolgenden position von der end pos, da alle darüber einen größeren step haben
            for(int a=route.Count-1;a>Int32.Parse(value[1])+1;a--){
                route.RemoveAt(a);
            }

            //beginne von ende und suche einen listeintrag mit step-1
            //gucke ob er nachbar von voheriger pos ist
            finaleRoute.Add(value[0]);
            for(int a=step-1;a>0;a--){
                for(int b=0;b<route.Count;b++){
                    //hat position step-1
                    if(Int32.Parse(route[b].Split(":")[2])==a){
                        //prüfe ob die position ein kleinerer nachbar von letzter finalRoute eintrag ist
                        if(IsNeighbour(route[b], finaleRoute[finaleRoute.Count-1])){
                            finaleRoute.Add(route[b]);
                        }
                    }
                }
            }
            //füge startposition hinzu
            finaleRoute.Add(npcPath[0]);
        }

        //reverse list
        finaleRoute.Reverse();

        return finaleRoute;
    }

    //weg für waiter
    public static List<string> getWaiterPath(int[] startPos, int[] endPos){
    
        //enthält die kopierte route
        List<string> route = new List<string>();

        //finale liste des waiters
        List<string> finaleRoute = new List<string>();

        //gucke ob endposition und startPos in der begehbaren zone liegt
        string[] startPosValue = npcDestinationInPath(startPos);
        string[] endPosValue = npcDestinationInPath(endPos);
        if(startPosValue!=null&&endPosValue!=null){

            //bau path neu für jeden waiter (funktioniert nur wenn tür mit start/stop pos von waiter verbunden ist)
            WaiterPathManager(startPos);

            for(int a=0;a<waiterPath.Count;a++){
                route.Add(waiterPath[a]);
            }

            int step = Int32.Parse(endPosValue[0].Split(":")[2]);

            //endposition wurde in liste gefunden, lösche alle werte die einen höheren step haben
            //beginne ab der nachfolgenden position von der end pos, da alle darüber einen größeren step haben
            for(int a=route.Count-1;a>Int32.Parse(endPosValue[1])+1;a--){
                route.RemoveAt(a);
            }

            //beginne von ende und suche einen listeintrag mit step-1
            //gucke ob er nachbar von voheriger pos ist
            finaleRoute.Add(endPosValue[0]);
            for(int a=step-1;a>0;a--){
                for(int b=0;b<route.Count;b++){
                    //hat position step-1
                    if(Int32.Parse(route[b].Split(":")[2])==a){
                        //prüfe ob die position ein kleinerer nachbar von letzter finalRoute eintrag ist
                        if(IsNeighbour(route[b], finaleRoute[finaleRoute.Count-1])){
                            finaleRoute.Add(route[b]);
                        }
                    }
                }
            }
            //füge startposition hinzu
            finaleRoute.Add(waiterPath[0]);
        }

        finaleRoute.Reverse();

        return finaleRoute;
    }

    //baut die list die alle möglichen zu begehbaren position des aktuellen waiters enthält mit der 
    //position wie viele schritte bis dahin benötigt werden
    public static void WaiterPathManager(int[] startPos){

        //leere die liste, damit sie neu erstellt werden kann
        waiterPath.Clear();

        //startposition ist die akutelle position des aktuellen waiters
        int step = 0;
        waiterPath.Add(startPos[0]+":"+(startPos[1])+":"+step);

        //erstellt eine neue liste
        BuildWaiterPath(step);
    }

    //baut die npcPath liste mit allen möglichen werten auf die der npc gehen kann
    public static void BuildWaiterPath(int step){

        //alle werte
        for(int a=0;a<waiterPath.Count;a++){

            //werte der aktuellen position
            int itemX = Int32.Parse(waiterPath[a].Split(":")[0]);
            int itemY = Int32.Parse(waiterPath[a].Split(":")[1]);
            int itemStep = Int32.Parse(waiterPath[a].Split(":")[2]);

            //suche alle felder mit dem aktuellen step
            if(itemStep==step){
                //suche von item rechts oben
                if(itemY-1>=0){
                    //position ist begehbar
                    if(gridMap[itemX, itemY-1]==0){
                        //gucke noch ob position nicht schon vorhanden ist in "position"
                        if(!InWaiterList(itemX, itemY-1)){
                            //füge den nachbarn hinzu mit einem höheren step der in der nächsten iteration gecheckt werden kann
                            waiterPath.Add(itemX+":"+(itemY-1)+":"+(step+1));
                        }
                    }
                }
                //rechts unten
                if(itemX+1<=PlayerController.gridSize-1){
                    if(gridMap[itemX+1,itemY]==0){
                        if(!InWaiterList(itemX+1, itemY)){
                            waiterPath.Add((itemX+1)+":"+itemY+":"+(step+1));
                        }
                    }
                }
                //links unten
                if(itemY+1<=PlayerController.gridSize-1){
                    if(gridMap[itemX,itemY+1]==0){
                        if(!InWaiterList(itemX, itemY+1)){
                            waiterPath.Add(itemX+":"+(itemY+1)+":"+(step+1));
                        }
                    }
                }
                //links oben
                if(itemX-1>=0){
                    if(gridMap[itemX-1,itemY]==0){
                        if(!InWaiterList(itemX-1, itemY)){
                            waiterPath.Add((itemX-1)+":"+itemY+":"+(step+1));
                        }
                    }
                }
            }
        }

        //wenn ein weiteres element hinzugefügt wurde rufe sich selbs nochmal auf
        if(Int32.Parse(waiterPath[waiterPath.Count-1].Split(":")[2])==(step+1)){
            //rufe BuildWaiterPath wieder auf und erhöhe step +1
            //wodurch alle positions auf nachbarn geprüft werden, die gerade hinzugefügt wurden
            BuildWaiterPath(step+1);
        }
    }

    //gucke ob endposition in npcPath vorhanden ist
    public static string[] npcDestinationInPath(int[] endPos){
        for(int a=0;a<npcPath.Count;a++){
            int x = Int32.Parse(npcPath[a].Split(":")[0]);
            int y = Int32.Parse(npcPath[a].Split(":")[1]);
            if(x==endPos[0]&&y==endPos[1]){
                return new string[]{npcPath[a],""+a};
            }
        }
        return null;
    }

    //gucke ob eine position der vorgänger einer anderen position ist
    public static bool IsNeighbour(string value, string parent){
        int vX = Int32.Parse(value.Split(":")[0]);
        int vY = Int32.Parse(value.Split(":")[1]);
        int pX = Int32.Parse(parent.Split(":")[0]);
        int pY = Int32.Parse(parent.Split(":")[1]);
        //rechts oben
        if(pX-1==vX&&pY==vY){
            return true;
        }
        //rechts unten
        if(pX==vX&&pY+1==vY){
            return true;
        }
        //links oben
        if(pX==vX&&pY-1==vY){
            return true;
        }
        //links unten
        if(pX+1==vX&&pY==vY){
            return true;
        }
        return false;
    }
}
