using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;


[CreateAssetMenu(menuName = "Game Question", fileName = "New Question")]
public class QuestionSO : ScriptableObject
{
    
    [SerializeField] public List<GameObject> componentObjects;
    [SerializeField] public List<Mesh> correctMeshes;
    [SerializeField] public GameObject questionObject;
    [SerializeField] public Material objectMaterial;
    private SliceConfirmation _sliceConfirmation;
    private Slice _slice;
    [SerializeField] public int maxCuts;
    private SliceOptions _defaultSliceOptions;
    
    // Start is called before the first frame update
    void Awake()
    {
        _defaultSliceOptions = new SliceOptions()
        {
            enableReslicing = true,
            maxResliceCount = 100,
            insideMaterial = objectMaterial
        };
        questionObject.name = $"{questionObject.name}{questionObject.GetInstanceID().ToString()}";
        questionObject.AddComponent<Slice>();
        questionObject.GetComponent<MeshRenderer>().material = objectMaterial;
        _slice = questionObject.GetComponent<Slice>();
        _slice.SetSliceOptions(_defaultSliceOptions);

        questionObject.AddComponent<SliceConfirmation>();
        _sliceConfirmation = questionObject.GetComponent<SliceConfirmation>();
        _sliceConfirmation.SetCorrectMeshes(correctMeshes);


    }

}
