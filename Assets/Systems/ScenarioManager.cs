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
        GameObjectManager.refresh(scenario.contentUI);
        return newDescriptor;
    }

    private bool contains (string item)
    {
        foreach (TMP_Dropdown.OptionData option in taskListUI.options)
        {
            if (option.text == item)
                return true;
        }
        return false;
    }

    public void addNewTask()
    {
        int i = 1;
        while (contains("Tâche " + i))
            i++;
        string defaultId = "Tâche " + i;

        // add new task to scenario
        scenario.scenario.Add(new Scenario.Task(defaultId));
        // add task to dropdown UI
        taskListUI.options.Add(new TMP_Dropdown.OptionData(defaultId));
        // show task
        if (scenario.scenario.Count == 1)
            showTask(scenario.scenario.Count - 1);
        else
            taskListUI.value = scenario.scenario.Count - 1;
        taskListUI.RefreshShownValue();
    }

    public void removeCurrentTask()
    {
        if (currentSelection >= 0 && currentSelection < scenario.scenario.Count)
        {
            scenario.scenario.RemoveAt(currentSelection);
            // update tasks that depends on this removed task
            foreach (Scenario.Task task in scenario.scenario)
            {
                for (int i = task.descriptors.Count - 1; i >= 0; i--)
                {
                    Scenario.RawDescriptor descriptor = task.descriptors[i];
                    if (descriptor.GetType() == typeof(Scenario.RawAntecedent))
                        if ((descriptor as Scenario.RawAntecedent).antecedent == currentSelection)
                            task.descriptors.RemoveAt(i);
                    if (descriptor.GetType() == typeof(Scenario.RawSubTask))
                        if ((descriptor as Scenario.RawSubTask).subTask == currentSelection)
                            task.descriptors.RemoveAt(i);
                }
            }
            taskListUI.options.RemoveAt(currentSelection);
        }

        currentSelection = -1;
        if (taskListUI.value != 0)
            taskListUI.value = 0;
        else
            showTask(0);
        taskListUI.RefreshShownValue();

    }

    public void showTask(int value)
    {
        // save previous task
        if (currentSelection >= 0 && currentSelection < taskListUI.options.Count) {
            Scenario.Task previousTask = new Scenario.Task(scenario.scenario[currentSelection].id);
            // parse all childs
            for (int i = 0; i < scenario.contentUI.transform.childCount; i++)
            {
                GameObject descUI = scenario.contentUI.transform.GetChild(i).gameObject;
                Descriptor descriptor = descUI.GetComponent<Descriptor>();
                // save name
                if (descriptor.GetType() == typeof(TaskName))
                    previousTask.id = descUI.GetComponentInChildren<TMP_InputField>().text;
                // save objective
                if (descriptor.GetType() == typeof(TaskObjective))
                {
                    previousTask.objective = descUI.GetComponentInChildren<TMP_InputField>(true).text;
                    previousTask.objectiveViewState = descUI.GetComponentInChildren<Toggle>().isOn;
                }
                // save complexity
                if (descriptor.GetType() == typeof(Complexity))
                    previousTask.descriptors.Add(new Scenario.RawComplexity(descUI.GetComponentInChildren<TMP_Dropdown>().value));
                // save artefacts
                if (descriptor.GetType() == typeof(Artefact))
                    previousTask.descriptors.Add(new Scenario.RawArtefact(descUI.GetComponentInChildren<TMP_InputField>().text));
                // save observations
                if (descriptor.GetType() == typeof(Observation))
                {
                    Transform contentArea = descUI.transform.Find("Content");
                    Scenario.RawObservation newObservation = new Scenario.RawObservation(descUI.GetComponentInChildren<Toggle>().isOn, contentArea.GetChild(0).GetComponentInChildren<TMP_InputField>(true).text);
                    foreach (Decision decision in contentArea.GetComponentsInChildren<Decision>(true))
                        newObservation.addRawDecision(decision.GetComponentInChildren<TMP_InputField>(true).text);
                    previousTask.descriptors.Add(newObservation);
                }
                // save working session
                if (descriptor.GetType() == typeof(WorkingSession))
                {
                    Transform contentArea = descUI.transform.Find("Content");
                    Scenario.RawWorkingSession newWorkingSession = new Scenario.RawWorkingSession(descUI.GetComponentInChildren<Toggle>().isOn, descUI.transform.Find("Header").GetComponentInChildren<TMP_InputField>(true).text, contentArea.GetChild(0).GetComponentInChildren<TMP_InputField>(true).text, contentArea.GetChild(1).GetComponentInChildren<TMP_InputField>(true).text);
                    foreach (Participant participant in contentArea.GetComponentsInChildren<Participant>(true))
                        newWorkingSession.addParticipant(participant.transform.GetChild(1).GetComponentInChildren<TMP_InputField>(true).text, participant.transform.GetChild(3).GetComponentInChildren<TMP_InputField>(true).text);
                    previousTask.descriptors.Add(newWorkingSession);
                }
                // save competency
                if (descriptor.GetType() == typeof(Competency)) {
                    Transform headerArea = descUI.transform.Find("Header");
                    previousTask.descriptors.Add(new Scenario.RawCompetency(descUI.GetComponentInChildren<Toggle>().isOn, headerArea.GetChild(1).GetComponent<TMP_Dropdown>().value, headerArea.GetChild(3).GetComponent<TMP_Dropdown>().value, descUI.transform.Find("Content").GetComponentInChildren<TMP_InputField>(true).text));
                }
                // save production
                if (descriptor.GetType() == typeof(Production))
                    previousTask.descriptors.Add(new Scenario.RawProduction(descUI.GetComponentInChildren<Toggle>().isOn, descUI.GetComponentInChildren<TMP_InputField>(true).text));
                // save antecedent
                if (descriptor.GetType() == typeof(Antecedent))
                    previousTask.descriptors.Add(new Scenario.RawAntecedent(descUI.GetComponentInChildren<TMP_Dropdown>(true).value));
                // save subtask
                if (descriptor.GetType() == typeof(SubTask))
                    previousTask.descriptors.Add(new Scenario.RawSubTask(descUI.GetComponentInChildren<TMP_Dropdown>(true).value));
            }

            // override Task
            scenario.scenario[currentSelection] = previousTask;
        }
        // remove all childs
        for (int i = 0; i < scenario.contentUI.transform.childCount; i++)
        {
            GameObject child = scenario.contentUI.transform.GetChild(i).gameObject;
            GameObjectManager.unbind(child);
            GameObject.Destroy(child);
        }

        if (value >= 0 && value < scenario.scenario.Count)
        {
            // Load new selected task
            Scenario.Task task = scenario.scenario[value];
            // load name
            GameObject taskName = addDescriptor(scenario.taskNamePrefab);
            taskName.GetComponentInChildren<TMP_InputField>().text = task.id;
            // load objective
            GameObject taskObjective = addDescriptor(scenario.taskObjectivePrefab);
            taskObjective.GetComponentInChildren<TMP_InputField>(true).text = task.objective;
            taskObjective.GetComponentInChildren<Toggle>().isOn = task.objectiveViewState;
            // load other descriptors
            foreach (Scenario.RawDescriptor descriptor in task.descriptors)
            {
                // load complexity
                if (descriptor.GetType() == typeof(Scenario.RawComplexity))
                {
                    GameObject taskComplexity = addDescriptor(scenario.taskComplexityPrefab);
                    taskComplexity.GetComponentInChildren<TMP_Dropdown>().value = (descriptor as Scenario.RawComplexity).complexity;
                }
                // load artefact
                if (descriptor.GetType() == typeof(Scenario.RawArtefact))
                {
                    GameObject taskArtefact = addDescriptor(scenario.taskArtefactPrefab);
                    taskArtefact.GetComponentInChildren<TMP_InputField>().text = (descriptor as Scenario.RawArtefact).artefact;
                }
                // load Observation
                if (descriptor.GetType() == typeof(Scenario.RawObservation))
                {
                    GameObject taskObservation = addDescriptor(scenario.taskObservationPrefab);
                    taskObservation.GetComponentInChildren<TMP_InputField>(true).text = (descriptor as Scenario.RawObservation).content;
                    foreach (string decisionContent in (descriptor as Scenario.RawObservation).decisions)
                    {
                        GameObject decision = taskObservation.GetComponent<Observation>().addDecision(scenario.taskDecisionPrefab);
                        decision.GetComponentInChildren<TMP_InputField>(true).text = decisionContent;
                    }
                    taskObservation.GetComponentInChildren<Toggle>().isOn = (descriptor as Scenario.RawObservation).viewState;
                }
                // load Working session
                if (descriptor.GetType() == typeof(Scenario.RawWorkingSession))
                {
                    GameObject taskWorkingSession = addDescriptor(scenario.taskWorkingSessionPrefab);
                    taskWorkingSession.transform.Find("Header").GetComponentInChildren<TMP_InputField>().text = (descriptor as Scenario.RawWorkingSession).id;
                    Transform contentArea = taskWorkingSession.transform.Find("Content");
                    contentArea.GetChild(0).GetComponentInChildren<TMP_InputField>(true).text = (descriptor as Scenario.RawWorkingSession).duration;
                    contentArea.GetChild(1).GetComponentInChildren<TMP_InputField>(true).text = (descriptor as Scenario.RawWorkingSession).organisation;
                    foreach (Scenario.RawParticipant participantContent in (descriptor as Scenario.RawWorkingSession).participants)
                    {
                        GameObject participant = taskWorkingSession.GetComponent<WorkingSession>().addParticipant(scenario.taskParticipantPrefab);
                        participant.transform.GetChild(1).GetComponentInChildren<TMP_InputField>(true).text = participantContent.profil;
                        participant.transform.GetChild(3).GetComponentInChildren<TMP_InputField>(true).text = participantContent.role;
                    }
                    taskWorkingSession.GetComponentInChildren<Toggle>().isOn = (descriptor as Scenario.RawWorkingSession).viewState;
                }
                // load competency
                if (descriptor.GetType() == typeof(Scenario.RawCompetency))
                {
                    GameObject taskCompetency = addDescriptor(scenario.taskCompetencyPrefab);
                    Transform headerArea = taskCompetency.transform.Find("Header");
                    headerArea.GetChild(1).GetComponent<TMP_Dropdown>().value = (descriptor as Scenario.RawCompetency).type;
                    headerArea.GetChild(3).GetComponent<TMP_Dropdown>().value = (descriptor as Scenario.RawCompetency).id;
                    taskCompetency.transform.Find("Content").GetComponentInChildren<TMP_InputField>(true).text = (descriptor as Scenario.RawCompetency).details;
                    taskCompetency.GetComponentInChildren<Toggle>().isOn = (descriptor as Scenario.RawCompetency).viewState;
                }
                // load production
                if (descriptor.GetType() == typeof(Scenario.RawProduction))
                {
                    GameObject taskProduction = addDescriptor(scenario.taskProductionPrefab);
                    taskProduction.GetComponentInChildren<TMP_InputField>(true).text = (descriptor as Scenario.RawProduction).production;
                    taskProduction.GetComponentInChildren<Toggle>().isOn = (descriptor as Scenario.RawProduction).viewState;
                }
                // load antecedent
                if (descriptor.GetType() == typeof(Scenario.RawAntecedent))
                {
                    GameObject taskAntecedent = addDescriptor(scenario.taskAntecedentPrefab);
                    TMP_Dropdown drop = taskAntecedent.GetComponentInChildren<TMP_Dropdown>(true);
                    drop.value = (descriptor as Scenario.RawAntecedent).antecedent;
                    drop.RefreshShownValue();
                }
                // load subtask
                if (descriptor.GetType() == typeof(Scenario.RawSubTask))
                {
                    GameObject taskSubTask = addDescriptor(scenario.taskSubTaskPrefab);
                    TMP_Dropdown drop = taskSubTask.GetComponentInChildren<TMP_Dropdown>(true);
                    drop.value = (descriptor as Scenario.RawSubTask).subTask;
                    drop.RefreshShownValue();
                }
            }

            // remember current selection
            currentSelection = value;
        }
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