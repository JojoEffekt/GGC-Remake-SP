using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class CounterController : MonoBehaviour
{
    //sucht alle tresen mit dem gleichen gericht bzw freie tresen herraus
    public static List<string> getAllCounterForDinner(string dinner){
        List<string> counters = new List<string>();
        //CONTINUE
        counters = FloorChildExtraDataController.getFCEDFromTyp("Counter", dinner);
        return counters;
    }
}
