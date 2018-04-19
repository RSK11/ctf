using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Weapons are usable by players and only hurt the enemy team
public abstract class Weapon : MonoBehaviour {

    public abstract void Attack();
    public int team;
}
