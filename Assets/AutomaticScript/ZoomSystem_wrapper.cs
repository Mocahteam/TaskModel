using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class ZoomSystem_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void setPause(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod ("ZoomSystem", "setPause", newState);
	}

	public void paintScenario(UnityEngine.GameObject go)
	{
		MainLoop.callAppropriateSystemMethod ("ZoomSystem", "paintScenario", go);
	}

	public void clearScenario(UnityEngine.GameObject go)
	{
		MainLoop.callAppropriateSystemMethod ("ZoomSystem", "clearScenario", go);
	}

	public void onNewMovableTask(UnityEngine.GameObject go)
	{
		MainLoop.callAppropriateSystemMethod ("ZoomSystem", "onNewMovableTask", go);
	}

}
