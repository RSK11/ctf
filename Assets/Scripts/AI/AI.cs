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
        brain = new NeuralNet(8, 5, 4, 4, .7);
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
            if (directions[3] > .5f)
            {
                JumpTry();
            }
            if (directions[4] > .5f)
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
        double enemyDir = 0;
        double enemyDist = sightRadius;
        double enemyHasFlag = 0;
        double haveFlag = 0;
        Collider[] hits = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (Collider hit in hits)
        {
            Vector3 ray = hit.transform.position - transform.position;
            if (Vector3.Angle(body.transform.forward, ray) < sightAngle)
            {
                if (hit.CompareTag("Contestant"))
                {
                    CTFPlayer player = hit.gameObject.GetComponent<CTFPlayer>();
                    if (player.team != team && ((ray.magnitude < enemyDist && enemyHasFlag == 0) || player.flag))
                    {
                        enemyDir = Vector3.SignedAngle(body.transform.forward, ray, Vector3.up) / 180f;
                        enemyDist = ray.magnitude;
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

        ins.Add(enemyDir);
        ins.Add(enemyDist);
        ins.Add(enemyHasFlag);
        ins.Add(Vector3.SignedAngle(body.transform.forward, goalCheck - transform.position, Vector3.up) / 180f);
        ins.Add((goalCheck - transform.position).magnitude);
        ins.Add(haveFlag);
        ins.Add(Vector3.SignedAngle(body.transform.forward, sim.goals[team].transform.position - transform.position, Vector3.up) / 180f);
        ins.Add((sim.goals[team].transform.position - transform.position).magnitude);

        directions = brain.Run(ins);
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
    public virtual Vector3 GetDirection()
    {
        Vector3 dir = body.transform.forward * (float)directions[1] + body.transform.right * (float)directions[0];

        // Ignore height and small movements
        dir.y = 0f;
        dir = dir.normalized * Mathf.Abs((float)directions[2]);
        if (dir.magnitude < .5f)
        {
            dir = new Vector3();
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