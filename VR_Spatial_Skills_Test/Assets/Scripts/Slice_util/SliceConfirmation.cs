using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;



public class SliceConfirmation : MonoBehaviour
{

    [SerializeField] private List<Mesh> correctMeshes; 
    List<Vector3[]> meshVertices= new List<Vector3[]>();

    public List<Mesh> GetCorrectMeshes()
    {
        return correctMeshes;
    }

    public void SetCorrectMeshes(List<Mesh> otherCorrectObjects)
    {
        correctMeshes = otherCorrectObjects;
    }
    
    public List<Vector3[]> GetCorrectVertices()
    {
        Vector3[] vertices;
        int[] triangles;
        Mesh mesh;
        Vector3 adjustmentVector = new Vector3(1, 1, 1);
        for (int index = 0; index < correctMeshes.Count; index++)
        {
            mesh = correctMeshes[index];
            vertices = mesh.vertices; 
            triangles = mesh.triangles;
            meshVertices.Add(new Vector3[triangles.Length]);
            
            for(int triIndex = 0;triIndex < triangles.Length; triIndex++)
            {
                meshVertices[index][triIndex] = vertices[triangles[triIndex]];
                
                Vector3.Scale(meshVertices[index][triIndex], adjustmentVector);
            }
        }
        return meshVertices;
    }
    
}
