﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Paramita.GameLogic.Items;
using System.Collections.Generic;

namespace Paramita.GameLogic.Levels
{
    public class TileMap
    {
        #region Fields
        private Tile[,] _tiles;
        private int _tilesWide;
        private int _tilesHigh;
        private string _mapName;
        private const string _unsupportedTileType = "A TileType was supplied that is not supported for this search.";
        #endregion


        #region Events
        public event EventHandler<ItemEventArgs> OnItemAdded;
        public event EventHandler<ItemEventArgs> OnItemRemoved;
        #endregion


        public TileMap(Tile[,] tiles, string name)
        {
            _tiles = tiles;
            _tilesWide = tiles.GetLength(0);
            _tilesHigh = tiles.GetLength(1);
            MapName = name;
            SubscribeToTileEvents();
        }

        #region Properties
        public int TilesWide
        {
            get { return _tilesWide; }
        }

        public int TilesHigh
        {
            get { return _tilesHigh; }
        }

        public string MapName
        {
            get { return _mapName; }
            private set { _mapName = value; }
        }
        #endregion

        private void SubscribeToTileEvents()
        {
            foreach(var tile in _tiles)
            {
                tile.OnItemAddedToTile += HandleItemAddedEvent;
                tile.OnItemRemovedFromTile += HandleItemRemovedEvent;
            }
        }


        #region Event Handlers
        private void HandleItemAddedEvent(object sender, ItemEventArgs e)
        {
            OnItemAdded?.Invoke(this, e);
        }

        private void HandleItemRemovedEvent(object sender, ItemEventArgs e)
        {
            OnItemRemoved?.Invoke(this, e);
        }
        #endregion


        #region TileType and ItemType Layer API
        public TileType[,] ConvertMapToTileTypes()
        {
            var typeArray = new TileType[_tilesHigh, _tilesWide];
            for(int i = 0; i < _tiles.GetLength(0); i++)
            {
                for(int j = 0; j < _tiles.GetLength(1); j++)
                {
                    typeArray[i, j] = _tiles[i, j].TileType;
                }
            }
            return typeArray;
        }


        public ItemType[,] ConvertMapToItemTypes()
        {
            var typeArray = new ItemType[_tilesHigh, _tilesWide];
            for (int i = 0; i < _tiles.GetLength(0); i++)
            {
                for (int j = 0; j < _tiles.GetLength(1); j++)
                {
                    var items = _tiles[i, j].InspectItems();
                    if(items.Length > 0)
                    {
                        typeArray[i, j] = items[0].ItemType;
                    }
                    else
                    {
                        typeArray[i, j] = ItemType.None;
                    }
                }
            }
            return typeArray;
        }
        #endregion


        #region Tile Getter and Setter API
        public void SetTile(Tile newTile)
        {
            if(newTile == null)
                throw new NullReferenceException();

            _tiles[newTile.TilePoint.X, newTile.TilePoint.Y] = newTile;
        }


        // Returns null if coord is outside the bounds of tiles[,]
        public Tile GetTile(Point point)
        {
            PointOutsideOfTileMapCheck(point);
            return _tiles[point.X, point.Y];
        }

        public Tile GetRandomWalkableTile()
        {
            var walkableTiles = GetWalkableTiles();

            int i = Dungeon._random.Next(walkableTiles.Count - 1);
                
            return walkableTiles[i];
        }

        private List<Tile> GetWalkableTiles()
        {
            var walkableTiles = new List<Tile>();

            for(int i = 0; i < _tilesWide; i++)
            {
                for (int j = 0; j < _tilesHigh; j++)
                {
                    var tile = GetTile(new Point(i, j));
                    if (tile.IsWalkable)
                        walkableTiles.Add(tile);
                }
            }
            if (walkableTiles.Count == 0)
                throw new NullReferenceException("No walkable tiles found.");

            return walkableTiles;

        }

        public void PlaceItemsOnRandomTiles(List<Item> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var tile = GetRandomWalkableTile();
                tile.AddItem(items[i]);
            }
        }
        #endregion


        // return the value of IsWalkable property on Tile at (x,y) on the map
        public bool IsTileWalkable(Point point)
        {
            PointOutsideOfTileMapCheck(point);
            return _tiles[point.X, point.Y].IsWalkable;
        }
        
        private void PointOutsideOfTileMapCheck(Point point)
        {
            if (point.X < 0 || point.X > TilesWide - 1
                || point.Y < 0 || point.Y > TilesHigh - 1)
            {
                throw new ArgumentOutOfRangeException("Point is outside of TileMap bounds.");
            }
        }

        public Tile FindTileType(TileType type)
        {
            switch(type)
            {
                case TileType.StairsUp:
                    return FindTileByType(TileType.StairsUp);
                case TileType.StairsDown:
                    return FindTileByType(TileType.StairsDown);
                default:
                    throw new NotImplementedException("Finding TileType." + type + " not implemented.");
            }
        }

        // conducts a linear search of @tiles and returns the first match encountered
        private Tile FindTileByType(TileType type)
        {
            for(int x = 0; x < TilesWide; x++)
            {
                for(int y = 0; y < TilesHigh; y++)
                {
                    if (_tiles[x, y].TileType == type)
                        return _tiles[x, y];
                }
            }
            throw new NullReferenceException("No matching tile found.");
        }
    }
}
