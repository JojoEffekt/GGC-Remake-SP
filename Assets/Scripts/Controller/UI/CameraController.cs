using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //referenz auf die main kamera
    public Camera cam;
    
    //bestimmt die border in der sich die camera bewegen kann
    public int camBorderRight = 24;
    public int camBorderBottom = 50;
    public int camBorderLeft = 28;
    public int camBorderUp = 1;

    private Vector2 mouseClickPos;
    private Vector2 mouseCurrentPos;

    private bool panning = false;
 
    private void Update()
    {
        //Wenn MouseBtnLinks gedrückt wird, speicher die MousePosition
        if (Input.GetKeyDown(KeyCode.Mouse0) && !panning){
            mouseClickPos = cam.ScreenToWorldPoint(Input.mousePosition);
            panning = true;
        }

        //solange MouseBtnLinks Gedrückt gehalten wird, berechne die differenz und setzte die auf cam
        if (panning){

            //berechnet die differenz der ursprungspos und der jetzigen
            mouseCurrentPos = cam.ScreenToWorldPoint(Input.mousePosition);
            var distance = mouseCurrentPos - mouseClickPos;

            //beschränke die Bewegungsfreiheit der cam
            if((cam.transform.position.x - distance.x)<camBorderRight&&(cam.transform.position.x - distance.x)>-camBorderLeft&&(cam.transform.position.y - distance.y)<camBorderUp&&(cam.transform.position.y - distance.y)>-camBorderBottom){
                //positioniert die Camera
                cam.transform.position += new Vector3(-distance.x,-distance.y,0);
            }
        }
 
        //Wenn MouseBtnLinks losgelassen wird, stop
        if (Input.GetKeyUp(KeyCode.Mouse0)){
            panning = false;
        }
    }
}
