using FYFY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddDescriptor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addDescriptor(GameObject prefab)
    {
        GameObject newDescriptor = Instantiate(prefab);
        newDescriptor.transform.SetParent(transform);
        GameObjectManager.bind(newDescriptor);
    }
}
