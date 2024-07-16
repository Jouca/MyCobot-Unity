using Mycobot.csharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Threading;
using System.Xml;
using Unity.Robotics.UrdfImporter.Control;
using UnityEngine;

public class MyCobotHandler
{
    public UnityMyCobot unityMyCobot;
    public PhysicalMyCobot physicalMyCobot;
    bool physical;
    bool both;
    public float cooldown = 0;
    private float[] angles = { 0, 0, 0, 0, 0, 0 };

    int joint_id = 1;
    ArticulationBody[] joints;

    public MyCobotHandler(bool physical, bool both = false, int speed = -1)
    {
        if (both)
        {
            if (speed < 0) throw new System.Exception("You need to select speed for Physical MyCobot.");
            physicalMyCobot = new PhysicalMyCobot(speed);
            unityMyCobot = new UnityMyCobot();
            joints = unityMyCobot.getJoints();
        } 
        else
        {
            if (physical)
            {
                if (speed < 0) throw new System.Exception("You need to select speed for Physical MyCobot.");
                physicalMyCobot = new PhysicalMyCobot(speed);
            }
            else
            {
                unityMyCobot = new UnityMyCobot();
                joints = unityMyCobot.getJoints();
            }
        }

        this.physical = physical;
        this.both = both;
    }

    /// <summary>
    /// Change the joint that is going to be controlled, using the right and left arrow keys.
    /// This method will change the angle of the joint that is going to be controlled.
    /// 
    /// Usage with Unity & Physical MyCobot
    /// <example>
    /// <code>
    /// MyCobotHandler mycobot_handler = new MyCobotHandler(true, both: true, speed: 80);
    /// mycobot_handler.changeJoint();
    /// </code>
    /// </example>
    /// </summary>
    public void changeJoint()
    {
        bool SelectionInput1 = Input.GetKeyDown("right");
        bool SelectionInput2 = Input.GetKeyDown("left");

        if (SelectionInput1)
        {
            this.joint_id = (this.joint_id + 1) % this.joints.Length;
        }
        else if (SelectionInput2)
        {
            if (this.joint_id == 0)
            {
                this.joint_id = (this.joints.Length - 1);
            }
            else
            {
                this.joint_id--;
            }
        }
    }

    /// <summary>
    /// Returns all joints to a default position.
    /// 
    /// Usage with Unity & Physical MyCobot
    /// <example>
    /// <code>
    /// MyCobotHandler mycobot_handler = new MyCobotHandler(true, both: true, speed: 80);
    /// mycobot_handler.resetPosition();
    /// </code>
    /// </example>
    /// </summary>
    public void resetPosition()
    {
        if (physical)
        {
            physicalMyCobot.resetPosition();
        }
        else
        {
            joints = unityMyCobot.resetPosition(joints);
        }
    }

    /// <summary>
    /// Update the robot's position.
    /// 
    /// Usage with Physical MyCobot only!
    /// <example>
    /// <code>
    /// MyCobotHandler mycobot_handler = new MyCobotHandler(true, both: true, speed: 80);
    /// mycobot_handler.updateRobot()
    /// </code>
    /// </example>
    /// </summary>
    public void updateRobot()
    {
        if (physical && both)
        {
            if (angles[0] == 0 && angles[1] == 0 && angles[2] == 0 && angles[3] == 0 && angles[4] == 0 && angles[5] == 0)
            {
                return;
            }
            joints = unityMyCobot.updateUnityRobot(angles, joints);
            return;
        }
        throw new System.Exception("You can't use this method with Unity MyCobot.");
    }

    /// <summary>
    /// Update the robot's position based on a GameObject.
    /// 
    /// Usage with Physical MyCobot only! (can however be used with Unity MyCobot for using IK of the physical robot)
    /// <example>
    /// <code>
    /// MyCobotHandler mycobot_handler = new MyCobotHandler(true, both: true, speed: 80);
    /// GameObject gm = GameObject.Find("Sphere");
    /// mycobot_handler.updateFollowGameObject(gm);
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="gm">
    /// The GameObject that the robot will follow.
    /// </param>
    public void updateFollowGameObject(GameObject gm)
    {
        if (physical)
        {
            physicalMyCobot.updateFollowGameObject(gm, fix_angle: 0);

            if (cooldown > 0)
            {
                cooldown -= Time.deltaTime;
            }
            else
            {
                angles = physicalMyCobot.getAngles();
                cooldown = 0.1f;
            }

            return;
        }
        throw new System.Exception("You can't use this method with Unity MyCobot.");
    }

    /// <summary>
    /// Update the robot's position based on an AR code (using gameObject for the robot to follow the cube broadcasting the AR code).
    /// 
    /// Usage with Physical MyCobot only! (can however be used with Unity MyCobot for using IK of the physical robot)
    /// </summary>
    /// <example>
    /// <code>
    /// MyCobotHandler mycobot_handler = new MyCobotHandler(true, both: true, speed: 80);
    /// GameObject gm = GameObject.Find("ArucoMarker");
    /// mycobot_handler.updateFollowAR(gm);
    /// </code>
    /// </example>
    /// <param name="gm">
    /// The GameObject that the robot will follow.
    /// </param>
    public void updateFollowAR(GameObject gm)
    {
        if (physical && both)
        {
            physicalMyCobot.updateFollowGameObject(gm, fix_angle: 90);
            return;
        }
        throw new System.Exception("You can't use this method with Unity MyCobot only.");
    }

    /// <summary>
    /// Controls for the robot. Supposed to be used in Unity's update method.
    /// 
    /// Usage with Unity & Physical MyCobot
    /// <example>
    /// <code>
    /// MyCobotHandler mycobot_handler = new MyCobotHandler(true, both: true, speed: 80);
    /// mycobot_handler.controls();
    /// </code>
    /// </example>
    /// </summary>
    public void controls()
    {
        // Reset position (R key)
        bool rKey = Input.GetKeyDown("r");
        if (rKey)
        {
            resetPosition();
        }

        // Activate/Desactivate suction pump (P key)
        bool pKey = Input.GetKeyDown("p");
        if (pKey)
        {
            if (physical)
            {
                physicalMyCobot.pumpHandler();
            }
        }
    }

    /// <summary>
    /// Get the MyCobot GameObject.
    /// 
    /// Usage with Unity only!
    /// <example>
    /// <code>
    /// MyCobotHandler mycobot_handler = new MyCobotHandler(true, both: true, speed: 80);
    /// GameObject gm = mycobot_handler.getMyCobot();
    /// </code>
    /// </example>
    /// </summary>
    /// <returns>
    /// The MyCobot GameObject.
    /// </returns>
    public GameObject getMyCobot()
    {
        if (physical)
        {
            return null;
        }
        else
        {
            return unityMyCobot.getMyCobot();
        }
    }
}
