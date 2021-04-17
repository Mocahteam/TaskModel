using UnityEngine;
using FYFY;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ScenarioManager : FSystem {

    public static ScenarioManager instance;

    private TMP_Dropdown taskListUI;
    private int currentSelection;


    private Scenario scenario;

    public ScenarioManager()
    {
        if (Application.isPlaying)
        {
            scenario = GameObject.Find("Scenario").GetComponent<Scenario>();
            taskListUI = GameObject.Find("Dropdown_selectTask").GetComponent<TMP_Dropdown>();
            addNewTask();
            taskListUI.value = 0;
            currentSelection = 0;
            taskListUI.RefreshShownValue();
        }
        instance = this;
    }

    private GameObject addDescriptor(GameObject prefab)
    {
        GameObject newDescriptor = GameObject.Instantiate(prefab);
        newDescriptor.transform.SetParent(scenario.contentUI.transform);
        GameObjectManager.bind(newDescriptor);
        return newDescriptor;
    }

    public void addNewTask()
    {
        string defaultId = "Tâche " + (scenario.scenario.Count+1);
        // add new task to scenario
        scenario.scenario.Add(new Scenario.Task(defaultId));
        // add task to dropdown UI
        taskListUI.options.Add(new TMP_Dropdown.OptionData(defaultId));
        // show task
        showTask(scenario.scenario.Count - 1);
    }

    public void showTask(int value)
    {
        // save previous task
        if (currentSelection >= 0 && currentSelection < taskListUI.options.Count) {
            Scenario.Task previousTask = scenario.scenario[currentSelection];
            for (int i = scenario.contentUI.transform.childCount - 1; i >= 0; i--)
            {
                GameObject descUI = scenario.contentUI.transform.GetChild(i).gameObject;
                Descriptor descriptor = descUI.GetComponent<Descriptor>();
                // save name
                if (descriptor.GetType() == typeof(TaskName))
                    previousTask.id = descUI.GetComponentInChildren<TMP_InputField>().text;
                // save objective
                if (descriptor.GetType() == typeof(TaskObjective))
                    previousTask.objective = descUI.GetComponentInChildren<TMP_InputField>().text;
                GameObject.Destroy(descUI);
            }
        }

        // Load current task
        Scenario.Task task = scenario.scenario[value];
        // load name
        GameObject taskName = addDescriptor(scenario.taskNamePrefab);
        taskName.GetComponentInChildren<TMP_InputField>().text = task.id;
        // load objective
        GameObject taskObjective = addDescriptor(scenario.taskObjectivePrefab);
        taskObjective.GetComponentInChildren<TMP_InputField>().text = task.objective;


        // select this new task
        taskListUI.value = value;
        taskListUI.RefreshShownValue();
        currentSelection = value;
    }

    // Use this to update member variables when system pause. 
    // Advice: avoid to update your families inside this function.
    protected override void onPause(int currentFrame)
    {

    }

    // Use this to update member variables when system resume.
    // Advice: avoid to update your families inside this function.
    protected override void onResume(int currentFrame)
    {

    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount)
    {

    }
}