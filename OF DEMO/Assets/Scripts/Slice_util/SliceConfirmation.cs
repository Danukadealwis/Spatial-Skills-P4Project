using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;



public class SliceConfirmation : MonoBehaviour
{
    [SerializeField] 
    private Vector3[] correctVertices;

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

    public List<Vector3[]> GetCorrectMeshes()
    {

        Mesh mesh;
        for (int index = 0; index < correctObjects.Count; index++)
        {
            mesh = correctObjects[index].GetComponentInChildren<MeshFilter>().sharedMesh;
            meshVertices.Add(mesh.vertices);
            Vector3 scaleVector = new Vector3(100f, 100f, 100f);
            for(int vertex = 0;vertex < meshVertices[index].Length; vertex++)
            {
                meshVertices[index][vertex].Scale(scaleVector);
            }
        }
        return meshVertices;
    }
}
