using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Replay
{
    public List<double> states;
    public double reward;

    public Replay(double xr, double zr, double ballz, double ballvx, double ballvz, double r)
    {
        states = new List<double>();
        states.Add(xr);
        states.Add(zr);
        states.Add(ballz);
        states.Add(ballvx);
        states.Add(ballvz);
        reward = r;
    }
}

public class Brain : MonoBehaviour
{
    public GameObject ball;                             //object to monitor

    ANN ann;                                            //the neural network

    float reward = 0;                                   //reward to associate with actions
    List<Replay> replayMemory = new List<Replay>();     //memory - list of past actions and rewards
    int mCapacity = 10000;                              //memory capacity

    float discount = 0.99f;                             //how much future states affect rewards
    float exploreRate = 100f;                           //chance of picking random action
    float maxExploreRate = 100f;                        //max chance value
    float minExploreRate = 0.01f;                       //min chance value
    float exploreDecay = 0.001f;                        //chance decay amount for each update

    Vector3 ballStartPos;                               //ball starting position
    int failCount = 0;                                  //count when the ball is dropped
    float tiltSpeed = 0.5f;                             //max angle to apply to tilting wach update
                                                        //make sure this is large enough so that q value
                                                        //multiplied by it, is large enough to recover balance
                                                        //when ball gets a good speed up

    float timer = 0;                                    //timer to keep track of balancing
    float maxBalanceTime = 0;                           //record time the ball is balanced

    // Start is called before the first frame update
    void Start()
    {
        ann = new ANN(5, 4, 1, 10, 0.2f);
        ballStartPos = ball.transform.position;
        Time.timeScale = 30f;
    }

    GUIStyle gStyle = new GUIStyle();
    private void OnGUI()
    {
        gStyle.fontSize = 25;
        gStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 600, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", gStyle);
        GUI.Label(new Rect(10, 25, 500, 30), "Fails: " + failCount, gStyle);
        GUI.Label(new Rect(10, 50, 500, 30), "Explore Rate: " + exploreRate, gStyle);
        GUI.Label(new Rect(10, 75, 500, 30), "Last Best Balance: " + maxBalanceTime, gStyle);
        GUI.Label(new Rect(10, 100, 500, 30), "This Balance: " + timer, gStyle);
        GUI.EndGroup();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            ResetBall();
        }
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        List<double> states = new List<double>();
        List<double> qs = new List<double>();

        states.Add(this.transform.rotation.x);
        states.Add(this.transform.rotation.z);
        states.Add(this.transform.position.z);
        states.Add(ball.GetComponent<Rigidbody>().angularVelocity.x);
        states.Add(ball.GetComponent<Rigidbody>().angularVelocity.z);

        qs = ANN.SoftMax(ann.CalculateOutput(states));
        double maxQ = qs.Max();
        int maxQIndex = qs.ToList().IndexOf(maxQ);
        exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);

        //check to see if we choose a random action
        if (UnityEngine.Random.Range(1, 100) < exploreRate)
        {
            maxQIndex = UnityEngine.Random.Range(0, 4);
        }

        //action 0 tilt right
        //action 1 tilt left
        //action 2 tilt forward
        //action 3 tilt backward
        //mapQIndex == 0 means action 0
        if (maxQIndex == 0)
        {
            this.transform.Rotate(Vector3.right, tiltSpeed * (float)qs[maxQIndex]);
        }
        else if (maxQIndex == 1)
        {
            this.transform.Rotate(Vector3.right, -tiltSpeed * (float)qs[maxQIndex]);
        }
        else if (maxQIndex == 2)
        {
            this.transform.Rotate(Vector3.forward, tiltSpeed * (float)qs[maxQIndex]);
        }
        else if (maxQIndex == 3)
        {
            this.transform.Rotate(Vector3.forward, -tiltSpeed * (float)qs[maxQIndex]);
        }

        if (ball.GetComponent<BallState>().dropped)
        {
            reward = -1f;
        }
        else
        {
            reward = 0.1f;
        }

        Replay lastMemory = new Replay(this.transform.rotation.x,
                                       this.transform.rotation.z,
                                       ball.transform.position.z,
                                       ball.GetComponent<Rigidbody>().angularVelocity.x,
                                       ball.GetComponent<Rigidbody>().angularVelocity.z,
                                       reward);

        if (replayMemory.Count > mCapacity)
        {
            replayMemory.RemoveAt(0);
        }

        replayMemory.Add(lastMemory);

        //Q learning starts here
        //upto this point all we did is get an inputs and getting the result from ann,
        //rewarding accordingly and then storing them.
        if (ball.GetComponent<BallState>().dropped)
        {
            //looping backwards so the quality of the last memory get carried
            //backwards up through the list so we can attributed it's blame through
            //the list
            for (int i = replayMemory.Count - 1; i >= 0; --i)
            {
                //foreach memory we ran the ann
                //first we found out what are the q values of the current memory
                List<double> currentMemoryQValues = new List<double>();
                //then we take the q values of the next memory
                List<double> nextMemoryQValues = new List<double>();
                currentMemoryQValues = ANN.SoftMax(ann.CalculateOutput(replayMemory[i].states));

                //find the maximum Q value of the current memories
                double maxQOld = currentMemoryQValues.Max();
                //which action gave that q value
                int action = currentMemoryQValues.ToList().IndexOf(maxQOld);

                double feedback;
                //checking if the current memory is the last memeory
                //or if that memory reward is -1, if it is -1, it means, that ball was dropped
                //and every memory after this is meaningless, because this is the end of the
                //memories sequance
                if ((i == replayMemory.Count - 1) || (replayMemory[i].reward == -1f))
                {
                    feedback = replayMemory[i].reward;
                }
                else
                {
                    nextMemoryQValues = ANN.SoftMax(ann.CalculateOutput(replayMemory[i + 1].states));
                    maxQ = nextMemoryQValues.Max();
                    feedback = (replayMemory[i].reward + discount * maxQ);
                }

                //adding the correct reward (Q value) to the current action
                currentMemoryQValues[action] = feedback;
                //using the feedback to train the ANN
                ann.Train(replayMemory[i].states, currentMemoryQValues);
            }

            if (timer > maxBalanceTime)
            {
                maxBalanceTime = timer;
            }

            timer = 0;

            ball.GetComponent<BallState>().dropped = false;
            this.transform.rotation = Quaternion.identity;
            ResetBall();
            replayMemory.Clear();
            failCount++;
        }
    }

    private void ResetBall()
    {
        ball.transform.position = ballStartPos;
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
