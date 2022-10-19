using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.TestTools;


/*
 Part of the OpenFracture Open Source Project by Greenheck D., Dearborn J
*/
[ExcludeFromCoverage]
public class Projectile : MonoBehaviour
{
    public GameObject projectile;
    public float initialVelocity;
    public KeyCode FireKey;

    private PlayerControls _playerControls;

    private void Start()
    {
        _playerControls = new PlayerControls();
        _playerControls.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerControls.OFTest.FireProjectile.WasPressedThisFrame())
        {
            // Remove other projectiles from the scene
            foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Projectile"))
            {
                GameObject.Destroy(obj);
            }

            var projectileInstance = GameObject.Instantiate(projectile, this.transform.position, Quaternion.identity);
            projectileInstance.GetComponent<Rigidbody>().velocity = initialVelocity * this.transform.forward;
        }
    }
}
