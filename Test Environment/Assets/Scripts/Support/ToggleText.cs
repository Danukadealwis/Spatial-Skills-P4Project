using UnityEngine;


/*
 Part of the OpenFracture Open Source Project by Greenheck D., Dearborn J
*/
public class ToggleText : MonoBehaviour
{
    public KeyCode toggleKey;

    public GameObject textObject;

    private PlayerControls _playerControls;
    // Start is called before the first frame update

    void Start()
    {
        _playerControls = new PlayerControls();
        _playerControls.Enable();
    }
    void Update()
    {
        if (_playerControls.OFTest.ToggleText.WasPressedThisFrame())
        {
            textObject.SetActive(!textObject.activeSelf);
        }
    }
}
