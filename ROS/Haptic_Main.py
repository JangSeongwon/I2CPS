
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
    Haptic_Rx = ORI.x
    Haptic_Ry = ORI.y
    Haptic_Rz = ORI.z
    # print('See Haptic info',Hapticx, Hapticy, Hapticz)
    
    robot_pos = [368.3, 33.3, 438.6, 175.5, 179.6, 175.7]
    '''ROS Coordinate (Hapticx = y, Hapticy = z, Hapticz = x)'''
    x = round(robot_pos[0] + Hapticz*2000, 1)
    y = round(robot_pos[1] - Hapticx*2000, 1)
    z = round(robot_pos[2] + Hapticy*2000, 1)

    robot_pos_c = get_current_posx()
    getfromtuple = robot_pos_c[0]
    current_x = round(getfromtuple[0], 1)
    current_y = round(getfromtuple[1], 1)
    current_z = round(getfromtuple[2], 1)

    current_Rx = round(getfromtuple[3], 1)
    current_Ry = round(getfromtuple[4], 1)
    current_Rz = round(getfromtuple[5], 1)

    eef = [x, y, z, current_Rx, current_Ry, current_Rz]
    velx=[500, 120]
    accx=[1000, 240]
    # if gx != x and gy != y and gz !=z:
    if (abs(current_x - x) < 1) and (abs(current_y - y) < 1) and (abs(current_z - z) < 1):
        end1 = time.time()
        # print(end1-start)
        return
    else:
        # print('gx gy gz', x, gx, y, gy, z, gz)
        amovel(eef, velx, accx)
        end2 = time.time()
        print(end2-start)

def callback_ori(msg):
    start = time.time()
    ORI = msg.orientation
    robot_home_ori = [169, 179.8, 169.3]
    robot_pos_fixed = get_current_posx()
    # print('Current Robot POS = ', robot_pos_fixed)

    Haptic_Rx = ORI.x
    Haptic_Ry = ORI.y
    # Haptic_Rz = ORI.z
    Rx = round(robot_home_ori[0] + Haptic_Rx, 1)
    Ry = round(robot_home_ori[0] + Haptic_Ry, 1)
    Rz = round(robot_home_ori[0], 1)

    Rx_c = robot_pos_fixed[0][3] 
    Ry_c = robot_pos_fixed[0][4]
    Rz_c = robot_pos_fixed[0][5]

    # print('Haptic Orientaion Input',Rx_c, Rx, Ry_c, Ry, Rz_c, Rz)

    getfromtuple = robot_pos_fixed[0]
    Fixed_x = round(getfromtuple[0], 1)
    Fixed_y = round(getfromtuple[1], 1)
    Fixed_z = round(getfromtuple[2], 1)

    eef = [Fixed_x, Fixed_y, Fixed_z, Rx, Ry, Rz]
    velx=[20, 120]
    accx=[40, 240]
    
    if (abs(Rx_c - Rx) < 0.5) and (abs(Ry_c - Ry) < 0.5) and (abs(Rz_c - Rz) < 0.5):
        end1 = time.time()
        # print(end1-start)
        return
    else:
        amovel(eef, velx, accx)
        robot_pos_check = get_current_posx()
        # print('Haptic Orientaion Input', 'x : 170 ', Rx, 'y : 180 ', Ry, 'z : 170 ', Rz)
        # print('Current Robot ORI = ', robot_pos_check[0][3], robot_pos_check[0][4], robot_pos_check[0][5])
        end = time.time()
        print(end-start)

    return
 

if __name__ == "__main__":
    rospy.init_node('Haptic')
    rate = rospy.Rate(20)

    while not rospy.is_shutdown():
        # rospy.Subscriber("/HapticInfo", PoseStamped, callback_haptic)
        # rospy.Subscriber(name = "/HapticOri", data_class = Pose, callback = callback_ori)
        # rospy.spin()
        
        #rate.sleep()    

        """Initial Doosan robot vel, acc Settings"""
        # set_velx(20,20)  # set global task speed: 30(mm/sec), 20(deg/sec)
        # set_accx(60,40)  # set global task accel: 60(mm/sec2), 40(deg/sec2)

        """Robot posx check"""
        robot_pos = get_current_posx()
        print('Current Robot POS = Rx : ', robot_pos[0][3], 'Ry : ', robot_pos[0][4], 'Rz : ', robot_pos[0][5])

        """ 
        ROS 상 각도 값
        Front 우측 45도 회전 : 85 -140 85
        Front 우측 90도 회전시 각도 : 90 -90 90
        다시 정면 : 120 180 120
        Front 왼쪽 45도 회전 : 92 122 94
        Front 왼쪽 회전시 각도 : 90 90 90
        다시 정면 : 105 180 (=-180) 105

        뒤로 90도 누웠을 때 : 179 -88 -175
        뒤로 135도 누웠을 때 : 175 -50 -175
        다시 정면 : 100 175 100
        앞으로 45도 누움 : 175 135 175
        앞으로 90도 누움 : 178 108 175

        뒤로 45도 눕고 앞으로 좀 갔을 때 : 179 -138 179 (앞, 뒤 움직임 문제 없음)
        앞으로 좀 누웠을 때 : 179 138 -179

        """
 
        


