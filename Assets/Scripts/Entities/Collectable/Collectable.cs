using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour, IInteractable
{
    public bool IsQueen;
    public bool IsKing;

    public void Interact(PlayerController player)
    {
        if (IsQueen) 
        { 
            //TODO: call the queen thing from tara
        }
        else
        {
            //TODO: call the king thing from tara
        }

        gameObject.SetActive(false);
    }
}
