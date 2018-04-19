using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIType {

    // The action the computer is performing
    public enum Action
    {
        Flag,
        Fight,
        Search
    };

    // Check the computer's surroundings
    protected abstract void Look(Vector3 forw);
    // Calculate a goal position
    protected abstract void SearchPos();
    // Reset the computer's knowledge about the flag
    public abstract void Reset();

    // The playtype: offense, support, or defense
    public CTFPlayer.OSD playType;
    // A reference to all the flag and location information
    protected CTFPlayer me;
    // The current target
    public GameObject target;
    // The current goal position
    public Vector3 targetPos;
    // The current action
    public Action act;
    // How long to wait in seconds between thinking of a new goal
    public float thinkTime;
    // The thinking timer
    public float timer = 0f;
    // The maximum distance the computer can detect objects at
    public float sightRad = 20f;

    // Determine the current goal and action
    public void Think(float tme, Vector3 forw)
    {
        if (me.flag == null)
        {
            // Detect nearby objects of importance
            Look(forw);

            timer += tme;

            // If it's thinking time or the target is dead
            if (timer >= thinkTime || (act == Action.Fight && target.GetComponent<CTFPlayer>().dead))
            {
                // If the computer no longer sees any targets, check the last know target position
                if (act != Action.Search)
                {
                    targetPos = target.transform.position;
                    act = Action.Search;
                }
                else
                {
                    // Calculate a new goal position
                    SearchPos();
                }
                timer = 0f;
            }
        }
        else
        {
            // The player has the flag, set the goal as the player's goal
            act = Action.Search;
            SearchPos();
            // If you're standing in your goal, carrying a flag
            if ((targetPos - me.transform.position).magnitude < 3f)
            {
                me.DropFlag(me.transform.position - 2 * forw);
                SearchPos();
            }
        }
    }
}
