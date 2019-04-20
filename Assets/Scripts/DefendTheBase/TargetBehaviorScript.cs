using UnityEngine;
using System.Collections;

public class TargetBehaviorScript : MonoBehaviour, Vuforia.ITrackableEventHandler
{

	#region PRIVATE_MEMBER_VARIABLES

	private Vuforia.TrackableBehaviour mTrackableBehaviour;


	#endregion // PRIVATE_MEMBER_VARIABLES

	#region UNITY_MONOBEHAVIOS_METHODS

	// Use this for initialization
	void Start ()
	{
		mTrackableBehaviour = GetComponent<Vuforia.TrackableBehaviour> ();
		if (mTrackableBehaviour) {
			mTrackableBehaviour.RegisterTrackableEventHandler (this);
		}

		OnTrackingLost ();
	}

	// Update is called once per frame
	void Update ()
	{
	}

	#endregion // UNITY_MONOBEHAVIOS_METHODS

	#region PUBLIC_METHODS

	/// Implementation of the ITrackableEventHandler function called when the
	/// tracking state changes.

	public void OnTrackableStateChanged (
		Vuforia.TrackableBehaviour.Status previousStatus,
		Vuforia.TrackableBehaviour.Status newStatus)
	{
		if (newStatus == Vuforia.TrackableBehaviour.Status.DETECTED ||
		    newStatus == Vuforia.TrackableBehaviour.Status.TRACKED ||
		    newStatus == Vuforia.TrackableBehaviour.Status.EXTENDED_TRACKED) {
			OnTrackingFound ();
		} else {
			OnTrackingLost ();
		}
	}

	#endregion // PUBLIC_METHODS


	#region PRIVATE_METHODS

	private void OnTrackingFound ()
	{
		EnableRendererAndCollider ();
        Debug.Log("xyz");
		// Inform the system that the target was found
		StartCoroutine (InformSpawnCtr (true));
	}

	private void OnTrackingLost ()
	{
		DisableRendererAndCollider ();

		// Inform the system that the target was lost
		StartCoroutine (InformSpawnCtr (false));
	}

	// inform SpanController that base was founded
	private IEnumerator InformSpawnCtr (bool isOn)
	{
		// move spawning position
		GameObject spawn = GameObject.FindGameObjectWithTag ("_SpawnController");

		yield return new WaitForSeconds (0.2f);

		// inform SpanController
		if (isOn) {
			spawn.GetComponent<SpawnScript> ().BaseOn (transform.position);
		} else {
			spawn.GetComponent<SpawnScript> ().BaseOff ();
		}
	}

	private void EnableRendererAndCollider ()
	{
		Debug.Log ("EnableRendererAndCollider");
		Renderer[] rendererComponents = GetComponentsInChildren<Renderer> (true);
		Collider[] colliderComponents = GetComponentsInChildren<Collider> (true);

		// Enable rendering:
		foreach (Renderer component in rendererComponents) {
			component.enabled = true;
		}

		// Enable colliders:
		foreach (Collider component in colliderComponents) {
			component.enabled = true;
		}
	}

	private void DisableRendererAndCollider ()
	{
		Debug.Log ("DisableRendererAndCollider");
		Renderer[] rendererComponents = GetComponentsInChildren<Renderer> (true);
		Collider[] colliderComponents = GetComponentsInChildren<Collider> (true);

		// Disable rendering:
		foreach (Renderer component in rendererComponents) {
			component.enabled = false;
		}

		// Disable colliders:
		foreach (Collider component in colliderComponents) {
			component.enabled = false;
		}

	}

	#endregion // PRIVATE_METHODS


}

