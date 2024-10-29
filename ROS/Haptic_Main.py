
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

def callback_pos(msg):
    start = time.time()
    POS = msg.pose.position
    ORI = msg.pose.orientation
    # print('Check Subscription')
    Hapticx = POS.x
    Hapticy = POS.y
    Hapticz = POS.z
    # print('See Haptic info',Hapticx, Hapticy, Hapticz)
    
    '''Doosan Robot Home Position'''
    robot_pos = [-33.208, 426.252, -374.960, 89.988, -87.678, 0.227]
    
    '''ROS Coordinate (Hapticx = ROSx, Hapticy = ROSy, Hapticz = -ROSz)'''
    x = round(robot_pos[0] + Hapticx*500, 1)
    y = round(robot_pos[1] + Hapticy*500, 1)
    z = round(robot_pos[2] - Hapticz*500, 1)

    robot_pos_c = get_current_posx(Haptic_coord)
    getfromtuple = robot_pos_c[0]
    current_x = round(getfromtuple[0], 3)
    current_y = round(getfromtuple[1], 3)
    current_z = round(getfromtuple[2], 3)

    current_Rx = round(getfromtuple[3], 3)
    current_Ry = round(getfromtuple[4], 3)
    current_Rz = round(getfromtuple[5], 3)

    eef = [x, y, z, current_Rx, current_Ry, current_Rz]
    velx=[500, 120]
    accx=[1000, 240]

    """Publish"""
    operator_trajectory = PoseStamped()
    operator_trajectory.header.stamp = rospy.Time.now()
    operator_trajectory.pose.position.x = current_x
    operator_trajectory.pose.position.y = current_y
    operator_trajectory.pose.position.z = current_z
    operator_trajectory.pose.orientation.x = current_Rx
    operator_trajectory.pose.orientation.y = current_Ry
    operator_trajectory.pose.orientation.x = current_Rz
    operator_trajectory.pose.orientation.w = 0
    # print(operator_trajectory.pose.position, operator_trajectory.pose.orientation)
    pub.publish(operator_trajectory)   

    """오차 범위"""
    if (abs(current_x - x) < 0.5) and (abs(current_y - y) < 0.5) and (abs(current_z - z) < 0.5):
        end1 = time.time()
        # print("No Move", end1-start)
        return
    else:
        amovel(eef, velx, accx, ref = Haptic_coord)
        end2 = time.time()
        print(end2-start)
    
def callback_ori(msg):
    start = time.time()
    ORI = msg.orientation
    robot_home_ori = [89.988, -87.678, 0.227]
    robot_pos_current = get_current_posx()
    getfromtuple = robot_pos_current[0]
    # print('Current Robot POS = ', robot_pos_fixed)

    '''
    Haptic Input Into New ROS Coordinates
    Haptic X: -130도~60도, Haptic Z: -150도~160도
    '''
    sensitivity_x = 2
    sensitivity_z = 3
    Haptic_Rx = (ORI.x / sensitivity_x)
    # Haptic_Ry = ORI.y
    Haptic_Rz = (ORI.z / sensitivity_z)
    Rx = round(robot_home_ori[0] + Haptic_Rz, 3)
    Ry = round(robot_home_ori[1] + Haptic_Rx, 3)
    Rz = round(robot_home_ori[2], 3)

    Rx_c = robot_pos_current[0][3] 
    Ry_c = robot_pos_current[0][4]
    Rz_c = robot_pos_current[0][5]
    # print('Haptic Orientaion Input : ', Rx, Ry, Rz)

    Fixed_x = round(getfromtuple[0], 3)
    Fixed_y = round(getfromtuple[1], 3)
    Fixed_z = round(getfromtuple[2], 3)

    eef = [Fixed_x, Fixed_y, Fixed_z, Rx, Ry, Rz]
    velx=[500, 120]
    accx=[1000, 240]

    """Publish"""
    operator_trajectory = PoseStamped()
    operator_trajectory.header.stamp = rospy.Time.now()
    operator_trajectory.pose.position.x = Fixed_x
    operator_trajectory.pose.position.y = Fixed_y
    operator_trajectory.pose.position.z = Fixed_z
    operator_trajectory.pose.orientation.x = Rx_c
    operator_trajectory.pose.orientation.y = Ry_c
    operator_trajectory.pose.orientation.x = Rz_c
    operator_trajectory.pose.orientation.w = 0
    # print(operator_trajectory.pose.position, operator_trajectory.pose.orientation)
    pub.publish(operator_trajectory) 
    
    if (abs(Rx_c - Rx) < 0.1) and (abs(Ry_c - Ry) < 0.1) and (abs(Rz_c - Rz) < 0.1):
        end1 = time.time()
        # print(end1-start)
        return
    else:
        amovel(eef, velx, accx, ref = Haptic_coord)
        # robot_pos_check = get_current_posx(Haptic_coord)
        # print('Haptic Orientaion Input', 'x : 90 ', Rx, 'y : -90 ', Ry, 'z : 0 ', Rz)
        # print('Current Robot ORI = ', robot_pos_check[0][3], robot_pos_check[0][4], robot_pos_check[0][5])
        end = time.time()
        print(end-start)  

    return

if __name__ == "__main__":
    
    rospy.init_node('Haptic')
    Vector_x = [0, -1, 0]
    Vector_y = [0, 0, 1]
    Origin = posx(0,0,0,0,0,0)
    Haptic_coord = set_user_cart_coord(u1 = Vector_x, v1 = Vector_y, pos = Origin)
    set_ref_coord(Haptic_coord)
    pub = rospy.Publisher('Operator_Traj', PoseStamped, queue_size=3)

    while not rospy.is_shutdown():
        rospy.Subscriber(name = "/HapticInfo", data_class = PoseStamped, callback = callback_pos)
        rospy.Subscriber(name = "/HapticOri", data_class = Pose, callback = callback_ori)
        r = rospy.Rate(50)
        rospy.spin()
        
        """Initial Doosan robot vel, acc Settings"""
        # set_velx(20,20)  # set global task speed: 30(mm/sec), 20(deg/sec)
        # set_accx(60,40)  # set global task accel: 60(mm/sec2), 40(deg/sec2)

        """Robot posx check with respect to Haptic Coordinates"""
        # robot_pos, sol = get_current_posx(Haptic_coord)
        # print('Current Robot POS = xyz : ', robot_pos[0], robot_pos[1], robot_pos[2],'Rx : ', robot_pos[3], 'Ry : ', robot_pos[4], 'Rz : ', robot_pos[5])

        # robot_joint_state = get_current_posj()
        # print('Current Joint States', robot_joint_state[0]*pi/180, robot_joint_state[1]*pi/180, robot_joint_state[2]*pi/180, robot_joint_state[3]*pi/180, robot_joint_state[4]*pi/180, robot_joint_state[5]*pi/180)

        """ 
        ROS 상 각도 값
        Front 우측 45도 회전 : 140 -89 -0.4
        Front 우측 90도 회전시 각도 : 179 -89 0.1
        다시 정면 : 91 -93 0
        Front 왼쪽 45도 회전 : 38 -92 -2
        Front 왼쪽 90도 회전 : 2 -90 -3
        다시 정면 : 89 -89 0

        뒤로 45도 누웠을 때 : 90 -137 0
        뒤로 90도 누웠을 때 : 96 -177 5
        뒤로 135도 누웠을 때 : 175 -50 -175
        앞으로 45도 누움 : 90 -53 0
        앞으로 90도 누움 : 89 -17 0

        뒤로 45도 눕고 왼쪽 회전 45도 : 49 -118 -22

        """

    


