using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The Capture the Flag Game handles keeping score, the time, and resetting the game. It also stores information that needs to be available globally.
// This should only exist once per project
public class CTFScript : MonoBehaviour {
    // The scores for each team
    public static int[] scores = new int[4];
    // The delay between games
    public static float delay = 11f;
    // The game duration in minutes
    public static float timeMin = 2f;
    // 
    public static float timer = 0f;
    public static float respawnTime = 5f;

    // Variables for setting game duration and respawn time in editor
    public float gameTime = 2f;
    public float resTime = 5f;

    // Variables for setting flags and goals in editor
    public FlagScript[] objs = new FlagScript[0];
    public GoalScript[] goals = new GoalScript[0];

    // The UI component for displaying the game result
    public ResultScript resultText;
    public static ResultScript resText;

    // The global flags and goals
    public static FlagScript[] flags;
    public static GoalScript[] goalObjs;

    void Start () {
        // Assign the editor values to the static variables
        flags = objs;
        goalObjs = goals;
        resText = resultText;
        timeMin = gameTime;
        respawnTime = resTime;

        // Begin the game
        Begin();
	}
	
	void FixedUpdate () {
        timer += Time.deltaTime;

        // If the game is over, publish the result and reset the game
        if (timer / 60f >= timeMin) {
            timer = timeMin * 60f;
            if (scores[0] > scores[1])
            {
                resText.PublishResult("Blue Team Wins!");
            }
            else if (scores[1] > scores[0])
            {
                resText.PublishResult("Red Team Wins!");
            }
            else
            {
                resText.PublishResult("It's a Tie!");
            }
            Reset();
        }
        // If the game is halfway to starting, remove the result notification
        else if (timer > -delay / 2 && timer < 0)
        {
            resText.EndResult();
        }
	}
    
    // Increment the score of the given team
    public static void Score(int team)
    {
        scores[team]++;
    }

    // Get the score of the given team
    public static int GetScore(int team)
    {
        return scores[team];
    }

    // Get the current time in seconds
    public static float GetTime()
    {
        return timer;
    }

    // Get the time in a format that can be displayed on screen
    public static string CurrentTime()
    {
        string zero = "";
        float tme = Mathf.Abs(timer);

        // Display the time as a countdown
        if (timer >= 0)
        {
            tme = timeMin * 60f - tme;
        }
        // If there is a missing zero, add it
        if (tme % 60 < 10)
        {
            zero = "0";
        }
        // Format the time
        return Mathf.FloorToInt(tme / 60f) + ":" + zero + Mathf.FloorToInt(tme % 60);
        }

    // Reset the game
    public static void Reset()
    {
        foreach (FlagScript obj in flags)
        {
            obj.Reset();
        }
        foreach (GoalScript goal in goalObjs)
        {
            goal.Reset();
        }

        Begin();
    }

    // Start the game
    public static void Begin()
    {
        // Start the game countdown
        timer = -delay;

        // Reset the scores
        for (int ind = 0; ind < scores.Length; ind++)
        {
            scores[ind] = 0;
        }
    }
}
