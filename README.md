
# MyCobot (IK & Camera version)

<center>
    <img src="https://external-preview.redd.it/Fgenl35V38AtzZ6BKiU0tmHRH_f4YjhNrMansQDYrkQ.jpg?width=1080&crop=smart&auto=webp&s=281b9d51aa0316e7962c013f645322aa083e4709" />
</center>

## Installation

1. Make sure to install it via the repository source code:

[https://github.com/Jouca/MyCobot/archive/refs/heads/ik.zip](https://github.com/Jouca/MyCobot/archive/refs/heads/ik.zip)

2. Drag **first** `Packages` folder into your Unity Project and after **make sure if you have already some packages installed that you need to update `packages-lock.json`**, after that make sure that Unity compiled the packages on the Project. Then, drag `Assets` folder into the project and compile it.
3. The robot should appear in Unity Hierarchy.
4. Create an empty GameObject.
5. Join the C# Script on `Assets` called `MyCobotMain.cs` to the GameObject
6. Open this file and edit it to implement your functionnalities.

## Documentation

To interact with the library, make sure to import the game component JointController (linked with `firefighter` game object) on your C# Script of your game object:
```cs
GameObject mycobot;
JointController jointController;

void Start()
{
    mycobot = GameObject.Find("firefighter");
    jointController = mycobot.GetComponentsInChildren<JointController>()[0];
}
```

Here's the functions that you can use (most of them in a Unity **Update()** function):

- `mycobot.changeJoint()` : This function allows to control MyCobot by changing the selected joint to control with arrows keys (LEFT/RIGHT ARROWS KEYS).

- `mycobot.updateControlledPhysicalRobot()` : This function is to make physical MyCobot synchronise positions.

- `mycobot.updateUnityRobot()` : This function is to make MyCobot Unity model synchronise positions.

- `mycobot.updateFollowGameObject(GameObject gm)` : This function allows MyCobot to follow a GameObject on Unity, this will only move the physical MyCobot. If you want to make it moving with MyCobot model, please use `mycobot.updateUnityRobot();`.

- `mycobot.controls()` : This function is to allow keyboard controls with MyCobot. Keys are assigned with a specific control. `"R key"` = Reset position of MyCobot, `"P key"` = Activate pump

- `mycobot.updateObjectPosition(GameObject gm)` : This function will allow you to update GameObject position around the Robot by the help of the Webcam. **This will only work with RED objects**.

- `mycobot.resetPosition()` : This function is for resetting the position of MyCobot.
## Usage/Examples

Make MyCobot following a RED object:

```cs
private void Update()
{
    jointController.updateFollowGameObject(GameObject.Find("Sphere"));
    jointController.updateUnityRobot();
    jointController.updateControlledPhysicalRobot();
    jointController.updateObjectPosition(GameObject.Find("Sphere"));
}
```

Control MyCobot using arrows keys:

```cs
private void Update()
{
    jointController.changeJoint();
    jointController.updateControlledPhysicalRobot();
    jointController.controls();
}
```

