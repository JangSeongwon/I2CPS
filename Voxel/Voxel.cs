using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Voxel
{
    public Vector3 position;
    public Color color;
    public bool isActive;
    public Voxel(Vector3 position, Color color, bool isActive = true)
    {
        this.position = position;
        this.color = color;
        this.isActive = isActive;
    }
}


