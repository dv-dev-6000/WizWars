using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WizWars
{
    class ButtonClass : StaticGraphic  
    {
        // Class Variables 
        private Texture2D m_txrSelected;
        private bool m_isSelected;

        public bool IsSelected
        {
            get
            {
                return m_isSelected;
            }
            set
            {
                m_isSelected = value;
            }
        }

        public ButtonClass(Vector2 pos, Texture2D txr, Texture2D txrSel) : base (pos, txr)
        {
            m_txrSelected = txrSel;
            m_isSelected = false;
        }

        public override void drawme(SpriteBatch sBatch)
        {
            // determines whether the button will be highlighted or not

            if (!m_isSelected)
            {
                sBatch.Draw(m_txr, new Vector2(m_pos.X * 64, (m_pos.Y * 64)-15), Color.LightGray);
            }
            else
            {
                sBatch.Draw(m_txrSelected, new Vector2(m_pos.X * 64, (m_pos.Y * 64) - 15), Color.White);
            }
        }
    }
}
