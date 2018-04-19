using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The goal is where a team scores, but also serves as the home base. It keeps track of the players and tells the computer players what play style to use.
public class GoalScript : CTFObject {

    // The players on this goal's team
    public CTFPlayer[] players = new CTFPlayer[0];
    // The human players on the team
    private List<CTFPlayer> humans = new List<CTFPlayer>();
    // The computer players on the team
    private List<CTFPlayer> cpus = new List<CTFPlayer>();

    void Start()
    {
        // Set each player's team and determine if they're human or not
        foreach (CTFPlayer gps in players)
        {
            gps.SetTeam(team);
            if (gps.GetComponent<AI>() != null)
            {
                cpus.Add(gps);
            }
            else
            {
                humans.Add(gps);
            }
        }
    }

    // Tell the Computer players what play style to use
    void FixedUpdate()
    {
        // The number of players using each play style
        int off = 0;
        int sup = 0;
        int def = 0;

        // Poll the human players for their positions
        foreach (CTFPlayer gps in humans)
        {
            if (gps.playType == CTFPlayer.OSD.Offense)
                off++;
            else if (gps.playType == CTFPlayer.OSD.Support)
                sup++;
            else
                def++;
        }

        // If not all the characters have positions
        if (off + sup + def < 3)
        {
            // Sort the computers by distance to this goal
            cpus.Sort(SortByDist);

            // Assign roles to the computers based on their distance to this goal
            foreach (CTFPlayer gps in cpus)
            {
                if (def < 1)
                {
                    gps.playType = CTFPlayer.OSD.Defense;
                    def++;
                }
                else if (sup < 1)
                {
                    gps.playType = CTFPlayer.OSD.Support;
                    sup++;
                }
                else
                {
                    gps.playType = CTFPlayer.OSD.Offense;
                }
            }
        }

    }

    // A comparator for sorting the computer players
    int SortByDist(CTFPlayer p1, CTFPlayer p2)
    {
        return (transform.position - p1.transform.position).magnitude.CompareTo((transform.position - p2.transform.position).magnitude);
    }

	// Check for an enemy flag entering the goal
	void OnTriggerEnter(Collider coll)
    {
        FlagScript flag = coll.GetComponent<FlagScript>();
        if (flag != null && flag.team != team)
        {
            // Increment the team's score
            flag.Score(team);

            // Reset the team's knowledge about the flag position
            foreach (CTFPlayer gps in players)
            {
                gps.goalCheck = false;
            }
        }
    }

    // Reset the players on the team
    override
    public void Reset()
    {
        foreach (CTFPlayer gps in players)
        {
            gps.dead = false;
            gps.Reset();
        }
    }
}
