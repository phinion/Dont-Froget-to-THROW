using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public Rigidbody2D rb;
    public GameObject shootingCharacter;

    public float timer = 5f;

    public float speed = 5f;

    public bool firstLand = false;
    public bool enemyShot = false;

    [SerializeField] private List<GameObject> blacklist;
    private bool hasStawped = false;


    // Use this for initialization
    void Start()
    {
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>().SetFocusObject(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(rb.velocity.magnitude);

        if(timer <= 0)
        {
            if (!hasStawped)
            {
                Stawp();
                hasStawped = true;
            }
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }

    // Collisions only calculate once
    void OnCollisionEnter2D(Collision2D col)
    {
        GameObject collisionObject = col.gameObject;

        if (!firstLand)
        {
            Debug.Log("bullet first land");
            firstLand = true;
            timer = 2.0f;

            if (enemyShot)
            {
                Enemy enemy = shootingCharacter.GetComponent<Enemy>();
                enemy.bulletLandingPosition = this.transform.position;
                if (col.gameObject.GetComponent<Player>())
                {
                    enemy.hitPlayer = true;
                }
                else
                {
                    enemy.hitPlayer = false;
                }
            }
        }

        if (collisionObject.GetComponent<Character>() != null && !blacklist.Contains(collisionObject))
        {
            Debug.Log("Bullet hit character");
            blacklist.Add(collisionObject);
            collisionObject.GetComponent<Character>().CollisionWithBullet(rb.velocity.magnitude);
        }

        if(collisionObject.layer == LayerMask.NameToLayer("outerWall"))
        {
            Debug.Log("bullet hit outer wall");
            timer = 0;
        }

    }

    // Bullet shots only counts once. It allows for ricochets to still work though
    void OnCollisionExit2D(Collision2D col)
    {
        GameObject collisionObject = col.gameObject;

        if (collisionObject.GetComponent<Character>() != null && blacklist.Contains(collisionObject))
        {
            Debug.Log("Bullet left character hitbox");
            blacklist.Remove(collisionObject);
        }

    }

    private void Stawp()
    {
        Destroy(this.gameObject);
        Debug.Log("Bullet now stawp");
        GameObject.Find("GameManager").GetComponent<GameManager>().EndTurn();
    }

}

    //void OnCollisionEnter2D(Collision2D col)
    //{

    //    GameObject collisionObject = col.gameObject;

    //    if (col.gameObject.GetComponent<Character>() || col.gameObject.layer == LayerMask.NameToLayer("groundLayer"))
    //    {

    //        if (!firstLand)
    //        {

    //            firstLand = true;

    //            if (enemyShot)
    //            {
    //                Enemy enemy = shootingCharacter.GetComponent<Enemy>();
    //                enemy.bulletLandingPosition = this.transform.position;
    //                if (col.gameObject.GetComponent<Character>())
    //                {
    //                    enemy.hitPlayer = true;
    //                }
    //                else
    //                {
    //                    enemy.hitPlayer = false;
    //                }
    //            }


    //            if (col.gameObject.GetComponent<Character>())
    //            {
    //                isCalculating = true;
    //                Destroy(this.gameObject, 2.0f);
    //            }

    //        }

    //        if (!inAir)
    //        {
    //            StartCoroutine(NextTurn());
    //        }

    //    }


    //}
