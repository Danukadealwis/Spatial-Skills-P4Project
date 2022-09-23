using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketRotation : MonoBehaviour
{
    [SerializeField] private GameObject cuttingDeskSocket;
    [SerializeField] private GameObject leftHand;
    public void updateAttachTransform(){
            XRSocketInteractor deskSocketInteractor = cuttingDeskSocket.GetComponent<XRSocketInteractor>();
            //XRBaseInteractable heldObject = leftHand.GetComponent<XRRayInteractor>().interactablesSelected.get;
    }
    //attach this script to socket, activate/update when object hovering over socket

    //socket gets rotation info of currently held object

    //assigns the rotation transform to the sockets transform

    
}
