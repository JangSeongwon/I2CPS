
import rospy
import os
import threading, time
import sys
from geometry_msgs.msg import PoseStamped
from geometry_msgs.msg import Pose
from sensor_msgs.msg import JointState

sys.dont_write_bytecode = True
sys.path.append( os.path.abspath(os.path.join(os.path.dirname(__file__),"../../../../common/imp")) ) # get import path : DSR_ROBOT.py 

# for single robot 
ROBOT_ID     = "dsr01"
ROBOT_MODEL  = "m1509"
import DR_init
DR_init.__dsr__id = ROBOT_ID
DR_init.__dsr__model = ROBOT_MODEL
from DSR_ROBOT import *
import time

def callback_haptic(msg):
    start = time.time()
    POS = msg.pose.position
    ORI = msg.pose.orientation
    # print('Check Subscription')
    Hapticx = POS.x
    Hapticy = POS.y
    Hapticz = POS.z
    # ox = ORI.x
    # oy = ORI.y
    # oz = ORI.z
    # ow = ORI.w 
    # print('See Haptic info',Hapticx, Hapticy, Hapticz)
    
    robot_pos = [367.5965576171875, 33.294639587402344, 441.7723693847656, 36.052101135253906, 179.90725708007812, 36.2788200378418]
    # ROS Coordinate (Hapticx = y, Hapticy = z, Hapticz = x)
    x = round(robot_pos[0] + Hapticz*2000, 1)
    y = round(robot_pos[1] - Hapticx*2000, 1)
    z = round(robot_pos[2] + Hapticy*2000, 1)

    robot_pos_c = get_current_posx()
    getfromtuple = robot_pos_c[0]
    gx = round(getfromtuple[0], 1)
    gy = round(getfromtuple[1], 1)
    gz = round(getfromtuple[2], 1)

    ox = robot_pos[3] 
    oy = robot_pos[4]
    oz = robot_pos[5]
    # ox = getfromtuple[3] 
    # oy = getfromtuple[4]
    # oz = getfromtuple[5]

    eef = [x, y, z, ox, oy, oz]
    velx=[500, 500]
    accx=[1000, 1000]
    # if gx != x and gy != y and gz !=z:
    if (abs(gx - x) < 1) and (abs(gy - y) < 1) and (abs(gz - z) < 1):
        # print('No Moving')
        end1 = time.time()
        # print( end1-start)
        return
    else:
        # print('gx gy gz', x, gx, y, gy, z, gz)
        amovel(eef, velx, accx)
        end2 = time.time()
        print('Time IK', end2-start)

def callback_ori(msg):
    start = time.time()
    ORI = msg.orientation
    robot_home_ori = [169.03416442871094, 179.75306701660156, 169.2607879638672]
    robot_pos_fixed = get_current_posx()
    # print('Current Robot POS = ', robot_pos_fixed)

    Haptic_Rx = ORI.x
    Haptic_Ry = ORI.y
    Haptic_Rz = ORI.z
    Rx = round(robot_home_ori[0] + Haptic_Rx*0.5, 1)
    Ry = round(robot_home_ori[0] + Haptic_Ry*0.5, 1)
    Rz = round(robot_home_ori[0] + Haptic_Rz*0.5, 1)
    # print('Haptic Orientaion Input', Rx, Ry, Rz)
    Rx_c = robot_pos_fixed[0][3] 
    Ry_c = robot_pos_fixed[0][4]
    Rz_c = robot_pos_fixed[0][5]

    getfromtuple = robot_pos_fixed[0]
    Fixed_x = round(getfromtuple[0], 1)
    Fixed_y = round(getfromtuple[1], 1)
    Fixed_z = round(getfromtuple[2], 1)

    eef = [Fixed_x, Fixed_y, Fixed_z, Rx, Ry, Rz]
    velx=[500, 500]
    accx=[1000, 1000]
    if (abs(Rx_c - Rx) < 0.5) and (abs(Ry_c - Ry) < 0.5) and (abs(Rz_c - Rz) < 0.5):
        end1 = time.time()
        # print(end1-start)
        return
    else:
        amovel(eef, velx, accx)
        end = time.time()
        # print('Time IK for ORI', end-start)

    return
 

if __name__ == "__main__":
    rospy.init_node('Haptic')

    while not rospy.is_shutdown():
        rospy.Subscriber("/HapticInfo", PoseStamped, callback_haptic)
        rospy.Subscriber("/HapticOri", Pose, callback_ori)
        rospy.spin()

        # rate = rospy.Rate(2)
        # rate.sleep()    
        # print("Running")
        # set_velx(20,20)  # set global task speed: 30(mm/sec), 20(deg/sec)
        # set_accx(60,40)  # set global task accel: 60(mm/sec2), 40(deg/sec2)

        # robot_pos = get_current_posx()
        # print('Current Robot POS = ', robot_pos)
 
        


