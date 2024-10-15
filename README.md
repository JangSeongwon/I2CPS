# I2CPS
Haptic - Unity - ROS 

--------------------------
*Unity Project Features*

1. Haptic Touch X in Unity    
Use Assets: https://assetstore.unity.com/packages/tools/integration/haptics-direct-for-unity-v1-197034
```
1. Actor : [HapticPlugin]
2. Collider : [HapticCollider]
3. Haptic Initializer : [SceneControl]
```

2. Doosan Robot in Unity
```
M1509 (6DOF): URDF
*Currently with 6FTsensor and Simple Motor-Drill

m1509_ori: Need to change Links(Rigid Body) into Articulation Body (M1509 Reference to copy and paste its components)

Collision : Adding Collisions
Moved with Joint inputs: [SetJointPositions in Initialization]

3. Other used Assets
 - Grid Material 'https://assetstore.unity.com/packages/2d/textures-materials/gridbox-prototype-materials-129127?srsltid=AfmBOoowjbEvLqTxcaygvjJtN8hxCxJowGYixQxBeUa_0jl3p8vigAHM'

```


--------------------------
*ROS with Unity*

ROS#: https://github.com/siemens/ros-sharp   
*URDF Importer: https://github.com/Unity-Technologies/URDF-Importer (ROSBridge URDF Importer와 충돌, ROS#만 사용)   

1. Haptic Touch X
```
From Haptic
Haptic information(Transform) sending: Use [EndEffectorPublisher.cs] from RosSharp
ROS topic Published: '/HapticInfo' in Approximately 25Hz

```

2. ROS
```
Using Rviz(Doosan Robot ROS Package)
1. I2CPS_haptic (alias in gedit ~/.bashrc) : launching virtual mode of doosan robot simulation (jsw_haptic.launch)
2. Python3 (dsr_Haptic_IK.py) : Moving Robot eef with [movel] & Haptic POS

ROS topic to Unity: [JointStatePublisher] '/dsr01m1509/joint_states'
ROS topic subscribed in Unity with: [JointValueSubscriber] (msg type: Sensor.JointState)

*Moveit IK Solver time Problem
Current vel 1000, acc 5000 / Threshold 1mm, 3mm Testing

```
```
Workspace
ROS topic for ToolEnd: /ToolEnd

```

--------------------------

