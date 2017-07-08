using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Project
{
    class Animation
    {
        public Texture2D AnimationStrip;

        // характеристики анимации
        int basicFrameIndex;
        int basicFrameIndex2;
        public int FrameTime;
        public int FrameWidth;
        public int FrameHeight;
        int framesAmount;
        int elapsedTime = 0;

        int animationSteps;
        int currentStep = 0;
        int framesInStep;

        public bool AnimationPlaying;
        bool Active = true;
        bool Looping = false;

        int currentFrame = 0;
        int currentFrameInStep = 0;
        public Vector2 Position;

        public void Initialize(Texture2D texture, 
            int framesAmount,
            int basicFrameIndex,
            int basicFrameIndex2,
            int frameTime,
            int animationSteps,
            int framesInStep)
        {
            this.AnimationStrip = texture;
            this.framesAmount = framesAmount;
            this.basicFrameIndex = basicFrameIndex;
            this.basicFrameIndex2 = basicFrameIndex2;
            this.FrameTime = frameTime;
            this.animationSteps = animationSteps;
            this.framesInStep = framesInStep;

            // Определить ширину кадра
            FrameWidth = AnimationStrip.Width / framesAmount;
        }

        public void Update(GameTime gameTime)
        {
            if (AnimationPlaying)
            {
                elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (elapsedTime > FrameTime)
                {
                    currentFrame++;
                    currentFrameInStep++;

                    if (currentFrameInStep == framesInStep)
                    {
                        currentFrame = basicFrameIndex;
                        currentFrameInStep = 0;
                        elapsedTime = 0;

                        AnimationPlaying = false;

                        currentStep++;
                        if (currentStep == animationSteps)
                            currentStep = 0;
                    }
                }

            }
        }

        public void PlayStep()
        {
            if (!AnimationPlaying)
            {
                AnimationPlaying = true;

                currentFrame = 1 + framesInStep * currentStep;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle destination, SpriteEffects effects)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(
                AnimationStrip, 
                destination, 
                new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, AnimationStrip.Height), 
                Color.Violet, 0f, 
                Vector2.Zero, 
                effects, 0);
            spriteBatch.End();
        }
    }
}
