using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;


[CreateAssetMenu(menuName = "Game Question", fileName = "New Question")]
public class QuestionSO : ScriptableObject
{
    
    [SerializeField] public List<GameObject> componentObjects;
    [SerializeField] public List<Mesh> correctMeshes;
    [SerializeField] public GameObject questionObject;
    [SerializeField] public Material objectMaterial;
    [SerializeField] public PhysicMaterial bounceMaterial;
    private SliceConfirmation _sliceConfirmation;
    private Slice _slice;
    [Range(1, 500)] [SerializeField] public float maxQuestionTime;
    [Range(1, 15)] [SerializeField] public int maxCuts;
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

        questionObject.GetComponent<XRGrabInteractable>().smoothRotation = true;
        questionObject.GetComponent<XRGrabInteractable>().smoothPosition = true;
        questionObject.GetComponent<XRGrabInteractable>().forceGravityOnDetach = true;
        questionObject.GetComponent<XRGrabInteractable>().interactionLayers = LayerMask.NameToLayer("Object");
        questionObject.GetComponent<XRGrabInteractable>().velocityScale = 0.25f;
        questionObject.GetComponent<XRGrabInteractable>().useDynamicAttach = true;
        questionObject.GetComponent<XRGrabInteractable>().snapToColliderVolume = false;
        questionObject.GetComponent<XRGrabInteractable>().movementType = XRBaseInteractable.MovementType.VelocityTracking;

        questionObject.GetComponent<Rigidbody>().useGravity = true;
        questionObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        
        questionObject.GetComponent<BoxCollider>().material = bounceMaterial;

        questionObject.GetComponent<Slice>().SetSliceOptions(_defaultSliceOptions);
        questionObject.GetComponent<SliceConfirmation>().SetCorrectMeshes(correctMeshes);
        questionObject.GetComponent<MeshRenderer>().material = objectMaterial;

    }

    

}
