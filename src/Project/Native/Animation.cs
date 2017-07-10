using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project.Native
{
    class Animation
    {
        public Texture2D AnimationStrip;

        // характеристики анимации
        int _basicFrameIndex;
        public int FrameTime;
        public int FrameWidth;
        public int FrameHeight;
        int _elapsedTime;

        int _animationSteps;
        int _currentStep;
        int _framesInStep;

        public bool AnimationPlaying;

        int _currentFrame;
        int _currentFrameInStep;
        public Vector2 Position;

        public void Initialize(Texture2D texture, 
            int framesAmount,
            int basicFrameIndex,
            int basicFrameIndex2,
            int frameTime,
            int animationSteps,
            int framesInStep)
        {
            AnimationStrip = texture;
            _basicFrameIndex = basicFrameIndex;
            FrameTime = frameTime;
            _animationSteps = animationSteps;
            _framesInStep = framesInStep;

            // Определить ширину кадра
            FrameWidth = AnimationStrip.Width / framesAmount;
        }

        public void Update(GameTime gameTime)
        {
            if (AnimationPlaying)
            {
                _elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_elapsedTime > FrameTime)
                {
                    _currentFrame++;
                    _currentFrameInStep++;

                    if (_currentFrameInStep == _framesInStep)
                    {
                        _currentFrame = _basicFrameIndex;
                        _currentFrameInStep = 0;
                        _elapsedTime = 0;

                        AnimationPlaying = false;

                        _currentStep++;
                        if (_currentStep == _animationSteps)
                            _currentStep = 0;
                    }
                }

            }
        }

        public void PlayStep()
        {
            if (!AnimationPlaying)
            {
                AnimationPlaying = true;

                _currentFrame = 1 + _framesInStep * _currentStep;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle destination, SpriteEffects effects)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(
                AnimationStrip, 
                destination, 
                new Rectangle(_currentFrame * FrameWidth, 0, FrameWidth, AnimationStrip.Height), 
                Color.Violet, 0f, 
                Vector2.Zero, 
                effects, 0);
            spriteBatch.End();
        }
    }
}
