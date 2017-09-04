﻿using Paramita.UI.Base;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Paramita.UI.Elements
{
    public class Sprite : Element
    {
        public Sprite(string id, Component parent, Vector2 position, Color unhighlighted, 
            Color highlighted) : base(id, parent, position, unhighlighted, highlighted)
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

        public override void SubscribeToEvents()
        {
        }

        public override void UnsubscribeFromEvents()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        protected override Rectangle CreateRectangle()
        {
            throw new NotImplementedException();
        }
    }
}
