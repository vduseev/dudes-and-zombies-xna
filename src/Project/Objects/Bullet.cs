using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project.Objects
{
    class Bullet
    {
        public Texture2D BulletSprite;

        float BulletMoveSpeed = 30f;

        public Vector2 Position;

        //int Damage = 10;

        public bool Active;

        LookDirection _bulletDirection;

        public int Width => BulletSprite.Width;

        public int Height => BulletSprite.Height;

        public void Initialize(Texture2D texture, Vector2 position, LookDirection bulletDirection)
        {
            BulletSprite = texture;
            Position = position;
            _bulletDirection = bulletDirection;
            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            if (_bulletDirection == LookDirection.Right)
            {
                Position.X += BulletMoveSpeed;
            }
            else
            {
                Position.X -= BulletMoveSpeed;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 bulletPosition = Position;
            bulletPosition.Y -= 30;
            spriteBatch.Begin();
            spriteBatch.Draw(BulletSprite, bulletPosition, null, Color.White, 0f, Vector2.Zero, 1f,

      SpriteEffects.None, 0f);
            spriteBatch.End();
        }
    }
}
