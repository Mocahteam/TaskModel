using FYFY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Observation : Descriptor
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void addEmptyDecision(GameObject prefab)
    {
        GameObject newDecision = Instantiate(prefab);
        float newHeight = newDecision.GetComponent<LayoutElement>().preferredHeight;
        //increase containers height
        RectTransform contentArea = transform.Find("Content").GetComponent<RectTransform>();
        contentArea.sizeDelta = new Vector2(contentArea.rect.width, contentArea.rect.height + newHeight);
        RectTransform observationArea = GetComponent<RectTransform>();
        observationArea.sizeDelta = new Vector2(observationArea.rect.width, observationArea.rect.height + newHeight);
        // bind GO to FYFY
        GameObjectManager.bind(newDecision);
        GameObjectManager.setGameObjectParent(newDecision, transform.Find("Content").gameObject, true);
    }
}
