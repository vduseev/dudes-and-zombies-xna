using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project
{
    public struct DebugLine
    {
        public string Description;
        public string Data;
    }

    class DebugDrawer : IDisposable
    {
        List<DebugLine> DebugOutput;
        SpriteFont font;

        Texture2D backGround;

        public bool Active = false;

        public float YInitial = 10;
        public float XInitial = 10;
        public float YStride = 12;

        public void Load(SpriteFont font)
        {
            DebugOutput = new List<DebugLine>();
            this.font = font;
        }

        public void AddDebugLine(string description, string data)
        {
            DebugLine debugLine;
            debugLine.Description = description;
            debugLine.Data = data;
            DebugOutput.Add(debugLine);
        }

        public void Clear()
        {
            DebugOutput.Clear();
        }

        public void DrawDebugInfo(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Color color)
        {
            if (Active)
            {
                int longestLineLength = 0;
                foreach (DebugLine dL in DebugOutput)
                {
                    if (dL.Description.Length + dL.Data.Length > longestLineLength) longestLineLength = dL.Description.Length + dL.Data.Length;
                }

                backGround = new Texture2D(graphicsDevice, 1, 1);
                backGround.SetData<Color>(new Color[] { Color.DarkSlateGray });

                float Y = 2* YInitial;
                spriteBatch.Begin();
                spriteBatch.Draw(
                    backGround, 
                    new Rectangle((int)XInitial, (int)YInitial, 140, DebugOutput.Count * 15),
                    new Color(255, 255, 255, 127));
                foreach (DebugLine dL in DebugOutput)
                {
                    spriteBatch.DrawString(font, dL.Description + ": " + dL.Data, new Vector2(2* XInitial, Y), color);
                    Y += YStride;
                }
                spriteBatch.End();
            }
        }

        public void Dispose()
        {
            backGround.Dispose();
        }

    }
}
