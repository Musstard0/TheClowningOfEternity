using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyController : MonoBehaviour
{
    public float MaxHealth = 3f;
    private float hp;
    public float EnemySpeed = 0.05f;
    GameObject player;
    public bool grounded;
    Rigidbody rb;
    public float jumpForce = 800;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        hp = MaxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(Jump(0.3f));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveToPlayer();
        animateRotation();
    }
    void MoveToPlayer()
    {
        ;
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, EnemySpeed);
        

    }
    private void animateRotation()
    {

        // transform.Translate(0, 2, 0);
        transform.LookAt(player.transform);
        transform.Rotate(new Vector3(270, 0, 0));
    }
    IEnumerator Jump(float time)
    {

        // Set the function as running

        // Do the job until running is set to false
       // Debug.Log(grounded);
        while (true)
        {
            if (grounded)
            {
                //rb.velocity = new Vector3(0,0,0) ;
                rb.AddRelativeForce(Vector3.forward * jumpForce);
            }
            yield return new WaitForSeconds(time);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("BOOOOOOOOOM");
        }
        if(collision.gameObject.tag == "Bullet")
        {
            Damaged();
        }
    }
    public void Damaged()
    {
        hp -= 1;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }

    }
    void Jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddRelativeForce(Vector3.up * jumpForce);
        }
    }
    void OnCollisionStay(Collision other)
    {
        foreach (ContactPoint contact in other.contacts)
        {
            if (Vector2.Angle(contact.normal, Vector3.up) < 60)
            {
                grounded = true;
            }
        }
    }
    void OnCollisionExit()
    {
        grounded = false;
    }
}
