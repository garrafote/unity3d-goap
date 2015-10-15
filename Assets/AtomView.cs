using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AtomView : MonoBehaviour {

    private InputField nameText;
    private Toggle stateToggle;

    void Awake()
    {
        nameText = transform.Find("Input").GetComponent<InputField>();
        stateToggle = transform.Find("Toggle").GetComponent<Toggle>();
    }

    public string GetName()
    {
        return nameText.text;
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }

    public bool GetState()
    {
        return stateToggle.isOn;
    }

    public void SetStatus(bool status)
    {
        stateToggle.isOn = status;
    }
    
}
