using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Native
{
    public class Fade
    {
        #region Attributes
        private const int ALPHA_INCREMENT = 3;
        private const int ALPHA_MIN = 1;
        private const int ALPHA_MAX = 254;

        private Texture2D fadeTexture = null;
        private bool isFading = false;
        private int fadeType = 1; // Default fade in
        private int alpha = ALPHA_MIN;
  
        private int posX, posY;
        private bool visible = false;
        private Rectangle sourceRect;
        #endregion

        #region Constructors
        public Fade(GraphicsDeviceManager graphics)
        {

            //Creando textura , todo negro

            fadeTexture = new Texture2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            Color[] data = new Color[fadeTexture.Height * fadeTexture.Width];

            for (int i = 0; i < fadeTexture.Height * fadeTexture.Width; i++)
            {

                data[i] = Color.Black;

            }
           fadeTexture.SetData(data);

           posX = 0;
           posY = 0;

           this.sourceRect = new Rectangle(0, 0, fadeTexture.Width, fadeTexture.Height);
        }

        public Fade(GraphicsDeviceManager graphics, Texture2D fadeTexture)
        {
            this.fadeTexture = fadeTexture;
            posX = 0;
            posY = 0;
            this.sourceRect = new Rectangle(0, 0, fadeTexture.Width, fadeTexture.Height);
        }

        public Fade(Texture2D fadeTexture,int x,int y)
        {
            this.fadeTexture = fadeTexture;
            posX = x;
            posY = y;
            this.sourceRect = new Rectangle(0, 0, fadeTexture.Width, fadeTexture.Height);
        }

        public Fade(Texture2D fadeTexture, int x, int y, bool visible)
        {
            this.fadeTexture = fadeTexture;
            posX = x;
            posY = y;
            this.visible = visible;
            this.sourceRect = new Rectangle(0, 0, fadeTexture.Width, fadeTexture.Height);
        }

        public Fade(Texture2D fadeTexture, int x, int y, Rectangle sourceRect)
        {
            this.fadeTexture = fadeTexture;
            posX = x;
            posY = y;
            this.sourceRect = sourceRect;
        }

        public Fade(Texture2D fadeTexture, int x, int y, Rectangle sourceRect, bool visible)
        {
            this.fadeTexture = fadeTexture;
            posX = x;
            posY = y;
            this.sourceRect = sourceRect;
            this.visible = visible;
        }
        #endregion

        #region Methods
        public void fadeIn()
        {
            if (!isFading)
            {
                fadeType = 1;
                isFading = true;
                alpha = ALPHA_MIN;
            }
        }

        public void fadeOut()
        {
            if (!isFading)
            {
                fadeType = 2;
                isFading = true;
                alpha = ALPHA_MAX;
            }

        }


        public void setVisible(bool value)
        {
            visible = value;
        }
        public int getX()
        {
            return posX;
        }

        public int getY()
        {
            return posY;
        }

        public void setX(int x)
        {
            posX = x;
        }

        public void setY(int y)
        {
            posY = y;
        }

        public Texture2D getTexture() 
        {
            return fadeTexture;
        }

        public void setTexture(Texture2D fadeTexture)
        {
            this.fadeTexture = fadeTexture;
        }

        public void Update(GameTime gameTime)
        {
           
            //Si esta activo el Fade
            if (isFading){
             //segun el tipo del fade, incrementamos o desincrementamos el alpha;
                switch (fadeType)
                {
                    case 1:  // Fade In
                        alpha += ALPHA_INCREMENT;
                        break;
                    case 2: //Fade Out
                        alpha -= ALPHA_INCREMENT;
                        break;
                }


                if (alpha >= 255 || alpha <= 0)
                    isFading = false;
            }

 
        }

        public  void Draw(SpriteBatch spriteBatch)
        {
            if (!isFading && visible) {
                spriteBatch.Begin();
                spriteBatch.Draw(fadeTexture, new Vector2(posX, posY), sourceRect, Color.White);
                spriteBatch.End();
            }

            if (isFading)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(fadeTexture,new Vector2(posX,posY),sourceRect,new Color((byte)255, (byte)255, (byte)255, (byte)alpha));
                spriteBatch.End();
            }
        }
        #endregion

    }
}