using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public Button IngredientsBTN; //1
    public Button RebuildBTN; //2

    public GameObject MainShop; //UI Elemente
    public GameObject IngredientsStore;
    public GameObject RebuildStore;

    public static void LoadName(string name){
        GameObject Main = GameObject.Find("UI/Main");
        Main.transform.Find("Name").gameObject.transform.GetChild(1).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = name;
    }

    public void buttonPressed(int btnNumber){
        if(btnNumber==1){//open ingredientsShop
            IngredientsStore.GetComponent<IngredientsUIController>().OpenShop();
        }else if(btnNumber==2){
            RebuildStore.GetComponent<RebuildUIController>().OpenShop();
            DeactivateUI();
        }

        DeactivateBTNs();
    }

    public void DeactivateBTNs(){
        IngredientsBTN.interactable = false;
        RebuildBTN.interactable = false;
    }
    public void DeactivateUI(){
        MainShop.SetActive(false);
    }

    public void ActivateBTNs(){
        IngredientsBTN.interactable = true;
        RebuildBTN.interactable = true;
    }
    public void ActivateUI(){
        MainShop.SetActive(true);
    }
}
