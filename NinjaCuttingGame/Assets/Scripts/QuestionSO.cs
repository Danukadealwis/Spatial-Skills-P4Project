using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;


[CreateAssetMenu(menuName = "Game Question", fileName = "New Question")]
public class QuestionSO : ScriptableObject
{
    
    [SerializeField] public List<GameObject> componentObjects; 
    // Start is called before the first frame update
    void Start()
    {
        componentObjects[0].GetComponentInChildren<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
