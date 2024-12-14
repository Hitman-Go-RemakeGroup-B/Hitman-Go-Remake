using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Path : MonoBehaviour
{
    public int Rows;
    public int Collums;
    public int UnitScale = 1;
    public Node NodePrefab;
    private Vector3 position;




    public GameObject[,] GridArray;

    // public List<Line> Lines; viktor nation what are we feeling? jaybe!? jaybe not?



    public void GenerateGrid()
    {
        if (NodePrefab == null) { Debug.LogWarning("You forgot the NodePrefab!!"); return; }
        if (GridArray != null) DestroyGrid();
        Debug.Log("Generating grid");
        //if (NodeGrid != null) { DestroyGrid(); }

        GridArray = new GameObject[Collums, Rows];
        position = new Vector3(transform.position.x + 1f / 2f, 0, transform.position.z + 1f / 2f);
        //-Vector3.right * Rows/2 - Vector3.forward*Collums/2
        for (int x = 0; x < Collums; x++)
        {
            for (int z = 0; z < Rows; z++)
            {
                Node node = Instantiate(NodePrefab, new Vector3(position.x + UnitScale * x, position.y, position.z + UnitScale * z), NodePrefab.transform.rotation, transform);
                node.GridCoordinate = new Vector2(x, z);
                GridArray[x, z] = node.gameObject;

            }
        }
    }

    public void DestroyGrid()
    {
        if (GridArray == null) { Debug.LogWarning("don't press this button if there is no grid >:("); return; }


        Debug.Log("there is no grid in ba sing se (destroying grid)");
        foreach (GameObject obj in GridArray)
        {
            DestroyImmediate(obj);
        }
        GridArray = null;
        //for (int x = 0; x < Collums; x++)
        //{
        //    for (int z = 0; z < Rows; z++)
        //    {
        //        DestroyImmediate(NodeGrid[x, z].gameObject);
        //    }
        //};

    }

    //bool TestDirection(int x, int z, int step, int direction)
    //{
    //    switch (direction)
    //    {
    //        case 1:
    //            return z + 1 < Rows && GridArray[x, z + 1]?.GetComponent<Node>().Visited == step;

    //        case 2:
    //            return x + 1 < Collums && GridArray[x + 1, z]?.GetComponent<Node>().Visited == step;
    //        case 3:
    //            return z - 1 > -1 && GridArray[x, z - 1]?.GetComponent<Node>().Visited == step;
    //        case 4:
    //            return x - 1 > -1 && GridArray[x - 1, z]?.GetComponent<Node>().Visited == step;
    //    }

    //    return false;
    //}

    public Node NodeFromWorldPos(Vector3 givenVector3)
    {
        float precentX = (givenVector3.x + Collums / 2f) / Collums;
        float precentY = (givenVector3.z + Rows / 2f) / Rows;
        precentX = Mathf.Clamp01(precentX);
        precentY = Mathf.Clamp01(precentY);

        int x = Mathf.RoundToInt((Collums - 1) * precentX);
        int z = Mathf.RoundToInt((Rows - 1) * precentY);


        return GridArray[x, z].GetComponent<Node>();

    }
    public Node GetNodeFromCoordinate(Vector2 givenCoordinate)
    {
        foreach (GameObject obj in GridArray)
        {
            var nodeNode = obj.GetComponent<Node>();
            if (nodeNode.GridCoordinate == givenCoordinate)
            { return nodeNode; }
        }

        return null;
    }
}
