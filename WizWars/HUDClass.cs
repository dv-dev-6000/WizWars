using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WizWars
{
    class HUDClass : StaticGraphic
    {
        // Class Variables
        private Texture2D m_wizTex;
        private float m_speed;

        public HUDClass(Vector2 pos, Texture2D mainTex, Texture2D wiz) : base(pos, mainTex)
        {
            m_wizTex = wiz;
        }

        public virtual void drawme(SpriteBatch sBatch, Texture2D shieldState, PlayerClass player)
        {
            // get player speed info
            m_speed = player.Speed;

            // speed unit conversion
            switch (m_speed) 
            {
                case 0.04f:
                    m_speed = 1;
                    break;
                case 0.06f:
                    m_speed = 2;
                    break;
                case 0.08f:
                    m_speed = 3;
                    break;
            }


            sBatch.Draw(m_txr, Position, Color.White);
            sBatch.Draw(m_wizTex, Position, Color.White);

            sBatch.DrawString(Game1.hudFont, "" + player.CauldronMax, new Vector2(Position.X + 122, Position.Y + 230), Color.Black);
            if (player.Speed <= 0.08f)
            {
                sBatch.DrawString(Game1.hudFont, "" + m_speed, new Vector2(Position.X + 122, Position.Y + 275), Color.Black);
            }
            else
            {
                sBatch.DrawString(Game1.debugFont, "max", new Vector2(Position.X + 110, Position.Y + 278), Color.Black);
            }
            sBatch.DrawString(Game1.hudFont, "" + player.ExplosionRange, new Vector2(Position.X + 122, Position.Y + 318), Color.Black);

            sBatch.Draw(shieldState, Position, Color.White);
        }
    }
}
