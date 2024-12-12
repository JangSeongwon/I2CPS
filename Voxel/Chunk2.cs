using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class Chunk2 : MonoBehaviour
{
    GameObject ChunkObj;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;
    public SphereCollider sphereCollider;
    public MeshFilter meshFilter;

    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    public byte[,,] voxelMap = new byte[VoxelData2.ChunkX, VoxelData2.ChunkY, VoxelData2.ChunkZ];
    public Material material; 
    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;
    public float surface_area;

    void Start()
    {
        meshRenderer.material = material;
        PopulateVoxelMap();
        UpdateChunk();
        CreateMesh();
        calculate_surface_area();

    }

    void ClearMeshData()
    {
        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        

    }

    public void calculate_surface_area()
    {
        surface_area = 2 * (VoxelData2.ChunkX * VoxelData2.ChunkY + VoxelData2.ChunkX * VoxelData2.ChunkZ + VoxelData2.ChunkY * VoxelData2.ChunkZ);
        print($"Surface Area: {surface_area}");
        if (surface_area >= 16000)
        {
            print($"Warning exceeded surface limit");
        }
    }

    void Update()
    {
        //UpdateChunk();
    }
    public Vector3 position
    {

        get { return ChunkObj.transform.position; }

    }

    public void UpdateChunk()
    {
        ClearMeshData();
        meshRenderer.material = material;
        for (int z = 0; z < VoxelData2.ChunkZ; z++)
        {
            for (int x = 0; x < VoxelData2.ChunkX; x++)
            {
                for (int y = 0; y < VoxelData2.ChunkY; y++)
                {
                    if (voxelMap[x,y,z] == 1)
                        AddVoxelDataToChunk(new Vector3(x, y, z));
                }
            }
        }
      
        CreateMesh();
    }

    public void EditVoxel(Vector3 pos, byte newID)
    {

        int xCheck = Mathf.FloorToInt((float)System.Math.Truncate((pos.x - 3.9250) / 0.0005));
        int yCheck = Mathf.FloorToInt((float)System.Math.Truncate((pos.y - 0.1388) / 0.0005));
        int zCheck = Mathf.FloorToInt((float)System.Math.Truncate((pos.z + 0.0460) / 0.0005));

        //print($"{xCheck}, {yCheck}, {zCheck}");

        //Removing 27 Voxels per contact
        voxelMap[xCheck, yCheck, zCheck] = newID;
        if (zCheck != 0)
            voxelMap[xCheck, yCheck, zCheck - 1] = newID;
        if (zCheck != VoxelData2.ChunkZ - 1)
            voxelMap[xCheck, yCheck, zCheck + 1] = newID;

        if (yCheck != 0)
        {
            voxelMap[xCheck, yCheck - 1, zCheck] = newID;
            if (zCheck != 0)
                voxelMap[xCheck, yCheck - 1, zCheck - 1] = newID;
            if (zCheck != VoxelData2.ChunkZ - 1)
                voxelMap[xCheck, yCheck - 1, zCheck + 1] = newID;
        }
        if (yCheck != VoxelData2.ChunkY - 1)
        {
            voxelMap[xCheck, yCheck + 1, zCheck] = newID;
            if (zCheck != 0)
                voxelMap[xCheck, yCheck + 1, zCheck - 1] = newID;
            if (zCheck != VoxelData2.ChunkZ - 1)
                voxelMap[xCheck, yCheck + 1, zCheck + 1] = newID;
        }

        if (xCheck != 0)
        {
            voxelMap[xCheck - 1, yCheck, zCheck] = newID;
            if (zCheck != 0)
                voxelMap[xCheck - 1, yCheck, zCheck - 1] = newID;
            if (zCheck != VoxelData2.ChunkZ - 1)
                voxelMap[xCheck - 1, yCheck, zCheck + 1] = newID;

            if (yCheck != 0)
            {
                voxelMap[xCheck - 1, yCheck - 1, zCheck] = newID;
                if (zCheck != 0)
                    voxelMap[xCheck - 1, yCheck - 1, zCheck - 1] = newID;
                if (zCheck != VoxelData2.ChunkZ - 1)
                    voxelMap[xCheck - 1, yCheck - 1, zCheck + 1] = newID;
            }
            if (yCheck != VoxelData2.ChunkY - 1)
            {
                voxelMap[xCheck - 1, yCheck + 1, zCheck] = newID;
                if (zCheck != 0)
                    voxelMap[xCheck - 1, yCheck + 1, zCheck - 1] = newID;
                if (zCheck != VoxelData2.ChunkZ - 1)
                    voxelMap[xCheck - 1, yCheck + 1, zCheck + 1] = newID;
            }
        }
        if (xCheck != VoxelData2.ChunkX - 1)
        {
            voxelMap[xCheck + 1, yCheck, zCheck] = newID;
            if (zCheck != 0)
                voxelMap[xCheck + 1, yCheck, zCheck - 1] = newID;
            if (zCheck != VoxelData2.ChunkZ - 1)
                voxelMap[xCheck + 1, yCheck, zCheck + 1] = newID;

            if (yCheck != 0)
            {
                voxelMap[xCheck + 1, yCheck - 1, zCheck] = newID;
                if (zCheck != 0)
                    voxelMap[xCheck + 1, yCheck - 1, zCheck - 1] = newID;
                if (zCheck != VoxelData2.ChunkZ - 1)
                    voxelMap[xCheck + 1, yCheck - 1, zCheck + 1] = newID;
            }
            if (yCheck != VoxelData2.ChunkY - 1)
            {
                voxelMap[xCheck + 1, yCheck + 1, zCheck] = newID;
                if (zCheck != 0)
                    voxelMap[xCheck + 1, yCheck + 1, zCheck - 1] = newID;
                if (zCheck != VoxelData2.ChunkZ - 1)
                    voxelMap[xCheck + 1, yCheck + 1, zCheck + 1] = newID;
            }
        }
        UpdateChunk();
    }

    void PopulateVoxelMap()
    {

        for (int z = 0; z < VoxelData2.ChunkZ; z++)
        {
            for (int x = 0; x < VoxelData2.ChunkX; x++)
            {
                for (int y = 0; y < VoxelData2.ChunkY; y++)
                {

                    voxelMap[x, y, z] = 1;

                }
            }
        }

    }

    bool CheckVoxel(Vector3 pos)
    {

        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (x < 0 || x > VoxelData2.ChunkX - 1 || y < 0 || y > VoxelData2.ChunkY - 1 || z < 0 || z > VoxelData2.ChunkZ - 1)
            return false;
        else
        {
            if (voxelMap[x, y, z] == 0)
                return false;
            return true;
        }
    }

    void AddVoxelDataToChunk(Vector3 pos)
    {

        for (int p = 0; p < 6; p++)
        {

            if (!CheckVoxel(pos + VoxelData2.faceChecks[p]))
            {

                vertices.Add(pos + VoxelData2.voxelVerts[VoxelData2.voxelTris[p, 0]]);
                vertices.Add(pos + VoxelData2.voxelVerts[VoxelData2.voxelTris[p, 1]]);
                vertices.Add(pos + VoxelData2.voxelVerts[VoxelData2.voxelTris[p, 2]]);
                vertices.Add(pos + VoxelData2.voxelVerts[VoxelData2.voxelTris[p, 3]]);

                AddTexture(GetTextureID(p));
                //uvs.Add(VoxelData2.voxelUvs[0]);
                //uvs.Add(VoxelData2.voxelUvs[1]);
                //uvs.Add(VoxelData2.voxelUvs[2]);
                //uvs.Add(VoxelData2.voxelUvs[3]);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                vertexIndex += 4;

            }
        }

    }

    void CreateMesh()
    {

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        //mesh.SetUVs(0, uvs);
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        //mesh.RecalculateBounds();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    void AddTexture(int textureID)
    {

        float y = textureID / VoxelData2.TextureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData2.TextureAtlasSizeInBlocks);

        x *= VoxelData2.NormalizedBlockTextureSize;
        y *= VoxelData2.NormalizedBlockTextureSize;

        y = 1f - y - VoxelData2.NormalizedBlockTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData2.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x + VoxelData2.NormalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData2.NormalizedBlockTextureSize, y + VoxelData2.NormalizedBlockTextureSize));


    }

    public int GetTextureID(int faceIndex)
    {

        switch (faceIndex)
        {

            case 0:
                return backFaceTexture;
            case 1:
                return frontFaceTexture;
            case 2:
                return topFaceTexture;
            case 3:
                return bottomFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                Debug.Log("Error in GetTextureID; invalid face index");
                return 0;


        }

    }
}
