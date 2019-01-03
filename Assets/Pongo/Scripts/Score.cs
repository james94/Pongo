using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {
    public PongoAgent PlayerAgent;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null && collision.rigidbody.tag == "ball") PlayerAgent.ScorePoint();
    }
}
