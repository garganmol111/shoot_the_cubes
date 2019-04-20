using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CubeBehaviorScript : MonoBehaviour {

	#region VARIABLES

	public float mScaleMax	= 10f;
	public float mScaleMin	= 2f;

	public int mCubeHealth	= 100;

	// Orbit max Speed
	public float mOrbitMaxSpeed = 30f;

	public float velocityToBase = 5.0f;
	public int damage = 10;

	// Orbit speed
	private float mOrbitSpeed;

	// Orbit direction
	private Vector3 mOrbitDirection;

	// Max Cube Scale
	private Vector3 mCubeMaxScale;

	// Growing Speed
	public float mGrowingSpeed	= 10f;
	private bool mIsCubeScaled	= false;

	private bool mIsAlive		= true;
	private AudioSource mExplosionFx;

	private GameObject mBase;
	private bool mIsBaseVisible = false;

	private Vector3 mRotationDirection;
	private Scene mScene;

	#endregion

	#region UNITY_METHODS

	void Start () {
		// Get Scene name
		mScene = SceneManager.GetActiveScene();
		CubeSettings();
	}

	void Update () {
		// makes the cube orbit and rotate
		RotateCube();

		if ( mScene.name == "DefendTheBase" ) {
			// move cube towards the base, when it's visible
			MoveToBase ();
		}

		// scale cube if needed
		if ( !mIsCubeScaled )
			ScaleObj();
	}
	#endregion


	#region PRIVATE_METHODS
	private void CubeSettings ()
	{
		// defining the orbit direction
		float x = Random.Range ( -1f, 1f );
		float y = Random.Range (-1f, 1f);
		float z = Random.Range ( -1f, 1f );

		// TODO update tutorial with new code
		// define stettings according to scene name
		if ( mScene.name == "ShootTheCubesMain" )
		{
			mOrbitDirection = new Vector3( x, y, z );
		}
		else if ( mScene.name == "DefendTheBase" )
		{
			// orbit only on y axys
			mOrbitDirection = new Vector3 (0, y, 0);

			// scale size must be limited
			mScaleMin = 1f;
			mScaleMax = 0.5f;

			velocityToBase = 0.8f;
		}

		// rotating around its axis
		float rx = Random.Range (-1f, 1f);
		float ry = Random.Range (-1f, 1f);
		float rz = Random.Range (-1f, 1f);

		mRotationDirection = new Vector3 (rx, ry, rz);


		// defining speed
		mOrbitSpeed = Random.Range (5f, mOrbitMaxSpeed);

		// defining scale
		float scale = Random.Range (mScaleMin, mScaleMax);
		mCubeMaxScale = new Vector3 (scale, scale, scale);

		// set cube scale to 0, to grow it lates
		transform.localScale = Vector3.zero;

		// getting Explosion Sound Effect
		mExplosionFx = GetComponent<AudioSource> ();
	}

	// Rotate the cube around the base
	private void RotateCube ()
	{
		// rotate around base or camera
		if (mIsBaseVisible && mBase != null && mIsAlive) {
			// rotate cube around base
			transform.RotateAround (
				mBase.transform.position, mOrbitDirection, mOrbitSpeed * Time.deltaTime);
		} else {
			transform.RotateAround (
				Camera.main.transform.position, mOrbitDirection, mOrbitSpeed * Time.deltaTime);
		}
		transform.Rotate (mRotationDirection * 100 * Time.deltaTime);
	}

	// Move the cube toward the base
	private void MoveToBase ()
	{
		// make the cube move towards the base only if base is present
		if (mIsBaseVisible && mIsAlive && gameObject != null && mBase != null) {
			float step = velocityToBase * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, mBase.transform.position, step);
		}
	}

	// Scale object from 0 to 1
	private void ScaleObj(){

		// growing obj
		if ( transform.localScale != mCubeMaxScale )
			transform.localScale = Vector3.Lerp( transform.localScale, mCubeMaxScale, Time.deltaTime * mGrowingSpeed );
		else
			mIsCubeScaled = true;
	}

	// make a damage on target
	private void TargetHit (GameObject target)
	{
		Debug.Log ("TargetHit: " + target.name);
		if (target.name == "Base") {
			// make damage on base
			MyBase baseCtr = target.GetComponent<MyBase> ();
			baseCtr.TakeHit (damage);
			StartCoroutine (DestroyCube ());
		}
	}

	// Destroy Cube
	private IEnumerator DestroyCube(){
		mIsAlive = false;

		mExplosionFx.Play();

		GetComponent<Renderer>().enabled = false;

		yield return new WaitForSeconds(mExplosionFx.clip.length);
		Destroy(gameObject);
	}

    #endregion

    #region PUBLIC_METHODS

    // Cube got Hit
	// return 'false' when cube was destroyed
	public bool Hit( int hitDamage ){
		mCubeHealth -= hitDamage;
		if ( mCubeHealth >= 0 && mIsAlive ) {
			StartCoroutine( DestroyCube());
			return true;
		}
		return false;
	}

	public void OnCollisionEnter (Collision col)
	{
		TargetHit (col.gameObject);
	}

	// Receive current base status
	public void SwitchBaseStatus (bool isOn)
	{
		// stop the cube on the movement toward base
		mIsBaseVisible = isOn;
		if (isOn) {
			mBase = GameObject.Find ("Base");
		} else {
			mBase = null;
		}
	}

	#endregion
}
