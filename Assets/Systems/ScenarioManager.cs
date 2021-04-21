using UnityEngine;
using FYFY;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Runtime.InteropServices;

public class ScenarioManager : FSystem {

    [DllImport("__Internal")]
    private static extern void Save(string name, string content); // call javascript

    [DllImport("__Internal")]
    private static extern void ShowLoadingButton(); // call javascript

    public static ScenarioManager instance;

    private TMP_Dropdown taskListUI;
    private TMP_InputField scenarioName;
    private int currentSelection;


    private Scenario scenario;

    public ScenarioManager()
    {
        if (Application.isPlaying)
        {
            scenario = GameObject.Find("ScenarioLoader").GetComponent<Scenario>();
            taskListUI = GameObject.Find("Dropdown_selectTask").GetComponent<TMP_Dropdown>();
            scenarioName = GameObject.Find("ScenarioName").GetComponent<TMP_InputField>();
            addNewTask();
            taskListUI.value = 0;
            currentSelection = 0;
            taskListUI.RefreshShownValue();
            if (!Application.isEditor)
                ShowLoadingButton();
        }
        instance = this;
    }

    public void saveScenario(GameObject UIError)
    {
        syncCurrentTask();
        if (scenarioName.text == "")
            GameObjectManager.setGameObjectState(UIError, true);
        else
        {
            Debug.Log(JsonUtility.ToJson(scenario.rawScenario));
            Save(scenarioName.text.EndsWith(".snr") ? scenarioName.text : scenarioName.text + ".snr", JsonUtility.ToJson(scenario.rawScenario));
        }
    }

    public void loadScenario()
    {
        currentSelection = -1;
        showTask(0);
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
        scenario.rawScenario.tasks.Add(new Scenario.RawTask(defaultId));
        // add task to dropdown UI
        taskListUI.options.Add(new TMP_Dropdown.OptionData(defaultId));
        // show task
        if (scenario.rawScenario.tasks.Count == 1)
            showTask(scenario.rawScenario.tasks.Count - 1);
        else
            taskListUI.value = scenario.rawScenario.tasks.Count - 1;
        taskListUI.RefreshShownValue();
    }

