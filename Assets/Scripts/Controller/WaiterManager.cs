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
        prefab = Resources.Load("") as GameObject;
    }

    //erstelle und speicher den waiter in einer liste
    void InitialisiereWaiter(string waiterInfo)
    {
        NWaiterPC waiter = new Waiter(prefab, PlayerMovementController.FindDoorPos());
        waiterList.append(waiter);
    }
}
