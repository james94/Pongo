using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PongoAgent : Agent
{
    public GameObject Paddle;
    public GameObject OpponentPaddle;
    public GameObject Goal;
    public int Score;
    public GameObject Ball;
    public TextMesh ScoreMesh;

    public float force = 1f;

    Vector3 StartingPosition;
    Rigidbody rb;
    Rigidbody ballRb;
    Rigidbody opponentRb;

    public float rewardPointScored = 1f;
    public float rewardPointLost = -1f;
    public float rewardBallHit = 0.01f;
    public float maxScore = 5;

    PongoAgent opponent;
    Ball gameBall;
    int score = 0;

    private void Awake()
    {

        StartingPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        ballRb = Ball.GetComponent<Rigidbody>();
        opponentRb = OpponentPaddle.GetComponent<Rigidbody>();
        opponent = OpponentPaddle.GetComponent<PongoAgent>();
        gameBall = Ball.GetComponent<Ball>();
    }

    public override void InitializeAgent()
    {
        score = 0;
        if (ScoreMesh != null) ScoreMesh.text = score.ToString();
    }

    // How to reinitialize when the game is reset. The Start() of an ML Agent
    public override void AgentReset()
    {
        transform.position = StartingPosition;
        rb.velocity = Vector3.zero;
        score = 0;
        if (ScoreMesh != null) ScoreMesh.text = score.ToString();
    }

    // Tell the ML algorithm everything you can about the current state
    public override void CollectObservations()
    {
        AddVectorObs(Ball.transform.position);
        AddVectorObs(ballRb.velocity);
        AddVectorObs(Paddle.transform.position);
        AddVectorObs(rb.velocity);
        AddVectorObs(Goal.transform.position - Ball.transform.position);
    }

    // What to do every step. The Update() of an ML Agent
    public override void AgentAction(float[] actions, string textAction)
    {
        // This example only uses continuous space
        if (brain.brainParameters.vectorActionSpaceType != SpaceType.discrete)
        {
            Debug.LogError("Must be discrete state type");
            return;
        }

        var action = (int)actions[0]; // The agent has only two possible actions: up, down (or nothing)
        
        if (action == 2) action = -1; // Unity ML Agents only does 0+
        if (action < 0) action = -1;
        if (action > 0) action = 1;

        rb.AddForce(new Vector3(0, 0, action * force));
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody && collision.rigidbody.tag.Equals("ball"))
        {
            HitBall();
        }
    }

    public void ScorePoint()
    {
        score++;
        if (score == maxScore)
        {
            opponent.Done();
            Done();
        }
        if (ScoreMesh != null) ScoreMesh.text = score.ToString();
        AddReward(rewardPointScored);
        opponent.LosePoint();
        gameBall.ResetBall();
    }

    public void LosePoint()
    {
        AddReward(rewardPointLost);
    }

    public void HitBall()
    {
        AddReward(rewardBallHit);
    }
}
