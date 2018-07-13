using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AI is handles a computer player's actions with goals defined by the AIType
public class AI : CTFPlayer
{
    // How far to stay away from walls
    public float wallDist = 8f;
    // Whether or not the computer player is trying to pass an obstacle
    bool blocked = false;
    public float sightRadius = 50f;
    [Range(0f,180f)]
    public float sightAngle = 45f;

    public MeshRenderer meshBody;

    protected NeuralNet brain;
    protected List<double> directions;

    // Initialize fields from the parent classes
    public virtual void Init(int team, Material mat, CTFSim ctf)
    {
        SetupGPS();
        SetupMov();
        SetTeam(team);
        sim = ctf;
        meshBody.material = mat;
        brain = new NeuralNet(8, 3, 3, 3, .7);
        goalCheck = sim.goals[(team + 1) % 2].transform.position;
    }

    public virtual void InitBrain(int team, Material mat, CTFSim ctf, NeuralNet net)
    {
        SetupGPS();
        SetupMov();
        SetTeam(team);
        sim = ctf;
        meshBody.material = mat;
        brain = net;
        goalCheck = sim.goals[(team + 1) % 2].transform.position;
    }

    // Handle the AI's moves
    void FixedUpdate()
    {
        if (!sim.loaded)
            return;
        // If the player is dead, wait until respawn
        if (dead)
        {
            BeDead();
            vel -= Time.deltaTime;
        }
        // Determine the current goal and move
        else
        {
            Look();
            if (directions[1] > .5f)
            {
                JumpTry();
            }
            if (directions[2] > .5f)
            {
                weap.Attack();
            }
            move = GetMove() * Speed;
            Move();
        }
    }

    public virtual void Look()
    {
        List<double> ins = new List<double>();
        double enemyAngle = 0;
        double enemyDist = 2;
        double enemyHasFlag = 0;
        double haveFlag = 0;
        Vector3 ray = new Vector3();
        Collider[] hits = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (Collider hit in hits)
        {
            ray = hit.transform.position - transform.position;
            if (Vector3.Angle(body.transform.forward, ray) < sightAngle)
            {
                if (hit.CompareTag("Contestant"))
                {
                    CTFPlayer player = hit.gameObject.GetComponent<CTFPlayer>();
                    if (player.team != team && ((ray.magnitude / sightRadius < enemyDist && enemyHasFlag == 0) || player.flag))
                    {
                        AngDist(hit.transform.position, out enemyAngle, out enemyDist);
                        if (player.flag)
                        {
                            enemyHasFlag = 1;
                        }
                    }
                }
                else if (hit.CompareTag("Flag"))
                {
                    if (hit.gameObject.GetComponent<FlagScript>().team != team)
                    {
                        goalCheck = hit.transform.position;
                    }
                }
            }
        }

        if (flag)
        {
            haveFlag = 1;
        }

        ins.Add(enemyAngle);
        ins.Add(enemyDist);
        ins.Add(enemyHasFlag);
        AngDist(goalCheck, out enemyAngle, out enemyDist);
        ins.Add(enemyAngle);
        ins.Add(enemyDist);
        ins.Add(haveFlag);
        AngDist(sim.goals[team].transform.position, out enemyAngle, out enemyDist);
        ins.Add(enemyAngle);
        ins.Add(enemyDist);

        directions = brain.Run(ins);
    }

    protected void AngDist(Vector3 pos, out double ang, out double dist)
    {
        Vector3 ray = pos - transform.position;
        ang = (Vector3.SignedAngle(body.transform.forward, ray, Vector3.up) + 180f) / 360f;
        dist = ray.magnitude / (10 * sightRadius);
    }

    // Handle the player's death
    public override void Die()
    {
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
    public Vector3 GetDirection()
    {
        Vector3 dir = body.transform.forward * Mathf.Cos(2 * Mathf.PI * (float)directions[0] - Mathf.PI) + body.transform.right * Mathf.Sin(2 * Mathf.PI * (float)directions[0] - Mathf.PI);

        // Ignore height and small movements
        dir.y = 0f;
        dir.Normalize();

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
            vel = delay;
        }
        velocity = new Vector3();
        health = 3;
        transform.position = home;
        body.transform.rotation = rot;
        dead = false;
        goalCheck = sim.goals[(team + 1) % 2].transform.position;
    }
}