    public void removeCurrentTask()
    {
        if (currentSelection >= 0 && currentSelection < scenario.rawScenario.tasks.Count)
        {
            scenario.rawScenario.tasks.RemoveAt(currentSelection);
            // update tasks that depends on this removed task
            foreach (Scenario.RawTask task in scenario.rawScenario.tasks)
            {
                for (int i = task.rawAntecedents.Count - 1; i >= 0; i--)
                    if (task.rawAntecedents[i].antecedent == currentSelection)
                        task.rawAntecedents.RemoveAt(i);

                for (int i = task.rawSubTasks.Count - 1; i >= 0; i--)
                    if (task.rawSubTasks[i].subTask == currentSelection)
                        task.rawSubTasks.RemoveAt(i);
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

    private void syncCurrentTask()
    {
        Scenario.RawTask syncTask = new Scenario.RawTask(scenario.rawScenario.tasks[currentSelection].id);
        // parse all childs
        for (int i = 0; i < scenario.contentUI.transform.childCount; i++)
        {
            GameObject descUI = scenario.contentUI.transform.GetChild(i).gameObject;
            Descriptor descriptor = descUI.GetComponent<Descriptor>();
            // save name
            if (descriptor.GetType() == typeof(TaskName))
                syncTask.id = descUI.GetComponentInChildren<TMP_InputField>().text;
            // save objective
            if (descriptor.GetType() == typeof(TaskObjective))
            {
                syncTask.objective = descUI.GetComponentInChildren<TMP_InputField>(true).text;
                syncTask.objectiveViewState = descUI.GetComponentInChildren<Toggle>().isOn;
            }
            // save complexity
            if (descriptor.GetType() == typeof(Complexity))
                syncTask.rawComplexities.Add(new Scenario.RawComplexity(i, descUI.GetComponentInChildren<TMP_Dropdown>().value));
            // save artefacts
            if (descriptor.GetType() == typeof(Artefact))
                syncTask.rawArtefacts.Add(new Scenario.RawArtefact(i, descUI.GetComponentInChildren<TMP_InputField>().text));
            // save observations
            if (descriptor.GetType() == typeof(Observation))
            {
                Transform contentArea = descUI.transform.Find("Content");
                Scenario.RawObservation newObservation = new Scenario.RawObservation(i, descUI.GetComponentInChildren<Toggle>().isOn, contentArea.GetChild(0).GetComponentInChildren<TMP_InputField>(true).text);
                foreach (Decision decision in contentArea.GetComponentsInChildren<Decision>(true))
                    newObservation.addRawDecision(decision.GetComponentInChildren<TMP_InputField>(true).text);
                syncTask.rawObservations.Add(newObservation);
            }
            // save working session
            if (descriptor.GetType() == typeof(WorkingSession))
            {
                Transform contentArea = descUI.transform.Find("Content");
                Scenario.RawWorkingSession newWorkingSession = new Scenario.RawWorkingSession(i, descUI.GetComponentInChildren<Toggle>().isOn, descUI.transform.Find("Header").GetComponentInChildren<TMP_InputField>(true).text, contentArea.GetChild(0).GetComponentInChildren<TMP_InputField>(true).text, contentArea.GetChild(1).GetComponentInChildren<TMP_InputField>(true).text);
                foreach (Participant participant in contentArea.GetComponentsInChildren<Participant>(true))
                    newWorkingSession.addParticipant(participant.transform.GetChild(1).GetComponentInChildren<TMP_InputField>(true).text, participant.transform.GetChild(3).GetComponentInChildren<TMP_InputField>(true).text);
                syncTask.rawWorkingSessions.Add(newWorkingSession);
            }
            // save competency
            if (descriptor.GetType() == typeof(Competency))
            {
                Transform headerArea = descUI.transform.Find("Header");
                syncTask.rawCompetencies.Add(new Scenario.RawCompetency(i, descUI.GetComponentInChildren<Toggle>().isOn, headerArea.GetChild(1).GetComponent<TMP_Dropdown>().value, headerArea.GetChild(3).GetComponent<TMP_Dropdown>().value, descUI.transform.Find("Content").GetComponentInChildren<TMP_InputField>(true).text));
            }
            // save production
            if (descriptor.GetType() == typeof(Production))
                syncTask.rawProductions.Add(new Scenario.RawProduction(i, descUI.GetComponentInChildren<Toggle>().isOn, descUI.GetComponentInChildren<TMP_InputField>(true).text));
            // save antecedent
            if (descriptor.GetType() == typeof(Antecedent))
                syncTask.rawAntecedents.Add(new Scenario.RawAntecedent(i, descUI.GetComponentInChildren<TMP_Dropdown>(true).value));
            // save subtask
            if (descriptor.GetType() == typeof(SubTask))
                syncTask.rawSubTasks.Add(new Scenario.RawSubTask(i, descUI.GetComponentInChildren<TMP_Dropdown>(true).value));
        }

        // override Task
        scenario.rawScenario.tasks[currentSelection] = syncTask;
    }

    public void showTask(int value)
    {
        // save previous task
        if (currentSelection >= 0 && currentSelection < taskListUI.options.Count) {
            syncCurrentTask();
        }
        // remove all childs
        for (int i = 0; i < scenario.contentUI.transform.childCount; i++)
        {
            GameObject child = scenario.contentUI.transform.GetChild(i).gameObject;
            GameObjectManager.unbind(child);
            GameObject.Destroy(child);
        }

        if (value >= 0 && value < scenario.rawScenario.tasks.Count)
        {
            // Load new selected task
            Scenario.RawTask task = scenario.rawScenario.tasks[value];
            // load name
            GameObject taskName = addDescriptor(scenario.taskNamePrefab);
            taskName.GetComponentInChildren<TMP_InputField>().text = task.id;
            // load objective
            GameObject taskObjective = addDescriptor(scenario.taskObjectivePrefab);
            taskObjective.GetComponentInChildren<TMP_InputField>(true).text = task.objective;
            taskObjective.GetComponentInChildren<Toggle>().isOn = task.objectiveViewState;
            
            // load other descriptors
            // find max id of descriptors
            int max = -1;
            foreach (Scenario.RawComplexity rawComplexity in task.rawComplexities)
                if (rawComplexity.pos > max)
                    max = rawComplexity.pos;
            foreach (Scenario.RawArtefact rawArtefact in task.rawArtefacts)
                if (rawArtefact.pos > max)
                    max = rawArtefact.pos;
            foreach (Scenario.RawObservation rawObservation in task.rawObservations)
                if (rawObservation.pos > max)
                    max = rawObservation.pos;
            foreach (Scenario.RawWorkingSession rawWorkingSession in task.rawWorkingSessions)
                if (rawWorkingSession.pos > max)
                    max = rawWorkingSession.pos;
            foreach (Scenario.RawCompetency rawCompetency in task.rawCompetencies)
                if (rawCompetency.pos > max)
                    max = rawCompetency.pos;
            foreach (Scenario.RawProduction rawProduction in task.rawProductions)
                if (rawProduction.pos > max)
                    max = rawProduction.pos;
            foreach (Scenario.RawAntecedent rawAntecedent in task.rawAntecedents)
                if (rawAntecedent.pos > max)
                    max = rawAntecedent.pos;
            foreach (Scenario.RawSubTask rawSubTask in task.rawSubTasks)
                if (rawSubTask.pos > max)
                    max = rawSubTask.pos;
            // create descripors in order
            for (int i = 0; i <= max; i++)
            {
                // look for descriptor associated to this pos in all sets
                // check complexity
                foreach (Scenario.RawComplexity rawComplexity in task.rawComplexities)
                    if (rawComplexity.pos == i)
                    {
                        GameObject taskComplexity = addDescriptor(scenario.taskComplexityPrefab);
                        taskComplexity.GetComponentInChildren<TMP_Dropdown>().value = rawComplexity.complexity;
                    }
                // check artefact
                foreach (Scenario.RawArtefact rawArtefact in task.rawArtefacts)
                    if (rawArtefact.pos == i)
                    {
                        GameObject taskArtefact = addDescriptor(scenario.taskArtefactPrefab);
                        taskArtefact.GetComponentInChildren<TMP_InputField>().text = rawArtefact.artefact;
                    }
                // check Observation
                foreach (Scenario.RawObservation rawObservation in task.rawObservations)
                    if (rawObservation.pos == i)
                    {
                        GameObject taskObservation = addDescriptor(scenario.taskObservationPrefab);
                        taskObservation.GetComponentInChildren<TMP_InputField>(true).text = rawObservation.content;
                        foreach (string decisionContent in rawObservation.decisions)
                        {
                            GameObject decision = taskObservation.GetComponent<Observation>().addDecision(scenario.taskDecisionPrefab);
                            decision.GetComponentInChildren<TMP_InputField>(true).text = decisionContent;
                        }
                        taskObservation.GetComponentInChildren<Toggle>().isOn = rawObservation.viewState;
                    }
                // check Working session
                foreach (Scenario.RawWorkingSession rawWorkingSession in task.rawWorkingSessions)
                    if (rawWorkingSession.pos == i)
                    {
                        GameObject taskWorkingSession = addDescriptor(scenario.taskWorkingSessionPrefab);
                        taskWorkingSession.transform.Find("Header").GetComponentInChildren<TMP_InputField>().text = rawWorkingSession.id;
                        Transform contentArea = taskWorkingSession.transform.Find("Content");
                        contentArea.GetChild(0).GetComponentInChildren<TMP_InputField>(true).text = rawWorkingSession.duration;
                        contentArea.GetChild(1).GetComponentInChildren<TMP_InputField>(true).text = rawWorkingSession.organisation;
                        foreach (Scenario.RawParticipant participantContent in rawWorkingSession.participants)
                        {
                            GameObject participant = taskWorkingSession.GetComponent<WorkingSession>().addParticipant(scenario.taskParticipantPrefab);
                            participant.transform.GetChild(1).GetComponentInChildren<TMP_InputField>(true).text = participantContent.profil;
                            participant.transform.GetChild(3).GetComponentInChildren<TMP_InputField>(true).text = participantContent.role;
                        }
                        taskWorkingSession.GetComponentInChildren<Toggle>().isOn = rawWorkingSession.viewState;
                    }
                // check competency
                foreach (Scenario.RawCompetency rawCompetency in task.rawCompetencies)
                    if (rawCompetency.pos == i)
                    {
                        GameObject taskCompetency = addDescriptor(scenario.taskCompetencyPrefab);
                        Transform headerArea = taskCompetency.transform.Find("Header");
                        headerArea.GetChild(1).GetComponent<TMP_Dropdown>().value = rawCompetency.type;
                        headerArea.GetChild(3).GetComponent<TMP_Dropdown>().value = rawCompetency.id;
                        taskCompetency.transform.Find("Content").GetComponentInChildren<TMP_InputField>(true).text = rawCompetency.details;
                        taskCompetency.GetComponentInChildren<Toggle>().isOn = rawCompetency.viewState;
                    }
                // check production
                foreach (Scenario.RawProduction rawProduction in task.rawProductions)
                    if (rawProduction.pos == i)
                    {
                        GameObject taskProduction = addDescriptor(scenario.taskProductionPrefab);
                        taskProduction.GetComponentInChildren<TMP_InputField>(true).text = rawProduction.production;
                        taskProduction.GetComponentInChildren<Toggle>().isOn = rawProduction.viewState;
                    }
                // check antecedent
                foreach (Scenario.RawAntecedent rawAntecedent in task.rawAntecedents)
                    if (rawAntecedent.pos == i)
                    {
                        GameObject taskAntecedent = addDescriptor(scenario.taskAntecedentPrefab);
                        TMP_Dropdown drop = taskAntecedent.GetComponentInChildren<TMP_Dropdown>(true);
                        drop.value = rawAntecedent.antecedent;
                        drop.RefreshShownValue();
                    }
                // check subtask
                foreach (Scenario.RawSubTask rawSubTask in task.rawSubTasks)
                    if (rawSubTask.pos > max)
                    {
                        GameObject taskSubTask = addDescriptor(scenario.taskSubTaskPrefab);
                        TMP_Dropdown drop = taskSubTask.GetComponentInChildren<TMP_Dropdown>(true);
                        drop.value = rawSubTask.subTask;
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