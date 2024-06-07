
# MyCobot




## Installation

1. Make sure to install it via the repository source code:

[https://github.com/Jouca/MyCobot/archive/refs/heads/main.zip](https://github.com/Jouca/MyCobot/archive/refs/heads/main.zip)

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

- `mycobot.resetPosition()` : This function is for resetting the position of MyCobot.
## Usage/Examples

Control MyCobot using arrows keys:

```cs
private void Update()
{
    jointController.changeJoint();
    jointController.updateControlledPhysicalRobot();
    jointController.controls();
}
```
