using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBScript : MonoBehaviour
{

    // Start is called before the first frame update
    public GameObject ButtonB;
    void Start()
    {
           changeColor();
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    private void changeColor()
    {
        ButtonB.GetComponent<Button>().image.color = Color.green;
    }

    public void printConsole(string message)
    {
        Debug.Log(message);
    }
}
