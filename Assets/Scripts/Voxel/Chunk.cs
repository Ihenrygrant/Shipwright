using System;
using UnityEngine;
using UnityEngine.Events;

public class Chunk : MonoBehaviour
{
    public ChunkData chunkInfo;
    public ShipChunkHandler ship;

    public MeshFilter filter;
    public MeshData meshData;

    private UnityEvent onBlockUpdate;
    public bool MeshUpdate = false;


    private bool willSerialize = true;

    public Chunk(ushort[] index, Vector3 location, ShipChunkHandler ship)
    {
        chunkInfo = new ChunkData();
        chunkInfo.chunkIndex = new ushort[3];
        chunkInfo.blockData = new BlockData[ChunkSettings.xSize, ChunkSettings.ySize, ChunkSettings.zSize];

        chunkInfo.chunkIndex = index;
        transform.position = location;
        this.ship = ship;
    }

    private void Awake()
    {

        if (filter == null)
        {
            filter = GetComponent<MeshFilter>();
        }

        //onBlockUpdate.AddListener(() => BuildChunk(this));
    }

    void Start()
    {
        //demoChunkInit();
    }



    private void Update()
    {
        
        if (MeshUpdate)
        {
            if (ship.runningChunkMeshUpdates <= ship.maxChunkMeshUpdates)
            {
                ship.runningChunkMeshUpdates++;

                SetMesh(meshData);
                MeshUpdate = false;
            }
        }
    }

    void SetMesh(MeshData generatedMesh)
    {

        Mesh mesh = filter.mesh;

        mesh.Clear();

        mesh.vertices = generatedMesh.generatedVertices.ToArray();
        mesh.triangles = generatedMesh.generatedTriangles.ToArray();
        mesh.uv = generatedMesh.generatedUV.ToArray();

        mesh.colors = generatedMesh.generatedColors.ToArray();
        mesh.normals = generatedMesh.generatedNormals.ToArray();

        filter.mesh = mesh;

        if (gameObject.GetComponent<MeshCollider>() != null)
        {
            Destroy(gameObject.GetComponent<MeshCollider>());
        }
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

        ship.runningChunkMeshUpdates--;

        if (true)
        {
            TheTide.utils.SerializeMesh serialize = gameObject.AddComponent<TheTide.utils.SerializeMesh>();
            serialize.Serialize();
            serialize.Rebuild();

        }
        //Debug.Log(Time.realtimeSinceStartup);
    }


    public void SetChunkData(ushort[] index, Vector3 location, ShipChunkHandler ship)
    {
        chunkInfo = new ChunkData();
        chunkInfo.chunkIndex = new ushort[3];
        chunkInfo.blockData = new BlockData[ChunkSettings.xSize, ChunkSettings.ySize, ChunkSettings.zSize];

        chunkInfo.chunkIndex[0] = index[0];
        chunkInfo.chunkIndex[1] = index[1];
        chunkInfo.chunkIndex[2] = index[2];

        transform.position = location;
        this.ship = ship;
        //Debug.Log(chunkInfo.chunkIndex[0] + "  " + chunkInfo.chunkIndex[1] + "  " + chunkInfo.chunkIndex[2]);
    }


    public void demoChunkInit()
    {

        for (var z = 0; z < ChunkSettings.xSize; ++z)
        {
            for (var y = 0; y < ChunkSettings.ySize; ++y)
            {
                for (var x = 0; x < ChunkSettings.zSize; ++x)
                {
                    chunkInfo.blockData[x, y, z] = new BlockData();

                    chunkInfo.blockData[x, y, z].Type = (byte)UnityEngine.Random.Range(0, 22);
                    chunkInfo.blockData[x, y, z].Health = (byte)UnityEngine.Random.Range(0, 255);
                    chunkInfo.blockData[x, y, z].Orientation = (byte)UnityEngine.Random.Range(0, 23);
                    chunkInfo.blockData[x, y, z].Status = (byte)BlockStatus.Occupied_Block;
                }
            }
        }
        BuildChunk(this);
    }

    public void singleBlockTypeChunkFIll()
    {

        for (var z = 0; z < ChunkSettings.xSize; ++z)
        {
            for (var y = 0; y < ChunkSettings.ySize; ++y)
            {
                for (var x = 0; x < ChunkSettings.zSize; ++x)
                {
                    chunkInfo.blockData[x, y, z] = new BlockData();

                    chunkInfo.blockData[x, y, z].Type = 16;
                    chunkInfo.blockData[x, y, z].Health = (byte)UnityEngine.Random.Range(0, 255);
                    chunkInfo.blockData[x, y, z].Orientation = 0;
                    chunkInfo.blockData[x, y, z].Status = (byte)BlockStatus.Occupied_Block;
                }
            }
        }
        BuildChunk(this);
    }

    public void EmptyChunkInit()
    {
        for (var z = 0; z < ChunkSettings.xSize; ++z)
        {
            for (var y = 0; y < ChunkSettings.ySize; ++y)
            {
                for (var x = 0; x < ChunkSettings.zSize; ++x)
                {
                    chunkInfo.blockData[x, y, z] = new BlockData();

                    chunkInfo.blockData[x, y, z].Type = 0;
                    chunkInfo.blockData[x, y, z].Health = (byte)UnityEngine.Random.Range(0, 255);
                    chunkInfo.blockData[x, y, z].Orientation = 0;
                    chunkInfo.blockData[x, y, z].Status = (byte)BlockStatus.Empty;
                }
            }
        }
        BuildChunk(this);
    }

    public void BuildChunk(Chunk updatedChunk)
    {
        if (ship == null) return;
        ship.NotifyChunkUpdate(updatedChunk, 10);
    }

    public void BuildChunk(Chunk updatedChunk, uint priority)
    {
        ship.NotifyChunkUpdate(updatedChunk, priority);
    }

    internal void SetBlock(int x, int y, int z, byte blockType, BlockStatus status)
    {
        chunkInfo.blockData[x, y, z].Type = blockType;
        chunkInfo.blockData[x, y, z].Status = (byte)status;

        BuildChunk(this, 0);
        //onBlockUpdate.Invoke();
    }

    internal void FillBlocks(byte[,,] BlockStatus)
    {
        for (var z = 0; z < ChunkSettings.xSize; ++z)
        {
            for (var y = 0; y < ChunkSettings.ySize; ++y)
            {
                for (var x = 0; x < ChunkSettings.zSize; ++x)
                {
                    //chunkInfo.blockData[x, y, z].Status = BlockStatus[x, y, z];
                }
            }
        }
        //onBlockUpdate.Invoke();
    }

    internal void FillBlocks(ChunkData chunkData)
    {

        for (var z = 0; z < ChunkSettings.xSize; ++z)
        {
            for (var y = 0; y < ChunkSettings.ySize; ++y)
            {
                for (var x = 0; x < ChunkSettings.zSize; ++x)
                {
                    chunkInfo.blockData[x, y, z] = new BlockData();

                    chunkInfo.blockData[x, y, z].Type = chunkData.blockData[x, y, z].Type;
                    chunkInfo.blockData[x, y, z].Health = chunkData.blockData[x, y, z].Health;
                    chunkInfo.blockData[x, y, z].Orientation = chunkData.blockData[x, y, z].Orientation;
                    chunkInfo.blockData[x, y, z].Status = chunkData.blockData[x, y, z].Status;
                }
            }
        }
        //onBlockUpdate.Invoke();
        BuildChunk(this);
    }


    internal BlockStatus GetBlockStatus(int x, int y, int z)
    {
        return (BlockStatus)chunkInfo.blockData[x, y, z].Status;
    }


}