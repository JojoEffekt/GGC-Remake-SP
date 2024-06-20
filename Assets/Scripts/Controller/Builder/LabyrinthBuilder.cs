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


        buildNPCpath();
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





    //TEST
    //path for npc (better performance)
    //sucht den path anhand eines start und end punktes, returnt leere list bei fehler
    public static void buildNPCpath(){
        //hole die door pos
        /*
        gucke ab türpos in "gridMap"(0 begehbar, 1 nicht begehbar)
        und baue eine 2te "gridMap" nur mit begehbaren feldern ab doorpos
        */

        List<string> positions = new List<string>();

        Debug.Log("gridMap.Length: "+gridMap.Length);
        for(int a=0;a<PlayerController.gridSize;a++){
            for(int b=0;b<PlayerController.gridSize;b++){
                Debug.Log("Coord: ["+a+":"+b+"] "+gridMap[a,b]);
            }
        }

        int[] doorPos = PlayerMovementController.FindDoorPos();

        //startposition ist die tür
        int step = 0;
        positions.Add(doorPos[0]+":"+(doorPos[1])+":"+step);

        npcPath(positions, step);
    }

    public static void npcPath(List<string> position, int step){

        //alle werte
        for(int a=0;a<position.Count;a++){

            //werte der aktuellen position
            int itemX = Int32.Parse(position[a].Split(":")[0]);
            int itemY = Int32.Parse(position[a].Split(":")[1]);
            int itemStep = Int32.Parse(position[a].Split(":")[2]);

            //suche alle felder mit dem aktuellen step
            if(itemStep==step){
                //suche von item rechts oben
                if(itemY-1>=0){
                    //position ist begehbar
                    if(gridMap[itemX, itemY-1]==0){
                        //gucke noch ob position nicht schon vorhanden ist in "position"
                        if(!InList(position, itemX, itemY-1)){
                            //füge den nachbarn hinzu mit einem höheren step der in der nächsten iteration gecheckt werden kann
                            position.Add(itemX+":"+(itemY-1)+":"+(step+1));
                            Debug.Log("Add: "+itemX+":"+(itemY-1));
                        }
                    }
                }
                //rechts unten
                if(itemX+1<=PlayerController.gridSize-1){
                    //Debug.Log("rechts unten "+gridMap[Int32.Parse(position[a].Split(":")[0])+1,Int32.Parse(position[a].Split(":")[1])]);
                    if(gridMap[itemX+1,itemY]==0){
                        if(!InList(itemX+1, itemY)){
                            position.Add((itemX+1)+":"+itemY+":"+(step+1));
                            Debug.Log("Add: "+(itemX+1)+":"+itemY);
                        }
                    }
                }
                //links unten
                if(itemY+1<=PlayerController.gridSize-1){
                    //Debug.Log("links unten "+gridMap[Int32.Parse(position[a].Split(":")[0]),Int32.Parse(position[a].Split(":")[1])+1]);
                    if(gridMap[itemX,itemY+1]==0){
                        if(!InList(position, itemX, itemY+1)){
                            position.Add(itemX+":"+(itemY+1)+":"+(step+1));
                            Debug.Log("Add: "+itemX+":"+(itemY+1));
                        }
                    }
                }
                //links oben
                if(itemX-1>=0){
                    //Debug.Log("links oben "+gridMap[Int32.Parse(position[a].Split(":")[0])-1,Int32.Parse(position[a].Split(":")[1])]);
                    if(gridMap[itemX-1,itemY]==0){
                        if(!InList(position, itemX-1, itemY)){
                            position.Add((itemX-1)+":"+itemY+":"+(step+1));
                            Debug.Log("Add: "+(itemX-1)+":"+itemY);
                        }
                    }
                }
            }
        }

        //wenn ein weiteres element hinzugefügt wurde rufe sich selbs nochmal auf
        if(position[position.Count-1].Split(":")[2].Equals(step+1)){
            //rufe npcPath wieder auf und erhöhe step +1
            npcPath(position, step+1);
        }else{
            //fertig, zeige alles
            Debug.Log("fertig!");
            /*for(int a=0;a<position.Count;a++){
                Debug.Log("positionen: "+position[a]);
            }*/
        }
    }

    public static bool InList(List<string> position, int x, int y){
        for(int a=0;a<position.Count;a++){
            int oldX = Int32.Parse(position[a].Split(":")[0]);
            int oldY = Int32.Parse(position[a].Split(":")[1]);
            if(oldX==x&&oldY==y){
                return true;
            }
        }
        return false;
    }
}
