using Mycobot.csharp;
using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.UrdfImporter.Control;
using UnityEngine;

public class UnityMyCobot
{
    GameObject mycobot;

    private float stiffness;
    private float damping;
    private float forceLimit;
    private int speed;
    private float torque;
    private float acceleration;

    // Start is called before the first frame update
    public UnityMyCobot()
    {
        try
        {
            mycobot = GameObject.Find("myCobot");
            if (mycobot == null)
            {
                throw new System.Exception("GameObject 'myCobot' not found.");
            }

            Controller controller = mycobot.GetComponent<Controller>();

            this.stiffness = controller.stiffness;
            this.damping = controller.damping;
            this.forceLimit = controller.forceLimit;
            this.speed = (int)controller.speed;
            this.torque = controller.torque;
            this.acceleration = controller.acceleration;

            Debug.Log("GameObject 'myCobot' successfully found.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("An error occurred while finding 'myCobot': " + e.Message);
        }
    }
    public ArticulationBody[] updateUnityRobot(float[] angles, ArticulationBody[] joints)
    {
        for (int i = angles.Length - 1; i >= 0; i--)
        {
            joints[i].SetDriveTarget(ArticulationDriveAxis.X, -angles[i]);
        }

        return joints;
    }

    public ArticulationBody[] getJoints()
    {
        return mycobot.GetComponentsInChildren<ArticulationBody>();
    }

    public ArticulationBody[] resetPosition(ArticulationBody[] joints)
    {
        for (int i = 0; i <= joints.Length - 1; i++)
        {
            joints[i].SetDriveTarget(0, 0);
        }

        return joints;
    }

    public GameObject getMyCobot()
    {
        return mycobot;
    }
}
