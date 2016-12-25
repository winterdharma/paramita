﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Paramita.Items;
using Paramita.Mechanics;
using Paramita.Scenes;
using System.Collections.Generic;

namespace Paramita.SentientBeings
{
    public class Player : SentientBeing, IContainer
    {
        // private fields
        private GameScene gameScene;
        private InputDevices inputDevices;
        private string name;
        private bool gender;
        private AnimatedSprite sprite;
        private Texture2D texture;
        private Item[] inventory;
        Dictionary<AnimationKey, Animation> animations = new Dictionary<AnimationKey, Animation>();

        // public properties
        public Vector2 Position
        {
            get { return sprite.Position; }
            set { sprite.Position = value; }
        }

        public Tile CurrentTile { get; set; }

        public AnimatedSprite Sprite
        {
            get { return sprite; }
        }

        public Dictionary<AnimationKey, Animation> PlayerAnimations
        {
            get { return animations; }
        }



        public Player(GameController game, string name, bool gender, Texture2D texture) : base(game)
        {
            gameScene = game.GameScene;
            inputDevices = game.InputDevices;
            this.name = name;
            this.gender = gender;
            this.texture = texture;

            inventory = new Item[10];
        }



        // Will be used when saved games are implemented
        public void SavePlayer() { }




        public override void Initialize()
        {
            Animation animation = new Animation(3, 32, 32, 0, 0);
            animations.Add(AnimationKey.WalkDown, animation);

            animation = new Animation(3, 32, 32, 0, 32);
            animations.Add(AnimationKey.WalkLeft, animation);

            animation = new Animation(3, 32, 32, 0, 64);
            animations.Add(AnimationKey.WalkRight, animation);

            animation = new Animation(3, 32, 32, 0, 96);
            animations.Add(AnimationKey.WalkUp, animation);

            sprite = new AnimatedSprite(texture, PlayerAnimations);
            sprite.CurrentAnimation = AnimationKey.WalkDown;
            sprite.IsAnimating = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            HandleInput();
            Position = gameScene.Map.GetTilePosition(CurrentTile);
            Sprite.Update(gameTime);
            base.Update(gameTime);
        }


        // This method polls InputDevices for player input and responds
        private void HandleInput()
        {
            // check if item was dropped
            if(inputDevices.DroppedItem() == true)
                DropItem();

            // check for movement input and store the direction returned
            Facing = inputDevices.MovedTo();
            
            // if the player attempted to move, update Player.CurrentAnimation
            if(Facing != Compass.None)
            {
                // update the player's Sprite
                SetCurrentAnimation(Facing);
                // get the tile player is attempting to move to
                Tile newTile = gameScene.Map.GetTile(
                    CurrentTile.TilePoint + Direction.GetPoint(Facing));
                if(newTile != null && newTile.IsWalkable == true)
                {
                    CurrentTile = newTile;
                    CheckForNewTileEvents();
                }
            }
        }



       /* 
        * Sets Player.CurrentAnimation to something appropriate for the Compass direction passed to it
        * Currently, the player animations are in four directions, but movement can be
        * in eight directions.
        */
        private void SetCurrentAnimation(Compass facing)
        {
            // set the animation according to the new facing
            if (Facing == Compass.North)
                Sprite.CurrentAnimation = AnimationKey.WalkUp;
            else if (Facing == Compass.South)
                Sprite.CurrentAnimation = AnimationKey.WalkDown;
            else if (Facing == Compass.Northeast || Facing == Compass.East || Facing == Compass.Southeast)
                Sprite.CurrentAnimation = AnimationKey.WalkRight;
            else if (Facing == Compass.Northwest || Facing == Compass.West || Facing == Compass.Southwest)
                Sprite.CurrentAnimation = AnimationKey.WalkLeft;
        }



       /*  
        *  This method is called immediately when CurrentTile is changed in Update() to check for
        *  any events that are triggered by the new tile, such as picking up items, changing levels, etc.
        */
        private void CheckForNewTileEvents()
        {
            // check for items in the new tile and pick the first item present
            if (CurrentTile.InspectItems().Length > 0)
                TryToPickUpItem();
            // check for events based on moving to TileTypes like StairsUp & StairsDown
            CheckForTileTypeEvent(CurrentTile.TileType);
        }

        // checks to see if there is an item to pick up and does so if one is there.
        // used when player moves to a new tile.
        private void TryToPickUpItem()
        {
            Item[] items = CurrentTile.InspectItems();
            if(AddItem(items[0]) == true)
            {
                CurrentTile.RemoveItem(items[0]);
                gameScene.Statuses.AddMessage("You picked up a " + items[0].ToString() + ".");
            }
        }


        // Checks the TileType of Player.CurrentTile for any events that when moving to them
        private void CheckForTileTypeEvent(TileType tileType)
        {
            switch(tileType)
            {
                case TileType.StairsUp:
                    if(gameScene.LevelNumber > 1)
                        gameScene.ChangeLevel(-1);
                    break;
                case TileType.StairsDown:
                    gameScene.ChangeLevel(1);
                    break;
            }
        }


        // Drops item onto the tile the player is located in. No item selection mechanism yet.
        private void DropItem()
        {
            Item item = RemoveItem(inventory[0]);
            if (item != null)
            {
                CurrentTile.AddItem(item);
                gameScene.Statuses.AddMessage("You dropped a " + item.ToString() + ".");
            }
        }

        private Compass GetDirectionFacing(AnimationKey currentAnimation)
        {
            switch(currentAnimation)
            {
                case AnimationKey.WalkUp:
                    return Compass.North;
                case AnimationKey.WalkDown:
                    return Compass.South;
                case AnimationKey.WalkLeft:
                    return Compass.West;
                case AnimationKey.WalkRight:
                    return Compass.East;
            }
            return Compass.None;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null, null, null,
                camera.Transformation);
            Sprite.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }



       /*
        * IContainer interface methods
        */
        public Item RemoveItem(Item item)
        {
            for(int x = 0; x < inventory.Length; x++)
            {
                if(inventory[x].Equals(item))
                {
                    inventory[x] = null;
                    return item;
                }
            }
            return null;
        }

        public bool AddItem(Item item)
        {
            for(int x = 0; x < inventory.Length; x++)
            {
                if(inventory[x] == null)
                {
                    inventory[x] = item;
                    return true;
                }
            }
            return false;
        }

        public Item[] InspectItems()
        {
            return inventory;
        }
    }
}
