
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

def callback_sub(msg):
    
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
    
    robot_pos = [367.0652, -32.8631, 319.2571, 91.1484, 179.9457, 91.4019]
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
    velx=[1000, 1000]
    accx=[5000, 5000]
    # if gx != x and gy != y and gz !=z:
    if (gx - x < 1) and (gy - y < 1) and (gz - z < 1):
        # print('No Moving')
        return
    else:
        # print('gx gy gz', x, gx, y, gy, z, gz)
        amovel(eef, velx, accx)
    
if __name__ == "__main__":
    rospy.init_node('Haptic_IK')

    while not rospy.is_shutdown():
        sub = rospy.Subscriber("/HapticInfo", PoseStamped, callback_sub)
        rospy.spin()

        # rate = rospy.Rate(2)
        # rate.sleep()    
        # print("Running")
        # set_velx(20,20)  # set global task speed: 30(mm/sec), 20(deg/sec)
        # set_accx(60,40)  # set global task accel: 60(mm/sec2), 40(deg/sec2)

        # robot_pos = get_current_posx()
        # print('Current Robot POS = ', robot_pos)
 
        


