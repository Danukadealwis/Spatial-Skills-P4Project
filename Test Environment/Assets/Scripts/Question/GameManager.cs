using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<int> _objectsSliced;
    private List<string> _fragmentRoots;
    private List<string> _slicedObjs;
    private int undoCount;

    // Start is called before the first frame update
    void Start()
    {
        _objectsSliced = new List<int>();
        _fragmentRoots = new List<string>();
        _slicedObjs = new List<string>();
        undoCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void UndoCut()  
    {
        // Would have to know the name of the objects that were just created for the last cut and destroy them
        // pre req: For each cut, store the name of the cut object. 
        
        // Also the name of the parent object and activate it again
        // for the

        
        if (_slicedObjs.Count != 0)
        {
            var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            GameObject obj = allObjects.Single(o => o.name == _slicedObjs.Last());
            GameObject root = GameObject.Find(_fragmentRoots.Last()).gameObject;
 
            Debug.Log("Name of obj: " + obj.name);
            Destroy(root.transform.gameObject);
            obj.SetActive(true);
            undoCount++;
            _fragmentRoots.RemoveAt(_fragmentRoots.Count - 1);
            _slicedObjs.RemoveAt(_slicedObjs.Count - 1);
            _objectsSliced.RemoveAt(_objectsSliced.Count - 1);
        }
        
        Debug.Log("Undo count:" + undoCount);
    }

    public void AddObjectSliced(int value, string fragmentRootName, string slicedObjName)

    {
        _objectsSliced.Add(value);
        _fragmentRoots.Add(fragmentRootName);
        _slicedObjs.Add(slicedObjName);
    }
    
    
    
}
