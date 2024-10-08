#!/usr/bin/env python
# -*- coding: utf-8 -*-
# ##
# @brief    [py example simple] motion basic test for doosan robot
# @author   Kab Kyoum Kim (kabkyoum.kim@doosan.com)   

import rospy
import os
import threading, time
import sys
from geometry_msgs.msg import PoseStamped
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

def Haptic():
    rospy.Subscriber("HapticInfo", PoseStamped, callback_sub)
    
def callback_sub(msg):
    POS = msg.pose.position
    ORI = msg.pose.orientation
    # print('Check Subscription', POS, ORI)
    Hapticx = POS.x
    Hapticy = POS.y
    Hapticz = POS.z
    # ox = ORI.x
    # oy = ORI.y
    # oz = ORI.z
    # ow = ORI.w 
    
    robot_pos = get_current_posx()
    getfromtuple = robot_pos[0]
    x = getfromtuple[0] + Hapticx*10
    y = getfromtuple[1] + Hapticy*10
    z = getfromtuple[2] + Hapticz*10
    ox = getfromtuple[3] 
    oy = getfromtuple[4]
    oz = getfromtuple[5]

    eef = posx(x, y, z, ox, oy, oz)
    velx=[10, 10]
    accx=[50, 50]
    movel(eef, velx, accx)
    print('GO = 10')

    
if __name__ == "__main__":
    rospy.init_node('Haptic_IK')
    robot_pos = [365.194122, 74.7998962, 521.999389]

    while not rospy.is_shutdown():
        t2 = threading.Thread(target=Haptic)
        t2.daemon = True 
        t2.run()         
        # print("Running")
        set_velx(20,20)  # set global task speed: 30(mm/sec), 20(deg/sec)
        set_accx(60,40)  # set global task accel: 60(mm/sec2), 40(deg/sec2)

        # robot_pos = get_current_posx()
        # print('Current Robot POS = 1', robot_pos)
        


