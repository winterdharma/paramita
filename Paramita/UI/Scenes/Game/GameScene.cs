﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Paramita.GameLogic;
using Paramita.GameLogic.Items;
using Paramita.UI.Base.Game;
using Paramita.GameLogic.Mechanics;
using System.Collections.Generic;
using Paramita.UI.Input;
using Microsoft.Xna.Framework.Input;
using Paramita.UI.Elements;

namespace Paramita.UI.Base
{

    public class GameScene : Scene
    {
        private TileMapPanel _tileMapPanel;        
        private static StatusPanel _statusPanel;
        private InventoryPanel _inventoryPanel;


        public GameScene(GameController game) : base(game) { }

        public Dungeon Dungeon { get; set; }

        public override void Initialize()
        {
            base.Initialize(); // This calls LoadContent()

            Dungeon = new Dungeon();

            _tileMapPanel = new TileMapPanel(this, Dungeon.GetCurrentLevelLayers(), 0);
            _statusPanel = new StatusPanel(this, GameController.ArialBold, 10, new Point(0,720), 1);
            _inventoryPanel = new InventoryPanel(this, 1);

            Components = InitializeComponents(_tileMapPanel, _statusPanel, _inventoryPanel);

            UserActions = InitializeUserActions(Components);
        }

        protected override void LoadContent()
        {
            ItemTextures.ItemTextureMap[ItemType.Coins] = _content.Load<Texture2D>("Images\\Items\\coins");
            ItemTextures.ItemTextureMap[ItemType.Meat] = _content.Load<Texture2D>("Images\\Items\\meat");
            ItemTextures.ItemTextureMap[ItemType.Shield] = _content.Load<Texture2D>("Images\\Items\\buckler");
            ItemTextures.ItemTextureMap[ItemType.ShortSword] = _content.Load<Texture2D>("Images\\Items\\short_sword");
            ItemTextures.ItemTextureMap[ItemType.Bite] = _content.Load<Texture2D>("transparent");
            ItemTextures.ItemTextureMap[ItemType.Fist] = _content.Load<Texture2D>("transparent");
            ItemTextures.ItemTextureMap[ItemType.None] = _content.Load<Texture2D>("transparent");

            TileMapPanel.Spritesheets.Add(SpriteType.Tile_Floor, _content.Load<Texture2D>("Images\\Tiles\\floor"));
            TileMapPanel.Spritesheets.Add(SpriteType.Tile_Door, _content.Load<Texture2D>("Images\\Tiles\\door"));
            TileMapPanel.Spritesheets.Add(SpriteType.Tile_Wall, _content.Load<Texture2D>("Images\\Tiles\\wall"));
            TileMapPanel.Spritesheets.Add(SpriteType.Tile_StairsUp, _content.Load<Texture2D>("Images\\Tiles\\stairs_up"));
            TileMapPanel.Spritesheets.Add(SpriteType.Tile_StairsDown, _content.Load<Texture2D>("Images\\Tiles\\stairs_down"));
            TileMapPanel.Spritesheets.Add(SpriteType.Actor_GiantRat, _content.Load<Texture2D>("Images\\SentientBeings\\giant_rat"));
            TileMapPanel.Spritesheets.Add(SpriteType.Actor_Player, _content.Load<Texture2D>("Images\\SentientBeings\\human_player"));

            InventoryPanel.DefaultTextures["background"] = _content.Load<Texture2D>("black_background1");
            InventoryPanel.DefaultTextures["minimize_icon"] = _content.Load<Texture2D>("Images\\Scenes\\minimize_icon");
            InventoryPanel.DefaultTextures["white_background"] = _content.Load<Texture2D>("Images\\Scenes\\inventory_bg_white");
            InventoryPanel.DefaultTextures["left_hand"] = _content.Load<Texture2D>("Images\\Scenes\\inventory_hand");
            InventoryPanel.DefaultTextures["right_hand"] = _content.Load<Texture2D>("Images\\Scenes\\inventory_hand");
            InventoryPanel.DefaultTextures["head"] = _content.Load<Texture2D>("Images\\Scenes\\inventory_head");
            InventoryPanel.DefaultTextures["body"] = _content.Load<Texture2D>("Images\\Scenes\\inventory_body");
            InventoryPanel.DefaultTextures["feet"] = _content.Load<Texture2D>("Images\\Scenes\\inventory_feet");
            InventoryPanel.DefaultTextures["other1"] = _content.Load<Texture2D>("Images\\Scenes\\inventory_other");
            InventoryPanel.DefaultTextures["other2"] = _content.Load<Texture2D>("Images\\Scenes\\inventory_other");
            InventoryPanel.DefaultTextures["other3"] = _content.Load<Texture2D>("Images\\Scenes\\inventory_other");
            InventoryPanel.DefaultTextures["other4"] = _content.Load<Texture2D>("Images\\Scenes\\inventory_other");
            InventoryPanel.DefaultTextures["other5"] = _content.Load<Texture2D>("Images\\Scenes\\inventory_other");
        }

