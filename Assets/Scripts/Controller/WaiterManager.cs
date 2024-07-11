using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaiterManager : MonoBehaviour
{
    //beinhaltet das waiter prefab
    private GameObject prefab;

    //beinhaltet alle waiter
    public List<Waiter> waiterList = new List<Waiter>();
    
    //lasse jeden waiter rein laden und eine function aufrufen
    //bei die der waiter initialisiert wird

    void Start()
    {
        //läd das prefab für den waiter
        prefab = Resources.Load("Prefabs/WaiterPrefab") as GameObject;
    }

    //wird von save and load aufgerufen
    //erstelle und speicher den waiter in einer liste
    public bool InitialisiereWaiter(string waiterInfo)
    {
        //prüfe auf empty string
        if(waiterInfo.Equals(""))
        {
            Debug.Log("string null");
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

                Debug.Log(waiterData[a]);
                waiterList[a].HairColor = new float[]{float.Parse(waiterData[a].Split(":")[1].Split("-")[0]), float.Parse(waiterData[a].Split(":")[1].Split("-")[1]), float.Parse(waiterData[a].Split(":")[1].Split("-")[2])};
                //namwwaiter:0,65-0,37-1-1;waitee2:1-1-1-1

                //CONTINUE
                //waiterList[waiterList.Count-1].RenderSkin(...);
            }
            return true;
        }       

        return false;
    }

    //baut den string, der die waiter infos enthält
    public string WaiterDataToSave()
    {
        /*
        semikolon für neuen waiter
        name,gender,hair,leg,skin,tshirt
        */

        string data = "";
        for(int a=0;a<waiterList.Count;a++)
        {
            data = data+";"+waiterList[a].Info();
        }

        return data;
    }
}
