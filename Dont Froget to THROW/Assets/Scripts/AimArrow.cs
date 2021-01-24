using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimArrow : MonoBehaviour {

	public bool isShoot = false;
	public GameObject currentCharacter;
	public Slider power;
	public float speed = 5f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		if (isShoot) {

			Vector2 direction = Camera.main.ScreenToWorldPoint (Input.mousePosition) - currentCharacter.transform.position;
			float angle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
			Quaternion rotation = Quaternion.AngleAxis (angle - 90f, Vector3.forward);
			transform.rotation = Quaternion.Slerp (transform.rotation, rotation, speed * Time.deltaTime);

			float distance = direction.magnitude;
			float maxPower = currentCharacter.GetComponent <CharacterController>().currentMaxPower;
			if (distance < 2f) {
				power.value = 0f;
				distance = 2f;
			} else if ((distance - 2f) * 20f > maxPower) {
				power.value = maxPower;
				distance = (maxPower / 20f) + 2f;
			} else {
				power.value = (distance - 2f) * 20f;
			}

			float deltaX = distance * Mathf.Cos(angle * Mathf.Deg2Rad);
			float deltaY = distance * Mathf.Sin(angle * Mathf.Deg2Rad);


			Vector3 newPosition = new Vector3 (currentCharacter.transform.position.x + deltaX, currentCharacter.transform.position.y + deltaY, currentCharacter.transform.position.z);
			//transform.position = Vector2.Lerp (transform.position, Camera.main.ScreenToWorldPoint (Input.mousePosition), speed);
			transform.position = Vector2.Lerp (transform.position, newPosition, speed);

			if ((transform.position.x < currentCharacter.transform.position.x && !currentCharacter.GetComponent<CharacterController> ().isFlipped) || (transform.position.x > currentCharacter.transform.position.x && currentCharacter.GetComponent<CharacterController> ().isFlipped)) {
				currentCharacter.GetComponent<CharacterController> ().flipSprite ();
			}

			if (Input.GetMouseButtonDown (0)) {
				Debug.Log ("mouse Down");

				deltaX = 2f * Mathf.Cos(angle * Mathf.Deg2Rad);
				deltaY = 2f * Mathf.Sin(angle * Mathf.Deg2Rad);

				Vector3 bulletSpawnPosition = new Vector3 (currentCharacter.transform.position.x + deltaX, currentCharacter.transform.position.y + deltaY, currentCharacter.transform.position.z);

				currentCharacter.GetComponent<CharacterController> ().shootBullet (bulletSpawnPosition,transform.rotation,direction* power.value);
			}

			if (Input.GetMouseButtonDown (1)) {
				Debug.Log ("mouse2 Down");
				power.value = maxPower;
				currentCharacter.GetComponent<CharacterController> ().changeState (2);
			}

		}

	}
}
