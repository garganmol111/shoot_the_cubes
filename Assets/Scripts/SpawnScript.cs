using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

using Vuforia;

public class SpawnScript : MonoBehaviour
{

	#region VARIABLES

	private bool mSpawningStarted = false;

	// Cube element to spawn
	public GameObject mCubeObj;
	// Qtd of Cubes to be Spawned
	public int mTotalCubes = 10;

	private int mCurrentCubes	= 0;

	// Time to spawn the Cubes
	public float mTimeToSpawn	= 1f;

	private int mDistanceFromBase = 5;

	private List<GameObject> mCubes;

	private bool mIsBaseOn;
	private Scene mScene;

	#endregion // VARIABLES


	#region UNITY_METHODS

	// Use this for initialization
	void Start ()
	{
		mScene = SceneManager.GetActiveScene ();
		mCubes = new List<GameObject> ();

		if (mScene.name == "ShootTheCubesMain") {
			StartSpawn ();
//			StartCoroutine( SpawnLoop() );
		}
	}

	// Update is called once per frame
	void Update ()
	{

	}

	#endregion // UNITY_METHODS

	#region PUBLIC_METHODS

	// Base was found by the tracker
	public void BaseOn (Vector3 basePosition)
	{
		Debug.Log ("SpawnScript2: BaseOn");

		mIsBaseOn = true;

		// change position
		SetPosition (basePosition);

		// start spawning process if necessary
		StartSpawn ();

		// inform all cubes on screen that base appeared
		InformBaseOnToCubes ();
	}

	// Base lost by the tracker
	public void BaseOff ()
	{	
		mIsBaseOn = false;
		mSpawningStarted = false;

		// inform all cubes on screen that base is lost
		InformBaseOffToCubes ();
	}

    public void DecCurrentCubes()
    {
        mCurrentCubes--;
    }
	#endregion // PUBLIC_METHODS


	#region PRIVATE_METHODS

	// We'll use a Coroutine to give a little
	// delay before setting the position
	private IEnumerator ChangePosition ()
	{
		Debug.Log ("ChangePosition");
		yield return new WaitForSeconds (0.2f);
		// Define the Spawn position only once

		// change the position only if Vuforia is active
		if (VuforiaBehaviour.Instance.enabled)
			SetPosition (null);
		
	}

	// Set position
	private void SetPosition (System.Nullable<Vector3> pos)
	{
		if (mScene.name == "ShootTheCubesMain") {
			// get the camera position
			Transform cam = Camera.main.transform;

			// set the position 10 units foward the camera position
			transform.position = cam.forward * 10;
		} else if (mScene.name == "DefendTheBase") {
			if (pos != null) {
				Vector3 basePosition = (Vector3)pos;
				transform.position = 
					new Vector3 (basePosition.x, basePosition.y + mDistanceFromBase, basePosition.z);
			}
		}

	}

	// Inform all spawned cubes of the base position
	private void InformBaseOnToCubes ()
	{
		//			Debug.Log("InformBaseOnToCubes");
		foreach (GameObject cube in mCubes) {
			cube.GetComponent<CubeBehaviorScript> ().SwitchBaseStatus (mIsBaseOn);
		}
	}

	// Inform to all cubes that the base is off
	private void InformBaseOffToCubes ()
	{
		//			Debug.Log("InformBaseOffToCubes");
		foreach (GameObject cube in mCubes) {
			cube.GetComponent<CubeBehaviorScript> ().SwitchBaseStatus (mIsBaseOn);
		}
	}

	// Start spawining process
	private void StartSpawn ()
	{
		if (!mSpawningStarted) {
			// begin spawn
			mSpawningStarted = true;
			StartCoroutine (SpawnLoop ());
		}
	}

	// Loop Spawning cube elements
	private IEnumerator SpawnLoop ()
	{
		if (mScene.name == "ShootTheCubesMain") {
			// Defining the Spawning Position
			StartCoroutine (ChangePosition ());
		}

		yield return new WaitForSeconds (0.2f);
		// Spawning the elements
		while (mCurrentCubes <= (mTotalCubes - 1)) {
			// Start the process with diferent condition
			// depending on the current stage name
			if (mScene.name == "ShootTheCubesMain" ||
			    (mScene.name == "DefendTheBase" && mIsBaseOn)) {

				mCubes.Add (SpawnElement ());
				mCubes [mCurrentCubes].GetComponent<CubeBehaviorScript> ().SwitchBaseStatus (mIsBaseOn);
				mCurrentCubes++;

			}

			yield return new WaitForSeconds (Random.Range (mTimeToSpawn, mTimeToSpawn * 3));
		}
	}

	// Spawn a cube
	private GameObject SpawnElement ()
	{
		// spawn the element on a random position, inside a imaginary sphere
		GameObject cube = Instantiate (mCubeObj, (Random.insideUnitSphere * 4) + transform.position, transform.rotation) as GameObject;
		// define a random scale for the cube
		float scale = Random.Range (6f, 15f);
		// change the cube scale
		cube.transform.localScale = new Vector3 (scale, scale, scale);
		return cube;
	}

	#endregion // PRIVATE_METHODS
}

