using FYFY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkingSession : Descriptor
{
    public TMPro.TMP_InputField id;
    public string previousValue;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addEmptyParticipant(GameObject prefab) // used in Unity Inspector
    {
        addParticipant(prefab);
    }

    public GameObject addParticipant(GameObject prefab)
    {
        GameObject newParticipant = Instantiate(prefab);
        float newHeight = newParticipant.GetComponent<LayoutElement>().preferredHeight;
        //increase containers height
        RectTransform contentArea = transform.Find("Content").GetComponent<RectTransform>();
        contentArea.sizeDelta = new Vector2(contentArea.rect.width, contentArea.rect.height + newHeight);
        RectTransform workingSessionArea = GetComponent<RectTransform>();
        workingSessionArea.sizeDelta = new Vector2(workingSessionArea.rect.width, workingSessionArea.rect.height + newHeight);
        // bind GO to FYFY
        GameObjectManager.bind(newParticipant);
        GameObjectManager.setGameObjectParent(newParticipant, transform.Find("Content").gameObject, true);
        return newParticipant;
    }

    public void copyWorkingSession()
    {
        string saveId = id.text;
        id.text = "";
        copyDescriptor();
        id.text = saveId;
    }
}
