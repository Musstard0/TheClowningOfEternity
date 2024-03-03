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
    public float JumpFrequency = 3;
    GameObject player;
    public bool grounded;
    Rigidbody rb;
    public float jumpForce = 800;

    MeshDestroy destroyComponent;
    // Start is called before the first frame update
    void Start()
    {
        destroyComponent = GetComponent<MeshDestroy>();
        rb = GetComponent<Rigidbody>();
        hp = MaxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(Jump(JumpFrequency));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveToPlayer();
        animateRotation();
    }
    void MoveToPlayer()
    {

        //transform.position = Vector3.MoveTowards(transform.position, player.transform.position, EnemySpeed);

    }
    private void animateRotation()
    {

        // transform.Translate(0, 2, 0);
        //transform.Rotate(new Vector3(270, 0, 0));
    }
    IEnumerator Jump(float time)
    {        
        // Do the job until running is set to false
       // Debug.Log(grounded);
        while (true)
        {
            if (grounded)
            {
                CheckPlayer();
                transform.LookAt(player.transform);
                //rb.velocity = new Vector3(0,0,0) ;
                rb.AddRelativeForce(Vector3.forward * jumpForce);
            }
            yield return new WaitForSeconds(time);
        }
    }

    void CheckPlayer()
    {
        // Define the layer mask to only consider the player layer

        // Cast a ray from the character's position towards the player
        RaycastHit hit;
        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, Mathf.Infinity) && Vector3.Distance(transform.position,player.transform.position) >=25)
        {
            // If the player is not in view
            if (hit.collider.CompareTag("Player") == false)
            {
                // Disable the collider
                GetComponent<Collider>().enabled = false;

                // Enable the collider after a delay
                StartCoroutine(EnableColliderAfterDelay(.25f));
                // Make the character jump extremely high
                rb.AddForce(Vector3.up * 10, ForceMode.Impulse);

            }
        }
    }

    IEnumerator EnableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Enable the collider
        GetComponent<Collider>().enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Health>().ChangeHealth(-5);
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
            destroyComponent.Die();
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
