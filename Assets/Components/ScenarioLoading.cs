using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ScenarioLoading : MonoBehaviour
{
    private class JavaScriptData{
        public string name;
        public string content;
    }

    public void loadingFile(string content) // Fonction appelée depuis le javascript (voir Assets/WebGLTemplates/Custom/index.html)
    {
        JavaScriptData jsd = JsonUtility.FromJson<JavaScriptData>(content);
        ScenarioManager.instance.loadScenario(jsd.name, jsd.content);
    }

    public void pasteData(string content) // Fonction appelée depuis le javascript (voir Assets/WebGLTemplates/Custom/index.html)
    {
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null) {
            TMP_InputField input = EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
            if (input != null)
            {
                int min = input.selectionAnchorPosition < input.caretPosition ? input.selectionAnchorPosition : input.caretPosition;
                int max = input.selectionAnchorPosition > input.caretPosition ? input.selectionAnchorPosition : input.caretPosition;
                input.text = input.text.Substring(0, min) + content + input.text.Substring(max);
                input.selectionAnchorPosition = min + content.Length;
                input.caretPosition = min + content.Length;
            }
        }
    }
}
