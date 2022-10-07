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
    [SerializeField] public Material targetMaterial;
    [SerializeField] public PhysicMaterial bounceMaterial;
    private SliceConfirmation _sliceConfirmation;
    private Slice _slice;
    [Range(1, 500)] [SerializeField] public float maxQuestionTime;
    [Range(1, 15)] [SerializeField] public int maxCuts;
    [Range(0.0f, 1.0f)][SerializeField] public float scalingEffect;
    private SliceOptions _defaultSliceOptions;
    
    // Start is called before the first frame update
    public void Awake()
    {
        _defaultSliceOptions = new SliceOptions()
        {
            enableReslicing = true,
            maxResliceCount = 100,
            insideMaterial = objectMaterial
        };


        questionObject.name = $"{questionObject.name}{questionObject.GetInstanceID().ToString()}";
        questionObject.layer = LayerMask.NameToLayer("Object");
        questionObject.transform.localScale = new Vector3(scalingEffect,scalingEffect,scalingEffect);
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

        foreach(var component in componentObjects){
            component.layer = LayerMask.NameToLayer("TargetObject");
            component.GetComponent<XRGrabInteractable>().smoothRotation = true;
            component.GetComponent<XRGrabInteractable>().smoothPosition = true;
            component.GetComponent<XRGrabInteractable>().forceGravityOnDetach = true;
            Debug.Log("int layers: " + component.GetComponent<XRGrabInteractable>().interactionLayers.value);
            component.GetComponent<XRGrabInteractable>().interactionLayers = 4;
            component.GetComponent<XRGrabInteractable>().velocityScale = 0.25f;
            component.GetComponent<XRGrabInteractable>().useDynamicAttach = true;
            component.GetComponent<XRGrabInteractable>().snapToColliderVolume = false;
            component.GetComponent<XRGrabInteractable>().movementType = XRBaseInteractable.MovementType.VelocityTracking;
            component.GetComponent<Rigidbody>().useGravity = true;
            component.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            component.GetComponent<BoxCollider>().material = bounceMaterial;
            component.transform.localScale = new Vector3(scalingEffect,scalingEffect,scalingEffect);
            component.GetComponent<MeshRenderer>().material = targetMaterial;

        }
    }
}
