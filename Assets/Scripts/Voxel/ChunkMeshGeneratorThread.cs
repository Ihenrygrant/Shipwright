using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum BlockDirections
{
    //X axis orientations
    x1,
    x2,
    x3,
    x4,
    xn1,
    xn2,
    xn3,
    xn4,

    //y axis orientations
    y1,
    y2,
    y3,
    y4,
    yn1,
    yn2,
    yn3,
    yn4,

    //z axis orientations
    z1,//default orientation
    z2,
    z3,
    z4,
    zn1,
    zn2,
    zn3,
    zn4,
}

public enum CardinalDirection
{
    West,
    East,
    Up,
    Down,
    North,
    South
}

public class ChunkMeshGeneratorThread
{

    public float AtlasElementSize = 0.0625f;
    public float AtlasOffsetRow = 0.0625f;
    public float AtlasOffsetCol = 0.0625f;

    public bool _threadRunning;
    public Thread _thread;
    ShipChunkHandler handler;

    List<Chunk> chunkJobs;

    public void Init(ShipChunkHandler handler)
    {
        chunkJobs = new List<Chunk>();
        _thread = new Thread(ThreadedWork);
        this.handler = handler;
    }

    public void SetData(Chunk chunkUpdate)
    {
        chunkJobs.Add(chunkUpdate);
    }


    public void ThreadedWork()
    {
        bool workDone = false;
        _threadRunning = true;


        // This pattern lets us interrupt the work at a safe point if neeeded.
        while (_threadRunning && !workDone)
        {
            for (int i = 0; i < chunkJobs.Count; i++)
            {
                chunkJobs[i].meshData = GenerateMesh(chunkJobs[i]);
                chunkJobs[i].MeshUpdate = true;
                Thread.Sleep(5);
            }

            workDone = true;
        }
        handler.NotifyThreadFinished();

        _threadRunning = false;
    }

    void OnDisable()
    {
        // If the thread is still running, we should shut it down,
        // otherwise it can prevent the game from exiting correctly.
        if (_threadRunning)
        {
            // This forces the while loop in the ThreadedWork function to abort.
            _threadRunning = false;

            // This waits until the thread exits,
            // ensuring any cleanup we do after this is safe. 
            _thread.Join();
        }

        // Thread is guaranteed no longer running. Do other cleanup tasks.
    }


    private MeshData GenerateMesh(Chunk updatedChunk)
    {
        MeshData mesh = new MeshData();

        var generatedVertices = new List<Vector3>();
        var generatedTriangles = new List<int>();
        var generatedUV = new List<Vector2>();

        var generatedColors = new List<Color>();
        var generatedNormals = new List<Vector3>();

        int triIndex = 0;

        for (var x = 0; x < ChunkSettings.xSize; ++x)
        {
            for (var y = 0; y < ChunkSettings.ySize; ++y)
            {
                for (var z = 0; z < ChunkSettings.zSize; ++z)
                {
                    //updatedChunk.chunkInfo.blockTriIndex[x, y, z] = triIndex;
                    var block = CheckBlock(updatedChunk, x, y, z);
                    //Debug.Log("here: " + block.status);
                    if (block.status == BlockStatus.Occupied_Block)
                    {
                        // current Block is valid
                        var adjacentBlocks = new Block[] {
                            CheckBlock(updatedChunk, x-1, y, z),
                            CheckBlock(updatedChunk, x+1, y, z),
                            CheckBlock(updatedChunk, x, y-1, z),
                            CheckBlock(updatedChunk, x, y+1, z),
                            CheckBlock(updatedChunk, x, y, z-1),
                            CheckBlock(updatedChunk, x, y, z+1)
                        };


                        foreach (var adj in adjacentBlocks)
                        {
                            if (adj.status == BlockStatus.Empty)
                            {
                                triIndex += 2;
                            }
                        }

                        GenerateMeshForAdjacentBlocks(block, updatedChunk.chunkInfo.blockData[x, y, z].Type, updatedChunk.chunkInfo.blockData[x, y, z].Orientation, adjacentBlocks, generatedColors, generatedTriangles, generatedVertices, generatedNormals, generatedUV);
                    }


                }
            }
        }



        mesh.generatedVertices = generatedVertices;
        mesh.generatedTriangles = generatedTriangles;
        mesh.generatedUV = generatedUV;

        mesh.generatedColors = generatedColors;
        mesh.generatedNormals = generatedNormals;

        return mesh;
    }