        #region User Actions
        protected override List<UserAction> InitializeUserActions(List<Component> components)
        {
            var inventoryPanel = (InventoryPanel)components.Find(c => c is InventoryPanel);
            var actionsList = new List<UserAction>
            {
                new UserAction(this, ToggleInventoryPanel, CanToggleInventoryPanel),
                new UserAction(this, SelectInventoryItem, CanSelectInventoryItem),
                new UserAction(this, FocusOnElement, CanFocusOnElement),
                new UserAction(this, StopFocusOnElement, CanStopFocusOnElement)
            };

            return actionsList;
        }

        private bool CanFocusOnElement(Tuple<Scene, UserInputEventArgs> context)
        {
            var eventArgs = context.Item2;            

            if (!(eventArgs.EventSource is Image))
                return false;

            if (eventArgs.EventType != EventType.MouseOver)
                return false;

            return true;
        }

        private void FocusOnElement(Scene parent, UserInputEventArgs eventArgs)
        {
            var image = (Image)eventArgs.EventSource;
            _inventoryPanel.Elements[image.Id].Highlight();
        }

        private bool CanStopFocusOnElement(Tuple<Scene, UserInputEventArgs> context)
        {
            var eventArgs = context.Item2;

            if (!(eventArgs.EventSource is Image))
                return false;

            if (eventArgs.EventType != EventType.MouseGone)
                return false;

            return true;
        }

        private void StopFocusOnElement(Scene parent, UserInputEventArgs eventArgs)
        {
            var image = (Image)eventArgs.EventSource;
            _inventoryPanel.Elements[image.Id].Unhighlight();
        }

        private bool CanSelectInventoryItem(Tuple<Scene, UserInputEventArgs> context)
        {
            var scene = context.Item1;
            var eventArgs = context.Item2;

            var inventory = (InventoryPanel)scene.Components.Find(c => c is InventoryPanel);
            var inputSources = new InputSource(new List<Element>()
            {
                inventory.Elements["left_hand_item"],
                inventory.Elements["right_hand_item"],
                inventory.Elements["head_item"],
                inventory.Elements["body_item"],
                inventory.Elements["feet_item"],
                inventory.Elements["other1_item"],
                inventory.Elements["other2_item"],
                inventory.Elements["other3_item"],
                inventory.Elements["other4_item"],
                inventory.Elements["other5_item"]
            }, 
                Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, 
                Keys.D0);


            if (!inputSources.Contains(eventArgs.EventSource))
                return false;

            if (eventArgs.EventType != EventType.LeftClick &&
                eventArgs.EventType != EventType.Keyboard)
                return false;

            // don't select an item with the keyboard if no item is visible on that slot
            if(eventArgs.EventSource is Keys key)
            {
                if (key == Keys.D1 && !inventory.Elements["left_hand_item"].Visible)    return false;
                if (key == Keys.D2 && !inventory.Elements["right_hand_item"].Visible)   return false;
                if (key == Keys.D3 && !inventory.Elements["head_item"].Visible)         return false;
                if (key == Keys.D4 && !inventory.Elements["body_item"].Visible)         return false;
                if (key == Keys.D5 && !inventory.Elements["feet_item"].Visible)         return false;
                if (key == Keys.D6 && !inventory.Elements["other1_item"].Visible)       return false;
                if (key == Keys.D7 && !inventory.Elements["other2_item"].Visible)       return false;
                if (key == Keys.D8 && !inventory.Elements["other3_item"].Visible)       return false;
                if (key == Keys.D9 && !inventory.Elements["other4_item"].Visible)       return false;
                if (key == Keys.D0 && !inventory.Elements["other5_item"].Visible)       return false;
            }

            return true;
        }

