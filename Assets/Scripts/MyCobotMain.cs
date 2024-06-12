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
        FindMyCobot();
        RetrieveJointController();
        
    }

    // Update is called once per frame
    void Update()
    {
        // Insert your code for MyCobot here
        // Example:

        jointController.changeJoint();
        jointController.updateControlledPhysicalRobot();
        jointController.controls();
    }
    
    private void FindMyCobot()
    {
        try
        {
            mycobot = GameObject.Find("myCobot");
            if (mycobot == null)
            {
                throw new System.Exception("GameObject 'myCobot' not found.");
            }
            Debug.Log("GameObject 'myCobot' successfully found.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("An error occurred while finding 'myCobot': " + e.Message);
        }
    }

    private void RetrieveJointController()
    {
        try
        {
            if (mycobot != null)
            {
                jointController = mycobot.GetComponentsInChildren<JointController>()[0];
                Debug.Log("JointController successfully retrieved.");
            }
        }
        catch (System.IndexOutOfRangeException e)
        {
            Debug.LogError("No JointController found in children of mycobot: " + e.Message);
        }
        catch (System.Exception e)
        {
            Debug.LogError("An error occurred while retrieving JointController: " + e.Message);
        }
    }
}
