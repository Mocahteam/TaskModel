using UnityEngine;
using FYFY;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ZoomSystem : FSystem {

	private Family f_lines = FamilyManager.getFamily(new AnyOfComponents(typeof(LineRenderer)));
    private Family f_zoomable = FamilyManager.getFamily(new AnyOfComponents(typeof(Zoom)));

    private Family f_enabledZoomArea = FamilyManager.getFamily(new AllOfComponents(typeof(Zoom)), new AnyOfProperties(PropertyMatcher.PROPERTY.ACTIVE_IN_HIERARCHY));
    private Family f_disabledZoomArea = FamilyManager.getFamily(new AllOfComponents(typeof(Zoom)), new NoneOfProperties(PropertyMatcher.PROPERTY.ACTIVE_IN_HIERARCHY));

    private Family f_movableTask = FamilyManager.getFamily(new AllOfComponents(typeof(MoveTask)));

    private Family f_displayedMovableTask = FamilyManager.getFamily(new AllOfComponents(typeof(MoveTask)), new AnyOfProperties(PropertyMatcher.PROPERTY.ACTIVE_IN_HIERARCHY));
    private Family f_hiddenMovableTask = FamilyManager.getFamily(new AllOfComponents(typeof(MoveTask)), new NoneOfProperties(PropertyMatcher.PROPERTY.ACTIVE_IN_HIERARCHY));

    public static ZoomSystem instance;

    private Scenario scenario;

    private Animation taskEditorAnimation;
    private Animation scenarioViewerAnimation;
    private bool firstAnimSkiped;

    public ZoomSystem()
    {
        if (Application.isPlaying)
        {
            scenario = GameObject.Find("ScenarioLoader").GetComponent<Scenario>();
            f_enabledZoomArea.addEntryCallback(paintScenario);
            f_disabledZoomArea.addEntryCallback(clearScenario);
            f_movableTask.addEntryCallback(onNewMovableTask);
            f_displayedMovableTask.addEntryCallback(onMovableTaskDisplayed);
            f_hiddenMovableTask.addEntryCallback(onMovableTaskHidden);
            taskEditorAnimation = GameObject.Find("TaskEditor").GetComponent<Animation>();
            scenarioViewerAnimation = GameObject.Find("ScenarioViewer").GetComponent<Animation>();
            firstAnimSkiped = false;
        }
        instance = this;
    }

    public void setPause(bool newState)
    {
        Pause = newState;
    }

    // Use this to update member variables when system pause. 
    // Advice: avoid to update your families inside this function.
    protected override void onPause(int currentFrame)
    {
        if (firstAnimSkiped)
        {
            taskEditorAnimation.Play("LeftToCenter");
            scenarioViewerAnimation.Play("moveRight");
        }
        firstAnimSkiped = true;
    }

    // Use this to update member variables when system resume.
    // Advice: avoid to update your families inside this function.
    protected override void onResume(int currentFrame)
    {
        taskEditorAnimation.Play("moveLeft");
        scenarioViewerAnimation.Play("RightToCenter");
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount)
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            foreach (GameObject panel in f_zoomable)
            {
                Zoom zoom = panel.GetComponent<Zoom>();
                if (zoom.transform.localScale.x + Input.mouseScrollDelta.y * zoom.scale > 0)
                {
                    zoom.transform.localScale = new Vector3(zoom.transform.localScale.x + Input.mouseScrollDelta.y * zoom.scale, zoom.transform.localScale.y + Input.mouseScrollDelta.y * zoom.scale, zoom.transform.localScale.z);
                    if (zoom.transform.localScale.x <= 1)
                        foreach (GameObject line in f_lines)
                        {
                            LineRenderer line_r = line.GetComponent<LineRenderer>();
                            float defaultWidth = 0.05f * zoom.transform.localScale.x;
                            line_r.widthCurve = new AnimationCurve(
                                 new Keyframe(0, defaultWidth)
                                 , new Keyframe(0.5f, defaultWidth)
                                 , new Keyframe(0.5001f, defaultWidth+0.3f*zoom.transform.localScale.x)
                                 , new Keyframe(0.6f, defaultWidth)
                                 , new Keyframe(1, defaultWidth));
                        }
                }

                foreach (Transform child in zoom.transform)
                    child.GetComponent<BoxCollider2D>().edgeRadius = 20 * zoom.transform.localScale.x;
            }
        }

        foreach (GameObject zoomablePanel in f_enabledZoomArea)
        {
            RectTransform rect = zoomablePanel.GetComponent<RectTransform>();

            float xMax = 0;
            float yMin = 0;
            foreach (RectTransform child in zoomablePanel.transform)
            {
                xMax = child.localPosition.x + child.rect.width / 2 > xMax ? child.localPosition.x + child.rect.width / 2 : xMax;
                yMin = child.localPosition.y - (child.rect.height / 2) < yMin ? child.localPosition.y - (child.rect.height / 2) : yMin;
            }

            rect.sizeDelta = new Vector2(xMax, -yMin);

            foreach (RectTransform child in zoomablePanel.transform)
            {
                if (child.localPosition.x - (child.rect.width / 2) < 0)
                    child.localPosition = new Vector3(child.rect.width / 2, child.localPosition.y, child.localPosition.z);
                if (child.localPosition.y + (child.rect.height / 2) > 0)
                    child.localPosition = new Vector3(child.localPosition.x, -(child.rect.height / 2), child.localPosition.z);
            }
        }
    }

    public void paintScenario(GameObject go)
    {
        Zoom zoom = go.GetComponent<Zoom>();

        int cpt = 0;
        foreach (Scenario.RawTask task in scenario.rawScenario.tasks)
        {
            GameObject newTask = GameObject.Instantiate(zoom.taskPrefab);
            task.linkedMovableUI = newTask;
            newTask.name = task.id;
            newTask.GetComponent<MoveTask>().scenarioId = cpt;
            newTask.GetComponentInChildren<TMP_Text>().text = task.id;
            newTask.transform.SetParent(go.transform);
            newTask.transform.localPosition = new Vector3(0, 0, 0);
            // enable "view subtasks"
            if (task.rawSubTasks.Count > 0)
            {
                Toggle toggle = newTask.transform.GetComponentInChildren<Toggle>(true);
                toggle.gameObject.SetActive(true);
                toggle.onValueChanged.AddListener(delegate (bool newState) { onViewSubTasks(newTask); });
            }
            GameObjectManager.bind(newTask);
            cpt++;
        }
    }

    public void clearScenario(GameObject go)
    {
        // remove all old childs
        foreach (Transform child in go.transform)
        {
            GameObjectManager.unbind(child.gameObject);
            GameObject.Destroy(child.gameObject);
        }
    }

    public void onNewMovableTask(GameObject go)
    {
        // update position and collider
        if (go.transform.localPosition.x == 0 && go.transform.localPosition.y == 0 && go.transform.localPosition.z == 0)
            go.transform.localPosition = new Vector3(go.transform.localPosition.x + (go.transform as RectTransform).rect.width / 2+200, go.transform.localPosition.y - (go.transform as RectTransform).rect.height / 2-200, 0);
        go.GetComponent<BoxCollider2D>().size = new Vector2((go.transform as RectTransform).rect.width, (go.transform as RectTransform).rect.height);

        MoveTask mt = go.GetComponent<MoveTask>();
        // process antecedents/nexts
        Scenario.RawTask task = scenario.rawScenario.tasks[mt.scenarioId];
        foreach (Scenario.RawAntecedent rawAntecedent in task.rawAntecedents)
        {
            // create an antecedent linker
            GameObject linker = GameObject.Instantiate(mt.antecedentPrefab);
            LinkedWith link = linker.GetComponent<LinkedWith>();
            // set link to the antecedent task
            link.link = scenario.rawScenario.tasks[rawAntecedent.antecedent].linkedMovableUI;
            GameObjectManager.bind(linker);
            // add linker to next task
            GameObjectManager.setGameObjectParent(linker, go, true);
            // move at the right of my antecedent
            go.transform.localPosition = new Vector3(link.link.transform.localPosition.x+100, link.link.transform.localPosition.y, 0);

            // create a next linker
            linker = GameObject.Instantiate(mt.nextPrefab);
            link = linker.GetComponent<LinkedWith>();
            // set link to the next task
            link.link = go;
            GameObjectManager.bind(linker);
            // add linker to the antecedent task
            GameObjectManager.setGameObjectParent(linker, scenario.rawScenario.tasks[rawAntecedent.antecedent].linkedMovableUI, true);
        }

        // process subtasks/parent
        foreach (Scenario.RawSubTask rawSubTask in task.rawSubTasks)
        {
            // create a sub task linker
            GameObject linker = GameObject.Instantiate(mt.subTaskPrefab);
            LinkedWith link = linker.GetComponent<LinkedWith>();
            // set link to the sub task
            link.link = scenario.rawScenario.tasks[rawSubTask.subTask].linkedMovableUI;
            GameObjectManager.bind(linker);
            // add linker to the parent task
            GameObjectManager.setGameObjectParent(linker, go, true);
            // hide sub task
            GameObjectManager.setGameObjectState(link.link, false);
            // hide line renderer
            GameObjectManager.setGameObjectState(linker, false);
            // move sub task under me
            link.link.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y - 80, 0);

            // create a parent linker
            linker = GameObject.Instantiate(mt.parentPrefab);
            link = linker.GetComponent<LinkedWith>();
            // set link to the parent task
            link.link = go;
            GameObjectManager.bind(linker);
            // add linker to the sub task
            GameObjectManager.setGameObjectParent(linker, scenario.rawScenario.tasks[rawSubTask.subTask].linkedMovableUI, true);
        }
        // By default hide this task => see onMovableTaskHidden for enabling
        GameObjectManager.setGameObjectState(go, false);
    }

    private void onViewSubTasks (GameObject task)
    {
        // hide all tasks => main tasks will automatically redisplayed see onMovableTaskHidden
        foreach (GameObject t in f_movableTask)
            GameObjectManager.setGameObjectState(t, false);
        Toggle toggle = task.GetComponentInChildren<Toggle>();
        if (toggle != null)
        {
            foreach (LinkedWith lw in task.GetComponentsInChildren<LinkedWith>())
                if (lw.type == LinkedWith.LinkType.SubTask)
                    GameObjectManager.setGameObjectState(lw.gameObject, toggle.isOn);
        }
    }

    private void onMovableTaskHidden(GameObject task)
    {
        if (task.transform.parent.gameObject.activeInHierarchy)
        {
            // display only root tasks (no antecedent, no parent)
            bool isRootTask = true;
            foreach (LinkedWith lw in task.GetComponentsInChildren<LinkedWith>())
                if (lw.type == LinkedWith.LinkType.Antecedent || lw.type == LinkedWith.LinkType.Parent)
                {
                    isRootTask = false;
                    break;
                }
            if (isRootTask)
                GameObjectManager.setGameObjectState(task, true);
        }
    }

    private void onMovableTaskDisplayed(GameObject task)
    {
        foreach (LinkedWith lw in task.GetComponentsInChildren<LinkedWith>(true))
        {
            if (lw.type == LinkedWith.LinkType.Antecedent || lw.type == LinkedWith.LinkType.Next || lw.type == LinkedWith.LinkType.Parent)
            {
                // display target link
                GameObjectManager.setGameObjectState(lw.link, true);
                // display line renderer
                GameObjectManager.setGameObjectState(lw.gameObject, true);
            }
            else if (lw.type == LinkedWith.LinkType.SubTask)
            {
                Toggle toggle = task.GetComponentInChildren<Toggle>();
                if (toggle != null && toggle.isOn)
                {
                    // display target link
                    GameObjectManager.setGameObjectState(lw.link, true);
                    // display line renderer
                    GameObjectManager.setGameObjectState(lw.gameObject, true);
                }
            }
        }
    }
}