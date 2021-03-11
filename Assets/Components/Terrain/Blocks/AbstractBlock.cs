using System.Collections;
using System.Collections.Generic;
using Antymology.Terrain;
using UnityEngine;

namespace Components.Terrain.Blocks
{
    public enum BlockType
    {
        Acidic,
        Air,
        Container,
        Grass,
        Mulch,
        Nest,
        Stone
    };

    public static class BlockHelper
    {
        public static BlockType GetBlockType(AbstractBlock block)
        {
            if (block.GetType() == typeof(AcidicBlock))
                return BlockType.Acidic;

            if (block.GetType() == typeof(AirBlock))
                return BlockType.Air;

            if (block.GetType() == typeof(ContainerBlock))
                return BlockType.Container;

            if (block.GetType() == typeof(GrassBlock))
                return BlockType.Grass;

            if (block.GetType() == typeof(MulchBlock))
                return BlockType.Mulch;

            if (block.GetType() == typeof(NestBlock))
                return BlockType.Nest;

            if (block.GetType() == typeof(StoneBlock))
                return BlockType.Stone;

            return BlockType.Air;
        }
    }
    

    public abstract class AbstractBlock
    {

        /// <summary>
        /// The texture map coordinates of this block.
        /// </summary>
        public abstract Vector2 tileMapCoordinate();

        /// <summary>
        /// If the block is visible or not.
        /// </summary>
        public abstract bool isVisible();

        /// <summary>
        /// The woorld x coordinate of this block.
        /// </summary>
        public int worldXCoordinate;

        /// <summary>
        /// The world y coordinate of this block.
        /// </summary>
        public int worldYCoordinate;

        /// <summary>
        /// The world z coordinate of this block.
        /// </summary>
        public int worldZCoordinate;
    }
}