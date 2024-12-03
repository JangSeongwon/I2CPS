using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialization_Voxel : MonoBehaviour
{
    public Material worldMaterial;
    private Container container;
    public Vector3 starting_position;

    void Start()
    {
        GameObject cont = new GameObject("Container");
        cont.transform.parent = transform;
        container = cont.AddComponent<Container>();
        GameObject voxelCollider = new GameObject($"VoxelCollider");
        container.Initialize(worldMaterial, starting_position);

        Voxel_Test1();
        //Voxel_Test2();
        Voxel_Test3();

        container.GenerateMesh();
        container.UploadMesh();
    }

    public void EditVoxel(Vector3 contactpoint, int remove)
    {
    }

    public void Voxel_Test1()
    {

        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 30; z++)
            {
                int randomBurrHeight = Random.Range(1, 4);
                // print($"See Height of Burr {randomBurrHeight * 0.001}, X {3.95 + x * 0.001f}");
                for (int y = 0; y < randomBurrHeight; y++)
                {
                    container[new Vector3(3.925f + x * 0.005f, 0.13f + y * 0.005f, -0.07f + z * 0.005f)] = new Voxel_Sep() { ID = 1 };

                }
            }
        }
    }

    public void Voxel_Test2()
    {

        for (int x = 0; x < 20; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                int randomBurrHeight = Random.Range(1, 10);
                // print($"See Height of Burr {randomBurrHeight * 0.001}, X {3.95 + x * 0.001f}");
                for (int y = 0; y < randomBurrHeight; y++)
                {
                    container[new Vector3(3.95f + x * 0.005f, 0.13f + y * 0.005f, -0.05f + z * 0.005f)] = new Voxel_Sep() { ID = 1 };

                }
            }
        }
    }

    public void Voxel_Test3()
    {

        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 30; z++)
            {
                int randomBurrHeight = Random.Range(1, 4);
                // print($"See Height of Burr {randomBurrHeight * 0.001}, X {3.95 + x * 0.001f}");
                for (int y = 0; y < randomBurrHeight; y++)
                {
                    container[new Vector3(4.025f + x * 0.005f, 0.13f + y * 0.005f, -0.07f + z * 0.005f)] = new Voxel_Sep() { ID = 1 };

                }
            }
        }
    }
}

