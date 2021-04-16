using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Participant : Descriptor
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    new public void removeDescriptor()
    {
        Debug.Log("Participant removeDescriptor");
        base.removeDescriptor();
        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.transform.parent as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.transform.parent.parent as RectTransform);

        //Problème de rafraichissement...
    }
}
