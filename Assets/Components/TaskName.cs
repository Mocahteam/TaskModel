using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaskName : Descriptor
{
    public TMP_Dropdown taskListUI;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        taskListUI = GameObject.Find("Dropdown_selectTask").GetComponent<TMP_Dropdown>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onNamedEdited(string input)
    {
        if (taskListUI != null)
        {
            taskListUI.options[taskListUI.value] = new TMP_Dropdown.OptionData(input);
            taskListUI.RefreshShownValue();
        }
    }
}
