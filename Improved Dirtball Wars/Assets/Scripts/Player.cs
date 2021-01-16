using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{

    public enum CharacterState
    {
        Aim,
        Move,
        Idle

    }
    public CharacterState currentState = CharacterState.Idle;

    private GameManager gameManager;

    public List<GameObject> PlayerCharacters;
    public Character currentCharacter;
    public int currentCharacterInt = 0;

    public Slider power;
    public GameObject arrow;

    public Collision2D lastCollision;

    //public bool isFlipped = false;
    private bool canShoot = false;
    public bool isPlayerTurn = false;

    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        arrow.SetActive(false);
        //healthBar.gameObject.SetActive (false);
    }

    // Update is called once per frame
    void Update()
    {

        if (isPlayerTurn)
        {
            checkMouseDown();
            checkCharacterEnergy();

            switch (currentState)
            {
                case CharacterState.Aim:
                    aimArrow();
                    break;
                case CharacterState.Move:
                    move();
                    break;
                case CharacterState.Idle:
                    break;
            }
        }
    }

    private void checkMouseDown()
    {

        //if (Input.GetMouseButtonDown(0) && currentState == CharacterState.Move)
        //{
        //	Debug.Log("Pressed primary button.");
        //	changeState(0);

        //} else 
        if (Input.GetMouseButtonDown(1) && currentState == CharacterState.Aim)
        {
            Debug.Log("Pressed secondary button.");
            changeState(1);
        }
    }

    private void checkCharacterEnergy()
    {
        if(currentCharacter.currentEnergy <= 0)
        {
            gameManager.EndTurn();
            currentCharacter.currentEnergy = currentCharacter.maxEnergy;
        }
    }

    public void changeState(int _state)
    {

        //if (isPlayerTurn) {
        //arrow.SetActive(false);
        arrow.SetActive(false);

        currentState = (CharacterState)_state;
        Debug.Log("current state = " + currentState);

        if(currentState == CharacterState.Aim)
        {
            arrow.SetActive(true);
        }

        if (currentState == CharacterState.Move)
        {
            power.value = currentCharacter.currentEnergy;
        }
        //}
    }
    void aimArrow()
    {

        isMouseOverUI();

        // Calculate what angle and direction the arrow should be facing
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - currentCharacter.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        arrow.transform.rotation = Quaternion.Slerp(arrow.transform.rotation, rotation, 5f * Time.deltaTime);

        //Debug.Log(angle);

        // Calculate how far the arrow can be positioned based on the player's current max power
        float distance = direction.magnitude;
        if (distance < 2f)
        {
            power.value = 0f;
            distance = 2f;
        }
        else if ((distance - 2f) * 20f > currentCharacter.currentEnergy)
        {
            power.value = currentCharacter.currentEnergy;
            distance = (currentCharacter.currentEnergy / 20f) + 2f;
        }
        else
        {
            power.value = (distance - 2f) * 20f;
        }

        // Calculate the arrows new position on screen using rotational values
        float deltaX = distance * Mathf.Cos(angle * Mathf.Deg2Rad);
        float deltaY = distance * Mathf.Sin(angle * Mathf.Deg2Rad);
        Vector3 newPosition = new Vector3(currentCharacter.transform.position.x + deltaX, currentCharacter.transform.position.y + deltaY, currentCharacter.transform.position.z);
        arrow.transform.position = Vector2.Lerp(arrow.transform.position, newPosition, 5f);

        // Flips the character sprite to the direction the arrow is facing
        if ((arrow.transform.position.x < currentCharacter.transform.position.x && !currentCharacter.isFlipped) || (arrow.transform.position.x > currentCharacter.transform.position.x && currentCharacter.isFlipped))
        {
            currentCharacter.flipSprite();
        }

        // THIS IS WHERE THE SHOOTING IS CALCULATED

        // Press left mouse button to shoot bullet
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            Debug.Log("mouse Down");

            deltaX = 2f * Mathf.Cos(angle * Mathf.Deg2Rad);
            deltaY = 2f * Mathf.Sin(angle * Mathf.Deg2Rad);
            direction.Normalize();

            Vector3 bulletSpawnPosition = new Vector3(currentCharacter.transform.position.x + deltaX, currentCharacter.transform.position.y + deltaY, currentCharacter.transform.position.z);
            Vector2 bulletForce = direction * power.value * 10f;
            //Debug.Log(bulletForce.magnitude);
            shootBullet(bulletSpawnPosition, arrow.transform.rotation, bulletForce);
        }

        // Reset max power
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("mouse2 Down");
            power.value = currentCharacter.currentEnergy;
        }

    }

    //player can't shoot if mouse is over ui
    private bool isMouseOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            canShoot = false;
        }
        else
        {
            canShoot = true;
        }

        return EventSystem.current.IsPointerOverGameObject();
    }

    public void shootBullet(Vector3 _position, Quaternion __rotation, Vector2 _force)
    {
        GameObject knife = Instantiate(currentCharacter.bulletPrefab, _position, __rotation);
        knife.GetComponent<Bullet>().shootingCharacter = this.gameObject;

        //play animation
        StartCoroutine(currentCharacter.chargeBullet(knife));

        knife.GetComponent<Rigidbody2D>().AddForce(_force);

        changeState(2);
        isPlayerTurn = false;
        //GameObject.Find ("GameManager").GetComponent<GameManager> ().EndTurn ();
    }

    void move()
    {

        if ((Input.GetAxisRaw("Horizontal") < 0 && !currentCharacter.isFlipped) || (Input.GetAxisRaw("Horizontal") > 0 && currentCharacter.isFlipped))
        {
            currentCharacter.flipSprite();
        }


        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            Debug.Log("moving");
            moveSprite();
            currentCharacter.animator.SetBool("isMoving", true);
        } else
        {
            currentCharacter.animator.SetBool("isMoving", false);
        }

        //Jump
        if (currentCharacter.isGrounded && Input.GetKeyDown(KeyCode.W))
        {
            if (currentCharacter.currentEnergy >= currentCharacter.jumpEnergyDrain)
            {
                currentCharacter.Jump();
                currentCharacter.currentEnergy -= currentCharacter.jumpEnergyDrain;
                power.value = currentCharacter.currentEnergy;
            }
        }
    }

    void moveSprite()
    {

        //Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"),0,0);
        //movement *= currentCharacter.moveSpeed * Time.deltaTime;

        float xInput = Input.GetAxisRaw("Horizontal");

        float moveAmount = xInput * currentCharacter.moveSpeed * Time.deltaTime;

        if (moveAmount < 0)
        {
            moveAmount *= -1f;
        }

        // Horizontal movement
        if (currentCharacter.currentEnergy >= moveAmount)
        {
            currentCharacter.currentEnergy -= currentCharacter.moveSpeed / currentCharacter.weight;
            power.value = currentCharacter.currentEnergy;

            //currentCharacter.transform.position = currentCharacter.transform.position + movement;
            //currentCharacter.rb.AddForce(movement * currentCharacter.moveSpeed * Time.deltaTime);

            Debug.Log("movinggg");
            //Vector2 newPos = new Vector2(currentCharacter.transform.position.x + movement,currentCharacter.transform.position.y + currentCharacter.rb.velocity.y * Time.deltaTime);
            //currentCharacter.rb.MovePosition(newPos);

            currentCharacter.rb.velocity = new Vector2(currentCharacter.moveSpeed * xInput, currentCharacter.rb.velocity.y);
            
        }

    }

    public void removeDeadCharactersInList()
    {
        for (int a = 0; a < PlayerCharacters.Count; a++)
        {
            if (PlayerCharacters[a].GetComponent<Character>().health == 0)
            {
                Destroy(PlayerCharacters[a]);
                PlayerCharacters.RemoveAt(a);
            }
        }
    }

}