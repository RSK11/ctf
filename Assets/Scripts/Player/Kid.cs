using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The human player. Slightly faster than computers and can double jump
public class Kid : CTFPlayer {

    // How long to use velocity when double jumping
    public float doubleJumpVel = .5f;
    // Whether or not the user has let go of the jump button
    public bool up = true;

    // The camera attached to the player
    public ThrdCam cam;

    // Initialize all of the parent components and get the camera
    void Start () {
        SetupGPS();
        SetupMov();
        cam = GetComponentInChildren<ThrdCam>();
    }

    void FixedUpdate()
    {
        // Wait for a respawn if dead
        if (dead)
        {
            BeDead();
            vel -= Time.deltaTime;
        }
        // Handle the x and z input
        else
        {
            move = Vector3.Normalize(cam.Forward() * Input.GetAxis("Vertical") + cam.Right() * Input.GetAxis("Horizontal")) * Speed;
            Move();
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Double Jump off of other game objects
        if (up && Mathf.Abs(hit.normal.y) < .3f && !cont.isGrounded && Input.GetKey(KeyCode.Space)) 
        {
            velocity.y = 0f;
            float mag = velocity.magnitude * Vector3.Dot(-Vector3.Normalize(velocity),hit.normal);
            velocity.x = mag  * hit.normal.x;
            velocity.z = mag * hit.normal.z;
            velocity.y = Jump;
            vel = doubleJumpVel;
            up = false;
        }
    }

    void Update()
    {
        // Handle input
        if (!dead)
        {
            // Attack
            if (Input.GetAxis("Fire1") > .7f)
            {
                weap.Attack();
            }
            // Drop the Flag
            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
            {
                DropFlag(transform.position + -2 * cam.Forward());
            }
            // Jump
            if (cont.isGrounded && Input.GetKey(KeyCode.Space) && up)
            {
                velocity.y = Jump;
                up = false;
            }
            // Jump release
            else if (!up && Input.GetKeyUp(KeyCode.Space))
            {
                up = true;
            }
        }
    }

    // Handle the player running out of health
    public override void Die()
    {
        DropFlag(transform.position + -2 * cam.Forward());
        transform.position += -Vector3.up;
        body.transform.Rotate(0f, 0f, 90f);
        dead = true;
        respawnTimer = 0f;
    }

    // Reset the player's components and return to the start position
    override
    public void Reset()
    {
        if (!dead)
        {
            vel = CTFScript.delay;
        }
        velocity = new Vector3();
        health = 3;
        transform.position = home;
        body.transform.rotation = rot;
        dead = false;
        cam.Reset();
    }
}
