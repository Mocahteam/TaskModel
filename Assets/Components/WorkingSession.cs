using FYFY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkingSession : Descriptor
{
    public TMPro.TMP_InputField id;
    public string previousValue;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addEmptyParticipant(GameObject prefab)
    {
        GameObject newParticipant = Instantiate(prefab);
        newParticipant.transform.SetParent(transform);
        GameObjectManager.bind(newParticipant);
    }
}
