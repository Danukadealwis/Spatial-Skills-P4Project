using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SliceConfirmation : MonoBehaviour
{

    [SerializeField] 
    private List<Vector3> correctVertices;  
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<float[]> GetCorrectVertices()
    {
        List<float[]> arrayVertices = new List<float[]>();
        
        arrayVertices.Add(new float[100]);
        arrayVertices.Add(new float[100]);
        arrayVertices.Add(new float[100]);


        for (int vertexNumber = 0; vertexNumber < correctVertices.Count; vertexNumber++)
        {
            Debug.Log("correctVertices[vertexNumber]: " + correctVertices[vertexNumber]);
            arrayVertices[0][vertexNumber] = correctVertices[vertexNumber].x;
            arrayVertices[0][vertexNumber] = correctVertices[vertexNumber].y;
            arrayVertices[0][vertexNumber] = correctVertices[vertexNumber].z;
            
            // arrayVertices[0](new float[]{correctVertices[vertexNumber].x}).ToArray();
            // arrayVertices[1].Concat(new float[]{correctVertices[vertexNumber].y}).ToArray();
            // arrayVertices[2].Concat(new float[]{correctVertices[vertexNumber].z}).ToArray();
        }
        
        Debug.Log("arrayVertices[0].Max(): " + arrayVertices[0].Max());
        return arrayVertices;
    }
}
