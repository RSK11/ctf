using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The goal is where a team scores, but also serves as the home base. It keeps track of the players and tells the computer players what play style to use.
public class GoalScript : CTFObject {

	// Check for an enemy flag entering the goal
	void OnTriggerEnter(Collider coll)
    {
        FlagScript flag = coll.GetComponent<FlagScript>();
        if (flag != null)
        {
            // Increment the team's score
            if (flag.team != team)
            {
                flag.Score(team);
            }
            else if (flag.par != null)
            {
                flag.par.DropFlag(transform.position);
            }
        }
    }

    // Reset the players on the team
    override
    public void Reset()
    {
    }
}
