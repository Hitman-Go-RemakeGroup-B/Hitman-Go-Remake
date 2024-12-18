using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
    [SerializeField] EntityType entityType;
    [SerializeField] private Node _startNode;
    [SerializeField] private Color _higlightColor;
    Node currentNode;
    Node lastNode;
    List<Node> currentPath;
    Path path;
    float timer = 0f;
    public BaseEntity BoardPice;
    // Testing
    [SerializeField] GameObject endPoint;
    private void Awake()
    {
        TurnsManager.OnEnemiesTurnStart += StartTurn;
        
        switch (entityType)
        {
            case EntityType.Pawn:
                BoardPice = new PawnEntity(_startNode, _higlightColor, Death);
                break;

            case EntityType.Rook:
                BoardPice = new RookEntity(_startNode, _higlightColor, Death);
                break;

            case EntityType.Bishop:
                BoardPice = new BishopEntity(_startNode, _higlightColor, Death);
                break;

            case EntityType.Knight:
                BoardPice = new knightEntity(_startNode, _higlightColor, Death);
                break;
        }
    }

    private void Death()
    {
        // trow it somewhere??
    }

    private void Start()
    {
        currentPath = new();
        path = FindObjectOfType<Path>();
        
        Node endNode = path.NodeFromWorldPos(endPoint.transform.position);
        currentPath = path.FindPath(currentNode.GridCoordinate, endNode.GridCoordinate);
        lastNode = currentPath[currentPath.Count - 1];
        Debug.Log("uwu");
    }

    private void Update()
    {
        if (currentNode == lastNode) { return; }
        Vector3 pos;
        if (timer < 1f)
        {
            timer += Time.deltaTime;
            pos = Vector3.Lerp(currentNode.transform.position, currentPath[0].transform.position, timer / 1);
            transform.position = pos;
            return;
        }
        transform.position = currentPath[0].transform.position;
        currentNode = currentPath[0];
        currentPath.RemoveAt(0);
        timer = -0.3f;

    }
    // Testing

    private void StartTurn()
    {

    }
}
