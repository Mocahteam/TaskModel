using UnityEngine;
using UnityEngine.UI;

public class Complexity : Descriptor
{
    private GameObject linkedButton;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        linkedButton = GameObject.Find("NewComplexity");
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
