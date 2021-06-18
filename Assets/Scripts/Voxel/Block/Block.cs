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

public class Block
{
    public byte Type;
    public byte Health;
    public byte Orientation;
    public byte Status;
}