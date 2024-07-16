using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Mycobot.csharp;

public class PhysicalMyCobot
{
    MyCobot mc;

    private int speed;

    private bool activatePump = false;

    double[] angles = { 0, 0, 0, 0, 0, 0 };

    // Start is called before the first frame update
    public PhysicalMyCobot(int speed)
    {
        this.speed = speed;

        // Start MyCobot engine
        string port = "COM3";
        mc = new MyCobot(port);

        mc.Open();
        Thread.Sleep(500);

        resetPosition();
    }

    public void resetPosition()
    {
        double[] default_angles = { 0, 0, 0, 0, 0, 0 };
        mc.SendAngles(default_angles, this.speed);
        Thread.Sleep(100);
    }

    public float[] updateControlledPhysicalRobot(int joint_id, ArticulationBody[] joints, float[] angles)
    {
        if (joints[joint_id].xDrive.target == angles[joint_id]) return angles;

        mc.SendOneAngle(joint_id, joints[joint_id].xDrive.target, this.speed);
        angles[joint_id] = joints[joint_id].xDrive.target;
        return angles;
    }
    public void pumpHandler()
    {
        activatePump = !activatePump;
        if (activatePump)
        {
            mc.SetBasicOut(0x5, 0x0);
        }
        else
        {
            mc.SetBasicOut(0x5, 0x1);
        }
    }

    public float[] getAngles()
    {
        float[] angles = new float[6];
        float[] floats = mc.GetAngles();
        for (int i = floats.Length - 1; i >= 0; i--)
        {
            angles[i] = floats[(floats.Length - 1) - i];
        }
        return angles;
    }

    public void updateFollowGameObject(GameObject gm, float fix_angle = 360)
    {
        GameObject head = GameObject.Find("joint6");
        Vector3 directionToTarget = new Vector3(0, head.transform.position.y, 0) - gm.transform.position;
        Vector3 localDirection = gm.transform.InverseTransformDirection(directionToTarget);
        float angle;
        if (fix_angle > 180 && fix_angle < -180)
        {
            angle = -(Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg) + gm.transform.rotation.y;
        }
        else
        {
            angle = fix_angle;
        }

        mc.SendCoords(new double[] {
            (gm.transform.position.z*877),
            -(gm.transform.position.x*877),
            (gm.transform.position.y*1015),
            angle,
            90,
            0
        }, this.speed, 0);
    }
}
