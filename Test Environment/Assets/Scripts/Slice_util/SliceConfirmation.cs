using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;



public class SliceConfirmation : MonoBehaviour
{

    [SerializeField] private List<GameObject> correctObjects; 
    List<Vector3[]> meshVertices= new List<Vector3[]>();
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<GameObject> GetCorrectObjects()
    {
        return correctObjects;
    }

    public void SetCorrectObjects(List<GameObject> otherCorrectObjects)
    {
        correctObjects = otherCorrectObjects;
    }
    
    public List<Vector3[]> GetCorrectMeshes()
    {
        Vector3[] vertices;
        int[] triangles;
        Mesh mesh;
        Vector3 adjustmentVector = new Vector3(1, 1, 1);
        for (int index = 0; index < correctObjects.Count; index++)
        {
            mesh = correctObjects[index].GetComponentInChildren<MeshFilter>().sharedMesh;
            vertices = mesh.vertices; 
            triangles = mesh.triangles;
            meshVertices.Add(new Vector3[triangles.Length]);
            
            for(int triIndex = 0;triIndex < triangles.Length; triIndex++)
            {
                meshVertices[index][triIndex] = vertices[triangles[triIndex]];
                
                Vector3.Scale(meshVertices[index][triIndex], adjustmentVector);
                Debug.Log("Output Vert: " + triIndex + " " + meshVertices[index][triIndex]);
            }
        }
        return meshVertices;
    }

    public void PrintVertices()
    {
        var meshes = GetCorrectMeshes();
        Debug.Log(meshes[1][1]);
    }
}
