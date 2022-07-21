using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{

    public InputActionReference horizontalLook;
    public InputActionReference verticalLook;
    
    public float lookspeed = 1f;
    public Transform cameraTransform;
    
    private float pitch;
    private float yaw;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        horizontalLook.action.performed += HandleHorizontalLookChange;
        verticalLook.action.performed += HandleVerticalLookChange;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // void HandleMoveForward(InputAction.CallbackContext obj)
    // {
    //     transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.)
    // }  
    void HandleHorizontalLookChange(InputAction.CallbackContext obj)
    {
        pitch += obj.ReadValue<float>();
        transform.localRotation = Quaternion.AngleAxis(pitch* lookspeed,Vector3.up);
    }
    
    void HandleVerticalLookChange(InputAction.CallbackContext obj)
    {
        yaw += obj.ReadValue<float>();
        cameraTransform.localRotation = Quaternion.AngleAxis(yaw* lookspeed,Vector3.left);
    }
}
