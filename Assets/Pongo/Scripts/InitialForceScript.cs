using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialForceScript : MonoBehaviour {

    public Vector3 InitialForce;
    public bool showBrokenBehavior = false;

    Rigidbody myRB;
    public float MinMagnitude = 0.1f; // Set this to 1 and have fun!

	// Use this for initialization
	void Start () {
        myRB = gameObject.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        if (myRB.velocity.x == 0f || myRB.velocity.z == 0f)
        {
            var f = Random.Range(-1f, 1f);
            while (Mathf.Abs(f) < 0.5f) f = Random.Range(-1f, 1f);
            myRB.AddForce(InitialForce * f);
        }
        while (myRB.velocity.magnitude < MinMagnitude)
        {
            myRB.velocity += new Vector3(myRB.velocity.x + (Mathf.Sign(myRB.velocity.x) * signOverride(myRB.velocity.magnitude) * 0.05f), 0, myRB.velocity.z + (Mathf.Sign(myRB.velocity.z) * signOverride(myRB.velocity.magnitude) * 0.05f));
        }
	}

    int signOverride(float val)
    {
        var result = 1;
        if (showBrokenBehavior == false && val == 0)
        {
            result = Random.Range(0, 2);
            if (result == 0) result = -1;

        }

        return result;
    }
}
