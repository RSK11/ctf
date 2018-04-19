using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AI is handles a computer player's actions with goals defined by the AIType
public class AI : CTFPlayer
{
    // How far to stay away from walls
    public float wallDist = 8f;
    // The AIType defines the computer player's goal
    public AIType aType;
    // Whether or not the computer player is trying to pass an obstacle
    bool blocked = false;

    // Initialize fields from the parent classes
    void Start()
    {
        SetupGPS();
        SetupMov();
        // Begin with the default AIType
        aType = new Direct(this, 3f);
    }

    // Handle the AI's moves
    void FixedUpdate()
    {
        // If the Team suggests a different playtype, change playtypes
        if (playType != aType.playType)
        {
            if (playType != OSD.Defense)
            {
                aType = new Direct(this, 3f);
            }
            else
            {
                aType = new Guard(this, 3f);
            }
        }

        // If the player is dead, wait until respawn
        if (dead)
        {
            BeDead();
            vel -= Time.deltaTime;
        }
        // Determine the current goal and move
        else
        {
            aType.Think(Time.deltaTime, body.transform.forward);
            move = GetMove() * Speed;
            Move();
        }
    }

    // Handle the player's death
    public override void Die()
    {
        // Reset the computer's knowledge
        aType.Reset();
        // If the computer has a flag, drop it
        DropFlag(transform.position + -body.transform.forward * 2);
        // Put the computer in a 'dead' position
        transform.position += -Vector3.up * 1.5f;
        body.transform.Rotate(0f, 0f, 90f);

        dead = true;
        respawnTimer = 0f;
    }

    // Determine the computer's move
    public Vector3 GetMove()
    {
        // Get the direction of the current goal
        Vector3 dir = GetDirection();

        // Perform a CapsuleCast to check for obstacles and enemies
        RaycastHit hit = new RaycastHit();
        Vector3 p1 = transform.position + (cont.radius + cont.stepOffset) * Vector3.up;
        if (Physics.CapsuleCast(p1,p1 + (cont.height - cont.stepOffset) * Vector3.up,cont.radius,dir,out hit, wallDist))
        {
            // Check for blocking objects
            if (hit.collider.GetComponent<CTFObject>() == null)
            {
                if (!hit.collider.CompareTag("Ignore"))
                {
                    dir = Block(dir, hit);
                }
                else
                {
                    blocked = false;
                }
            }
            // Check for enemies
            else if (hit.collider.CompareTag("Contestant"))
            {
                // Ignore teammates
                if (hit.collider.GetComponent<CTFPlayer>().team == team)
                {
                    dir = Block(dir, hit);
                }
                // Jump dead bodies
                else if (hit.collider.GetComponent<CTFPlayer>().dead)
                {
                    velocity.y = Jump;
                }
                // Attack the enemy, but don't crash into them
                else
                {
                    if ((hit.collider.transform.position - transform.position).magnitude < wallDist / 2)
                    {
                        dir = new Vector3();
                    }
                    weap.Attack();
                }
            }
            else
            {
                blocked = false;
            }
        }
        else
        {
            blocked = false;
        }

        return dir;
    }

    // Returns the direction of the current goal
    private Vector3 GetDirection()
    {
        Vector3 dir = new Vector3();

        // Get the direction
        if (aType.act == AIType.Action.Search)
        {
            dir = aType.targetPos - transform.position;
        }
        else
        {
            dir = aType.target.transform.position - transform.position;
        }

        // Ignore height and small movements
        dir.y = 0f;
        if (dir.magnitude < 1f)
        {
            dir = new Vector3();
        }
        else
        {
            dir.Normalize();
        }

        return dir;
    }

    // Change the direction to perpendicular to the blocking object and within 90 degrees of the previous direction
    private Vector3 Recalc(Vector3 dir, RaycastHit hit)
    {
        Vector3 right = new Vector3(hit.normal.z, 0f, -hit.normal.x);
        if (Vector3.Dot(dir, right) >= 0)
        {
            return right;
        }
        return -right;
    }

    // The computer is blocked by an obstacle
    private Vector3 Block(Vector3 dir, RaycastHit hit)
    {
        if (!blocked)
        {
            blocked = true;
            // Recalculate the current direction
            return Recalc(dir, hit);
        }
        return move / Speed;
    }

    // Return the computer to its starting position and status
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
    }
}