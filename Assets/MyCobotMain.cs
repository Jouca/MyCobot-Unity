using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCobotMain : MonoBehaviour
{
    GameObject mycobot;
    JointController jointController;

    // Start is called before the first frame update
    void Start()
    {
        mycobot = GameObject.Find("firefighter");
        jointController = mycobot.GetComponentsInChildren<JointController>()[0];
    }

    // Update is called once per frame
    void Update()
    {
        // Insert your code for MyCobot here
        // Example:

        /*jointController.changeJoint();
        jointController.updateControlledPhysicalRobot();
        jointController.controls();*/
    }
}
