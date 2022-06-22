using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WizWars
{
    class CauldronClass : Animated2D
    {
        private float m_countDown;
        private int m_expRange;
        private int m_belongsTo;

        // Cauldron Accessor methods
        public float Countdown 
        {
            get
            {
                return m_countDown;
            }
            set
            {
                m_countDown = value;
            }
        }

        public int ExplosionRange 
        {
            get
            {
                return m_expRange;
            }
        }

        public int BelongsTo
        {
            get
            {
                return m_belongsTo;
            }
        }

        public CauldronClass(Texture2D spriteSheet, Vector2 position, int rows, int cols, int fps, int currRange, int belongsTo) : base(spriteSheet, position, rows, cols, fps)
        {
            m_countDown = 3;
            m_expRange = currRange;
            m_belongsTo = belongsTo;
        }

        public void updateMe(GameTime gt)
        {
            m_countDown -= (float)gt.ElapsedGameTime.TotalSeconds;
        }

        public override void drawme(SpriteBatch sBatch, GameTime gt)
        {
            m_updateTrigger += (float)gt.ElapsedGameTime.TotalSeconds * m_framesPerSecond;

            if (m_updateTrigger >= 1)
            {
                m_updateTrigger = 0;
                m_srcRect.X += m_srcRect.Width;
                if (m_srcRect.X == m_txr.Width)
                    m_srcRect.X = 128;
            }

            sBatch.Draw(m_txr, new Vector2(m_pos.X * 64, m_pos.Y * 64), m_srcRect, Color.White);
        }
    }
}
