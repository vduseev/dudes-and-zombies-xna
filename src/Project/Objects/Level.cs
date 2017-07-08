using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project
{
    class Level
    {
        // Характеристики уровня
        #region
        public int cWidth;
        public int cHeight;
        public int cGroundLevel;
        public int Width;
        public int Height;
        public int GroundLevel;
        public Vector2 Position;
        #endregion

        // Общие для игры объекты
        GraphicsDevice device;
        GraphicsDeviceManager graphics;
        ContentManager content;
        DebugDrawer debugDrawer;

        // Общие для игры переменные
        float Scale;
        // Разрешение экрана
        int ScreenWidth;
        int ScreenHeight;

        // Текстуры уровня
        #region
        Texture2D GroundTexture;
        Texture2D GroundNearTexture;
        Texture2D TreesFirstTexture;
        Texture2D TreesSecondTexture;
        Texture2D TreesThirdTexture;
        Texture2D TreesFourthTexture;
        Texture2D HillsNearTexture;
        Texture2D HillsFarTexture;
        Texture2D BackgroundTexture;
        Texture2D BorderTexture;
        #endregion

        Texture2D FogTexture;
        Native.Fade Fade;

        // Объекты уровня
        Player player1;
        Player player2;
        ZombieHandler zombieHandler;
        Random random;

        // bullet texture
        Texture2D bulletTexture;

        List<Bullet> bulletBeams;

        // насколько быстро могут происходить выстрелы
        TimeSpan bulletSpawnTime;
        TimeSpan previousBulletSpawnTime;
        // Переменные управления
        KeyboardState prevK;
        KeyboardState currK;
        GamePadState prevPad1K;
        GamePadState currPad1K;
        GamePadState prevPad2K;
        GamePadState currPad2K;

        int sRate = 1;

        public Level(GraphicsDevice device, GraphicsDeviceManager graphics, ContentManager content, float scale)
        {
            // Копирование ссылок на общеигровые объекты
            #region
            this.device = device;
            this.graphics = graphics;
            this.content = content;
            this.Scale = scale;
            ScreenWidth = graphics.PreferredBackBufferWidth;
            ScreenHeight = graphics.PreferredBackBufferHeight;
            #endregion

            // Размеры уровня
            #region
            cWidth = 30 * Physics.World.PixelsPerMetr;
            cHeight = 5 * Physics.World.PixelsPerMetr;
            cGroundLevel = (int)(1.1f * Physics.World.PixelsPerMetr);
            Width = (int)(cWidth * Scale);
            Height = (int)(cHeight * Scale);
            GroundLevel = (int)(cGroundLevel * Scale);
            #endregion

            // Разместить уровень посередине
            Position = new Vector2(ScreenWidth / 2 - Width / 2, 0);

            // init our Bullet
            bulletBeams = new List<Bullet>();
            const float SECONDS_IN_MINUTE = 60f;
            const float RATE_OF_FIRE = 200f;
            bulletSpawnTime = TimeSpan.FromSeconds(SECONDS_IN_MINUTE / RATE_OF_FIRE);
            previousBulletSpawnTime = TimeSpan.Zero;
        }

        public void LoadContent()
        {
            #region Текстуры
            // Текстуры уровня
            GroundTexture = content.Load<Texture2D>("Textures/Level/ground1");
            GroundNearTexture = content.Load<Texture2D>("Textures/Level/ground2");
            TreesFirstTexture = content.Load<Texture2D>("Textures/Level/tree1");
            TreesSecondTexture = content.Load<Texture2D>("Textures/Level/tree2");
            TreesThirdTexture = content.Load<Texture2D>("Textures/Level/tree3");
            TreesFourthTexture = content.Load<Texture2D>("Textures/Level/tree4");
            HillsNearTexture = content.Load<Texture2D>("Textures/Level/hills1");
            HillsFarTexture = content.Load<Texture2D>("Textures/Level/hills2");

            FogTexture = content.Load<Texture2D>("Textures/Level/fog");

            // Текстура фона
            Color BackgroundColor = Color.DarkSlateBlue;
            // Текстура границ
            Color BorderColor = Color.Black;
            BackgroundTexture = new Texture2D(device, 1, 1);
            BackgroundTexture.SetData<Color>(new Color[] { BackgroundColor });
            BorderTexture = new Texture2D(device, 1, 1);
            BorderTexture.SetData<Color>(new Color[] { BorderColor });
            #endregion

            // Bullet texture

            bulletTexture = content.Load<Texture2D>("Textures/Dude/head");
            // Создание объекта Игрок
            #region
            player1 = new Player(Scale, 1.2f);
            player2 = new Player(Scale, 1.2f);
            player1.LoadContent(content, Color.Red);
            player2.LoadContent(content, Color.Blue);
            player1.Position = // Относительно уровня
                new Vector2(-Position.X + ScreenWidth / 2, 
                            ScreenHeight / 2 + cHeight / 2 - cGroundLevel);
            player2.Position = // Относительно уровня
                new Vector2(-Position.X + ScreenWidth / 2 + 2 * Physics.World.PixelsPerMetr , 
                            ScreenHeight / 2 + cHeight / 2 - cGroundLevel);
            player1.CurrentDirection = LookDirection.Left;
            player2.CurrentDirection = LookDirection.Right;
            #endregion

            // Дебаг отрисовщик
            #region
            debugDrawer = new DebugDrawer();
            SpriteFont font = content.Load<SpriteFont>("DebugFont");
            debugDrawer.Load(font);
            #endregion

            // Fade объект
            Fade = new Native.Fade(graphics);
            Fade.fadeOut();
        }

        public void Update(GameTime gameTime)
        {
            Width = (int)(cWidth * Scale);
            Height = (int)(cHeight * Scale);
            // Высота земли, земляного уровня от пола
            GroundLevel = (int)(cGroundLevel * Scale);
            
            // Затемнение
            Fade.Update(gameTime);
            
            // Управление
            prevK = currK;
            currK = Keyboard.GetState();

            // Джойстик
            prevPad1K = currPad1K;
            currPad1K = GamePad.GetState(PlayerIndex.One);
            prevPad2K = currPad2K;
            currPad2K = GamePad.GetState(PlayerIndex.Two);


            // Player1
            #region
            int safeDistance = (int)(1 * Physics.World.PixelsPerMetr * Scale);
            if (currK.IsKeyDown(Keys.A) || currPad1K.DPad.Left == ButtonState.Pressed)
                if (player1.Position.X > safeDistance)
                {
                    player1.Move(gameTime, LookDirection.Left);
                }
            if (currK.IsKeyDown(Keys.D) || currPad1K.DPad.Right == ButtonState.Pressed)
                if (player1.Position.X < Width - safeDistance)
                {
                    player1.Move(gameTime, LookDirection.Right);
                }
            if (currK.IsKeyDown(Keys.W) || currPad1K.Buttons.A == ButtonState.Pressed)
                player1.Move(gameTime, LookDirection.Up);
            if (currK.IsKeyDown(Keys.Space) || currPad1K.Buttons.X == ButtonState.Pressed)
            {
                FireBullet(gameTime, player1.Position, player1.CurrentDirection);
            }
            #endregion

            // Player2
            #region
            if (currK.IsKeyDown(Keys.Left) || currPad2K.DPad.Left == ButtonState.Pressed)
                player2.Move(gameTime, LookDirection.Left);
            if (currK.IsKeyDown(Keys.Right) || currPad2K.DPad.Right == ButtonState.Pressed)
                player2.Move(gameTime, LookDirection.Right);
            if (currK.IsKeyDown(Keys.Up) || currPad2K.Buttons.A == ButtonState.Pressed)
                player2.Move(gameTime, LookDirection.Up);
            if (currK.IsKeyDown(Keys.Enter) || currPad2K.Buttons.X == ButtonState.Pressed)
            {
                FireBullet(gameTime, player2.Position, player2.CurrentDirection);
            }
            #endregion

            // Обновление игрока
            player1.Update(gameTime, Scale, ScreenHeight / 2 + cHeight / 2 - cGroundLevel);
            player2.Update(gameTime, Scale, ScreenHeight / 2 + cHeight / 2 - cGroundLevel);

            // Увеличение масштаба
            #region
            if (currK.IsKeyDown(Keys.RightControl) && currK.IsKeyDown(Keys.OemPlus) && prevK.IsKeyUp(Keys.OemPlus))
                Scale += 0.1f;
            if (currK.IsKeyDown(Keys.RightControl) && currK.IsKeyDown(Keys.OemMinus) && prevK.IsKeyUp(Keys.OemMinus))
                Scale -= 0.1f;
            if (Scale < 0.8f) { Scale = 0.8f; sRate *= -1; }
            if (Scale > 1.6f) { Scale = 1.6f; sRate *= -1; }
            #endregion

            // Скрыть/Показать Debug Drawer
            #region
            //if (currK.IsKeyDown(Keys.RightControl) && currK.IsKeyDown(Keys.Tab) && prevK.IsKeyUp(Keys.Tab))
            //    debugDrawer.Active = !debugDrawer.Active;
            if (currK.IsKeyDown(Keys.Tab))
                debugDrawer.Active = !debugDrawer.Active;
            #endregion

            //Scale += sRate * 0.005f;
            // update bulletbeams
            for (var i = 0; i < bulletBeams.Count; i++)
            {
                bulletBeams[i].Update(gameTime);
                // Remove the beam when its deactivated or is at the end of the screen.
                Console.WriteLine("Bullet position = " + bulletBeams[i].Position.X);
                Console.WriteLine("Width = " + Width);
                if (!bulletBeams[i].Active || bulletBeams[i].Position.X > Width)
                {
                    bulletBeams.Remove(bulletBeams[i]);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            Vector2 player1DrawPosition = 
                new Vector2((player1.Position.X + Position.X) * Scale, (player1.Position.Y - ScreenHeight / 2) * Scale + ScreenHeight / 2);
            Vector2 player2DrawPosition = 
                new Vector2((player2.Position.X + Position.X) * Scale, (player2.Position.Y - ScreenHeight / 2) * Scale + ScreenHeight / 2);

            int screenCenterX = ScreenWidth / 2;
            int playerScreenCenterShiftX = (int)(player1DrawPosition.X - screenCenterX);
            int shiftSafe = (int)(30 * Scale);
            if (Position.X * Scale - playerScreenCenterShiftX <= -shiftSafe && 
                Position.X * Scale + Width - playerScreenCenterShiftX >= ScreenWidth + shiftSafe)
            {
                player1DrawPosition.X = screenCenterX;
                Position.X -= playerScreenCenterShiftX;
                player2DrawPosition.X = (player2.Position.X + Position.X) * Scale;
            }

            // Небо
            DrawSky(spriteBatch);            
            // Земля
            DrawGround(spriteBatch);
            // Границы экрана
            DrawBorders(spriteBatch); 
                
            player1.Draw(spriteBatch, player1DrawPosition);
            player2.Draw(spriteBatch, player2DrawPosition);

            // Дымка
            DrawFog(spriteBatch);

            Fade.Draw(spriteBatch);

            debugDrawer.Clear();
            debugDrawer.AddDebugLine("Scale", Scale.ToString());
            debugDrawer.AddDebugLine("Level", Position.X.ToString());
            debugDrawer.AddDebugLine("PL1dx", player1DrawPosition.X.ToString());
            debugDrawer.AddDebugLine("PL1dy", player1DrawPosition.Y.ToString());
            debugDrawer.AddDebugLine("PL1x", player1.Position.X.ToString());
            debugDrawer.AddDebugLine("PL2y", player1.Position.Y.ToString());
            debugDrawer.AddDebugLine("Height", Height.ToString());
            debugDrawer.AddDebugLine("Bullets", bulletBeams.Count.ToString());
            debugDrawer.DrawDebugInfo(spriteBatch, graphicsDevice, Color.LightGreen);

        }

        private void DrawSky(SpriteBatch spriteBatch)
        {
            float proportion = HillsFarTexture.Width / (float)HillsFarTexture.Height;

            spriteBatch.Begin();
            //Нарисовать фон
            //spriteBatch.Draw(BackgroundTexture,
            //    new Rectangle(
            //        (int)Position.X, // x.zero
            //        (int)(ScreenHeight / 2 - Height / 2), // y.zero
            //        Width,
            //        Height),
            //    Color.White);
            int AmountOfTextures = (int)(ScreenWidth / (ScreenWidth * Scale)) + 1;
            for (int i = -AmountOfTextures; i < 2 * AmountOfTextures; i++)
            {
                spriteBatch.Draw(HillsFarTexture,
                    new Rectangle(
                        (int)(ScreenWidth / 2 - HillsFarTexture.Width * Scale + i * ScreenWidth * Scale),
                        (int)(ScreenHeight / 2 - Height / 2),
                        (int)(ScreenWidth * Scale),
                        (int)(ScreenWidth * Scale / proportion)),
                    Color.White);
            }
            spriteBatch.End();
        }

        private void DrawGround(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            int AmountOfTextures;
            float Proportion;

            AmountOfTextures = (int)(Width / (Scale * HillsNearTexture.Width));
            Proportion = HillsNearTexture.Width / (float)GroundTexture.Width;

            // Нарисовать ближние холмы
            for (int i = 0; i < AmountOfTextures + 1; i++)
            {
                spriteBatch.Draw(HillsNearTexture,
                    // destination rectangle
                    new Rectangle(
                        (int)((Position.X * Proportion + HillsNearTexture.Width * i) * Scale), // x.zero
                        (int)(Position.Y * Scale + ScreenHeight / 2 + Height / 2
                        - GroundLevel - HillsNearTexture.Height * Scale - 78 * Scale), // y.zero
                        (int)(HillsNearTexture.Width * Scale), // width
                        (int)(HillsNearTexture.Height * Scale)), // height
                    // source
                    new Rectangle(0, 0, HillsNearTexture.Width, HillsNearTexture.Height),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0);
            }

            AmountOfTextures = (int)(Width / (Scale * TreesFourthTexture.Width));
            Proportion = TreesFourthTexture.Width / (float)GroundTexture.Width;

            // Нарисовать деревья 4
            for (int i = 0; i < AmountOfTextures + 1; i++)
            {
                spriteBatch.Draw(TreesFourthTexture,
                    // destination rectangle
                    new Rectangle(
                        (int)((- 150 + Position.X * Proportion + TreesFourthTexture.Width * i) * Scale), // x.zero
                        (int)(Position.Y * Scale + ScreenHeight / 2 + Height / 2
                        - GroundLevel - TreesFourthTexture.Height * Scale - 78 * Scale), // y.zero
                        (int)(TreesFourthTexture.Width * Scale), // width
                        (int)(TreesFourthTexture.Height * Scale)), // height
                    // source
                    new Rectangle(0, 0, TreesFourthTexture.Width, TreesFourthTexture.Height),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0);
            }

            AmountOfTextures = (int)(Width / (Scale * TreesThirdTexture.Width));
            Proportion = TreesThirdTexture.Width / (float)GroundTexture.Width;

            // Нарисовать деревья 3
            for (int i = 0; i < AmountOfTextures + 1; i++)
            {
                spriteBatch.Draw(TreesThirdTexture,
                    // destination rectangle
                    new Rectangle(
                        (int)((-160 + Position.X * Proportion + TreesThirdTexture.Width * i) * Scale), // x.zero
                        (int)(Position.Y * Scale + ScreenHeight / 2 + Height / 2
                        - GroundLevel - TreesThirdTexture.Height * Scale - 62 * Scale), // y.zero
                        (int)(TreesThirdTexture.Width * Scale), // width
                        (int)(TreesThirdTexture.Height * Scale)), // height
                    // source
                    new Rectangle(0, 0, TreesThirdTexture.Width, TreesThirdTexture.Height),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0);
            }

            AmountOfTextures = (int)(Width / (Scale * TreesSecondTexture.Width));
            Proportion = TreesSecondTexture.Width / (float)GroundTexture.Width;
            
            // Нарисовать деревья 2
            for (int i = 0; i < AmountOfTextures + 1; i++)
            {
                spriteBatch.Draw(TreesSecondTexture,
                    // destination rectangle
                    new Rectangle(
                        (int)((-140 + Position.X * Proportion + TreesSecondTexture.Width * i) * Scale), // x.zero
                        (int)(Position.Y * Scale + ScreenHeight / 2 + Height / 2 
                        - GroundLevel - TreesSecondTexture.Height * Scale - 36 * Scale), // y.zero
                        (int)(TreesSecondTexture.Width * Scale), // width
                        (int)(TreesSecondTexture.Height * Scale)), // height
                    // source
                    new Rectangle(0, 0, TreesSecondTexture.Width, TreesSecondTexture.Height),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0);
            }

            AmountOfTextures = (int)(Width / (Scale * TreesFirstTexture.Width));
            Proportion = TreesFirstTexture.Width / (float)GroundTexture.Width;

            // Нарисовать деревья 1
            for (int i = 0; i < AmountOfTextures + 1; i++)
            {
                spriteBatch.Draw(TreesFirstTexture,
                    // destination rectangle
                    new Rectangle(
                        (int)((Position.X * Proportion + TreesFirstTexture.Width * i) * Scale), // x.zero
                        (int)(Position.Y * Scale + ScreenHeight / 2 + Height / 2 
                        - GroundLevel - TreesFirstTexture.Height * Scale + 10 * Scale), // y.zero
                        (int)(TreesFirstTexture.Width * Scale), // width
                        (int)(TreesFirstTexture.Height * Scale)), // height
                    // source
                    new Rectangle(0, 0, TreesFirstTexture.Width, TreesFirstTexture.Height),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0);
            }

            // Вычисление масшабирования !!!! ЗДЕСЬ НЕТ МАСШТАБИРОВАНИЯ
            AmountOfTextures = (int)(Width / (Scale * GroundTexture.Width));
            
            // Нарисовать поверхность второй земли
            for (int i = 0; i < AmountOfTextures + 1; i++)
            {
                spriteBatch.Draw(GroundTexture,
                    // destination rectangle
                    new Rectangle(
                        (int)(Position.X * Scale + GroundTexture.Width * Scale * i), // x.zero
                        (int)(Position.Y + ScreenHeight / 2 + Height / 2 - GroundLevel), // y.zero
                        (int)(GroundTexture.Width * Scale), // width
                        (int)(GroundTexture.Height * Scale)), // height
                    // source
                    new Rectangle(0, 0, GroundTexture.Width, GroundTexture.Height),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0);
            }

            AmountOfTextures = (int)(Width / (Scale * 1.2f * GroundNearTexture.Width));

            // Нарисовать поверхность земли
            for (int i = 0; i < AmountOfTextures + 1; i++)
            {
                spriteBatch.Draw(GroundNearTexture,
                    // destination rectangle
                    new Rectangle(
                        (int)(Position.X * 1.2f + GroundNearTexture.Width * Scale * i), // x.zero
                        (int)(Position.Y + ScreenHeight / 2 + Height / 2 - GroundNearTexture.Height * Scale), // y.zero
                        (int)(GroundNearTexture.Width * Scale), // width
                        (int)(GroundNearTexture.Height * Scale)), // height
                    // source
                    new Rectangle(0, 0, GroundNearTexture.Width, GroundNearTexture.Height),
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
            int BorderHeight = (ScreenHeight - Height) / 2;
            spriteBatch.Draw(BorderTexture, new Rectangle(0, 0, ScreenWidth, BorderHeight), Color.White); // верхняя
            spriteBatch.Draw(BorderTexture, new Rectangle(0, ScreenHeight - BorderHeight, ScreenWidth, BorderHeight), Color.White); // нижняя
            spriteBatch.End();
        }

        private void DrawFog(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(FogTexture,
                new Rectangle(
                    0,
                    (int)(ScreenHeight / 2 - Height / 2),
                    ScreenWidth,
                    Height),
                Color.White);
            spriteBatch.End();
        }

        protected void FireBullet(GameTime gameTime, Vector2 position, LookDirection lookDirection)
        {
            // govern the rate of fire for our Bullets
            if (gameTime.TotalGameTime - previousBulletSpawnTime > bulletSpawnTime)
            {
                previousBulletSpawnTime = gameTime.TotalGameTime;
                // Add the bullet to our list.
                AddBullet(position, lookDirection);
            }
        }

        protected void AddBullet(Vector2 position, LookDirection lookDirection)
        {
            Bullet bullet = new Bullet();
            
            bullet.Initialize(bulletTexture, position, lookDirection);
           
            bulletBeams.Add(bullet);
            Console.WriteLine("!!!");

        }
    }
}
