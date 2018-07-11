using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A contestant in the Capture the Flag Game
public abstract class CTFPlayer : Movement{
    public CTFSim sim;
    // The way the player is playing
    public enum OSD
    {
        Offense,
        Support,
        Defense
    };

    // The flag the player is carrying
    public FlagScript flag;
    // The player's weapon
    public Weapon weap;
    // Whether the player is dead
    public bool dead = false;
    // The timer counting to when the player respawns
    public float respawnTimer = 0f;
    //
    // Whether the player has visited the enemy goal, used mostly for AI
    public Vector3 goalCheck = new Vector3();
    // The current play type
    public OSD playType = OSD.Support;

    public float respawnDelay = 5f;


    // Initialize the starting position
	public void SetupGPS() {
        home = transform.position;
	}

    // Drop the carried flag at the specified position
    public void DropFlag(Vector3 pos)
    {
        if (flag != null)
        {
            flag.Drop(pos);
            flag = null;
        }
    }

    // Wait until it is time to respawn
    public void BeDead()
    {
        respawnTimer += Time.deltaTime;
        if (respawnTimer >= respawnDelay)
        {
            Reset();
        }
    }

    // The player's forward vector
    public Vector3 Forward()
    {
        return body.transform.forward;
    }

    // Set the team the player is on and the team for the weapon to ignore
    public void SetTeam(int teem)
    {
        team = teem;
        weap.team = teem;
    }
}
