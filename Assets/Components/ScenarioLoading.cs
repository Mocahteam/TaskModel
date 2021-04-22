using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
