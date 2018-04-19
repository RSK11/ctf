using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The bat weapon for players to use
[RequireComponent(typeof(Rigidbody))]
public class Bat : Weapon {

    // The swing duration
    public float Duration = 1f;
    // The attack cooldown
    public float Cooldown = 1f;
    // Whether or not hitting is enabled
    private bool hit = false;

    // If timer < 0 swinging, if < cooldown disabled, else ready
    public float timer = 0f;

    // The original position of the bat used for the current procedural animation
    public Vector3 home;
    public Vector3 homeRot;

    // The goal positions for the bat to use in the procedural animation
    public Vector3 goal = new Vector3();
    public Vector3 goalRot = new Vector3();
    private Vector3 prev;

	void Start () {
        // Initialize the data
        timer = 0f;
        home = transform.localPosition;
        homeRot = transform.localRotation.eulerAngles;
	}
	
	void FixedUpdate () {
        // Determine if the bat should animate or stay at rest
        if (timer < Cooldown)
        {
            timer += Time.deltaTime;
            if (timer >= 0)
            {
                transform.localPosition = home;
                transform.localRotation = Quaternion.Euler(homeRot);
                hit = false;
            }
            else
            {
                Swing();
            }
        }
	}

    // Damage any enemy player hit
    void OnTriggerEnter(Collider coll)
    {
        if (timer < 0f)
        {
            CTFPlayer gps = coll.GetComponent<CTFPlayer>();
            if (gps != null && gps.team != team && hit)
            {
                gps.Damage((coll.ClosestPoint(transform.position) - transform.position).normalized);
                hit = false;
            }
        }
    }

    // Perform the bat swing animation
    private void Swing()
    {
        float temp = -timer;
        temp = Mathf.Abs(temp - (Duration / 2f)) / -(Duration / 2f) + 1;
        transform.localPosition = home + (goal - home) * temp;
        transform.localRotation = Quaternion.Slerp(Quaternion.Euler(homeRot), Quaternion.Euler(goalRot), temp);
    }

    // Attempt to swing the bat if the cooldown time has passed
    public override void Attack()
    {
        if (timer >= Cooldown)
        {
            timer = -Duration;
            hit = true;
        }
    }
}
