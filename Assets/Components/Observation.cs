using FYFY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observation : Descriptor
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

    public void addEmptyDecision(GameObject prefab)
    {
        GameObject newDecision = Instantiate(prefab);
        newDecision.transform.SetParent(transform);
        GameObjectManager.bind(newDecision);
    }
}
