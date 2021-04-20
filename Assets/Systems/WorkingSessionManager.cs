using UnityEngine;
using FYFY;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class WorkingSessionManager : FSystem {

	private Family f_workingSession = FamilyManager.getFamily(new AnyOfComponents(typeof(WorkingSession)));

    public static WorkingSessionManager instance;

    public WorkingSessionManager()
    {
        if (Application.isPlaying)
        {

            f_workingSession.addEntryCallback(onNewWorkingSession);
            f_workingSession.addExitCallback(delegate(int unused) { resynckWorkingSessionId(null); });

            foreach (GameObject ws in f_workingSession)
                ws.GetComponent<WorkingSession>().id.onEndEdit.AddListener(delegate (string inputText){ onEndEdit(ws, inputText); });
        }
        instance = this;
    }

    private void onNewWorkingSession(GameObject go)
    {
        int max = 0;
        WorkingSession ws = go.GetComponent<WorkingSession>();
        if (ws.id.text == "")
        {
            foreach (GameObject go_ws in f_workingSession)
            {
                int val;
                if (int.TryParse(go_ws.GetComponent<WorkingSession>().id.text, out val))
                    if (val > max)
                        max = val;
            }
            ws.id.text = "" + (max + 1);
            ws.previousValue = ws.id.text;
        }
        else
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