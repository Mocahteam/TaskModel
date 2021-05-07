using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Decision : Descriptor
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

    new public void copyDescriptor()
    {
        base.copyDescriptor();

        float newHeight = (transform as RectTransform).rect.height;
        //increase containers height
        RectTransform contentArea = transform.parent as RectTransform;
        contentArea.sizeDelta = new Vector2(contentArea.rect.width, contentArea.rect.height + newHeight);
        RectTransform workingSessionArea = contentArea.parent as RectTransform;
        workingSessionArea.sizeDelta = new Vector2(workingSessionArea.rect.width, workingSessionArea.rect.height + newHeight);
    }

    public override void resizeContainer()
    {
        float oldHeight = (transform as RectTransform).rect.height;
        //increase containers height
        RectTransform contentArea = transform.parent as RectTransform;
        contentArea.sizeDelta = new Vector2(contentArea.rect.width, contentArea.rect.height - oldHeight);
        RectTransform workingSessionArea = contentArea.parent as RectTransform;
        workingSessionArea.sizeDelta = new Vector2(workingSessionArea.rect.width, workingSessionArea.rect.height - oldHeight);
    }

    new void resize(GameObject grasp)
    {
        base.resize(grasp);

        RectTransform content_rt = (grasp.transform.parent.parent.parent.parent as RectTransform);
        content_rt.sizeDelta = new Vector2(content_rt.sizeDelta.x, content_rt.sizeDelta.y + step);
    }
}
