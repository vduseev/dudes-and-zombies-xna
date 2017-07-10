using Microsoft.Xna.Framework;

namespace Project.Native
{
    class FrameCounter
    {
        int _frameCounter;
        public int Fps;
        float _counterTime;

        public void Update(GameTime gameTime)
        {
            _counterTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_counterTime >= 1000)
            {
                Fps = _frameCounter;
                _counterTime = 0f;
                _frameCounter = 0;
            }

        }

        public void Frame()
        {
            _frameCounter++;
        }
    }
}
