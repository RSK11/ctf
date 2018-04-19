using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Direct is an AIType that prioritizes scoring. They immediately go to check the opponent's goal for the flag and will attack enemies who get within half the distance
public class Direct : AIType {
    public Direct(CTFPlayer gps, float tme)
    {
        playType = CTFPlayer.OSD.Offense;
        thinkTime = tme;
        me = gps;
        sightRad = 40f;
        me.goalCheck = false;
        timer = thinkTime;
        act = Action.Search;
    }

    public override void Reset()
    {
        me.goalCheck = false;
        timer = thinkTime;
        act = Action.Search;
        SearchPos();
    }

    protected override void Look(Vector3 forw)
    {
        // Sphere cast to find objects within the given radius
        Collider[] things = Physics.OverlapSphere(me.gameObject.transform.position, sightRad);

        foreach (Collider coll in things)
        {
            // If the object is in front of the AI and not an object from the same team
            Vector3 dist = coll.transform.position - me.gameObject.transform.position;
            if (Vector3.Dot(forw, dist.normalized) > .2f && coll.GetComponent<CTFObject>() != null && coll.GetComponent<CTFObject>().team != me.team)
            {
                if (coll.CompareTag("Flag"))
                {
                    // Target the flag
                    target = coll.gameObject;
                    act = Action.Flag;
                    timer = 0f;
                }
                else if (coll.CompareTag("Contestant") && act != Action.Flag && !coll.GetComponent<CTFPlayer>().dead && me.health > 1)
                {
                    // Target the enemy
                    target = coll.gameObject;
                    act = Action.Fight;
                    timer = 0f;
                }
                else if (coll.CompareTag("Goal") && dist.magnitude < sightRad - 20f)
                {
                    // Checked the goal with enough error to see the flag if it's there
                    me.goalCheck = true;
                }
            }
        }

        // If the AI didn't see anything important, but sees its goal
        if (act == Action.Search && (targetPos - me.transform.position).magnitude < 10f)
        {
            SearchPos();
        }
    }

    override
    protected void SearchPos()
    {
        // If the AI is carrying a flag
        if (me.flag != null)
        {
            targetPos = CTFScript.goalObjs[me.team].transform.position;
        }
        // If the AI hasn't checked the opponent's goal for the flag
        else if (!me.goalCheck)
        {
            targetPos = CTFScript.goalObjs[(me.team + 1) % CTFScript.goalObjs.Length].transform.position;
        }
        // Otherwise check a random spot on the map you can't see
        else
        {
            targetPos = me.transform.position;
            while ((targetPos - me.transform.position).magnitude < sightRad)
            {
                targetPos = new Vector3(Random.Range(20f, 300f), 0f, Random.Range(20f, 300f));
            }
        }
    }
}
