using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Project
{
    class Bullet
    {
        public Texture2D BulletSprite;

        float BulletMoveSpeed = 30f;

        public Vector2 Position;

        int Damage = 10;

        public bool Active;

        int Range;

        LookDirection LookDirection;

        public int Width
        {
            get { return BulletSprite.Width; }
        }

        public int Height
        {
            get { return BulletSprite.Height; }
        }

        public void Initialize(Texture2D texture, Vector2 position, LookDirection lookDirection)
        {
            BulletSprite = texture;
            Position = position;
            LookDirection = lookDirection;
            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            Position.X += BulletMoveSpeed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Console.WriteLine("+++");
            SpriteEffects effects = SpriteEffects.None;
            spriteBatch.Begin();
            spriteBatch.Draw(BulletSprite, Position, null, Color.White, 0f, Vector2.Zero, 1f,

      SpriteEffects.None, 0f);
            spriteBatch.End();
            Console.WriteLine("+++2");
        }
    }
}
