using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Native
{
    class FrameCounter
    {
        int frameCounter = 0;
        public int FPS;
        float counterTime = 0f;

        public void Update(GameTime gameTime)
        {
            counterTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (counterTime >= 1000)
            {
                FPS = frameCounter;
                counterTime = 0f;
                frameCounter = 0;
            }

        }

        public void Frame()
        {
            frameCounter++;
        }
    }
}
