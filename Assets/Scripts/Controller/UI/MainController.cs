using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public Button IngredientsBTN; //1

    public GameObject IngredientsStore;

    public void buttonPressed(int btnNumber){
        if(btnNumber==1){//open ingredientsShop
            IngredientsStore.GetComponent<IngredientsUIController>().OpenShop();
        }

        DeactivateBTNs();
    }

    public void DeactivateBTNs(){
        IngredientsBTN.interactable = false;
    }

    public void ActivateBTNs(){
        IngredientsBTN.interactable = true;
    }
}
