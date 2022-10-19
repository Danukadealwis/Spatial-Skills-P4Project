using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.HID;


/*
 Part of the OpenFracture Open Source Project by Greenheck D., Dearborn J
*/

[ExcludeFromCoverage]
public class CameraController : MonoBehaviour
{
    [Tooltip("Acceleration of the player")]
    public float acceleration = 100.0f;

    [Tooltip("Maximum speed of the player while walking")]
    public float maxSpeed = 5.0f;

    [Tooltip("Sensitivity of the mouse for pan / tilt.")]
    public float mouseSensitivity = 0.1f;
    
    private float startTime = 0f;
    private float elapsedTime = 0f;

    private PlayerControls _playerControls;

    void Start()
    {
        _playerControls = new PlayerControls();
        _playerControls.Enable();
        startTime = Time.time;
    }

    void Update()
    {
        float dx = _playerControls.Player.Look.ReadValue<Vector2>().x * mouseSensitivity;
        float dy = _playerControls.Player.Look.ReadValue<Vector2>().y  * mouseSensitivity;

        if (elapsedTime > 0.5f)
        {
            this.transform.parent.Rotate(Vector3.up, dx);

            // Clamp pitch to [-80, 80] degrees
            var currentPitch = this.transform.eulerAngles.x;
            if (currentPitch > 180f) currentPitch -= 360f;
            var newPitch = Mathf.Clamp(currentPitch - dy, -80f, 80f);
            this.transform.localEulerAngles = new Vector3(newPitch, 0, 0);
        }
        else
        {
            elapsedTime = Time.time - startTime;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Check for player movement. We can handle input here because it is continuous and
        // not instantaneous like jumping.
        var rigidbody = this.transform.parent.GetComponent<Rigidbody>();
        var inputMovement = _playerControls.Player.Move.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(inputMovement.x,0,  inputMovement.y);
        
        rigidbody.AddRelativeForce(moveDirection * acceleration, ForceMode.Acceleration);
        
        // Clamp the player's velocity in the X and Z directions
        Vector2 xzVelocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.z);
        if (xzVelocity.magnitude > maxSpeed)
        {
            var xzClampedVelocity = maxSpeed * xzVelocity.normalized;
            rigidbody.velocity = new Vector3(xzClampedVelocity.x, rigidbody.velocity.y, xzClampedVelocity.y);
        }
    }
}
