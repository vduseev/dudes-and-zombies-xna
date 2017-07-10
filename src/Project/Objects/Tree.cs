using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.Physics;

namespace Project.Objects
{
    class Tree
    {
        public Texture2D Texture;
        public Vector2 Position; // Относительно экрана
        public float Scale;
        public int CWidth;
        public int CHeight;
        public int Width;
        public int Height;
        public float Proportion;

        public void LoadContent(Texture2D texture, float scale)
        {
            Texture = texture;
            Scale = scale;

            Proportion = Texture.Height / (float)Texture.Width;

            CWidth = 2 * World.PixelsPerMetr;
            CHeight = (int)(CWidth * Proportion);
        }

        public void Update(float scale)
        {
            Scale = scale;
            Width = (int)(CWidth * Scale);
            Height = (int)(CHeight * Scale);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2((float)(Width / 2.0), Height);
            spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), 
                new Rectangle(0, 0, Texture.Width, Texture.Height),
                Color.White, 0f, origin, SpriteEffects.None, 0);
        }
    }
}