        private void SelectInventoryItem(Scene parent, UserInputEventArgs eventArgs)
        {
            GameScene scene = (GameScene)parent;
            if (eventArgs.EventSource is Image image)
            {
                int index = scene._inventoryPanel._inventoryItems.FindIndex(id => id.Equals(image.Id));
                
                scene._inventoryPanel.ItemSelected = index + 1;
            }
            else
            {
                if(eventArgs.EventSource is Keys key)
                {
                    if (key is Keys.D0)
                        scene._inventoryPanel.ItemSelected = (int)key - 38;
                    else
                        scene._inventoryPanel.ItemSelected = (int)key - 48;
                }
            }
        }

        private bool CanToggleInventoryPanel(Tuple<Scene, UserInputEventArgs> context)
        {
            var scene = context.Item1;
            var eventArgs = context.Item2;

            var inventory = (InventoryPanel)scene.Components.Find(c => c is InventoryPanel);
            var inputSources = new InputSource(new List<Element>()
            {
                inventory.Elements["minimize_icon"],
                inventory.Elements["background_closed"],
                inventory.Elements["heading"]
            }, Keys.I);


            if (!inputSources.Contains(eventArgs.EventSource))
                return false;

            if (eventArgs.EventType != EventType.LeftClick && 
                eventArgs.EventType != EventType.Keyboard)
                return false;

            if (eventArgs.EventSource == inventory.Elements["heading"] && inventory.IsOpen)
                return false;

            return true;
        }

