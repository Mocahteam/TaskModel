using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class MoveTask : MonoBehaviour, IPointerDownHandler
{
    public GameObject antecedentPrefab;
    public GameObject nextPrefab;
    public GameObject subTaskPrefab;
    public GameObject parentPrefab;
    public int scenarioId;

    private TMP_Dropdown taskSelector;
    private Button editScenario;

    int clicked = 0;
    float clicktime = 0;
    public float doubleClickDelay = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        taskSelector = GameObject.Find("Dropdown_selectTask").GetComponent<TMP_Dropdown>();
        editScenario = GameObject.Find("EditScenario").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void moveTask()
    {
        transform.position = Input.mousePosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Time.time - clicktime > doubleClickDelay)
            clicked = 0;
        clicked++;
        if (clicked == 1) clicktime = Time.time;
        else if (Time.time - clicktime < doubleClickDelay)
        {
            clicked = 0;
            clicktime = 0;
            taskSelector.value = scenarioId;
            taskSelector.RefreshShownValue();
            editScenario.onClick.Invoke();
        }
    }
}
