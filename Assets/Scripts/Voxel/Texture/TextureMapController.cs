using System.Collections.Generic;
using UnityEngine;

public static class TextureMapController {

    static float AtlasElementSize = 0.0625f;
    static float AtlasOffsetRow = 0.0625f;
    static float AtlasOffsetCol = 0.0625f;

    static int PackPixelSize = 64;

    public static Sprite[] sprites;

    //static Dictionary<string, int> BlockIDs;
    public static List<string> BlockIds = new List<string>();
    public static Dictionary<string, Vector2[]> AtlasUVList;
    public static Dictionary<string, Dictionary<string, Vector2[]>> BlockUVDictionary;


    public static void Initialize()
    {
        sprites = Resources.LoadAll<Sprite>("Atlas16x16");
        AtlasUVList = new Dictionary<string, Vector2[]>();
        BlockUVDictionary = new Dictionary<string, Dictionary<string, Vector2[]>>();

        for (int i = 0; i < sprites.Length; i++)
        {
            Vector2[] uvs = new Vector2[4];

            int ColOffset = (int)(sprites[i].rect.x / PackPixelSize);
            int RowOffset = (int)(sprites[i].rect.y / PackPixelSize);

            uvs[0] = new Vector2(0 + AtlasOffsetCol * ColOffset, 0 + AtlasOffsetRow * RowOffset);
            uvs[1] = new Vector2(0 + AtlasOffsetCol * ColOffset, AtlasElementSize + AtlasOffsetRow * RowOffset);
            uvs[2] = new Vector2(AtlasElementSize + AtlasOffsetCol * ColOffset, 0 + AtlasOffsetRow * RowOffset);
            uvs[3] = new Vector2(AtlasElementSize + AtlasOffsetCol * ColOffset, AtlasElementSize + AtlasOffsetRow * RowOffset);

            AtlasUVList.Add(sprites[i].name, uvs);
        }

        BlockUVDictionary.Add("Dirt", CreateBlockDictionary("Dirt", "Dirt", "Dirt", "Dirt", "Dirt", "Dirt"));
        BlockUVDictionary.Add("Grass", CreateBlockDictionary("Grass_Side", "Grass_Side", "Grass_Top", "Dirt", "Grass_Side", "Grass_Side"));
        BlockUVDictionary.Add("Bedrock", CreateBlockDictionary("Bedrock", "Bedrock", "Bedrock", "Bedrock", "Bedrock", "Bedrock"));
        BlockUVDictionary.Add("Bookshelf", CreateBlockDictionary("Bookshelf", "Bookshelf", "Plank", "Plank", "Bookshelf", "Bookshelf"));
        BlockUVDictionary.Add("Brick", CreateBlockDictionary("Brick", "Brick", "Brick", "Brick", "Brick", "Brick"));
        BlockUVDictionary.Add("Clay", CreateBlockDictionary("Clay", "Clay", "Clay", "Clay", "Clay", "Clay"));
        BlockUVDictionary.Add("Coal", CreateBlockDictionary("Coal", "Coal", "Coal", "Coal", "Coal", "Coal"));
        BlockUVDictionary.Add("Coal_ore", CreateBlockDictionary("Coal_ore", "Coal_ore", "Coal_ore", "Coal_ore", "Coal_ore", "Coal_ore"));
        BlockUVDictionary.Add("Cobblestone", CreateBlockDictionary("Cobblestone", "Cobblestone", "Cobblestone", "Cobblestone", "Cobblestone", "Cobblestone"));
        BlockUVDictionary.Add("Mossy_Cobblestone", CreateBlockDictionary("Mossy_Cobblestone", "Mossy_Cobblestone", "Mossy_Cobblestone", "Mossy_Cobblestone", "Mossy_Cobblestone", "Mossy_Cobblestone"));
        BlockUVDictionary.Add("Diamond_Block", CreateBlockDictionary("Diamond_Block", "Diamond_Block", "Diamond_Block", "Diamond_Block", "Diamond_Block", "Diamond_Block"));
        BlockUVDictionary.Add("Emerald_Block", CreateBlockDictionary("Emerald_Block", "Emerald_Block", "Emerald_Block", "Emerald_Block", "Emerald_Block", "Emerald_Block"));
        BlockUVDictionary.Add("Endstone", CreateBlockDictionary("Endstone", "Endstone", "Endstone", "Endstone", "Endstone", "Endstone"));
        BlockUVDictionary.Add("Farmland_Dry", CreateBlockDictionary("Dirt", "Dirt", "Farmland_Dry", "Farmland_Dry", "Dirt", "Dirt"));
        BlockUVDictionary.Add("Gravel", CreateBlockDictionary("Gravel", "Gravel", "Gravel", "Gravel", "Gravel", "Gravel"));
        BlockUVDictionary.Add("HardClay", CreateBlockDictionary("HardClay", "HardClay", "HardClay", "HardClay", "HardClay", "HardClay"));
        BlockUVDictionary.Add("IronPlate", CreateBlockDictionary("IronPlate", "IronPlate", "IronPlate", "IronPlate", "IronPlate", "IronPlate"));
        BlockUVDictionary.Add("Log", CreateBlockDictionary("Log_side", "Log_side", "Log_top", "Log_top", "Log_side", "Log_side"));
        BlockUVDictionary.Add("Planks", CreateBlockDictionary("Plank", "Plank", "Plank", "Plank", "Plank", "Plank"));
        BlockUVDictionary.Add("Stone", CreateBlockDictionary("Stone", "Stone", "Stone", "Stone", "Stone", "Stone"));
        BlockUVDictionary.Add("StoneBricks", CreateBlockDictionary("StoneBricks", "StoneBricks", "StoneBricks", "StoneBricks", "StoneBricks", "StoneBricks"));
        BlockUVDictionary.Add("RustedPlate_side", CreateBlockDictionary("RustedPlate_side", "RustedPlate_side", "RustedPlate_top", "RustedPlate_top", "RustedPlate_side", "RustedPlate_side"));
        BlockUVDictionary.Add("SteelPlate", CreateBlockDictionary("SteelPlate", "SteelPlate", "SteelPlate", "SteelPlate", "SteelPlate", "SteelPlate"));
        BlockUVDictionary.Add("RustedMetal", CreateBlockDictionary("RustedMetal", "RustedMetal", "RustedMetal", "RustedMetal", "RustedMetal", "RustedMetal"));




        BlockUVDictionary.Add("Furnace_Off", CreateBlockDictionary("Furnace_Side", "Furnace_Side", "TNT_side", "TNT_side", "Furnace_Front_Off", "Furnace_Side"));

        foreach (KeyValuePair<string, Dictionary<string, Vector2[]>> blockEntry in BlockUVDictionary)
        {
            BlockIds.Add(blockEntry.Key);
        }
    }

