using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public Button IngredientsBTN; //1
    public Button RebuildBTN; //2
    public Button DinnerBTN; //3
    public Button PlayerUIBTN; // 4

    public GameObject MainShop; //UI Elemente
    public GameObject IngredientsStore;
    public GameObject RebuildStore;
    public GameObject DinnerStore;
    public GameObject PlayerUIStore;

    public static void LoadName(string name){
        GameObject Main = GameObject.Find("UI/Main");
        Main.transform.Find("Name").gameObject.transform.GetChild(1).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = name;
    }

    public void buttonPressed(int btnNumber){
        if(btnNumber==1){//open ingredientsShop
            IngredientsStore.GetComponent<IngredientsUIController>().OpenShop();
        }else if(btnNumber==2){//open RebuildShop
            RebuildStore.GetComponent<RebuildUIController>().OpenShop();
            DeactivateUI();
        }else if(btnNumber==3){//open DinnerShop
            DinnerStore.GetComponent<DinnerUIController>().OpenShop();
        }else if(btnNumber==4){//open Player Design
            PlayerUIStore.GetComponent<PlayerUIController>().OpenShop();
        }

        DeactivateBTNs();
    }

    public void DeactivateBTNs(){
        IngredientsBTN.interactable = false;
        RebuildBTN.interactable = false;
        DinnerBTN.interactable = false;
        PlayerUIBTN.interactable = false;
    }
    public void DeactivateUI(){
        MainShop.SetActive(false);
    }

    public void ActivateBTNs(){
        IngredientsBTN.interactable = true;
        RebuildBTN.interactable = true;
        DinnerBTN.interactable = true;
        PlayerUIBTN.interactable = true;
    }
    public void ActivateUI(){
        MainShop.SetActive(true);
    }
}
