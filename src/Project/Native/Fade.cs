using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project.Native
{
    public class Fade
    {
        #region Attributes
        const int AlphaIncrement = 3;
        const int AlphaMin = 1;
        const int AlphaMax = 254;

        Texture2D _fadeTexture;
        bool _isFading;
        int _fadeType = 1; // Default fade in
        int _alpha = AlphaMin;
  
        int _posX, _posY;
        bool _visible;
        Rectangle _sourceRect;
        #endregion

        #region Constructors
        public Fade(GraphicsDeviceManager graphics)
        {

            //Creando textura , todo negro

            _fadeTexture = new Texture2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            Color[] data = new Color[_fadeTexture.Height * _fadeTexture.Width];

            for (int i = 0; i < _fadeTexture.Height * _fadeTexture.Width; i++)
            {

                data[i] = Color.Black;

            }
           _fadeTexture.SetData(data);

           _posX = 0;
           _posY = 0;

           _sourceRect = new Rectangle(0, 0, _fadeTexture.Width, _fadeTexture.Height);
        }

        public Fade(Texture2D fadeTexture)
        {
            _fadeTexture = fadeTexture;
            _posX = 0;
            _posY = 0;
            _sourceRect = new Rectangle(0, 0, fadeTexture.Width, fadeTexture.Height);
        }

        public Fade(Texture2D fadeTexture,int x,int y)
        {
            _fadeTexture = fadeTexture;
            _posX = x;
            _posY = y;
            _sourceRect = new Rectangle(0, 0, fadeTexture.Width, fadeTexture.Height);
        }

        public Fade(Texture2D fadeTexture, int x, int y, bool visible)
        {
            _fadeTexture = fadeTexture;
            _posX = x;
            _posY = y;
            _visible = visible;
            _sourceRect = new Rectangle(0, 0, fadeTexture.Width, fadeTexture.Height);
        }

        public Fade(Texture2D fadeTexture, int x, int y, Rectangle sourceRect)
        {
            _fadeTexture = fadeTexture;
            _posX = x;
            _posY = y;
            _sourceRect = sourceRect;
        }

        public Fade(Texture2D fadeTexture, int x, int y, Rectangle sourceRect, bool visible)
        {
            _fadeTexture = fadeTexture;
            _posX = x;
            _posY = y;
            _sourceRect = sourceRect;
            _visible = visible;
        }
        #endregion

        #region Methods
        public void FadeIn()
        {
            if (!_isFading)
            {
                _fadeType = 1;
                _isFading = true;
                _alpha = AlphaMin;
            }
        }

        public void FadeOut()
        {
            if (!_isFading)
            {
                _fadeType = 2;
                _isFading = true;
                _alpha = AlphaMax;
            }

        }


        public void SetVisible(bool value)
        {
            _visible = value;
        }
        public int GetX()
        {
            return _posX;
        }

        public int GetY()
        {
            return _posY;
        }

        public void SetX(int x)
        {
            _posX = x;
        }

        public void SetY(int y)
        {
            _posY = y;
        }

        public Texture2D GetTexture() 
        {
            return _fadeTexture;
        }

        public void SetTexture(Texture2D fadeTexture)
        {
            _fadeTexture = fadeTexture;
        }

        public void Update(GameTime gameTime)
        {
           
            //Si esta activo el Fade
            if (_isFading){
             //segun el tipo del fade, incrementamos o desincrementamos el alpha;
                switch (_fadeType)
                {
                    case 1:  // Fade In
                        _alpha += AlphaIncrement;
                        break;
                    case 2: //Fade Out
                        _alpha -= AlphaIncrement;
                        break;
                }


                if (_alpha >= 255 || _alpha <= 0)
                    _isFading = false;
            }

 
        }

        public  void Draw(SpriteBatch spriteBatch)
        {
            if (!_isFading && _visible) {
                spriteBatch.Begin();
                spriteBatch.Draw(_fadeTexture, new Vector2(_posX, _posY), _sourceRect, Color.White);
                spriteBatch.End();
            }

            if (_isFading)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(_fadeTexture,new Vector2(_posX,_posY),_sourceRect,new Color((byte)255, (byte)255, (byte)255, (byte)_alpha));
                spriteBatch.End();
            }
        }
        #endregion

    }
}