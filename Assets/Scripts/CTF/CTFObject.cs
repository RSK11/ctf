using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Objects related to the Capture the Flag Game
public abstract class CTFObject : MonoBehaviour {
    // The position to reset to
    public Vector3 home = new Vector3();
    // The team the object is associated with
    public int team = 0;
    // Allows the Game to reset the object
    public abstract void Reset();
}
