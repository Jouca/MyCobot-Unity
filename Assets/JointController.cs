/*

joints2 = Y - 0
joints3 = X - 90
joints4 = Y - 0
joints5 = Y - 90
joints6 = X - -90
joints6_flange = X - 0
*/

using UnityEngine;
using System.Threading;
using Mycobot.csharp;
using Unity.Robotics.UrdfImporter.Control;

public class JointController : MonoBehaviour
{
    ArticulationBody[] joints;
    MyCobot mc;
    int joint_id = 1;

    private volatile float[] angles;

    private float stiffness;
    private float damping;
    private float forceLimit;
    private int speed;
    private float torque;
    private float acceleration;

    public double test;

    private bool wait = false;
    private bool activatePump = false;

    public enum RotationDirection { None = 0, Positive = 1, Negative = -1 };

    void Start()
    {
        this.joints = this.getJoints();

        // Add Controller component to firefighter
        GameObject firefighter = gameObject.transform.parent.gameObject.transform.parent.gameObject;
        Controller controller = firefighter.GetComponent<Controller>();

        this.stiffness = controller.stiffness;
        this.damping = controller.damping;
        this.forceLimit = controller.forceLimit;
        this.speed = (int)controller.speed;
        this.torque = controller.torque;
        this.acceleration = controller.acceleration;

        // Start MyCobot engine
        string port = "COM3";
        mc = new MyCobot(port);

        mc.Open();
        Thread.Sleep(500);

        resetPosition();
    }

    private void Update()
    {
        this.changeJoint();

        //this.updateControlledPhysicalRobot();

        this.updateFollowGameObject(GameObject.Find("Sphere"));
        //this.updateUnityRobot();
        this.controls();
    }

    private ArticulationBody[] getJoints()
    {
        return this.GetComponentsInChildren<ArticulationBody>();
    }

    private void changeJoint()
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

    private void updateControlledPhysicalRobot()
    {
        if (!wait)
        {
            mc.SendOneAngle(this.joint_id, this.joints[this.joint_id].xDrive.target, this.speed);
            Thread.Sleep(100);
        }
    }

    private void updateFollowGameObject(GameObject gm)
    {
        GameObject head = GameObject.Find("joint6");
        Vector3 directionToTarget = new Vector3(0, head.transform.position.y, 0) - gm.transform.position;
        Vector3 localDirection = gm.transform.InverseTransformDirection(directionToTarget);
        float angle = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;

        mc.SendCoords(new double[] {
            -(gm.transform.position.z*877),
            (gm.transform.position.x*877),
            (gm.transform.position.y*1015),
            -angle,
            -90,
            0
        }, this.speed, 0);
        Thread.Sleep(100);
    }

    private void updateUnityRobot()
    {
        angles = mc.GetAngles();
        Thread.Sleep(100);

        for (int i = angles.Length - 1; i >= 0; i--)
        {
            if (this.joints[i + 1].xDrive.target != angles[i])
                this.joints[i+1].SetDriveTarget(ArticulationDriveAxis.X, angles[i]);
        }
    }

    private void controls()
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
            pumpHandler();
        }
    }

    private void resetPosition()
    {
        wait = true;

        for (int i = 0; i <= this.joints.Length - 1; i++)
        {
            this.joints[i].SetDriveTarget(0, 0);
        }

        double[] default_angles = { 0, 0, 0, 0, 0, 0 };
        mc.SendAngles(default_angles, this.speed);
        Thread.Sleep(100);

        wait = false;
    }

    private void pumpHandler()
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
}
