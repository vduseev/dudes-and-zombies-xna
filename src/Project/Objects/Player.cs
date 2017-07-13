using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project.Physics;
using Project.Native;

namespace Project.Objects
{
    enum PlayerState2 { Care, Repair, Pause, Free }

    class Player : IDisposable
    {
        // Характеристики игрока
        public bool Jump;
        //public int Health;

        //public PlayerState State;
        public Vector2 Position; // Положение относительно уровня
        //Vector2 Origin;

        public LookDirection CurrentDirection;
        LookDirection _previousDirection;

        // Размеры
        #region
        int _cHeadWidth;
        int _cHeadHeight;
        int _cBodyWidth;
        int _cBodyHeight;
        int _cLegsWidth;
        int _cLegsHeight;

        int _headWidth;
        int _headHeight;
        int _bodyWidth;
        int _bodyHeight;
        int _legsWidth;
        int _legsHeight;
        int _height;
        #endregion

        float heightInMetres;
        public Vector2 Speed = Vector2.Zero;
        public float RunSpeed = 2f;
        public float JumpSpeed = 5f;

        // Объекты класса Игрок
        Animation _walkAnimation;
        Texture2D _bodyTexture;
        Texture2D _headTexture;

        // Общеигровые объекты
        public float Scale;

        public Player(float scale, float heightInMetres)
        {
            // Копирование ссылок на общеигровые объекты
            Scale = scale;
            this.heightInMetres = heightInMetres;
        }

        public void LoadContent(ContentManager content, Color color)
        {
            // Тело
            string bodyPath = color == Color.Red ? "Textures/Dude/bodyRed" : "Textures/Dude/bodyBlue";
            _bodyTexture = content.Load<Texture2D>(bodyPath);

            // Голова
            _headTexture = content.Load<Texture2D>("Textures/Dude/head");

            // Ноги
            Texture2D walkAnimationTexture = content.Load<Texture2D>("Textures/Dude/legs");
            _walkAnimation = new Animation();
            _walkAnimation.Initialize(
                texture: walkAnimationTexture,
                framesAmount: 7,
                basicFrameIndex: 0,
                basicFrameIndex2: 5,
                frameTime: 100,
                animationSteps: 2,
                framesInStep: 3
            );

            // Вычисление пропорционального соотношения
            float headProportion = _headTexture.Height / (float)_headTexture.Width;
            _cHeadHeight = (int)(_headTexture.Height * heightInMetres);
            _cHeadWidth = (int)(_cHeadHeight / headProportion);
            
            _cBodyHeight = (int)(_bodyTexture.Height * heightInMetres);
            _cBodyWidth = _cHeadWidth;

            float legsProportion = walkAnimationTexture.Height / (float)_walkAnimation.FrameWidth;
            _cLegsHeight = (int)(walkAnimationTexture.Height * heightInMetres);
            _cLegsWidth = (int)(_cLegsHeight / legsProportion);
            
        }

        public void Update(GameTime gameTime, float scale, int groundLevel)
        {
            Scale = scale;
            _headHeight = (int)(_cHeadHeight * Scale);
            _headWidth = (int)(_cHeadWidth * Scale);
            _bodyHeight = (int)(_cBodyHeight * Scale);
            _bodyWidth = (int)(_cBodyWidth * Scale);
            _legsHeight = (int)(_cLegsHeight * Scale);
            _legsWidth = (int)(_cLegsWidth * Scale);
            _height = _headHeight + _bodyHeight + _legsHeight;

            Position += Speed;

            if (Jump)
                Speed.Y += World.G;

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

            _walkAnimation.Update(gameTime);
        }

        public void Move(GameTime gameTime, LookDirection direction)
        {
            float amount = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 20.0f;
            if (CurrentDirection != LookDirection.Up)
                _previousDirection = CurrentDirection;
            CurrentDirection = direction;
            if (CurrentDirection == LookDirection.Left)
            {
                Position.X -= RunSpeed * amount;
                _walkAnimation.PlayStep();
            }
            else if (CurrentDirection == LookDirection.Right)
            {
                Position.X += RunSpeed * amount;
                _walkAnimation.PlayStep();
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
                if (_previousDirection == LookDirection.Right) effects = SpriteEffects.FlipHorizontally;
                else if (_previousDirection == LookDirection.Left) effects = SpriteEffects.None;
            }
            else effects = SpriteEffects.None;
            #endregion

            // Нарисовать голову и тело
            #region
            spriteBatch.Begin();
            spriteBatch.Draw(
                _headTexture,
                new Rectangle((int)(drawPosition.X - _headWidth / 2.0), (int)(drawPosition.Y - _height), _headWidth, _headHeight),
                new Rectangle(0, 0, _headTexture.Width, _headTexture.Height),
                Color.White,
                0f,
                Vector2.Zero,
                effects,
                0);
            spriteBatch.Draw(
                _bodyTexture,
                new Rectangle((int)(drawPosition.X - _bodyWidth / 2.0), (int)(drawPosition.Y - _height + _headHeight), _bodyWidth, _bodyHeight),
                new Rectangle(0, 0, _bodyTexture.Width, _bodyTexture.Height),
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
            _walkAnimation.Draw(
                spriteBatch,
                new Rectangle((int)(drawPosition.X - _legsWidth / 2.0), (int)(drawPosition.Y - _legsHeight), _legsWidth, _legsHeight),
                effects);
            #endregion
        }

        public void Dispose()
        {
            _headTexture.Dispose();
            _bodyTexture.Dispose();
        }

    }
}
