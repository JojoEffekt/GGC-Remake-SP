using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DinnerAnim : MonoBehaviour
{
    public int dinnerAnim = 0; //0=idle, 1=inAnim, 2=ExitAnim

    //wird aufgerufen um eine dinnerAnimation zu starten, zw zu warten
    public void Controller(){
        StartCoroutine(UpdateDinnerUI());
    }

    public void Update(){
        if(dinnerAnim==2){
            dinnerAnim = 0;
        }
    }

    //aktualisiert die UI des anzufertigenden dinners in 1 sekunde gewartet wird
    public IEnumerator UpdateDinnerUI(){
        dinnerAnim = 1;
        yield return new WaitForSeconds(1f);
        dinnerAnim = 2;
    }
}