    private void GenerateMeshForAdjacentBlocks(Block block, int blockType, ushort blockOrientation,Block[] adjacentBlocks, List<Color> generatedColors, List<int> generatedTriangles, List<Vector3> generatedVertices, List<Vector3> generatedNormals, List<Vector2> generatedUV)
    {
        foreach (var adj in adjacentBlocks)
        {

            if (adj.status == BlockStatus.Empty)
            {
                var triangles = new List<int>();

                Vector3 currentPosition = new Vector3(block.x, block.y, block.z);
                Vector3 adjPosition = new Vector3(adj.x, adj.y, adj.z);
                Vector3 diff = currentPosition - adjPosition;
                bool xDiff = block.x != adj.x;
                bool yDiff = block.y != adj.y;
                bool zDiff = block.z != adj.z;
                Vector3 u = Vector3.zero;
                Vector3 v = Vector3.zero;
                if (xDiff)
                {
                    u = Vector3.forward;
                    v = Vector3.up;
                }
                else if (yDiff)
                {
                    u = Vector3.right;
                    v = Vector3.forward;
                }
                else
                {
                    u = Vector3.right;
                    v = Vector3.up;
                }

                u *= 0.5f;
                v *= 0.5f;

                // Generate actual wall
                Vector3 quadPosition = (adjPosition + currentPosition) / 2.0f;
                int currentIndex = generatedVertices.Count;


                triangles.Add(currentIndex + 0);
                triangles.Add(currentIndex + 1);
                triangles.Add(currentIndex + 2);
                triangles.Add(currentIndex + 2);
                triangles.Add(currentIndex + 1);
                triangles.Add(currentIndex + 3);



                if (diff.x > 0 || diff.y > 0 || diff.z < 0)
                {
                    triangles.Reverse();
                }

                var normal = (adjPosition - currentPosition).normalized;

                GetVerticeOrientation((BlockDirections)blockOrientation, ConvertNormalToDirection(normal), generatedVertices, quadPosition, u, v);

                generatedNormals.Add(normal);
                generatedNormals.Add(normal);
                generatedNormals.Add(normal);
                generatedNormals.Add(normal);

                int ColOffset = blockType % 16;
                int RowOffset = blockType / 16;

                Vector2[] vector2s = new Vector2[4];
                vector2s = TextureMapController.GetBlockUV(blockType, (BlockDirections)blockOrientation, ConvertNormalToDirection(normal), vector2s);

                generatedUV.Add(vector2s[0]);
                generatedUV.Add(vector2s[1]);
                generatedUV.Add(vector2s[2]);
                generatedUV.Add(vector2s[3]);

                generatedTriangles.AddRange(triangles);

                generatedColors.Add(Color.black);
                generatedColors.Add(Color.black);
                generatedColors.Add(Color.black);
                generatedColors.Add(Color.black);

            }
        }
    }

    private void AddVertices_UpwardFacing(List<Vector3> generatedVertices, Vector3 quadPosition, Vector3 u, Vector3 v)
    {
        generatedVertices.Add(quadPosition - u - v);
        generatedVertices.Add(quadPosition - u + v);
        generatedVertices.Add(quadPosition + u - v);
        generatedVertices.Add(quadPosition + u + v);
    }

    private void AddVertices_DownwardFacing(List<Vector3> generatedVertices, Vector3 quadPosition, Vector3 u, Vector3 v)
    {
        generatedVertices.Add(quadPosition + u + v);
        generatedVertices.Add(quadPosition + u - v);
        generatedVertices.Add(quadPosition - u + v);
        generatedVertices.Add(quadPosition - u - v);
    }

    private void AddVertices_RightwardFacing(List<Vector3> generatedVertices, Vector3 quadPosition, Vector3 u, Vector3 v)
    {
        generatedVertices.Add(quadPosition - u + v);
        generatedVertices.Add(quadPosition + u + v);
        generatedVertices.Add(quadPosition - u - v);
        generatedVertices.Add(quadPosition + u - v);
    }

    private void AddVertices_LeftwardFacing(List<Vector3> generatedVertices, Vector3 quadPosition, Vector3 u, Vector3 v)
    {
        generatedVertices.Add(quadPosition + u - v);
        generatedVertices.Add(quadPosition - u - v);
        generatedVertices.Add(quadPosition + u + v);
        generatedVertices.Add(quadPosition - u + v);
    }

