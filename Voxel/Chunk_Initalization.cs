using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk_Initialization : MonoBehaviour
{
    private Voxel[,,] voxels;
    private int chunkSize = 16;

    void Start()
    {
        voxels = new Voxel[chunkSize, chunkSize, chunkSize];
        InitializeVoxels();

    }

    private void InitializeVoxels()
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    voxels[x, y, z] = new Voxel(transform.position + new Vector3(x, y, z), Color.white);
                }
            }
        }
    }
    void OnDrawGizmos()
    {
        if (voxels != null)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        if (voxels[x, y, z].isActive)
                        {
                            Gizmos.color = voxels[x, y, z].color;
                            Gizmos.DrawCube(transform.position + new Vector3(x, y, z), Vector3.one);
                        }
                    }
                }
            }
        }
    }

    public void Initialize(int size)
    {
        this.chunkSize = size;
        voxels = new Voxel[size, size, size];
        InitializeVoxels();
    }

}


