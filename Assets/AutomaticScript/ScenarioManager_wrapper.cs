using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class ScenarioManager_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void addNewTask()
	{
		MainLoop.callAppropriateSystemMethod ("ScenarioManager", "addNewTask", null);
	}

	public void removeCurrentTask()
	{
		MainLoop.callAppropriateSystemMethod ("ScenarioManager", "removeCurrentTask", null);
	}

	public void showTask(System.Int32 value)
	{
		MainLoop.callAppropriateSystemMethod ("ScenarioManager", "showTask", value);
	}

}
