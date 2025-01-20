using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TestScript : MonoBehaviour
{

    [SerializeField] public Vector2 vector = Vector2.up;
    int i = 1;



    public void MegaDebug()
    {
        vector = Quaternion.Euler(0, 0, -90f * i) * vector;
        Debug.Log(vector.ToString());
        i++;
    }
}
