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
            player.GotQueen = true;
        }
        else
        {
            player.GotKing = true;
        }

        gameObject.SetActive(false);
    }
}
