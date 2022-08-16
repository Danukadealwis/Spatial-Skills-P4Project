using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class SliceConfirmation : MonoBehaviour
{

    [SerializeField] 
    private Vector3[] correctVertices;

    [SerializeField] private List<GameObject> correctObjects;
    private List<Mesh> meshes;
    List<Vector3[]> meshVertices;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Vector3[]> GetCorrectMeshes()
    {
        for (int index = 0; index < correctObjects.Count; index++)
        {
            meshes.Add(correctObjects[index].GetComponent<MeshFilter>().mesh);
            meshVertices[index] = meshes[index].vertices;
        }

        return meshVertices;
    }

    // public bool testCorrectVertices()
    // {
    //     
    // }
    
    // public List<float[]> GetCorrectVertices()
    // {
    //     List<float[]> arrayVertices = new List<float[]>();
    //     
    //     arrayVertices.Add(new float[100]);
    //     arrayVertices.Add(new float[100]);
    //     arrayVertices.Add(new float[100]);
    //
    //
    //     for (int vertexNumber = 0; vertexNumber < correctVertices.Count; vertexNumber++)
    //     {
    //         Debug.Log("correctVertices[vertexNumber]: " + correctVertices[vertexNumber]);
    //         arrayVertices[0][vertexNumber] = correctVertices[vertexNumber].x;
    //         arrayVertices[0][vertexNumber] = correctVertices[vertexNumber].y;
    //         arrayVertices[0][vertexNumber] = correctVertices[vertexNumber].z;
    //         
    //         // arrayVertices[0](new float[]{correctVertices[vertexNumber].x}).ToArray();
    //         // arrayVertices[1].Concat(new float[]{correctVertices[vertexNumber].y}).ToArray();
    //         // arrayVertices[2].Concat(new float[]{correctVertices[vertexNumber].z}).ToArray();
    //     }
    //     
    //     Debug.Log("arrayVertices[0].Max(): " + arrayVertices[0].Max());
    //     return arrayVertices;
    // }

    public Vector3[] GetCorrectVertices()
    {
        return correctVertices;
    }

    // Need a function where two intersection points are provided in the form of v23 and v13. 
    // The function uses 
}
