using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaiterManager : MonoBehaviour
{
    //beinhaltet das waiter prefab
    private GameObject prefab;

    //beinhaltet alle waiter
    public List<Waiter> waiterList = new List<Waiter>();
    
    //lasse jeden waiter rein laden und eine function aufrufen
    //bei die der waiter initialisiert wird

    //wird von save and load aufgerufen
    //erstelle und speicher den waiter in einer liste
    public bool InitialisiereWaiter(string waiterInfo)
    {
        //läd das prefab für den waiter
        prefab = Resources.Load("Prefabs/WaiterPrefab") as GameObject;

        //prüfe auf empty string
        if(waiterInfo.Equals(""))
        {
            return false;
        }
        else
        {
            string[] waiterData = waiterInfo.Split(";");
            for(int a=0;a<waiterData.Length;a++)
            {
                //erzeuge den waiter und speicher ihn in liste
                Waiter waiter = new Waiter(prefab, PlayerMovementController.FindDoorPos());
                waiterList.Add(waiter);

                /*
                ; für neuen waiter, : für neuen parameter
                name,gender,hair,skin,tshirt,leg
                */

                //läd waiter spezifische daten aus dem speicher
                waiterList[a].Name = waiterData[a].Split(":")[0];
                waiterList[a].IsBoy = Convert.ToBoolean(waiterData[a].Split(":")[1]);
                waiterList[a].HairColor = new float[]{float.Parse(waiterData[a].Split(":")[2].Split("-")[0]), float.Parse(waiterData[a].Split(":")[2].Split("-")[1]), float.Parse(waiterData[a].Split(":")[2].Split("-")[2])};
                waiterList[a].SkinColor = new float[]{float.Parse(waiterData[a].Split(":")[3].Split("-")[0]), float.Parse(waiterData[a].Split(":")[3].Split("-")[1]), float.Parse(waiterData[a].Split(":")[3].Split("-")[2])};
                waiterList[a].TshirtColor = new float[]{float.Parse(waiterData[a].Split(":")[4].Split("-")[0]), float.Parse(waiterData[a].Split(":")[4].Split("-")[1]), float.Parse(waiterData[a].Split(":")[4].Split("-")[2])};
                waiterList[a].HoseColor = new float[]{float.Parse(waiterData[a].Split(":")[5].Split("-")[0]), float.Parse(waiterData[a].Split(":")[5].Split("-")[1]), float.Parse(waiterData[a].Split(":")[5].Split("-")[2])};
                //namwwaiter:True:0,65-0,37-1:0,5-0,5-1:1-1-0,3:1-1-1;waiter2:False:0,65-0,37-1:0,5-0,5-1:1-1-0,3:1-1-1
            }
            return true;
        }       
        return false;
    }

    //baut den string, der die waiter infos enthält
    public string WaiterDataToSave()
    {
        string data = "";
        for(int a=0;a<waiterList.Count;a++)
        {
            if(a==0)
            {
                data = waiterList[a].Info();
            }
            else
            {
                data = data+";"+waiterList[a].Info();
            }
        }

        Debug.Log("speicher dtata "+data);
        return data;
    }
}
