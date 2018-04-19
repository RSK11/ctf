using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Guard is an AIType that prioritizes protecting the flag. They circle the goal and attack enemies within the guard radius of the goal.
public class Guard : AIType {

    private static float GuardRadius = 70f;
    private static float PatrolRadius = 15f;

    public Guard(CTFPlayer gps, float tme)
    {
        playType = CTFPlayer.OSD.Defense;
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
                else if (coll.CompareTag("Contestant") && act != Action.Flag && !coll.GetComponent<CTFPlayer>().dead)
                {
                    // If the enemy is close to our goal
                    if ((CTFScript.goalObjs[me.team].transform.position - coll.transform.position).magnitude < GuardRadius)
                    {
                        // Target the enemy
                        target = coll.gameObject;
                        act = Action.Fight;
                        timer = 0f;
                    }
                    else
                    {
                        // Go back to guarding
                        act = Action.Search;
                        timer = 0f;
                        SearchPos();
                    }
                }
            }
        }

        // If the AI didn't see anything important, but sees its goal
        if (act == Action.Search && (targetPos - me.transform.position).magnitude < 5f)
        {
            SearchPos();
        }
    }

    override
    protected void SearchPos()
    {
        targetPos = new Vector3(Mathf.Cos(CTFScript.GetTime()), 0f, Mathf.Sin(CTFScript.GetTime())) * PatrolRadius;
        targetPos = CTFScript.goalObjs[me.team].transform.position + targetPos;
    }
}
