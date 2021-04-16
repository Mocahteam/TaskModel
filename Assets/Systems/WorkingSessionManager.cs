using UnityEngine;
using FYFY;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class WorkingSessionManager : FSystem {

	private Family f_workingSession = FamilyManager.getFamily(new AnyOfComponents(typeof(WorkingSession)));
    private Family f_participants = FamilyManager.getFamily(new AnyOfComponents(typeof(Participant)));

    public static WorkingSessionManager instance;

    private Dictionary<int, Transform> participantId2contentAreaTr;

    public WorkingSessionManager()
    {
        if (Application.isPlaying)
        {
            participantId2contentAreaTr = new Dictionary<int, Transform>();

            f_workingSession.addEntryCallback(onNewWorkingSession);
            f_workingSession.addExitCallback(delegate(int unused) { resynckWorkingSessionId(null); });
            f_participants.addEntryCallback(onNewParticipant);
            f_participants.addExitCallback(onParticipantRemoved);

            foreach (GameObject ws in f_workingSession)
                ws.GetComponent<WorkingSession>().id.onEndEdit.AddListener(delegate (string inputText){ onEndEdit(ws, inputText); });

            foreach (GameObject part in f_participants)
                participantId2contentAreaTr.Add(part.GetInstanceID(), part.transform.parent.parent);
        }
        instance = this;
    }

    private void onNewWorkingSession(GameObject go)
    {
        int max = 0;
        foreach (GameObject go_ws in f_workingSession)
        {
            int val;
            if (int.TryParse(go_ws.GetComponent<WorkingSession>().id.text, out val))
                if (val > max)
                    max = val;
        }
        WorkingSession ws = go.GetComponent<WorkingSession>();
        ws.id.text = ""+(max + 1);
        ws.previousValue = ws.id.text;
        ws.id.onEndEdit.AddListener(delegate (string inputText) { onEndEdit(go, inputText); });
    }

    private void onEndEdit(GameObject go, string inputText)
    {
        WorkingSession ws = go.GetComponent<WorkingSession>();
        if (inputText != ws.previousValue)
        {
            int newVal;
            if (!int.TryParse(inputText, out newVal) || newVal <= 0)
                inputText = "1";
            ws.id.text = inputText;
            ws.previousValue = inputText;
            resynckWorkingSessionId(go);
        }
    }

    private void resynckWorkingSessionId(GameObject go)
    {

        List<GameObject> orderedSessions = new List<GameObject>(f_workingSession);
        orderedSessions.Sort(delegate (GameObject x, GameObject y)
        {
            int xID = -1;
            int.TryParse(x.GetComponent<WorkingSession>().id.text, out xID);
            int yID = -1;
            int.TryParse(y.GetComponent<WorkingSession>().id.text, out yID);
            if (xID < yID) return -1;
            else if (xID > yID) return 1;
            else if (x == go) return -1; // les ids sont égaux, dans ce cas on considère celui que l'on vient d'éditer comme le plus petit, donc si go est égal à x on considère x < y => -1
            else if (y == go) return 1; // les ids sont égaux, dans ce cas on considère celui que l'on vient d'éditer comme le plus petit, donc si go est égal à y on considère x > y => 1
            else return 0;
        });

        int cpt = 1;
        foreach (GameObject orderedGo in orderedSessions)
        {
            WorkingSession ws = orderedGo.GetComponent<WorkingSession>();
            ws.id.text = "" + cpt;
            ws.previousValue = ws.id.text;
            orderedGo.GetComponent<Animation>().Play("animationPulse");
            cpt++;
        }
    }

    private void onNewParticipant(GameObject go)
    {
        participantId2contentAreaTr.Add(go.GetInstanceID(), go.transform.parent.parent);
    }

    private void onParticipantRemoved (int gameObjectInstanceId)
    {
        if (participantId2contentAreaTr.ContainsKey(gameObjectInstanceId))
        {
            Transform contentArea = participantId2contentAreaTr[gameObjectInstanceId];
            if (contentArea != null)
                MainLoop.instance.StartCoroutine(delayForceRebuild(contentArea));
        }

    }

    private IEnumerator delayForceRebuild(Transform go)
    {
        yield return new WaitForSeconds(0.5f);
        LayoutRebuilder.ForceRebuildLayoutImmediate(go as RectTransform);
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