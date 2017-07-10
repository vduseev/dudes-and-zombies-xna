using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project.Native
{
    public struct DebugLine
    {
        public string Description;
        public string Data;
    }

    class DebugDrawer : IDisposable
    {
        List<DebugLine> _debugOutput;
        SpriteFont _font;

        Texture2D _backGround;

        public bool Active = false;

        public float YInitial = 10;
        public float XInitial = 10;
        public float YStride = 12;

        public void Load(SpriteFont font)
        {
            _debugOutput = new List<DebugLine>();
            _font = font;
        }

        public void AddDebugLine(string description, string data)
        {
            DebugLine debugLine;
            debugLine.Description = description;
            debugLine.Data = data;
            _debugOutput.Add(debugLine);
        }

        public void Clear()
        {
            _debugOutput.Clear();
        }

        public void DrawDebugInfo(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Color color)
        {
            if (Active)
            {
                int longestLineLength = 0;
                foreach (DebugLine dL in _debugOutput)
                {
                    if (dL.Description.Length + dL.Data.Length > longestLineLength) longestLineLength = dL.Description.Length + dL.Data.Length;
                }

                _backGround = new Texture2D(graphicsDevice, 1, 1);
                _backGround.SetData(new[] { Color.DarkSlateGray });

                float y = 2* YInitial;
                spriteBatch.Begin();
                spriteBatch.Draw(
                    _backGround, 
                    new Rectangle((int)XInitial, (int)YInitial, 140, _debugOutput.Count * 15),
                    new Color(255, 255, 255, 127));
                foreach (DebugLine dL in _debugOutput)
                {
                    spriteBatch.DrawString(_font, dL.Description + ": " + dL.Data, new Vector2(2* XInitial, y), color);
                    y += YStride;
                }
                spriteBatch.End();
            }
        }

        public void Dispose()
        {
            _backGround.Dispose();
        }

    }
}
