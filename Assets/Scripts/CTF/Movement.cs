using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Movement handles moving objects with Character Controllers
[RequireComponent(typeof(CharacterController))]
public abstract class Movement : Stats {

    // The jump velocity to use
    public float Jump = 1.5f;
    // The percentage of the previous velocity to use
    public float Damp = 1f;
    // The movement speed to use
    public float Speed = 10f;
    // The gravity on the object
    public float Gravity = 1.2f;
    // The rotation speed of the object
    public float RotateSpeed = 9f;

    // The body to rotate, as well as the initial rotation
    public GameObject body;
    public Quaternion rot;

    // The current move
    public Vector3 move;
    // The current velocity
    protected Vector3 velocity;
    // The character controller
    protected CharacterController cont;
    // The timer determining how long to move using velocity
    protected float vel = 0f;

    // Initialize the movement components
	public void SetupMov () {
        velocity = new Vector3();
        cont = GetComponent<CharacterController>();
        if (body != null)
        {
            rot = body.transform.rotation;
        }
        vel = CTFScript.delay;
	}
    
    // Hurt this object and move it in the given knockback direction
    public override void Damage(Vector3 dir)
    {
        health--;
        vel = .3f;
        velocity = dir * knockback;
        velocity.y = 0f;
        if (health == 0)
        {
            Die();
        }
    }

    // Move the object by the current move
    protected void Move()
    {
        // Damp the velocity
        velocity = velocity * Damp;

        // Handle movement input, or use the velocity depending on the timer
        if (vel <= 0f)
        {
            velocity.x = move.x;
            velocity.z = move.z;
            RotateBody();
        }
        else
        {
            vel -= Time.deltaTime;
        }

        // If the object is not on the ground, apply the gravity acceleration to the velocity
        if (!cont.isGrounded)
        {
            velocity.y -= Gravity;
        }

        // Move the object
        cont.Move(velocity * Time.deltaTime);
    }

    // Rotate the body in the direction of the current move
    protected void RotateBody()
    {
        if (body != null && move.magnitude > 0)
        {
            // Get the current rotation
            Vector3 angs = body.transform.rotation.eulerAngles;

            // Calculate the new rotation and determine which direction to turn
            float y = Mathf.Atan2(move.x, move.z) * 180 / Mathf.PI;
            float othery = -y + 2 * ((180 * -y / Mathf.Abs(y)) + y);
            if (othery - angs.y < y - angs.y)
            {
                y = othery;
            }
            
            // Rotate the body
            body.transform.rotation = Quaternion.Lerp(body.transform.rotation, Quaternion.Euler(angs.x, y, angs.z), Time.deltaTime * RotateSpeed);
            angs = body.transform.rotation.eulerAngles;
            body.transform.rotation = Quaternion.Euler(angs.x, -((-angs.y) % 360), angs.z);
        }
    }
}
