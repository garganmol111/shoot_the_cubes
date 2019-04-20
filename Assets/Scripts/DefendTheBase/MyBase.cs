using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MyBase : MonoBehaviour
{
	#region VARIABLE

	public float rotationSpeed = 10f;

	public int health = 100;
	public AudioClip explosionSoundFx;
	public AudioClip hitSoundFx;
	// TODO choose a different sound for the Hit

	private bool mIsAlive = true;
	private AudioSource mAudioSource;
	public Slider mHealthSlider;

	#endregion // VARIABLES


	#region UNITY_METHODS

	// Use this for initialization
	void Start ()
	{
		mAudioSource = GetComponent<AudioSource> ();
		mHealthSlider.maxValue = health;
		mHealthSlider.value = health;
	}
	
	// Update is called once per frame
	void Update ()
	{
		RotateBase ();
	}

	#endregion // UNITY_REGION

	#region PRIVATE_METHODS

	private void RotateBase ()
	{
		if ( mIsAlive && gameObject != null ) {
			// implement object rotation
			transform.Rotate ( Vector3.up, rotationSpeed * Time.deltaTime);
		}
	}

	// Destroy base
	private IEnumerator DestroyBase ()
	{
		mIsAlive = false;
		mAudioSource.clip = explosionSoundFx;
		mAudioSource.Play ();

		GetComponent<Renderer> ().enabled = false;

		// inform all Enemyes that Base is Lost
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		foreach (GameObject e in enemies) {
			e.gameObject.GetComponent<CubeBehaviorScript> ().SwitchBaseStatus (false);
		}

		yield return new WaitForSeconds (mAudioSource.clip.length);
		Destroy (gameObject);

	}

	#endregion // PRIVATE_METHODS

	#region PUBLIC_METHODS

	// receive damage
	public void TakeHit (int damage)
	{
		health -= damage;

		mHealthSlider.value = health;

		if (health <= 0) {
			StartCoroutine (DestroyBase ());
		} else {
			mAudioSource.clip = hitSoundFx;
			mAudioSource.Play ();
		}
	}

	#endregion // PUBLIC_METHODS
}

