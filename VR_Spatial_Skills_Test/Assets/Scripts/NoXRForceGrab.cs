using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NoXRForceGrab : MonoBehaviour
{
    [SerializeField] GameObject LeftHand; 
    // Start is called before the first frame update
    public void checkHovered()
    {
        GameObject hoveredObject = LeftHand.GetComponent<XRRayInteractor>().interactablesHovered[0].transform.gameObject;
        if(hoveredObject.layer.Equals(LayerMask.NameToLayer("TargetObject")))
        {
            LeftHand.GetComponent<XRRayInteractor>().useForceGrab = true;
        }else{
            LeftHand.GetComponent<XRRayInteractor>().useForceGrab = false;
        }
    }

}
