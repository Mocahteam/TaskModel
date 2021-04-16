using UnityEngine;
using FYFY;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class ObservationManager : FSystem {

    private Family f_decisions = FamilyManager.getFamily(new AnyOfComponents(typeof(Decision)));

    public static ObservationManager instance;

    private Dictionary<int, Transform> decisionId2contentAreaTr;

    public ObservationManager()
    {
        if (Application.isPlaying)
        {
            decisionId2contentAreaTr = new Dictionary<int, Transform>();

            f_decisions.addEntryCallback(onNewDecision);
            f_decisions.addExitCallback(onDecisionRemoved);

            foreach (GameObject decision in f_decisions)
                decisionId2contentAreaTr.Add(decision.GetInstanceID(), decision.transform.parent.parent);
        }
        instance = this;
    }

    private void onNewDecision(GameObject go)
    {
        decisionId2contentAreaTr.Add(go.GetInstanceID(), go.transform.parent.parent);
    }

    private void onDecisionRemoved (int gameObjectInstanceId)
    {
        if (decisionId2contentAreaTr.ContainsKey(gameObjectInstanceId))
        {
            Transform contentArea = decisionId2contentAreaTr[gameObjectInstanceId];
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