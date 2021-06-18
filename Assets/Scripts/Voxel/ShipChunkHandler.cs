using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;

public class ShipChunkHandler : MonoBehaviour {

    //Threads
    public List<ChunkGameObject> JobQueue = new List<ChunkGameObject>();
    public ChunkMeshGeneratorThread ChunkMeshGeneratorThreads;
    int runningThreads = 0;
    int maximumThreads = 1;

    //Chunks
    GameObject chunkPrefab;
    public GameObject DemoChunks;
    public GameObject DemoExplosion;

    static int xShipSize = 5;
    static int yShipSize = 5;
    static int zShipSize = 5;

    public GameObject[,,] chunks = new GameObject[xShipSize, yShipSize, zShipSize];

    public ushort runningChunkMeshUpdates = 0;
    public ushort maxChunkMeshUpdates = 3;


    private GameObject ChunkEmpty;

    private void Awake()
    {
        TextureMapController.Initialize();
    }

    void Start()
    {

    }

    private void Update()
    {
        if (JobQueue.Count > 0 && runningThreads <= maximumThreads)
        {
            for (int i = runningThreads; i < maximumThreads ; i++ ) {
                if (JobQueue.Count == 0) break;
                runningThreads++;

                ChunkMeshGeneratorThread thread = new ChunkMeshGeneratorThread();
                thread.Init(this);

                for (int j = 0; j < 16; j++)
                {
                    if (JobQueue.Count == 0) break;
                    thread.SetData(JobQueue[0]);
                    JobQueue.Remove(JobQueue[0]);
                }

                thread._thread.Start();
            }
        }
    }

