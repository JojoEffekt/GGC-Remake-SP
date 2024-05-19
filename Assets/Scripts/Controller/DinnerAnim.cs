using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DinnerAnim : MonoBehaviour
{
    public bool isDinnerAnim = true;

    public bool Controller(){
        StartCoroutine(UpdateDinnerUI());
    }

    //aktualisiert die Ui den anzufertigen dinners
    public IEnumerator UpdateDinnerUI(){
        isDinnerAnim = true;
        yield return new WaitForSeconds(1f);
        Debug.Log("1sec gewartet!");
        isDinnerAnim = false;
    }
}
