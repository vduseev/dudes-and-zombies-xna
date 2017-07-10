using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project.Physics;
using Project.Native;

namespace Project.Objects
{
    class Level: IDisposable
    {
        // Характеристики уровня
        #region
        public int CWidth;
        public int CHeight;
        public int CGroundLevel;
        public int Width;
        public int Height;
        public int GroundLevel;
        public Vector2 Position;
        #endregion

        // Общие для игры объекты
        GraphicsDevice _graphicsDevice;
        GraphicsDeviceManager _graphicsDeviceManager;
        ContentManager _contentManager;
        DebugDrawer _debugDrawer;

        // Общие для игры переменные
        float _scale;
        // Разрешение экрана
        int _screenWidth;
        int _screenHeight;

        // Текстуры уровня
        #region
        Texture2D _groundTexture;
        Texture2D _groundNearTexture;
        Texture2D _treesFirstTexture;
        Texture2D _treesSecondTexture;
        Texture2D _treesThirdTexture;
        Texture2D _treesFourthTexture;
        Texture2D _hillsNearTexture;
        Texture2D _hillsFarTexture;
        Texture2D _backgroundTexture;
        Texture2D _borderTexture;
        #endregion

        Texture2D _fogTexture;
        Fade _fade;

        // Объекты уровня
        Player _player1;
        Player _player2;

        // bullet texture
        Texture2D _bulletTexture;

        List<Bullet> _bulletBeams;

        // насколько быстро могут происходить выстрелы
        TimeSpan _bulletSpawnTime;
        TimeSpan _previousBulletSpawnTime;
        // Переменные управления
        KeyboardState _prevK;
        KeyboardState _currK;
        GamePadState _currPad1K;
        GamePadState _currPad2K;

        public Level(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsDeviceManager, ContentManager contentManager, float scale)
        {
            // Копирование ссылок на общеигровые объекты
            #region
            _graphicsDevice = graphicsDevice;
            _graphicsDeviceManager = graphicsDeviceManager;
            _contentManager = contentManager;
            _scale = scale;
            _screenWidth = graphicsDeviceManager.PreferredBackBufferWidth;
            _screenHeight = graphicsDeviceManager.PreferredBackBufferHeight;
            #endregion

            // Размеры уровня
            #region
            CWidth = 30 * World.PixelsPerMetr;
            CHeight = 5 * World.PixelsPerMetr;
            CGroundLevel = (int)(1.1f * World.PixelsPerMetr);
            Width = (int)(CWidth * _scale);
            Height = (int)(CHeight * _scale);
            GroundLevel = (int)(CGroundLevel * _scale);
            #endregion

            // Разместить уровень посередине
            Position = new Vector2(_screenWidth / 2 - Width / 2, 0);

            // init our Bullet
            _bulletBeams = new List<Bullet>();
            const float secondsInMinute = 60f;
            const float rateOfFire = 300f;
            _bulletSpawnTime = TimeSpan.FromSeconds(secondsInMinute / rateOfFire);
            _previousBulletSpawnTime = TimeSpan.Zero;
        }

        public void LoadContent()
        {
            #region Текстуры
            // Текстуры уровня
            _groundTexture = _contentManager.Load<Texture2D>("Textures/Level/ground1");
            _groundNearTexture = _contentManager.Load<Texture2D>("Textures/Level/ground2");
            _treesFirstTexture = _contentManager.Load<Texture2D>("Textures/Level/tree1");
            _treesSecondTexture = _contentManager.Load<Texture2D>("Textures/Level/tree2");
            _treesThirdTexture = _contentManager.Load<Texture2D>("Textures/Level/tree3");
            _treesFourthTexture = _contentManager.Load<Texture2D>("Textures/Level/tree4");
            _hillsNearTexture = _contentManager.Load<Texture2D>("Textures/Level/hills1");
            _hillsFarTexture = _contentManager.Load<Texture2D>("Textures/Level/hills2");

            _fogTexture = _contentManager.Load<Texture2D>("Textures/Level/fog");

            // Текстура фона
            Color backgroundColor = Color.DarkSlateBlue;
            // Текстура границ
            Color borderColor = Color.Black;
            _backgroundTexture = new Texture2D(_graphicsDevice, 1, 1);
            _backgroundTexture.SetData(new[] { backgroundColor });
            _borderTexture = new Texture2D(_graphicsDevice, 1, 1);
            _borderTexture.SetData(new[] { borderColor });
            #endregion

            // Bullet texture

            _bulletTexture = _contentManager.Load<Texture2D>("Textures/Level/bullet");
            // Создание объекта Игрок
            #region
            _player1 = new Player(_scale, 1.2f);
            _player2 = new Player(_scale, 1.2f);
            _player1.LoadContent(_contentManager, Color.Red);
            _player2.LoadContent(_contentManager, Color.Blue);
            _player1.Position = // Относительно уровня
                new Vector2((float)(-Position.X + _screenWidth / 2.0), 
                            _screenHeight / 2 + CHeight / 2 - CGroundLevel);
            _player2.Position = // Относительно уровня
                new Vector2((float)(-Position.X + _screenWidth / 2.0 + 2 * World.PixelsPerMetr) , 
                            _screenHeight / 2 + CHeight / 2 - CGroundLevel);
            _player1.CurrentDirection = LookDirection.Left;
            _player2.CurrentDirection = LookDirection.Right;
            #endregion

            // Дебаг отрисовщик
            #region
            _debugDrawer = new DebugDrawer();
            SpriteFont font = _contentManager.Load<SpriteFont>("DebugFont");
            _debugDrawer.Load(font);
            #endregion

            // Fade объект
            _fade = new Fade(_graphicsDeviceManager);
            _fade.FadeOut();
        }

