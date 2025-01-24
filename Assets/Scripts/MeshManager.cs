using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManager : MonoBehaviour
{
    Controller[] controllers;
    [SerializeField] BoardPiece[] BoardPices;

    private void Awake()
    {
        controllers = FindObjectsOfType<Controller>();

        foreach (Controller controller in controllers)
        {
            controller.OnChangeBoardPiece += ChangeMesh;
        }
    }

    private void ChangeMesh(MeshFilter filter, EntityType type, MeshRenderer meshRenderer)
    {
        foreach (var piece in BoardPices)
        {
            if (piece.entityType != type) continue;

            filter.mesh = piece.Mesh;
            filter.transform.localScale = piece.Scale;
            filter.transform.localPosition = piece.Position;
            meshRenderer.material = piece.Material;
        }
    }
}

[System.Serializable]
public struct BoardPiece
{
    public Mesh Mesh;
    public Material Material;
    public Vector3 Position;
    public Vector3 Scale;
    public EntityType entityType;
}
