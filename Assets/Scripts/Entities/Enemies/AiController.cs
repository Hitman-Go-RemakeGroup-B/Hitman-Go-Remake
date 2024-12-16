using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
// Testing
    Node currentNode;
    List<Node> currentPath;
    [SerializeField] GameObject endPoint;
    Path path;
    int pathIndex =0;
    float timer = 0f;
    private void Start()
    {
        currentPath = new();
        path = FindObjectOfType<Path>();
        currentNode = path.NodeFromWorldPos(transform.position);
        Node endNode = path.NodeFromWorldPos(endPoint.transform.position);
        currentPath = path.FindPath(currentNode.GridCoordinate, endNode.GridCoordinate);
    }

    private void Update()
    {
        Vector3 pos ;
        if(timer < 1f )
        {
            timer += Time.deltaTime;
            pos = Vector3.Lerp(currentNode.transform.position, currentPath[0].transform.position, timer/1);
            transform.position = pos;
            return;
        }
        transform.position = currentPath[0].transform.position;
        currentNode = currentPath[0];
        currentPath.RemoveAt(0);
        timer = -0.3f;
    }
// Testing
}
