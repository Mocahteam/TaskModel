using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Production : Descriptor
{
    private GameObject linkedButton;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        linkedButton = GameObject.Find("NewProduction");
    }

    // Update is called once per frame
    void Update()
    {

    }

    new public void removeDescriptor()
    {
        linkedButton.GetComponent<Button>().interactable = true;
        base.removeDescriptor();
    }
}
