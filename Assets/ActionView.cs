using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GOAP;
using System.Collections.Generic;

public class ActionView : MonoBehaviour {
   
    private InputField nameText;
    private InputField costText;
    private Transform preconditionsContainer;
    private Transform postconditionsContainer;

    void Awake()
    {
        nameText = transform.Find("Info/NameInput").GetComponent<InputField>();
        costText = transform.Find("Info/CostInput").GetComponent<InputField>();
        preconditionsContainer = transform.Find("Vertical/PreconditionsContainer").GetComponent<Transform>();
        postconditionsContainer = transform.Find("Vertical/PostconditionsContainer").GetComponent<Transform>();
    }

    public string GetName()
    {
        return nameText.text;
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }
    
    public int GetCost()
    {
        int value;
        if (!int.TryParse(costText.text, out value))
        {
            value = 1;
        }

        return value;
    }

    public void SetCost(int cost)
    {
        costText.text = cost.ToString();
    }

    public List<AtomView> GetPreconditions()
    {
        var atoms = new List<AtomView>();

        foreach (Transform atom in preconditionsContainer)
        {
            var atomView = atom.GetComponent<AtomView>();
            if (atomView) atoms.Add(atomView);
        }

        return atoms;
    }

    public List<AtomView> GetPostconditions()
    {
        var atoms = new List<AtomView>();

        foreach (Transform atom in postconditionsContainer)
        {
            var atomView = atom.GetComponent<AtomView>();
            if (atomView) atoms.Add(atomView);
        }

        return atoms;
    }

    public void SetPrecondition(string name, bool state)
    {
        CreateAtom(preconditionsContainer, name, state);
    }


    public void SetPostcondition(string name, bool state)
    {
        CreateAtom(postconditionsContainer, name, state);
    }

    private AtomView CreateAtom(Transform container, string atom, bool status)
    {
        var atomView = container.GetComponent<PopulateContainer>().AddPrefabToContainerReturn().GetComponent<AtomView>();

        atomView.SetName(atom);
        atomView.SetStatus(status);

        return atomView;
    }

  
}
