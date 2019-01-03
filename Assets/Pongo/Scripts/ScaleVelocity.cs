using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScaleVelocity : MonoBehaviour {
    public GameObject ball;
    public Vector3 CollisionScale = Vector3.one;
    public Vector3 RandomWiggle = Vector3.zero;
    public float ImpartZVelocityAmount = 0.25f;
    public UnityEvent OnBounce = new UnityEvent();

    Rigidbody myRigidBody;

    private void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null && collision.rigidbody.gameObject == ball)
        {
            var impart = 0f;

            if (myRigidBody != null)
            {
                impart = myRigidBody.velocity.z * ImpartZVelocityAmount;
            }

            if (OnBounce != null) OnBounce.Invoke();

            collision.rigidbody.velocity = Vector3.Scale(collision.rigidbody.velocity, CollisionScale) + (Random.Range(-1f, 1f) * RandomWiggle) + new Vector3(0, 0, impart);
        }

    }
}
