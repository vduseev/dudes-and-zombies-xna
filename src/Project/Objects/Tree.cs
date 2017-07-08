using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project
{
    class Tree
    {
        public Texture2D Texture;
        public Vector2 Position; // Относительно экрана
        public float Scale;
        public int cWidth;
        public int cHeight;
        public int Width;
        public int Height;
        public float Proportion;

        public void LoadContent(Texture2D texture, float scale)
        {
            this.Texture = texture;
            this.Scale = scale;

            Proportion = (float)Texture.Height / (float)Texture.Width;

            cWidth = (int)(2 * Physics.World.PixelsPerMetr);
            cHeight = (int)(cWidth * Proportion);
        }

        public void Update(float scale)
        {
            this.Scale = scale;
            Width = (int)(cWidth * Scale);
            Height = (int)(cHeight * Scale);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 Origin = new Vector2(Width / 2, Height);
            spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), 
                new Rectangle(0, 0, Texture.Width, Texture.Height),
                Color.White, 0f, Origin, SpriteEffects.None, 0);
        }
    }
}
