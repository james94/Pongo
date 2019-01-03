using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {
    Rigidbody myRB;

    Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.position;
    }

    // Use this for initialization
    void Start () {
        myRB = gameObject.GetComponent<Rigidbody>();
    }

    public void ResetBall()
    {
        transform.position = startPosition;
        myRB.velocity = Vector3.zero;
        myRB.rotation = Quaternion.identity;
    }
}
