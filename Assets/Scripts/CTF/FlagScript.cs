using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The flag for capturing in the game
public class FlagScript : CTFObject {

    public Map map;

    // The flag's position relative to the parent
    public Vector3 offset = new Vector3();
    // How far behind the parent to be when carried
    public float amnt = -2f;
    // How long the flag takes to rise from the ground
    public float riseTime = 3f;
    // The position to rise from
    public Vector3 dropoff = new Vector3();
    // Whether the flag is being carried
    private bool attached = false;
    // Whether the flag is rising from the ground
    private bool rising = false;
    // The timer for rising from the ground
    float timer = 0f;
    // The player carrying the flag
    public CTFPlayer par;

	void Start () {
        home = transform.position;
        timer = 0f;
        rising = true;
	}

	void Update () {
        // Perform the rising from the ground animation
        if (rising)
        {
            if (timer >= riseTime)
            {
                rising = false;
                transform.position = home;
            }
            else
            {
                transform.position = home + ((riseTime - timer) / riseTime) * dropoff;
                timer += Time.deltaTime;
            }
        }
        // If the flag is not being carried, check if gets picked up
        else if (!attached) {
            Collider[] hits = Physics.OverlapCapsule(transform.position, transform.position + Vector3.up * 12f, .25f);
            for (int ind = 0; ind < hits.Length && !attached; ind++)
            {
                CTFPlayer player = hits[ind].GetComponent<CTFPlayer>();
                if (player != null && player.flag == null && !player.dead)
                {
                    par = player;
                    player.flag = this;
                    attached = true;
                }
            }
        }
        // If the flag is being carried, position it accordingly
        else
        {
            transform.position = par.transform.position + offset + par.Forward() * amnt;
        }
	}

    // Move to the ground and clear the current parent
    public void Drop(Vector3 ground)
    {
        if (par != null)
        {
            par.flag = null;
            par = null;
        }
        transform.position = ground;
        attached = false;
    }

    // The flag has reached a goal, so increment the other team's score and reset the position
    public void Score(int teem)
    {
        map.sim.Score(teem);
        Reset();
    }

    // Reset the flag position and start the rising animation
    public override void Reset()
    {
        Drop(home);
        timer = 0f;
        rising = true;
    }
}
