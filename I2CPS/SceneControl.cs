// This code contains 3D SYSTEMS Confidential Information and is disclosed to you
// under a form of 3D SYSTEMS software license agreement provided separately to you.
//
// Notice
// 3D SYSTEMS and its licensors retain all intellectual property and
// proprietary rights in and to this software and related documentation and
// any modifications thereto. Any use, reproduction, disclosure, or
// distribution of this software and related documentation without an express
// license agreement from 3D SYSTEMS is strictly prohibited.
//
// ALL 3D SYSTEMS DESIGN SPECIFICATIONS, CODE ARE PROVIDED "AS IS.". 3D SYSTEMS MAKES
// NO WARRANTIES, EXPRESSED, IMPLIED, STATUTORY, OR OTHERWISE WITH RESPECT TO
// THE MATERIALS, AND EXPRESSLY DISCLAIMS ALL IMPLIED WARRANTIES OF NONINFRINGEMENT,
// MERCHANTABILITY, AND FITNESS FOR A PARTICULAR PURPOSE.
//
// Information and code furnished is believed to be accurate and reliable.
// However, 3D SYSTEMS assumes no responsibility for the consequences of use of such
// information or for any infringement of patents or other rights of third parties that may
// result from its use. No license is granted by implication or otherwise under any patent
// or patent rights of 3D SYSTEMS. Details are subject to change without notice.
// This code supersedes and replaces all information previously supplied.
// 3D SYSTEMS products are not authorized for use as critical
// components in life support devices or systems without express written approval of
// 3D SYSTEMS.
//
// Copyright (c) 2021 3D SYSTEMS. All rights reserved.
using RosSharp.RosBridgeClient.MessageTypes.Moveit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HapticGUI;
using UnityEngine;
using RosSharp.RosBridgeClient;

public class SceneControl : MonoBehaviour
{
    public HapticPlugin HPlugin = null;
    private float dbAS;
    public GameObject DeviceInfo;
    public GameObject Device1;
    public GameObject[] StageBorders;
    RosSharp.RosBridgeClient.ForceFeedbackSubscriber ForceFeedbackSubscriber;
    private List<double> lateral3;
    private List<double> torque3;

    private void UpdateKeys()
    {
        if (HPlugin != null)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (!HPlugin.isNavTranslation())
                {
                    HPlugin.EnableNavTranslation();
                    HPlugin.DisableNavRotation();
                    print("Rotation->Translation");

                }
                else
                {
                    HPlugin.DisableNavTranslation();
                    print("No Translation");
                }

            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!HPlugin.isNavRotation())
                {
                    HPlugin.EnableNavRotation();
                    HPlugin.DisableNavTranslation();
                }
                else
                {
                    HPlugin.DisableNavRotation();
                }

            }

            if (Input.GetKeyDown(KeyCode.N))
            {

                HPlugin.EnableVibration();
            }
            if (Input.GetKeyDown(KeyCode.M))
            {

                HPlugin.DisableVibration();
            }

            if (Input.GetKey("escape"))
            {
                Application.Quit();
            }

        }

    }


    private void Start()
    {
        //TextMeshPro TMesh = DeviceInfo.GetComponent<TextMeshPro>();
        //TMesh.text = HPlugin.DeviceIdentifier + "\n" + HPlugin.ModelType + "\n" + HPlugin.SerialNumber + "\n" + HPlugin.MaxForce.ToString("F") + " N";
        
        Device1.SetActive(true);
        Device1.GetComponent<VirtualHaptic>().ShowGizmo = true;
        Device1.GetComponent<VirtualHaptic>().ShowLabels = true;
            
    }


    // Update is called once per frame
    private void Update()
    {      
        UpdateKeys();
        ForceFeedbackSubscriber = (RosSharp.RosBridgeClient.ForceFeedbackSubscriber)this.GetComponentInParent(typeof(RosSharp.RosBridgeClient.ForceFeedbackSubscriber));
        StartCoroutine(GiveForceFeedback());
    }

    private IEnumerator GiveForceFeedback()
    {
        //lateral3 = (ForceFeedbackSubscriber.Fx, ForceFeedbackSubscriber.Fy, ForceFeedbackSubscriber.Fz);
        //torque3 = (ForceFeedbackSubscriber.Mx, ForceFeedbackSubscriber.My, ForceFeedbackSubscriber.Mz);

        //HPlugin.setForce(Device1, lateral3, torque3);

        yield return new WaitForSeconds(0.01f);
    }
}
