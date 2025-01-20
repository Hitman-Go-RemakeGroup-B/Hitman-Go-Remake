using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public BaseEntity BoardPice;

    private void Awake()
    {
        Path path = FindObjectOfType<Path>();
        BoardPice = new(path.NodeFromWorldPos(transform.position),new(0,0),new(path.CollumsX,path.RowsZ),Death,transform);
    }

    public void Death() 
    {
        // ToDo: send you lost action?
    }
}
