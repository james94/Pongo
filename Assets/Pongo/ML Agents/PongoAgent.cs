using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

/**
*   Updating PongoAgent.cs class to be compatible with ML Agents version 2.0.1
*   https://docs.unity3d.com/Packages/com.unity.ml-agents@2.0/api/Unity.MLAgents.html
*   Its easier to install MLAgents package with Unity 3D 2021.3.13f1's Package Manager
*   compared to Unity 2018
*/

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

    BrainParameters brainParameters;

    // Awake is when you are interacting with yourself and initializing yourself
    // It is important you are only do internal references to your game object and its components
    private void Awake()
    {

        StartingPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    // Theres still a race condition to who has started first, but everyone has been through the awake process
    // everyone awakes first in a random order, but you know that everyone has been awake and should be self initialized
    // Thus, in Awake() we cache our objects own rigid body while in start, we cache other objects rigid bodies.
    private void Start()
    {
        ballRb = Ball.GetComponent<Rigidbody>();
        opponentRb = OpponentPaddle.GetComponent<Rigidbody>();
        opponent = OpponentPaddle.GetComponent<PongoAgent>();
        gameBall = Ball.GetComponent<Ball>();
    }

    // InitializeAgent() has been removed according to Unity ML Agent's migration guide. Its renamed to Initialize()
    // Ref: https://github.com/Unity-Technologies/ml-agents/blob/release_18/docs/Migrating.md
    public override void Initialize()
    {
        score = 0;
        if (ScoreMesh != null) ScoreMesh.text = score.ToString();
    }

    // AgentReset() has been removed according to Unity ML Agent's migration guide. Its renamed to OnEpisodeBegin()
    // Ref: https://github.com/Unity-Technologies/ml-agents/blob/release_18/docs/Migrating.md
    // How to reinitialize when the game is reset. The Start() of an ML Agent
    public override void OnEpisodeBegin()
    {
        transform.position = StartingPosition;
        rb.velocity = Vector3.zero;
        score = 0;
        if (ScoreMesh != null) ScoreMesh.text = score.ToString();
    }

    // CollectObservations() has been removed according to Unity ML Agent's migration guide. Its renamed to CollectObservations(VectorSensor sensor)
    // Replace all calls to AddVectorObs() with sensor.AddObservation(...) or sensor.AddOneHotObservation(...). The sensor was passed to the method
    // Ref: https://github.com/Unity-Technologies/ml-agents/blob/release_18/docs/Migrating.md
    // Tell the ML algorithm everything you can about the current state
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(Ball.transform.position);
        sensor.AddObservation(ballRb.velocity);
        sensor.AddObservation(Paddle.transform.position);
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(Goal.transform.position - Ball.transform.position);
    }

    // AgentAction() has been removed according to Unity ML Agent's migration guide. Its renamed to OnActionReceived()
    // Ref: https://github.com/Unity-Technologies/ml-agents/blob/release_18/docs/Migrating.md
    // Referring to Unity ML Agent's docs, I see OnActionReceived only has one argument passed, no longer needs textAction
    // I see OnActionReceived(actions) argument type was changed from float[] to ActionBuffers
    // Ref: https://docs.unity3d.com/Packages/com.unity.ml-agents@2.0/api/Unity.MLAgents.Actuators.ActionBuffers.html
    // Ref: https://github.com/Unity-Technologies/ml-agents/blob/release_18/docs/Migrating.md
    // What to do every step. The Update() of an ML Agent
    public override void OnActionReceived(ActionBuffers actions)
    {

        // brainParameters.vectorActionSpaceType has been changed to brainParameters.ActionSpect.NumContinuousActions according to Unity ML Agent's migration guide
            // Ref: https://github.com/Unity-Technologies/ml-agents/blob/release_18/docs/Migrating.md
        // Update: This example only uses discrete space
        // NOTE: Cant find similar to brainParameters.vectorActionSpaceType and SpaceType.discrete in latest ML Agents API
        // if (brainParameters.ActionSpec.NumContinuousActions != brainParameters.ActionSpec.NumDiscreteActions)
        // {
        //     Debug.LogError("Must be discrete state type");
        //     return;
        // }

        // Earlier we would check if we have discrete action, if not, then log error. In our case, we can directly choose
        // discreteActions from ActionBuffers actions object now with latest ML Agents API. So, I dont think we need that
        // check above anymore
        var action = (int)actions.DiscreteActions[0]; // The agent has only two possible vectorAction: up, down (or nothing)
        
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
            // Done() was removed according to Unity ML Agent's migration guide. Its renamed to EndEpisode()
            // Ref: https://github.com/Unity-Technologies/ml-agents/blob/release_18/docs/Migrating.md
            opponent.EndEpisode();
            EndEpisode();
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
