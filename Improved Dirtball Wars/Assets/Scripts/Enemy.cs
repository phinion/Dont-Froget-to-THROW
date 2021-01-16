using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public GameManager gameManager;

	public List<GameObject> EnemyCharacters;
	public Character currentCharacter;
	public int currentCharacterInt = 0;

	public GameObject currentTargerPlayerObject = null;

	public bool facingLeft = true;

	public float tempAngle;
	public float tempPower;

	public float angleUB;
	public float angleLB;

	public Vector2 bulletLandingPosition;
	public bool hitPlayer = false;

	// Use this for initialization
	void Start () {
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void EnemyTurn()
    {
		Debug.Log("enemy shot");
		AimAI();
	}

	void AimAI()
    {
		if (currentTargerPlayerObject == null)
		{
			//select random target
			currentTargerPlayerObject = gameManager.player.PlayerCharacters[UnityEngine.Random.Range(0,gameManager.player.PlayerCharacters.Count-1)];
			//select random power
			tempPower = UnityEngine.Random.Range(0f, 100f);

			if(currentTargerPlayerObject.transform.position.x < currentCharacter.gameObject.transform.position.x)
            {
				facingLeft = true;
				angleLB =90f;
				angleUB =180f;
            }
            else
            {
				facingLeft = false;
				angleLB =0f;
				angleUB =90f;
			}

		}
		else
		{

			if (!hitPlayer)
			{
				//if too short power+= <=10 
				if (bulletLandingPosition.x < currentCharacter.transform.position.x)
				{
					tempPower += UnityEngine.Random.Range(0f, 10f);
					if (tempPower > 100) { tempPower = 100f; }

					if (facingLeft)
					{
						angleLB = tempAngle;
					}
					else
					{
						angleUB = tempAngle;
					}
				}
				else //if too far power -= <10
				{
					tempPower -= UnityEngine.Random.Range(0f, 10f);
					if (tempPower < 0) { tempPower = 0f; }

					if (facingLeft)
					{
						angleUB = tempAngle;
					}
					else
					{
						angleLB = tempAngle;
					}
				}

				//if too short power+= <=10 
				//if angle >150 then decrease
				//if angle <120 then increase

				//if too far power -= <10
				//if angle >60 then decrease
				//if angle <30 then increase
			}
		}

		//select random angle in ublb
		tempAngle = UnityEngine.Random.Range(angleLB, angleUB);

		//make shot
		shootBullet(tempAngle, tempPower);
		Debug.Log("enemy shot");

	}

	public void shootBullet(float _angle, float _power)
    {

		float deltaX = Mathf.Cos(_angle * Mathf.Deg2Rad);
		float deltaY = Mathf.Sin(_angle * Mathf.Deg2Rad);

		Vector2 direction = new Vector2(deltaX, deltaY);
		Quaternion rotation = Quaternion.AngleAxis(_angle, Vector3.forward);
		Vector3 bulletSpawnPosition = new Vector3(currentCharacter.transform.position.x + 2f * deltaX, currentCharacter.transform.position.y + 2f * deltaY, currentCharacter.transform.position.z);

		direction.Normalize();

		Vector2 bulletForce = direction * _power * 10f;

		GameObject knife = Instantiate(currentCharacter.bulletPrefab, bulletSpawnPosition, rotation);
		knife.GetComponent<Bullet>().shootingCharacter = this.gameObject;
		knife.GetComponent<Bullet>().enemyShot = true;

		//play animation

		knife.GetComponent<Rigidbody2D>().AddForce(bulletForce);

		//GameObject.Find("GameManager").GetComponent<GameManager>().EndTurn();
	}

	public void removeDeadCharactersInList()
	{
		for (int a = 0; a < EnemyCharacters.Count; a++)
		{
			if (EnemyCharacters[a].GetComponent<Character>().health == 0)
			{
				Destroy(EnemyCharacters[a]);
				EnemyCharacters.RemoveAt(a);
			}
		}
	}
}