        private void ToggleInventoryPanel(Scene parent, UserInputEventArgs eventArgs)
        {
            var panel = (InventoryPanel)parent.Components.Find(c => c is InventoryPanel);
            panel.TogglePanelState();
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            Dungeon.Update();

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        #region Event Handling
        private void OnIKeyPressed(object sender, EventArgs e)
        {
            InvokeUserInputEvent(new UserInputEventArgs(EventType.Keyboard, null, Keys.I));
        }

        private void MovePlayerWest(object sender, EventArgs e)
        {
            Dungeon.MovePlayer(Compass.West);
        }

        private void MovePlayerEast(object sender, EventArgs e)
        {
            Dungeon.MovePlayer(Compass.East);
        }

        private void MovePlayerNorth(object sender, EventArgs e)
        {
            Dungeon.MovePlayer(Compass.North);
        }

        private void MovePlayerSouth(object sender, EventArgs e)
        {
            Dungeon.MovePlayer(Compass.South);
        }

        private void PlayerDropItemEventHandler(object sender, InventoryEventArgs e)
        {
            Dungeon.PlayerDropItem(e.InventorySlot, e.InventoryItem);
        }

        private void PlayerEquipItemEventHandler(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PlayerUseItemEventHandler(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void SubscribeToKeyboardEvents()
        {
            Input.D0KeyPressed += OnD0KeyPressed;
            Input.D1KeyPressed += OnD1KeyPressed;
            Input.D2KeyPressed += OnD2KeyPressed;
            Input.D3KeyPressed += OnD3KeyPressed;
            Input.D4KeyPressed += OnD4KeyPressed;
            Input.D5KeyPressed += OnD5KeyPressed;
            Input.D6KeyPressed += OnD6KeyPressed;
            Input.D7KeyPressed += OnD7KeyPressed;
            Input.D8KeyPressed += OnD8KeyPressed;
            Input.D9KeyPressed += OnD9KeyPressed;
            Input.IKeyPressed += OnIKeyPressed;
            Input.LeftKeyPressed += MovePlayerWest;
            Input.RightKeyPressed += MovePlayerEast;
            Input.UpKeyPressed += MovePlayerNorth;
            Input.DownKeyPressed += MovePlayerSouth;
            _inventoryPanel.OnPlayerDroppedItem += PlayerDropItemEventHandler;
            _inventoryPanel.OnPlayerEquippedItem += PlayerEquipItemEventHandler;
            _inventoryPanel.OnPlayerUsedItem += PlayerUseItemEventHandler;
        }

        protected override void UnsubscribeFromKeyboardEvents()
        {
            Input.D0KeyPressed -= OnD0KeyPressed;
            Input.D1KeyPressed -= OnD1KeyPressed;
            Input.D2KeyPressed -= OnD2KeyPressed;
            Input.D3KeyPressed -= OnD3KeyPressed;
            Input.D4KeyPressed -= OnD4KeyPressed;
            Input.D5KeyPressed -= OnD5KeyPressed;
            Input.D6KeyPressed -= OnD6KeyPressed;
            Input.D7KeyPressed -= OnD7KeyPressed;
            Input.D8KeyPressed -= OnD8KeyPressed;
            Input.D9KeyPressed -= OnD9KeyPressed;
            Input.IKeyPressed -= OnIKeyPressed;
            Input.LeftKeyPressed -= MovePlayerWest;
            Input.RightKeyPressed -= MovePlayerEast;
            Input.UpKeyPressed -= MovePlayerNorth;
            Input.DownKeyPressed -= MovePlayerSouth;
            _inventoryPanel.OnPlayerDroppedItem -= PlayerDropItemEventHandler;
            _inventoryPanel.OnPlayerEquippedItem -= PlayerEquipItemEventHandler;
            _inventoryPanel.OnPlayerUsedItem -= PlayerUseItemEventHandler;
        }

        private void OnD0KeyPressed(object sender, EventArgs e)
        {
            InvokeUserInputEvent(new UserInputEventArgs(EventType.Keyboard, null, Keys.D0));
        }

        private void OnD1KeyPressed(object sender, EventArgs e)
        {
            InvokeUserInputEvent(new UserInputEventArgs(EventType.Keyboard, null, Keys.D1));
        }

        private void OnD2KeyPressed(object sender, EventArgs e)
        {
            InvokeUserInputEvent(new UserInputEventArgs(EventType.Keyboard, null, Keys.D2));
        }

        private void OnD3KeyPressed(object sender, EventArgs e)
        {
            InvokeUserInputEvent(new UserInputEventArgs(EventType.Keyboard, null, Keys.D3));
        }

        private void OnD4KeyPressed(object sender, EventArgs e)
        {
            InvokeUserInputEvent(new UserInputEventArgs(EventType.Keyboard, null, Keys.D4));
        }

        private void OnD5KeyPressed(object sender, EventArgs e)
        {
            InvokeUserInputEvent(new UserInputEventArgs(EventType.Keyboard, null, Keys.D5));
        }

        private void OnD6KeyPressed(object sender, EventArgs e)
        {
            InvokeUserInputEvent(new UserInputEventArgs(EventType.Keyboard, null, Keys.D6));
        }

        private void OnD7KeyPressed(object sender, EventArgs e)
        {
            InvokeUserInputEvent(new UserInputEventArgs(EventType.Keyboard, null, Keys.D7));
        }

        private void OnD8KeyPressed(object sender, EventArgs e)
        {
            InvokeUserInputEvent(new UserInputEventArgs(EventType.Keyboard, null, Keys.D8));
        }

        private void OnD9KeyPressed(object sender, EventArgs e)
        {
            InvokeUserInputEvent(new UserInputEventArgs(EventType.Keyboard, null, Keys.D9));
        }
        #endregion
    }
}
