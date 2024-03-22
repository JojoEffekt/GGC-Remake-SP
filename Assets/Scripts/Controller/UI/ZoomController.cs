using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomController : MonoBehaviour
{
    public Camera cam;
    
    public void Start(){
        //initialisiert cam properties
        cam.enabled = true;
        cam.orthographic = true;
    }

    //verändert den zoom der cam
    public void CamInput(int value){

        //überprüft boundaries von cam
        if((cam.orthographicSize+value)>5&&(cam.orthographicSize+value)<20){
            cam.orthographicSize += value;
        }
    }
}
