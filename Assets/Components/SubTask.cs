using TMPro;
using UnityEngine;

public class SubTask : Descriptor
{
    void Awake()
    {
        TMP_Dropdown taskListUI = GameObject.Find("Dropdown_selectTask").GetComponent<TMP_Dropdown>();
        // update list
        GetComponentInChildren<TMP_Dropdown>().ClearOptions();
        GetComponentInChildren<TMP_Dropdown>().AddOptions(taskListUI.options);
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
