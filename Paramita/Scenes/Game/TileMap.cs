﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

// need to continue getting this working - merging RogueSharp Map
// and the TileLayer class together.

namespace Paramita.Scenes
{
    public class TileMap
    {
        [ContentSerializer(CollectionItemName = "Tiles")]
        private Tile[,] tiles;
        
        Point cameraPoint;
        Point viewPoint;
        Point min;
        Point max;
        Rectangle destination;

        
        [ContentSerializer]
        public string MapName { get; private set; }

        [ContentSerializer]
        public TileSet TileSet { get; set; }

        // Number of tiles wide and high
        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }

        // Size of map in pixels
        public int WidthInPixels
        {
            get { return MapWidth * TileEngine.TileWidth; }
        }
        public int HeightInPixels
        {
            get { return MapHeight * TileEngine.TileHeight; }
        }





        public TileMap(TileSet tileSet, int width, int height, string name)
        {
            MapWidth = width;
            MapHeight = height;
            TileSet = tileSet;
            MapName = name;
            tiles = PopulateTiles(MapWidth, MapHeight);
        }




        private Tile[,] PopulateTiles(int cols, int rows)
        {
            Tile[,] tiles = new Tile[cols, rows];
            for(int x = 0; x < cols; x++)
            {
                for(int y = 0; y < rows; y++ )
                {
                    tiles[x, y] = new Tile(x, y, TileType.Floor, true, true);
                }
            }

            return tiles;
        }

        public void SetTile(int x, int y, Tile newTile)
        {
            tiles[x, y] = newTile;
        }


        public Tile GetTile(int x, int y)
        {
            return tiles[x, y];
        }


        


        public void Update(GameTime gameTime)
        {
            // implement in future if the map needs to change its tiles dynamically
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            cameraPoint = TileEngine.PixelsToTileCoords(camera.Position);
            viewPoint = TileEngine.PixelsToTileCoords(
                new Vector2(
                    (camera.Position.X + TileEngine.ViewportRectangle.Width),
                    (camera.Position.Y + TileEngine.ViewportRectangle.Height)
                )
            );

            min.X = Math.Max(0, cameraPoint.X - 1);
            min.Y = Math.Max(0, cameraPoint.Y - 1);
            max.X = Math.Min(viewPoint.X + 1, MapWidth);
            max.Y = Math.Min(viewPoint.Y + 1, MapHeight);
            destination = new Rectangle(0, 0, TileEngine.TileWidth, TileEngine.TileHeight);
            Tile tile;

            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null, null, null,
                camera.Transformation);

            for (int y = min.Y; y < max.Y; y++)
            {
                destination.Y = y * TileEngine.TileHeight;
                for (int x = min.X; x < max.X; x++)
                {
                    tile = GetTile(x, y);
                    destination.X = x * TileEngine.TileWidth;
                    spriteBatch.Draw(
                    TileSet.Texture,
                    destination,
                    TileSet.GetRectForTileType(tile.TileType),
                    Color.White);
                }
            }
            spriteBatch.End();
        }
    }
}