    static Dictionary<string, Vector2[]> CreateBlockDictionary(string WestID, string EastID, string UpID, string DownID, string NorthID, string SouthID)
    {
        Dictionary<string, Vector2[]> newEntry = new Dictionary<string, Vector2[]>();
        newEntry.Add("West", AtlasUVList[WestID]);
        newEntry.Add("East", AtlasUVList[EastID]);
        newEntry.Add("Up", AtlasUVList[UpID]);
        newEntry.Add("Down", AtlasUVList[DownID]);
        newEntry.Add("North", AtlasUVList[NorthID]);
        newEntry.Add("South", AtlasUVList[SouthID]);

        return newEntry;
    }

    public static Vector2[] GetBlockUV(int blockType, Orentations direction, CardinalDirection side, Vector2[] uvs)
    {
        Vector2[] vector2s = new Vector2[4];

        //TODO this code can probably be made smaller, it has alot of cases.

        if (side == CardinalDirection.West)
        {
            switch (direction)
            {
                case Orentations.x1:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;
                case Orentations.x2:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;
                case Orentations.x3:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;
                case Orentations.x4:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;

                case Orentations.xn1:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;
                case Orentations.xn2:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;
                case Orentations.xn3:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;
                case Orentations.xn4:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;

                case Orentations.y1:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;
                case Orentations.y2:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;
                case Orentations.y3:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;
                case Orentations.y4:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;

                case Orentations.yn1:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;
                case Orentations.yn2:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;
                case Orentations.yn3:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;
                case Orentations.yn4:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;

                case Orentations.z1:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;
                case Orentations.z2:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;
                case Orentations.z3:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;
                case Orentations.z4:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;

                case Orentations.zn1:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;
                case Orentations.zn2:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;
                case Orentations.zn3:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;
                case Orentations.zn4:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;

                default:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;
            }
        }
        else if (side == CardinalDirection.East)
        {
            switch (direction)
            {
                case Orentations.x1:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;
                case Orentations.x2:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;
                case Orentations.x3:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;
                case Orentations.x4:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;

                case Orentations.xn1:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;
                case Orentations.xn2:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;
                case Orentations.xn3:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;
                case Orentations.xn4:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;

                case Orentations.y1:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;
                case Orentations.y2:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;
                case Orentations.y3:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;
                case Orentations.y4:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;

                case Orentations.yn1:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;
                case Orentations.yn2:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;
                case Orentations.yn3:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;
                case Orentations.yn4:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;

                case Orentations.z1:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;
                case Orentations.z2:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;
                case Orentations.z3:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;
                case Orentations.z4:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;

                case Orentations.zn1:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;
                case Orentations.zn2:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;
                case Orentations.zn3:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;
                case Orentations.zn4:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;

                default:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;
            }
        }
        else if (side == CardinalDirection.Up)
        {
            switch (direction)
            {
                case Orentations.x1:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;
                case Orentations.x2:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;
                case Orentations.x3:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;
                case Orentations.x4:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;

                case Orentations.xn1:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;
                case Orentations.xn2:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;
                case Orentations.xn3:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;
                case Orentations.xn4:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;

                case Orentations.y1:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;
                case Orentations.y2:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;
                case Orentations.y3:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;
                case Orentations.y4:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;

                case Orentations.yn1:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;
                case Orentations.yn2:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;
                case Orentations.yn3:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;
                case Orentations.yn4:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;

                case Orentations.z1:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;
                case Orentations.z2:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;
                case Orentations.z3:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;
                case Orentations.z4:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;

                case Orentations.zn1:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;
                case Orentations.zn2:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;
                case Orentations.zn3:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;
                case Orentations.zn4:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;

                default:vector2s = BlockUVDictionary[BlockIds[blockType]][side.ToString()];break;
            }
        }
        else if (side == CardinalDirection.Down)
        {
                switch (direction)
                {
                    case Orentations.x1:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;
                    case Orentations.x2:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;
                    case Orentations.x3:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;
                    case Orentations.x4:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;

                    case Orentations.xn1:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;
                    case Orentations.xn2:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;
                    case Orentations.xn3:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;
                    case Orentations.xn4:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;


                    case Orentations.y1:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;
                    case Orentations.y2:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;
                    case Orentations.y3:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;
                    case Orentations.y4:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;

                    case Orentations.yn1:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;
                    case Orentations.yn2:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;
                    case Orentations.yn3:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;
                    case Orentations.yn4:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;

                    case Orentations.z1:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;
                    case Orentations.z2:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;
                    case Orentations.z3:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;
                    case Orentations.z4:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;

                    case Orentations.zn1:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;
                    case Orentations.zn2:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;
                    case Orentations.zn3:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;
                    case Orentations.zn4:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;

                    default:vector2s = BlockUVDictionary[BlockIds[blockType]][side.ToString()];break;
                }
            }
        else if (side == CardinalDirection.North)
        {
            switch (direction)
            {
                case Orentations.x1:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;
                case Orentations.x2:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;
                case Orentations.x3:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;
                case Orentations.x4:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;

                case Orentations.xn1:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;
                case Orentations.xn2:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;
                case Orentations.xn3:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;
                case Orentations.xn4:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;

                case Orentations.y1:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;
                case Orentations.y2:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;
                case Orentations.y3:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;
                case Orentations.y4:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;

                case Orentations.yn1:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;
                case Orentations.yn2:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;
                case Orentations.yn3:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;
                case Orentations.yn4:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;

                case Orentations.z1:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;
                case Orentations.z2:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;
                case Orentations.z3:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;
                case Orentations.z4:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;

                case Orentations.zn1:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;
                case Orentations.zn2:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;
                case Orentations.zn3:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;
                case Orentations.zn4:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;

                default:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;
            }
        }
        else if (side == CardinalDirection.South)
        {
            switch (direction)
            {
                case Orentations.x1:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;
                case Orentations.x2:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;
                case Orentations.x3:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;
                case Orentations.x4:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;

                case Orentations.xn1:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;
                case Orentations.xn2:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;
                case Orentations.xn3:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;
                case Orentations.xn4:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;

                case Orentations.y1:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;
                case Orentations.y2:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;
                case Orentations.y3:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;
                case Orentations.y4:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;

                case Orentations.yn1:vector2s = BlockUVDictionary[BlockIds[blockType]]["Up"];break;
                case Orentations.yn2:vector2s = BlockUVDictionary[BlockIds[blockType]]["West"];break;
                case Orentations.yn3:vector2s = BlockUVDictionary[BlockIds[blockType]]["Down"];break;
                case Orentations.yn4:vector2s = BlockUVDictionary[BlockIds[blockType]]["East"];break;

                case Orentations.z1:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;
                case Orentations.z2:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;
                case Orentations.z3:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;
                case Orentations.z4:vector2s = BlockUVDictionary[BlockIds[blockType]]["South"];break;

                case Orentations.zn1:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;
                case Orentations.zn2:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;
                case Orentations.zn3:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;
                case Orentations.zn4:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;

                default:vector2s = BlockUVDictionary[BlockIds[blockType]]["North"];break;
            }
        }

        return vector2s;
    }

}
