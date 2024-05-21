using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DinnerAnim : MonoBehaviour
{
    public int dinnerAnim = 0; //0=idle, 1=inAnim, 2=ExitAnim

    public void Controller(){
        StartCoroutine(UpdateDinnerUI());
    }

    public void Update(){
        if(dinnerAnim==2){
            dinnerAnim = 0;
            //Debug.Log("Anim Fertig!");
        }
    }

    //aktualisiert die Ui den anzufertigen dinners
    public IEnumerator UpdateDinnerUI(){
        //Debug.Log("Anim Start!");
        dinnerAnim = 1;
        yield return new WaitForSeconds(1f);
        dinnerAnim = 2;
    }
}
