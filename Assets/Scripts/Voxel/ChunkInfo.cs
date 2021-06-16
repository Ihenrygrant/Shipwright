using System;
using UnityEngine;
using UnityEngine.Events;

/*
 * chunkBlockStatus
 * 0 - Empty Space
 * 1 - Occupied by a block
 * 2 - Occupied by a destroyed block
 * 3 - Occupied by an object
 */

public enum BlockStatus
{
    Empty,
    Occupied_Block,
    Occupied_DestroyedBlock,
    Occupied_Object
}

public static class ChunkSettings
{
    public static ushort xSize = 16;
    public static ushort ySize = 16;
    public static ushort zSize = 16;
}

[Serializable]
public class ChunkData
{
    public ushort[] chunkIndex;
    public BlockData[,,] blockData;
}

public class BlockData
{
    public byte Type;
    public byte Health;
    public byte Orientation;
    public byte Status;
}