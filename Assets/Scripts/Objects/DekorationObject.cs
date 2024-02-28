using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DekorationObject : StandardObject
{
    //constructor
    public DekorationObject(string type, string objectName, int rotation, int price, float coordCorrectionX, float coordCorrectionY, int coordX, int coordY) : base(type, objectName, rotation, price, coordCorrectionX, coordCorrectionY, coordX, coordY){
    }
}
