using UnityEngine;
using UnityEngine.InputSystem;

public class Slicer : MonoBehaviour
{
    [SerializeField] GameManager gameManager;


    public void OnTriggerStay(Collider collider)
    {
        var MeshRenderer = collider.gameObject.GetComponent<MeshRenderer>();
        if(MeshRenderer != null){
        var materials = collider.gameObject.GetComponent<MeshRenderer>().materials;
        foreach(var material in materials){
                if (material.name.StartsWith("HighlightSlice"))
                {
                    material.SetVector("CutPlaneNormal", this.transform.up);
                    material.SetVector("CutPlaneOrigin", this.transform.position);
                }
            }
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        var MeshRenderer = collider.gameObject.GetComponent<MeshRenderer>();
        if(MeshRenderer != null){
            var materials = collider.gameObject.GetComponent<MeshRenderer>().materials;
            foreach(var material in materials){
                if (material.name.StartsWith("HighlightSlice"))
                {
                    material.SetVector("CutPlaneOrigin", Vector3.positiveInfinity);
                }
            }
        }
    }


    
    
    public void sliceMesh()
    {
            Debug.Log("Trigger pressed: ");
            var mesh = this.GetComponent<MeshFilter>().sharedMesh;
            var center = mesh.bounds.center;
            var extents = mesh.bounds.extents;

            extents = new Vector3(extents.x * this.transform.localScale.x,
                                  extents.y * this.transform.localScale.y,
                                  extents.z * this.transform.localScale.z);
                                  
            // Cast a ray and find the nearest object
            RaycastHit[] hits = Physics.BoxCastAll(this.transform.position, extents, this.transform.forward, this.transform.rotation, extents.z);
            
            foreach(RaycastHit hit in hits)
            {
                var obj = hit.collider.gameObject;
                var sliceObj = obj.GetComponent<Slice>();

                if (sliceObj != null)
                {   
                    gameManager.DeactivateSocket();
                    sliceObj.GetComponent<MeshRenderer>()?.material.SetVector("CutPlaneOrigin", Vector3.positiveInfinity);
                    sliceObj.ComputeSlice(this.transform.up, this.transform.position);
                }
            }
    }
}
