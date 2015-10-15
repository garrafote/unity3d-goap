using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ContainerItem : MonoBehaviour {

    public Text label;

    public void Index(int index)
    {
        label.text = index.ToString();
    }

    public void RemoveFromContainer()
    {
        var container = transform.GetComponentInParent<PopulateContainer>();
        if (container) container.RemoveFromContainer(transform);
    }
}
