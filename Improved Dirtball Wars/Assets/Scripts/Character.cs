using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

	public GameObject lastCollision;
	public HealthBar healthBar;
	public GameObject bulletPrefab;
	public Rigidbody2D rb;

	public Animator animator;

	public float health = 100f;
	public float moveSpeed = 5f;
	public float weight = 20f;

	public float maxEnergy = 100f;
	public float currentEnergy;


	public float jumpVelocity = 20f;
	public float jumpEnergyDrain = 20f;

	public bool isFlipped = false;

	public bool isGrounded;
	public LayerMask groundLayers;

	public float healthBarYOffset = 1f;
	private Quaternion healthBarRotation;
	private bool displayingHealthBar = false;

	// Use this for initialization
	void Start () {
		healthBar.gameObject.SetActive (false);
		healthBarRotation = healthBar.gameObject.transform.rotation;
		groundLayers = LayerMask.GetMask("groundLayer");

		currentEnergy = maxEnergy;

		animator = gameObject.GetComponent<Animator>();
		rb = gameObject.GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update() {
		isGrounded = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - 0.5f), 0.5f, groundLayers);

		if (displayingHealthBar)
		{
			healthBar.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + healthBarYOffset, transform.position.z);
			healthBar.gameObject.transform.rotation = healthBarRotation;
		}

		if (isGrounded)
		{
			animator.SetBool("IsJump", false);
		}
		else
		{

			if (rb.velocity.y > 0.01f)
			{
				animator.SetBool("IsJump", true);
				animator.SetFloat("Jump", 0.5f);

			}
			else if (rb.velocity.y < -0.01f)
			{
				animator.SetBool("IsJump", true);
				animator.SetFloat("Jump", -0.5f);
			}

		}

	}

	public void Jump()
    {
		Debug.Log("JUMPING");
		//rb.AddForce(Vector2.up * jumpVelocity);
		rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
	}

	//void OnCollisionEnter2D(Collision2D col){

	//	Bullet bullet = col.gameObject.GetComponent<Bullet> ();

	//	if (bullet != null && col.gameObject != lastCollision) {
	//		Debug.Log ("bullet hit character");
	//		bullet.isCalculating = true;

	//		lastCollision = col.gameObject;
	//		healthBar.gameObject.SetActive (true);


	//		float damage = bullet.rb.velocity.magnitude;

	//		damage *= 10f;
	//		Debug.Log (damage);

	//		health -= damage;
	//		if (health <= 0f) {
	//			health = 0f;
	//		}
	//		healthBar.SetSize (health / 100f);


	//		StartCoroutine (HideHealthBar ());
	//	}
	//}
	public void flipSprite()
	{

		//Vector2 scale = transform.localScale;

		isFlipped = !isFlipped;
		transform.Rotate(0f, 180f, 0f);
		//scale.x *= -1f;
		//transform.localScale = scale;

	}

	public IEnumerator chargeBullet(GameObject _bullet)
    {
		_bullet.GetComponent<Animator>().SetBool("Charging",true);

		yield return new WaitForSeconds(1);

		_bullet.GetComponent<Animator>().SetBool("Charging", false);
	}

	public void CollisionWithBullet(float _damage)
    {
		healthBar.gameObject.SetActive(true);
		displayingHealthBar = true;

		_damage *= 10f;
		Debug.Log(_damage);

		health -= _damage;
		if (health <= 0f)
		{
			health = 0f;
		}
		healthBar.SetSize(health / 100f);


		StartCoroutine(HideHealthBar());
	}

	IEnumerator HideHealthBar(){
		Debug.Log ("hiding bar");
		yield return new WaitForSeconds (2);
		healthBar.gameObject.SetActive (false);
		displayingHealthBar = false;

		if (health == 0f)
		{
			Debug.Log(this.gameObject.name + " died");
		}
		//GameObject.Find("GameManager").GetComponent<GameManager>().EndTurn();
	}

}