    private void GetVerticeOrientation(BlockDirections direction, CardinalDirection side, List<Vector3> generatedVertices, Vector3 quadPosition, Vector3 u, Vector3 v)
    {

        if (side == CardinalDirection.West)
        {
            switch (direction)
            {
                case BlockDirections.x1:AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.x2:AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.x3:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.x4:AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v);break;

                case BlockDirections.xn1:AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.xn2:AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.xn3:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.xn4:AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v);break;

                case BlockDirections.y1:AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.y2:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.y3:AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.y4:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;

                case BlockDirections.yn1: AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.yn2: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.yn3: AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.yn4: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;

                case BlockDirections.z1: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.z2: AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.z3: AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.z4: AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v); break;

                case BlockDirections.zn1: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.zn2: AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.zn3: AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.zn4: AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v); break;

                default:break;
            }

        }
        else if (side == CardinalDirection.East)
        {
            switch (direction)
            {
                case BlockDirections.x1:AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.x2:AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.x3:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.x4:AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v);break;

                case BlockDirections.xn1:AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.xn2:AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.xn3:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.xn4:AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v);break;

                case BlockDirections.y1:AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.y2:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.y3:AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.y4:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;

                case BlockDirections.yn1: AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.yn2: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.yn3: AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.yn4: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;

                case BlockDirections.z1: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.z2: AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.z3: AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.z4: AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v); break;

                case BlockDirections.zn1: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.zn2: AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.zn3: AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.zn4: AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v); break;

                default:break;
            }

        }
        else if (side == CardinalDirection.Up)
        {
            switch (direction)
            {
                case BlockDirections.x1:AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.x2: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.x3:AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.x4:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;

                case BlockDirections.xn1:AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.xn2:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.xn3:AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.xn4:AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v);break;

                case BlockDirections.y1:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.y2:AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.y3:AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.y4:AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v);break;

                case BlockDirections.yn1: AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.yn2: AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.yn3: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.yn4: AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v); break;

                case BlockDirections.z1: AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.z2: AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.z3: AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.z4: AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v); break;

                case BlockDirections.zn1: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.zn2: AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.zn3: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.zn4: AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v); break;

                default:break;
            }

        }
        else if (side == CardinalDirection.Down)
        {
            switch (direction)
            {
                case BlockDirections.x1:AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.x2: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.x3:AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.x4:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;

                case BlockDirections.xn1:AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.xn2:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.xn3:AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.xn4:AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v);break;

                case BlockDirections.y1:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.y2:AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.y3:AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.y4:AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v);break;

                case BlockDirections.yn1: AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.yn2: AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.yn3: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.yn4: AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v); break;

                case BlockDirections.z1: AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.z2: AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.z3: AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.z4: AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v); break;

                case BlockDirections.zn1: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.zn2: AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.zn3: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.zn4: AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v); break;

                default:break;
            }

        }
        else if (side == CardinalDirection.North)
        {
            switch (direction)
            {
                case BlockDirections.x1:AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.x2:AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.x3:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.x4:AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v);break;

                case BlockDirections.xn1:AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.xn2:AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.xn3:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.xn4:AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v);break;

                case BlockDirections.y1:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.y2:AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.y3:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.y4:AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v);break;

                case BlockDirections.yn1: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.yn2: AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.yn3: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.yn4: AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v); break;

                case BlockDirections.z1: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.z2: AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.z3: AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.z4: AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v); break;

                case BlockDirections.zn1: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.zn2: AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.zn3: AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.zn4: AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v); break;

                default:break;
            }

        }
        else if (side == CardinalDirection.South)
        {
            switch (direction)
            {
                case BlockDirections.x1:AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.x2:AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.x3:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.x4:AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v);break;

                case BlockDirections.xn1:AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.xn2:AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.xn3:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.xn4:AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v);break;

                case BlockDirections.y1:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.y2:AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.y3:AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v);break;
                case BlockDirections.y4:AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v);break;

                case BlockDirections.yn1: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.yn2: AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.yn3: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.yn4: AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v); break;

                case BlockDirections.z1: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.z2: AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.z3: AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.z4: AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v); break;

                case BlockDirections.zn1: AddVertices_UpwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.zn2: AddVertices_LeftwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.zn3: AddVertices_DownwardFacing(generatedVertices, quadPosition, u, v); break;
                case BlockDirections.zn4: AddVertices_RightwardFacing(generatedVertices, quadPosition, u, v); break;

                default:break;
            }

        }

        //Debug.Log(direction + " " + side);
    }

    private CardinalDirection ConvertNormalToDirection(Vector3 normal)
    {
        if (normal.x != 0)
        {
            if (normal.x == 1)
            {
                return CardinalDirection.West;
            }
            else
            {
                return CardinalDirection.East;
            }
        }
        else if(normal.y != 0)
        {
            if (normal.y == 1)
            {
                return CardinalDirection.Up;
            }
            else
            {
                return CardinalDirection.Down;
            }      
        }
        else if(normal.z != 0)
        {
            if (normal.z == 1)
            {
                return CardinalDirection.North;
            }
            else
            {
                return CardinalDirection.South;
            }
        }

        return CardinalDirection.Up;
    }

    private Block CheckBlock(Chunk updatedChunk, int x, int y, int z)
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

    #region Greedy Mesh
    /*
    public static Mesh ReduceMesh(Chunk chunk)
    {
        List vertices = new List();
        List elements = new List();
        List uvs = new List();
        List colours = new List();

        List noCollision = World.noCollision;

        int size = World.CHUNK_SIZE;

        //Sweep over 3-axes
        for (int d = 0; d < 3; d++)
        {

            int i, j, k, l, w, h, u = (d + 1) % 3, v = (d + 2) % 3;

            int[] x = new int[3];
            int[] q = new int[3];
            int[] mask = new int[(size + 1) * (size + 1)];

            q[d] = 1;

            for (x[d] = -1; x[d] < size;)
            {

                // Compute the mask
                int n = 0;
                for (x[v] = 0; x[v] < size; ++x[v])
                {
                    for (x[u] = 0; x[u] < size; ++x[u], ++n)
                    {


                        int a = 0; if (0 <= x[d]) { a = (int)World.GetBlock(chunk, x[0], x[1], x[2]).Type; if (noCollision.IndexOf(a) != -1) { a = 0; } }
                        int b = 0; if (x[d] < size - 1) { b = (int)World.GetBlock(chunk, x[0] + q[0], x[1] + q[1], x[2] + q[2]).Type; if (noCollision.IndexOf(b) != -1) { b = 0; } }
                        if (a != -1 && b != -1 && a == b) { mask[n] = 0; }
                        else if (a > 0)
                        {
                            a = 1;
                            mask[n] = a;
                        }

                        else
                        {
                            b = 1;
                            mask[n] = -b;
                        }

                    }


                }

                // Increment x[d]
                ++x[d];

                // Generate mesh for mask using lexicographic ordering
                n = 0;
                for (j = 0; j < size; ++j)
                {
                    for (i = 0; i < size;)
                    {
                        var c = mask[n]; if (c > -3)
                        {
                            // Compute width
                            for (w = 1; c == mask[n + w] && i + w < size; ++w) { }

                            // Compute height
                            bool done = false;
                            for (h = 1; j + h < size; ++h)
                            {
                                for (k = 0; k < w; ++k) { if (c != mask[n + k + h * size]) { done = true; break; } }
                                if (done) break;
                            }                             // Add quad                             bool flip = false;                             x[u] = i;                             x[v] = j;                             int[] du = new int[3];                             int[] dv = new int[3];                             if (c > -1)
                            {
                                du[u] = w;
                                dv[v] = h;
                            }
                            else
                            {
                                flip = true;
                                c = -c;
                                du[u] = w;
                                dv[v] = h;
                            }


                            Vector3 v1 = new Vector3(x[0], x[1], x[2]);
                            Vector3 v2 = new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]);
                            Vector3 v3 = new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]);
                            Vector3 v4 = new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]);

                            if (c > 0 && !flip)
                            {
                                AddFace(v1, v2, v3, v4, vertices, elements, 0);
                            }

                            if (flip)
                            {
                                AddFace(v4, v3, v2, v1, vertices, elements, 0);
                            }

                            // Zero-out mask
                            for (l = 0; l < h; ++l)
                                for (k = 0; k < w; ++k)
                                {
                                    mask[n + k + l * size] = 0;
                                }

                            // Increment counters and continue
                            i += w; n += w;
                        }

                        else
                        {
                            ++i;
                            ++n;
                        }
                    }
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = elements.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();


        return mesh;

    }

    private static void AddFace(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, List vertices, List elements, int order)
    {
        if (order == 0)
        {
            int index = vertices.Count;

            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            vertices.Add(v4);

            elements.Add(index);
            elements.Add(index + 1);
            elements.Add(index + 2);
            elements.Add(index + 2);
            elements.Add(index + 3);
            elements.Add(index);

        }

        if (order == 1)
        {
            int index = vertices.Count;

            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            vertices.Add(v4);

            elements.Add(index);
            elements.Add(index + 3);
            elements.Add(index + 2);
            elements.Add(index + 2);
            elements.Add(index + 1);
            elements.Add(index);

        }
        public static Mesh ReduceMesh(Chunk chunk)
        {
            List vertices = new List();
            List elements = new List();
            List uvs = new List();
            List colours = new List();

            List noCollision = World.noCollision;

            int size = World.CHUNK_SIZE;

            //Sweep over 3-axes
            for (int d = 0; d < 3; d++)
            {

                int i, j, k, l, w, h, u = (d + 1) % 3, v = (d + 2) % 3;

                int[] x = new int[3];
                int[] q = new int[3];
                int[] mask = new int[(size + 1) * (size + 1)];

                q[d] = 1;

                for (x[d] = -1; x[d] < size;)
                {

                    // Compute the mask
                    int n = 0;
                    for (x[v] = 0; x[v] < size; ++x[v])
                    {
                        for (x[u] = 0; x[u] < size; ++x[u], ++n)
                        {


                            int a = 0; if (0 <= x[d]) { a = (int)World.GetBlock(chunk, x[0], x[1], x[2]).Type; if (noCollision.IndexOf(a) != -1) { a = 0; } }
                            int b = 0; if (x[d] < size - 1) { b = (int)World.GetBlock(chunk, x[0] + q[0], x[1] + q[1], x[2] + q[2]).Type; if (noCollision.IndexOf(b) != -1) { b = 0; } }
                            if (a != -1 && b != -1 && a == b) { mask[n] = 0; }
                            else if (a > 0)
                            {
                                a = 1;
                                mask[n] = a;
                            }

                            else
                            {
                                b = 1;
                                mask[n] = -b;
                            }

                        }


                    }

                    // Increment x[d]
                    ++x[d];

                    // Generate mesh for mask using lexicographic ordering
                    n = 0;
                    for (j = 0; j < size; ++j)
                    {
                        for (i = 0; i < size;)
                        {
                            var c = mask[n]; if (c > -3)
                            {
                                // Compute width
                                for (w = 1; c == mask[n + w] && i + w < size; ++w) { }

                                // Compute height
                                bool done = false;
                                for (h = 1; j + h < size; ++h)
                                {
                                    for (k = 0; k < w; ++k) { if (c != mask[n + k + h * size]) { done = true; break; } }
                                    if (done) break;
                                }                             // Add quad                             bool flip = false;                             x[u] = i;                             x[v] = j;                             int[] du = new int[3];                             int[] dv = new int[3];                             if (c > -1)
                                {
                                    du[u] = w;
                                    dv[v] = h;
                                }
                            else
                            {
                                    flip = true;
                                    c = -c;
                                    du[u] = w;
                                    dv[v] = h;
                                }


                                Vector3 v1 = new Vector3(x[0], x[1], x[2]);
                                Vector3 v2 = new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]);
                                Vector3 v3 = new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]);
                                Vector3 v4 = new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]);

                                if (c > 0 && !flip)
                                {
                                    AddFace(v1, v2, v3, v4, vertices, elements, 0);
                                }

                                if (flip)
                                {
                                    AddFace(v4, v3, v2, v1, vertices, elements, 0);
                                }

                                // Zero-out mask
                                for (l = 0; l < h; ++l)
                                    for (k = 0; k < w; ++k)
                                    {
                                        mask[n + k + l * size] = 0;
                                    }

                                // Increment counters and continue
                                i += w; n += w;
                            }

                            else
                            {
                                ++i;
                                ++n;
                            }
                        }
                    }
                }
            }

            Mesh mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = elements.ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();


            return mesh;

        }

        private static void AddFace(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, List vertices, List elements, int order)
        {
            if (order == 0)
            {
                int index = vertices.Count;

                vertices.Add(v1);
                vertices.Add(v2);
                vertices.Add(v3);
                vertices.Add(v4);

                elements.Add(index);
                elements.Add(index + 1);
                elements.Add(index + 2);
                elements.Add(index + 2);
                elements.Add(index + 3);
                elements.Add(index);

            }

            if (order == 1)
            {
                int index = vertices.Count;

                vertices.Add(v1);
                vertices.Add(v2);
                vertices.Add(v3);
                vertices.Add(v4);

                elements.Add(index);
                elements.Add(index + 3);
                elements.Add(index + 2);
                elements.Add(index + 2);
                elements.Add(index + 1);
                elements.Add(index);

            }



        }
    }
    */
    #endregion
}
