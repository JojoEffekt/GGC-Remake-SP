using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InfoBtn : MonoBehaviour
{
    public void PrintInfo(){
        Debug.Log("sooo");
    }

    Camera m_Camera;
    void Awake(){
        m_Camera = Camera.main;
    }
    void Update(){
        Mouse mouse = Mouse.current;
        if (mouse.leftButton.wasPressedThisFrame)
        {
            Vector3 mousePosition = mouse.position.ReadValue();
            Ray ray = m_Camera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)){
                Debug.Log("hiz:" + hit);
            }
        }
    }
}
