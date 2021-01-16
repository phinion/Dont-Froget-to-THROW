using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour {

	public enum CharacterState{
		Shoot,
		Move,
		Idle
	
	}

	public Slider power;
	public GameObject aimArrow;
	public GameObject bulletPrefab;
	public HealthBar healthBar;

	private Rigidbody2D rb;
	private BoxCollider2D bc;

	public Collision2D lastCollision;

	public LayerMask groundLayers;

	public float health = 100f;
	public float moveSpeed = 5f;
	public float weight = 20f;
	public float currentMaxPower = 100f;
	public float jumpVelocity = 20f;

	public CharacterState currentState = CharacterState.Idle;
	public bool isFlipped = false;
	public bool isGrounded;

	// Use this for initialization
	void Start () {
		aimArrow.SetActive (false);
		healthBar.gameObject.SetActive (false);
		rb = this.GetComponent<Rigidbody2D> ();
		bc = this.GetComponent<BoxCollider2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		
		switch(currentState){
		case CharacterState.Move:
			move ();
			break;
		case CharacterState.Shoot:
			break;
		case CharacterState.Idle:
			break;

		}

	}


	public void changeState(int _state){
		currentState = (CharacterState)_state;
		Debug.Log ("current state = " + currentState);

		if (currentState == CharacterState.Shoot) {
			setArrow (true);
		} else {
			setArrow (false);
		}

		if (currentState == CharacterState.Move) {
			power.value = currentMaxPower;
		}

	}

	void move(){

		if ((Input.GetAxisRaw ("Horizontal") < 0 && !isFlipped) || (Input.GetAxisRaw ("Horizontal") > 0 && isFlipped)) {
			flipSprite ();
		}


		if (Input.GetAxisRaw ("Horizontal") != 0) {
			moveSprite ();
		}

		isGrounded = Physics2D.OverlapCircle (new Vector2 (transform.position.x, transform.position.y - 0.5f),0.5f, groundLayers);
			
		//Jump
		if (isGrounded && Input.GetKeyDown (KeyCode.W)) {
			rb.velocity = Vector2.up * 8f;
		}

	}

	/*private bool IsGrounded(){
		RaycastHit2D rc = Physics2D.BoxCast (bc.bounds.center, bc.bounds.size, 0f, Vector2.down * .1f);
		Debug.Log (rc.collider);
		return rc.collider != null;
	}*/


	public void flipSprite() {

		//Vector2 scale = transform.localScale;

		isFlipped = !isFlipped;
		transform.Rotate (0f, 180f, 0f);
		//scale.x *= -1f;
		//transform.localScale = scale;

	}

	void moveSprite(){

		Vector3 movement = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0f, 0f);
		float moveAmount = Input.GetAxisRaw ("Horizontal") * Time.deltaTime * moveSpeed;

		if (moveAmount < 0) {
			moveAmount *= -1f;
		}

		// Horizontal movement
		if (currentMaxPower >= moveAmount) {
			currentMaxPower -= moveSpeed / weight;
			power.value = currentMaxPower;
			transform.position += movement * Time.deltaTime * moveSpeed;
		}

	}

	void setArrow(bool _isShoot){
		aimArrow.SetActive (_isShoot);
		aimArrow.GetComponent<AimArrow> ().isShoot = _isShoot;
	}


	public void shootBullet(Vector3 _position, Quaternion __rotation, Vector2 _force){
		GameObject knife = Instantiate (bulletPrefab, _position, __rotation);
		knife.GetComponent<Rigidbody2D> ().AddForce(_force);
	}

	void OnCollisionEnter2D(Collision2D col){

		Bullet bullet = col.gameObject.GetComponent<Bullet> ();

		if (bullet != null && col != lastCollision) {
			Debug.Log ("bullet hit character");

			lastCollision = col;
			healthBar.gameObject.SetActive (true);


			float damage = bullet.rb.velocity.magnitude;

			damage *= 20f;
			Debug.Log (damage);

			health -= damage;
			if (health <= 0f) {
				health = 0f;
			}
			healthBar.SetSize (health / 100f);

				
			StartCoroutine (HideHealthBar ());
			Destroy (col.gameObject,2f);
		}
	}

	IEnumerator HideHealthBar(){
		Debug.Log ("hiding bar");
		yield return new WaitForSeconds (2);
		healthBar.gameObject.SetActive (false);
	}

}
