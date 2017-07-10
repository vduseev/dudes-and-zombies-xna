using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project.Objects;

namespace Project.Game
{
    public class GameManager : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch _spriteBatch;

        // Менеджер уровня
        Level _currentLevel;

        // Общеигровые характеристики
        float Scale = 1f;

        public GameManager()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1024,
                PreferredBackBufferHeight = 650,
                IsFullScreen = false
            };
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Создания объекта: уровень
            _currentLevel = new Level(GraphicsDevice, graphics, Content, Scale);

            // Инициализация уровня
            _currentLevel.LoadContent();
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _currentLevel.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);

            // Отрисовка уровня
            _currentLevel.Draw(_spriteBatch, GraphicsDevice);

            base.Draw(gameTime);
        }
    }
}
