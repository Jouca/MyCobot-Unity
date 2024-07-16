using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCobotMain : MonoBehaviour
{
    MyCobotHandler mycobot_handler;
    public GameObject object_follow;

    // Start is called before the first frame update
    void Start()
    {
        mycobot_handler = new MyCobotHandler(true, both: true, speed: 80);
    }

    // Update is called once per frame
    void Update()
    {
        // Insert your code for MyCobot here
        // Examples:

        /*mycobot_handler.changeJoint();
        mycobot_handler.controls();
        mycobot_handler.updateRobot();*/

        /*GameObject gameObject = GameObject.Find("Sphere");
        mycobot_handler.updateFollowGameObject(gameObject);
        mycobot_handler.updateRobot();*/

        mycobot_handler.updateFollowAR(object_follow);
        mycobot_handler.updateRobot();
    }
}
