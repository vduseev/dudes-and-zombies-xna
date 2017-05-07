using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project
{
    enum PlayerState { Care, Repair, Pause, Free }

    enum LookDirection { Left, Right, Up, Down }

    class Player
    {
        // Характеристики игрока
        public bool Alive;
        public bool Jump = false;
        public int Health;

        public PlayerState State;
        public Vector2 Position; // Положение относительно уровня
        Vector2 Origin;

        public LookDirection CurrentDirection;
        LookDirection previousDirection;

        // Размеры
        #region
        int cHeadWidth;
        int cHeadHeight;
        int cBodyWidth;
        int cBodyHeight;
        int cLegsWidth;
        int cLegsHeight;
        int cHeight;

        int headWidth;
        int headHeight;
        int bodyWidth;
        int bodyHeight;
        int legsWidth;
        int legsHeight;
        int height;
        #endregion

        float heightInMetres;
        public float Proportion;
        public Vector2 Speed = Vector2.Zero;
        public float RunSpeed = 2f;
        public float JumpSpeed = 5f;

        // Объекты класса Игрок
        Animation walkAnimation;
        Texture2D bodyTexture;
        Texture2D headTexture;

        // Общеигровые объекты
        public float Scale;

        public Player(float scale, float heightInMetres)
        {
            // Копирование ссылок на общеигровые объекты
            this.Scale = scale;
            this.heightInMetres = heightInMetres;
        }

        public void LoadContent(ContentManager content, Color color)
        {
            // Тело
            string bodyPath;
            if (color == Color.Red)
                bodyPath = "Textures/Dude/bodyRed";
            else
                bodyPath = "Textures/Dude/bodyBlue";
            bodyTexture = content.Load<Texture2D>(bodyPath);

            // Голова
            headTexture = content.Load<Texture2D>("Textures/Dude/head");

            // Ноги
            Texture2D walkAnimationTexture = content.Load<Texture2D>("Textures/Dude/legs");
            walkAnimation = new Animation();
            walkAnimation.Initialize(
                texture: walkAnimationTexture,
                framesAmount: 7,
                basicFrameIndex: 0,
                basicFrameIndex2: 5,
                frameTime: 100,
                animationSteps: 2,
                framesInStep: 3
            );

            // Вычисление пропорционального соотношения
            Proportion = (float)headTexture.Height / (float)headTexture.Width;
            cHeadHeight = (int)((float)headTexture.Height * heightInMetres);
            cHeadWidth = (int)(cHeadHeight / Proportion);

            Proportion = (float)bodyTexture.Height / (float)bodyTexture.Width;
            cBodyHeight = (int)(bodyTexture.Height * heightInMetres);
            cBodyWidth = cHeadWidth;

            Proportion = (float)walkAnimationTexture.Height / (float)walkAnimation.FrameWidth;
            cLegsHeight = (int)(walkAnimationTexture.Height * heightInMetres);
            cLegsWidth = (int)(cLegsHeight / Proportion);

            cHeight = cHeadHeight + cBodyHeight + cLegsHeight;
        }

        public void Update(GameTime gameTime, float scale, int groundLevel)
        {
            this.Scale = scale;
            headHeight = (int)(cHeadHeight * Scale);
            headWidth = (int)(cHeadWidth * Scale);
            bodyHeight = (int)(cBodyHeight * Scale);
            bodyWidth = (int)(cBodyWidth * Scale);
            legsHeight = (int)(cLegsHeight * Scale);
            legsWidth = (int)(cLegsWidth * Scale);
            height = headHeight + bodyHeight + legsHeight;

            Position += Speed;

            if (Jump)
                Speed.Y += Physics.World.g;

            if (Position.Y > groundLevel)
            {
                Jump = false;
                Speed.Y = 0f;
                Position.Y = groundLevel;
            }

            if (Position.Y < groundLevel && !Jump) // Scale minus
            {
                Position.Y = groundLevel;
            }

            walkAnimation.Update(gameTime);
        }

        public void Move(GameTime gameTime, LookDirection direction)
        {
            float amount = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 20.0f;
            if (CurrentDirection != LookDirection.Up)
                previousDirection = CurrentDirection;
            this.CurrentDirection = direction;
            if (CurrentDirection == LookDirection.Left)
            {
                Position.X -= RunSpeed * amount;
                walkAnimation.PlayStep();
            }
            else if (CurrentDirection == LookDirection.Right)
            {
                Position.X += RunSpeed * amount;
                walkAnimation.PlayStep();
            }
            else if (CurrentDirection == LookDirection.Up && !Jump)
            {
                Jump = true;
                Speed.Y -= JumpSpeed;
            }

        }

        public void Draw(SpriteBatch spriteBatch, Vector2 drawPosition)
        {
            // Повернуть текстуру по направлению взгляда
            #region
            SpriteEffects effects = SpriteEffects.None;
            if (CurrentDirection == LookDirection.Right) effects = SpriteEffects.FlipHorizontally;
            else if (CurrentDirection == LookDirection.Left) effects = SpriteEffects.None;
            else if (CurrentDirection == LookDirection.Up)
            {
                if (previousDirection == LookDirection.Right) effects = SpriteEffects.FlipHorizontally;
                else if (previousDirection == LookDirection.Left) effects = SpriteEffects.None;
            }
            else effects = SpriteEffects.None;
            #endregion

            // Нарисовать голову и тело
            #region
            spriteBatch.Begin();
            spriteBatch.Draw(
                headTexture,
                new Rectangle((int)(drawPosition.X - headWidth / 2), (int)(drawPosition.Y - height), headWidth, headHeight),
                new Rectangle(0, 0, headTexture.Width, headTexture.Height),
                Color.White,
                0f,
                Vector2.Zero,
                effects,
                0);
            spriteBatch.Draw(
                bodyTexture,
                new Rectangle((int)(drawPosition.X - bodyWidth / 2), (int)(drawPosition.Y - height + headHeight), bodyWidth, bodyHeight),
                new Rectangle(0, 0, bodyTexture.Width, bodyTexture.Height),
                Color.White,
                0f,
                Vector2.Zero,
                effects,
                0);
            spriteBatch.End();
            #endregion

            // нарисовать ноги (анимация)
            #region
            if (effects == SpriteEffects.FlipHorizontally) effects = SpriteEffects.None;
            walkAnimation.Draw(
                spriteBatch,
                new Rectangle((int)(drawPosition.X - legsWidth / 2), (int)(drawPosition.Y - legsHeight), legsWidth, legsHeight),
                effects);
            #endregion
        }

    }
}