    IEnumerator JobHandler()
    {
        while (true)
        {
            while (JobQueue.Count > 0)
            {
                Init(JobQueue[0]);
                _thread.Start();

                while (_threadRunning)
                {
                    yield return new WaitForEndOfFrame();
                }
                _thread.Abort();
                JobQueue.Remove(JobQueue[0]);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
        }
    }


    IEnumerator CreateChunk(ushort x, ushort y, ushort z)
    {
        GameObject newChunk = Instantiate(chunkPrefab, transform);

        ushort[] index = new ushort[3];
        index[0] = x;
        index[1] = y;
        index[2] = z;

        newChunk.GetComponent<ChunkGameObject>().SetChunkData(index, newChunk.transform.position + new Vector3(x * 16, y * 16, z * 16), this);

        chunks[x, y, z] = newChunk;
        yield return null;
    }

    public void NotifyChunkUpdate(ChunkGameObject chunk, uint priority)
    {
        //Debug.Log(chunk.chunkInfo.chunkIndex[0] + "  " + chunk.chunkInfo.chunkIndex[1] + "  " + chunk.chunkInfo.chunkIndex[2]);
        switch (priority)
        {
            case 0:
                JobQueue.Insert(0, chunk);
                break;
            default:
                JobQueue.Add(chunk);
                break;
        }

    }

    public void NotifyThreadFinished()
    {
        runningThreads--;
    }

    public void ClearChunks()
    {
        for (int x = 0; x < xShipSize; x++)
        {
            for (int y = 0; y < yShipSize; y++)
            {
                for (int z = 0; z < zShipSize; z++)
                {
                    Destroy(chunks[x, y, z]);
                }
            }
        }
        ClearData();
    }

    public void ClearData()
    {
        Array.Clear(chunks, 0, chunks.Length);
    }

    public void ResetChunks()
    {
        ClearChunks();

        if (transform.Find("Chunks")) Destroy(transform.Find("Chunks"));

        Instantiate(DemoChunks, transform);
    }


    public void Save(string filename)
    {
        List<ChunkData> chunkInfos = new List<ChunkData>();
        for (int x = 0; x < xShipSize; x++)
        {
            for (int y = 0; y < yShipSize; y++)
            {
                for (int z = 0; z < zShipSize; z++)
                {
                    Debug.Log(chunks[x, y, z]);
                    chunkInfos.Add(chunks[x, y, z].GetComponent<ChunkGameObject>().chunkInfo);
                }
            }
        }

        ChunkFlatBufferController chunkFlatBufferController = new ChunkFlatBufferController();
        byte[] fbbChunks = chunkFlatBufferController.PackChunk(chunkInfos);

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        if (File.Exists(filename)) File.Delete(filename);
        using (FileStream fileStream = File.Open(filename, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, fbbChunks);
        }
    }

    //TODO thread this
    public void Load(string filename)
    {


        List<ChunkData> UnpackedchunkInfos = new List<ChunkData>();

        ChunkFlatBufferController chunkFlatBufferController = new ChunkFlatBufferController();

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        byte[] fbbChunks;

        if (!File.Exists(filename)) return;
        using (FileStream fileStream = File.Open(filename, FileMode.Open))
        {
            fbbChunks = (byte[])binaryFormatter.Deserialize(fileStream);
        }

        UnpackedchunkInfos = chunkFlatBufferController.UnpackChunk(fbbChunks);


        ClearChunks();

        if (chunkPrefab == null) chunkPrefab = Resources.Load("Chunk") as GameObject;
        if (transform.Find("Chunks") == null)
        {
            GameObject chunks = new GameObject();
            chunks.name = "Chunks";
            chunks.transform.SetParent(transform);
            ChunkEmpty = chunks;
        }

        for (int i = 0; i < UnpackedchunkInfos.Count; i++)
        {
            int z = i % zShipSize;
            int y = (i / zShipSize) % yShipSize;
            int x = i / (yShipSize * zShipSize);

            string name = x+"l"+y+"l"+z;
            GameObject gameObject = Instantiate(chunkPrefab, transform);
            gameObject.name = name;
            chunks[x, y, z] = gameObject;
            gameObject.transform.SetParent(ChunkEmpty.transform);

            ChunkGameObject chunk  = gameObject.GetComponent<ChunkGameObject>();
            chunk.SetChunkData(UnpackedchunkInfos[i].index, chunk.transform.position + new Vector3(x * ChunkSettings.xSize, y * ChunkSettings.ySize, z * ChunkSettings.zSize), this);
            chunk.FillBlocks(UnpackedchunkInfos[i]);

        }

    }


    public void DemoLoadChunks()
    {
        Debug.Log(Time.realtimeSinceStartup);

        if (chunkPrefab == null) chunkPrefab = Resources.Load("Chunk") as GameObject;
        ClearChunks();

        if (transform.Find("Chunks") == null)
        {
            GameObject chunks = new GameObject();
            chunks.name = "Chunks";
            chunks.transform.SetParent(transform);
            ChunkEmpty = chunks;
        }

        for (ushort x = 0; x < xShipSize; x++)
        {
            for (ushort y = 0; y < yShipSize; y++)
            {
                for (ushort z = 0; z < zShipSize; z++)
                {
                    GameObject newChunk = Instantiate(chunkPrefab, transform);
                    newChunk.name = x + "I" + y + "I" + z;

                    ushort[] index = new ushort[3];
                    index[0] = x;
                    index[1] = y;
                    index[2] = z;

                    ChunkGameObject chunkRef = newChunk.GetComponent<ChunkGameObject>();
                    chunkRef.SetChunkData(index, newChunk.transform.position + new Vector3(x * ChunkSettings.xSize, y * ChunkSettings.ySize, z * ChunkSettings.zSize), this);
                    chunks[x, y, z] = newChunk;
                    newChunk.transform.SetParent(ChunkEmpty.transform);

                    chunkRef.demoChunkInit();
                }
            }
        }
        Debug.Log(Time.realtimeSinceStartup);
        JobHandler();
    }

    public void DemoLoadChunkSingle()
    {
        Debug.Log(Time.realtimeSinceStartup);

        if (chunkPrefab == null) chunkPrefab = Resources.Load("Chunk") as GameObject;
        ClearChunks();

        if (transform.Find("Chunks") == null)
        {
            GameObject chunks = new GameObject();
            chunks.name = "Chunks";
            chunks.transform.SetParent(transform);
            ChunkEmpty = chunks;
        }

        for (ushort x = 0; x < xShipSize; x++)
        {
            for (ushort y = 0; y < yShipSize; y++)
            {
                for (ushort z = 0; z < zShipSize; z++)
                {
                    GameObject newChunk = Instantiate(chunkPrefab, transform);
                    newChunk.name = x + "I" + y + "I" + z;

                    ushort[] index = new ushort[3];
                    index[0] = x;
                    index[1] = y;
                    index[2] = z;

                    ChunkGameObject chunkRef = newChunk.GetComponent<ChunkGameObject>();
                    chunkRef.SetChunkData(index, newChunk.transform.position + new Vector3(x * ChunkSettings.xSize, y * ChunkSettings.ySize, z * ChunkSettings.zSize), this);
                    chunks[x, y, z] = newChunk;
                    newChunk.transform.SetParent(ChunkEmpty.transform);

                    if (x == xShipSize / 2 && y == yShipSize / 2 && z == zShipSize / 2) chunkRef.singleBlockTypeChunkFIll();
                    else chunkRef.EmptyChunkInit();
                }
            }
        }
        Debug.Log(Time.realtimeSinceStartup);
        JobHandler();
    }


    public void DestroyShip()
    {


        var explosion = Instantiate(DemoExplosion, transform.position, transform.rotation);


        for (ushort x = 0; x < xShipSize; x++)
        {
            for (ushort y = 0; y < yShipSize; y++)
            {
                for (ushort z = 0; z < zShipSize; z++)
                {
                    var r = chunks[x, y, z].AddComponent<Rigidbody>();
                    r.useGravity = false;

                    chunks[x, y, z].GetComponent<Rigidbody>().AddForce(new Vector3(x, y, z)*25);// * UnityEngine.Random.Range(0.0F, 10.0F));
                    chunks[x, y, z].GetComponent<Rigidbody>().AddTorque(new Vector3(UnityEngine.Random.Range(1.0F, 1.0F), UnityEngine.Random.Range(0.0F, 0.1F), UnityEngine.Random.Range(0.0F, 0.1F)));
                }
            }
        }
    }

    void CreateExplosion()
    {

    }

    #region Threading Code
    public float AtlasElementSize = 0.333f;
    public float AtlasOffsetRow = 0.333f;
    public float AtlasOffsetCol = 0.333f;

    bool _threadRunning;
    public Thread _thread;

    ChunkGameObject loadedChunk;

    public void Init(ChunkGameObject UpdatedChunk)
    {
        // Begin our heavy work on a new thread.
        //_thread = new Thread(ThreadedWork);
        loadedChunk = UpdatedChunk;
    }


    private Block CheckBlock(ChunkGameObject updatedChunk, int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0 ||
            x >= ChunkSettings.xSize || y >= ChunkSettings.ySize || z >= ChunkSettings.zSize)
        {
            return new Block(x, y, z, BlockStatus.Empty);
        }
        return new Block(x, y, z, updatedChunk.GetBlockStatus(x, y, z));
    }

    private class Block
    {
        public int x;
        public int y;
        public int z;
        public BlockStatus status;

        public Block(int x, int y, int z, BlockStatus status)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.status = status;
        }
    }
    #endregion
}
