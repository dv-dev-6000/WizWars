using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WizWars
{
    class PowerUpClass : MotionGraphic
    {
        // Class Variables 
        private string m_type;
        private bool m_isCollected;
        private float m_shieldCount;

        public string Type
        {
            get
            {
                return m_type;
            }
        }

        public float ShieldTime
        {
            get
            {
                return m_shieldCount;
            }
        }

        public bool Collected 
        {
            get
            {
                return m_isCollected;
            }
            set
            {
                m_isCollected = value;
            }
        }


        public PowerUpClass (Vector2 position, Texture2D txr, string type) : base(position, txr)       // TYPES: "SpeedBoost", "ExtraCaul", "IncreaseRange"
        {
            m_velocity = Vector2.Zero;
            m_type = type;
            m_isCollected = false;

            m_shieldCount = 1.2f;
        }

        public void updateMe(GameTime gt)
        {
            m_shieldCount -= (float)gt.ElapsedGameTime.TotalSeconds;                                    // Reduce countdown 
        }

        public override void drawme(SpriteBatch sBatch)
        {
            sBatch.Draw(m_txr, new Vector2(m_pos.X*64, m_pos.Y*64), Color.White);
        }
    }
}