        public void Update(GameTime gameTime)
        {
            Width = (int)(CWidth * _scale);
            Height = (int)(CHeight * _scale);
            // Высота земли, земляного уровня от пола
            GroundLevel = (int)(CGroundLevel * _scale);
            
            // Затемнение
            _fade.Update(gameTime);
            
            // Управление
            _prevK = _currK;
            _currK = Keyboard.GetState();

            // Джойстик
            _currPad1K = GamePad.GetState(PlayerIndex.One);
            _currPad2K = GamePad.GetState(PlayerIndex.Two);


            // Player1
            #region
            int safeDistance = (int)(1 * World.PixelsPerMetr * _scale);
            if (_currK.IsKeyDown(Keys.A) || _currPad1K.DPad.Left == ButtonState.Pressed)
                if (_player1.Position.X > safeDistance)
                {
                    _player1.Move(gameTime, LookDirection.Left);
                }
            if (_currK.IsKeyDown(Keys.D) || _currPad1K.DPad.Right == ButtonState.Pressed)
                if (_player1.Position.X < Width - safeDistance)
                {
                    _player1.Move(gameTime, LookDirection.Right);
                }
            if (_currK.IsKeyDown(Keys.W) || _currPad1K.Buttons.A == ButtonState.Pressed)
                _player1.Move(gameTime, LookDirection.Up);
            Vector2 player1DrawPosition =
                new Vector2((_player1.Position.X + Position.X) * _scale, (float)((_player1.Position.Y - _screenHeight / 2.0) * _scale + _screenHeight / 2.0));
            if (_currK.IsKeyDown(Keys.Space) || _currPad1K.Buttons.X == ButtonState.Pressed)
            {
                FireBullet(gameTime, player1DrawPosition, _player1.CurrentDirection);
            }
            #endregion

            // Player2
            #region
            if (_currK.IsKeyDown(Keys.Left) || _currPad2K.DPad.Left == ButtonState.Pressed)
                _player2.Move(gameTime, LookDirection.Left);
            if (_currK.IsKeyDown(Keys.Right) || _currPad2K.DPad.Right == ButtonState.Pressed)
                _player2.Move(gameTime, LookDirection.Right);
            if (_currK.IsKeyDown(Keys.Up) || _currPad2K.Buttons.A == ButtonState.Pressed)
                _player2.Move(gameTime, LookDirection.Up);
            Vector2 player2DrawPosition =
               new Vector2((_player2.Position.X + Position.X) * _scale, (float)((_player2.Position.Y - _screenHeight / 2.0) * _scale + _screenHeight / 2.0));
            if (_currK.IsKeyDown(Keys.Enter) || _currPad2K.Buttons.X == ButtonState.Pressed)
            {
                FireBullet(gameTime, player2DrawPosition, _player2.CurrentDirection);
            }
            #endregion

            // Обновление игрока
            _player1.Update(gameTime, _scale, _screenHeight / 2 + CHeight / 2 - CGroundLevel);
            _player2.Update(gameTime, _scale, _screenHeight / 2 + CHeight / 2 - CGroundLevel);

            // Увеличение масштаба
            #region
            if (_currK.IsKeyDown(Keys.RightControl) && _currK.IsKeyDown(Keys.OemPlus) && _prevK.IsKeyUp(Keys.OemPlus))
                _scale += 0.1f;
            if (_currK.IsKeyDown(Keys.RightControl) && _currK.IsKeyDown(Keys.OemMinus) && _prevK.IsKeyUp(Keys.OemMinus))
                _scale -= 0.1f;
            if (_scale < 0.8f) { _scale = 0.8f; }
            if (_scale > 1.6f) { _scale = 1.6f; }
            #endregion

            // Скрыть/Показать Debug Drawer
            #region
            if (_currK.IsKeyDown(Keys.LeftControl) && _currK.IsKeyDown(Keys.Tab) && _prevK.IsKeyUp(Keys.Tab))
                _debugDrawer.Active = !_debugDrawer.Active;
            //if (currK.IsKeyDown(Keys.Tab))
            //    debugDrawer.Active = !debugDrawer.Active;
            #endregion

            //Scale += sRate * 0.005f;
            // update bulletbeams
            for (var i = 0; i < _bulletBeams.Count; i++)
            {
                _bulletBeams[i].Update(gameTime);
                // Remove the beam when its deactivated or is at the end of the screen.
                if (!_bulletBeams[i].Active || _bulletBeams[i].Position.X > Width || _bulletBeams[i].Position.X < 0)
                {
                    _bulletBeams.Remove(_bulletBeams[i]);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            Vector2 player1DrawPosition = 
                new Vector2((_player1.Position.X + Position.X) * _scale, (float)((_player1.Position.Y - _screenHeight / 2.0) * _scale + _screenHeight / 2.0));
            Vector2 player2DrawPosition = 
                new Vector2((_player2.Position.X + Position.X) * _scale, (float)((_player2.Position.Y - _screenHeight / 2.0) * _scale + _screenHeight / 2.0));

            int screenCenterX = _screenWidth / 2;
            int playerScreenCenterShiftX = (int)(player1DrawPosition.X - screenCenterX);
            int shiftSafe = (int)(30 * _scale);
            if (Position.X * _scale - playerScreenCenterShiftX <= -shiftSafe && 
                Position.X * _scale + Width - playerScreenCenterShiftX >= _screenWidth + shiftSafe)
            {
                player1DrawPosition.X = screenCenterX;
                Position.X -= playerScreenCenterShiftX;
                player2DrawPosition.X = (_player2.Position.X + Position.X) * _scale;
            }

            // Небо
            DrawSky(spriteBatch);            
            // Земля
            DrawGround(spriteBatch);
            // Границы экрана
            DrawBorders(spriteBatch); 
                
            _player1.Draw(spriteBatch, player1DrawPosition);
            _player2.Draw(spriteBatch, player2DrawPosition);

            // Дымка
            DrawFog(spriteBatch);

            _fade.Draw(spriteBatch);
            
            // Draw the lasers.
            foreach (Bullet b in _bulletBeams)
            {
                b.Draw(spriteBatch);
            }

            _debugDrawer.Clear();
            _debugDrawer.AddDebugLine("Scale", _scale.ToString(CultureInfo.CurrentCulture));
            _debugDrawer.AddDebugLine("Level", Position.X.ToString(CultureInfo.CurrentCulture));
            _debugDrawer.AddDebugLine("PL1dx", player1DrawPosition.X.ToString(CultureInfo.CurrentCulture));
            _debugDrawer.AddDebugLine("PL1x", _player1.Position.X.ToString(CultureInfo.CurrentCulture));
            _debugDrawer.AddDebugLine("PL2dx", player2DrawPosition.X.ToString(CultureInfo.CurrentCulture));
            _debugDrawer.AddDebugLine("PL2x", _player2.Position.X.ToString(CultureInfo.CurrentCulture));
            _debugDrawer.AddDebugLine("Height", Height.ToString());
            _debugDrawer.AddDebugLine("Bullets", _bulletBeams.Count.ToString());
            _debugDrawer.DrawDebugInfo(spriteBatch, graphicsDevice, Color.LightGreen);
        }

        private void DrawSky(SpriteBatch spriteBatch)
        {
            float proportion = _hillsFarTexture.Width / (float)_hillsFarTexture.Height;

            spriteBatch.Begin();
            int amountOfTextures = (int)(_screenWidth / (_screenWidth * _scale)) + 1;
            for (int i = -amountOfTextures; i < 2 * amountOfTextures; i++)
            {
                spriteBatch.Draw(_hillsFarTexture,
                    new Rectangle(
                        (int)(_screenWidth / 2.0 - _hillsFarTexture.Width * _scale + i * _screenWidth * _scale),
                        _screenHeight / 2 - Height / 2,
                        (int)(_screenWidth * _scale),
                        (int)(_screenWidth * _scale / proportion)),
                    Color.White);
            }
            spriteBatch.End();
        }

        private void DrawGround(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            int amountOfTextures = (int)(Width / (_scale * _hillsNearTexture.Width));
            float proportion = _hillsNearTexture.Width / (float)_groundTexture.Width;

            // Нарисовать ближние холмы
            for (int i = 0; i < amountOfTextures + 1; i++)
            {
                spriteBatch.Draw(_hillsNearTexture,
                    // destination rectangle
                    new Rectangle(
                        (int)((Position.X * proportion + _hillsNearTexture.Width * i) * _scale), // x.zero
                        (int)(Position.Y * _scale + _screenHeight / 2.0 + Height / 2.0
                        - GroundLevel - _hillsNearTexture.Height * _scale - 78 * _scale), // y.zero
                        (int)(_hillsNearTexture.Width * _scale), // width
                        (int)(_hillsNearTexture.Height * _scale)), // height
                    // source
                    new Rectangle(0, 0, _hillsNearTexture.Width, _hillsNearTexture.Height),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0);
            }

            amountOfTextures = (int)(Width / (_scale * _treesFourthTexture.Width));
            proportion = _treesFourthTexture.Width / (float)_groundTexture.Width;

            // Нарисовать деревья 4
            for (int i = 0; i < amountOfTextures + 1; i++)
            {
                spriteBatch.Draw(_treesFourthTexture,
                    // destination rectangle
                    new Rectangle(
                        (int)((- 150 + Position.X * proportion + _treesFourthTexture.Width * i) * _scale), // x.zero
                        (int)(Position.Y * _scale + _screenHeight / 2.0 + Height / 2.0
                        - GroundLevel - _treesFourthTexture.Height * _scale - 78 * _scale), // y.zero
                        (int)(_treesFourthTexture.Width * _scale), // width
                        (int)(_treesFourthTexture.Height * _scale)), // height
                    // source
                    new Rectangle(0, 0, _treesFourthTexture.Width, _treesFourthTexture.Height),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0);
            }

            amountOfTextures = (int)(Width / (_scale * _treesThirdTexture.Width));
            proportion = _treesThirdTexture.Width / (float)_groundTexture.Width;

            // Нарисовать деревья 3
            for (int i = 0; i < amountOfTextures + 1; i++)
            {
                spriteBatch.Draw(_treesThirdTexture,
                    // destination rectangle
                    new Rectangle(
                        (int)((-160 + Position.X * proportion + _treesThirdTexture.Width * i) * _scale), // x.zero
                        (int)(Position.Y * _scale + _screenHeight / 2.0 + Height / 2.0
                        - GroundLevel - _treesThirdTexture.Height * _scale - 62 * _scale), // y.zero
                        (int)(_treesThirdTexture.Width * _scale), // width
                        (int)(_treesThirdTexture.Height * _scale)), // height
                    // source
                    new Rectangle(0, 0, _treesThirdTexture.Width, _treesThirdTexture.Height),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0);
            }

            amountOfTextures = (int)(Width / (_scale * _treesSecondTexture.Width));
            proportion = _treesSecondTexture.Width / (float)_groundTexture.Width;
            
            // Нарисовать деревья 2
            for (int i = 0; i < amountOfTextures + 1; i++)
            {
                spriteBatch.Draw(_treesSecondTexture,
                    // destination rectangle
                    new Rectangle(
                        (int)((-140 + Position.X * proportion + _treesSecondTexture.Width * i) * _scale), // x.zero
                        (int)(Position.Y * _scale + _screenHeight / 2.0 + Height / 2.0 
                        - GroundLevel - _treesSecondTexture.Height * _scale - 36 * _scale), // y.zero
                        (int)(_treesSecondTexture.Width * _scale), // width
                        (int)(_treesSecondTexture.Height * _scale)), // height
                    // source
                    new Rectangle(0, 0, _treesSecondTexture.Width, _treesSecondTexture.Height),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0);
            }

            amountOfTextures = (int)(Width / (_scale * _treesFirstTexture.Width));
            proportion = _treesFirstTexture.Width / (float)_groundTexture.Width;

            // Нарисовать деревья 1
            for (int i = 0; i < amountOfTextures + 1; i++)
            {
                spriteBatch.Draw(_treesFirstTexture,
                    // destination rectangle
                    new Rectangle(
                        (int)((Position.X * proportion + _treesFirstTexture.Width * i) * _scale), // x.zero
                        (int)(Position.Y * _scale + _screenHeight / 2.0 + Height / 2.0 
                        - GroundLevel - _treesFirstTexture.Height * _scale + 10 * _scale), // y.zero
                        (int)(_treesFirstTexture.Width * _scale), // width
                        (int)(_treesFirstTexture.Height * _scale)), // height
                    // source
                    new Rectangle(0, 0, _treesFirstTexture.Width, _treesFirstTexture.Height),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0);
            }

            // Вычисление масшабирования !!!! ЗДЕСЬ НЕТ МАСШТАБИРОВАНИЯ
            amountOfTextures = (int)(Width / (_scale * _groundTexture.Width));
            
            // Нарисовать поверхность второй земли
            for (int i = 0; i < amountOfTextures + 1; i++)
            {
                spriteBatch.Draw(_groundTexture,
                    // destination rectangle
                    new Rectangle(
                        (int)(Position.X * _scale + _groundTexture.Width * _scale * i), // x.zero
                        (int)(Position.Y + _screenHeight / 2.0 + Height / 2.0 - GroundLevel), // y.zero
                        (int)(_groundTexture.Width * _scale), // width
                        (int)(_groundTexture.Height * _scale)), // height
                    // source
                    new Rectangle(0, 0, _groundTexture.Width, _groundTexture.Height),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0);
            }

            amountOfTextures = (int)(Width / (_scale * 1.2f * _groundNearTexture.Width));

            // Нарисовать поверхность земли
            for (int i = 0; i < amountOfTextures + 1; i++)
            {
                spriteBatch.Draw(_groundNearTexture,
                    // destination rectangle
                    new Rectangle(
                        (int)(Position.X * 1.2f + _groundNearTexture.Width * _scale * i), // x.zero
                        (int)(Position.Y + _screenHeight / 2.0 + Height / 2.0 - _groundNearTexture.Height * _scale), // y.zero
                        (int)(_groundNearTexture.Width * _scale), // width
                        (int)(_groundNearTexture.Height * _scale)), // height
                    // source
                    new Rectangle(0, 0, _groundNearTexture.Width, _groundNearTexture.Height),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0);
            }
            spriteBatch.End();
        }

        private void DrawBorders(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            // Нарисовать границы
            int borderHeight = (_screenHeight - Height) / 2;
            spriteBatch.Draw(_borderTexture, new Rectangle(0, 0, _screenWidth, borderHeight), Color.White); // верхняя
            spriteBatch.Draw(_borderTexture, new Rectangle(0, _screenHeight - borderHeight, _screenWidth, borderHeight), Color.White); // нижняя
            spriteBatch.End();
        }

        private void DrawFog(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_fogTexture,
                new Rectangle(
                    0,
                    _screenHeight / 2 - Height / 2,
                    _screenWidth,
                    Height),
                Color.White);
            spriteBatch.End();
        }

        protected void FireBullet(GameTime gameTime, Vector2 position, LookDirection lookDirection)
        {
            // govern the rate of fire for our Bullets
            if (gameTime.TotalGameTime - _previousBulletSpawnTime > _bulletSpawnTime)
            {
                _previousBulletSpawnTime = gameTime.TotalGameTime;
                // Add the bullet to our list.
                AddBullet(position, lookDirection);
            }
        }

        protected void AddBullet(Vector2 position, LookDirection lookDirection)
        {
            Bullet bullet = new Bullet();
            
            bullet.Initialize(_bulletTexture, position, lookDirection);
           
            _bulletBeams.Add(bullet);

        }

        public void Dispose()
        {
            #region
            _groundTexture.Dispose();
            _groundNearTexture.Dispose();
            _treesFirstTexture.Dispose();
            _debugDrawer.Dispose();
            _treesSecondTexture.Dispose();
            _treesThirdTexture.Dispose();
            _treesFourthTexture.Dispose();
            _hillsNearTexture.Dispose();
            _hillsFarTexture.Dispose();
            _backgroundTexture.Dispose();
            _borderTexture.Dispose();
            _fogTexture.Dispose();
            _player1.Dispose();
            _player2.Dispose();
            _bulletTexture.Dispose();
            #endregion
        }
    }
}
