using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WizWars
{
    class ExplosionClass : Animated2D
    {
        private int m_expRange, m_rangeEast, m_rangeSouth, m_rangeWest, m_rangeNorth;
        private Vector2 m_origin;
        private Vector2 m_tilesize;
        private float m_countDown, m_fadeAlt, m_rot;
        private bool m_blocksBroken;
        private Rectangle m_collBoxX, m_collBoxY;
        private int m_collOffset;

        private Texture2D m_beamTex, m_beamCapTex, m_beamVertTex, m_beamCapVertTex;

        public float ExpCount 
        {
            get
            {
                return m_countDown;
            }
        }

        public Rectangle Xrect
        {
            get
            {
                return m_collBoxX;
            }
        }
        public Rectangle Yrect
        {
            get
            {
                return m_collBoxY;
            }
        }


        public ExplosionClass(Texture2D spriteSheet, Vector2 position, int rows, int cols, int fps, int range, Texture2D beam, Texture2D beamCap, Texture2D beamVert, Texture2D beamCapVert) : base(spriteSheet, position, rows, cols, fps)
        {
            m_expRange = range;
            m_origin = new Vector2(m_txr.Width / 2, m_txr.Height / 2);
            m_tilesize = new Vector2(64, 64);
            m_countDown = 1;
            m_blocksBroken = false;

            m_beamTex = beam;
            m_beamCapTex = beamCap;
            m_beamVertTex = beamVert;
            m_beamCapVertTex = beamCapVert;

            m_rot = 0;

            m_srcRect = new Rectangle(0, 0, beam.Width / m_cols, beam.Height / m_rows);

            m_fadeAlt = 0.3f;

            m_collOffset = 20;
        }

        public void updateMe(GameTime gt, MapClass currentMap)
        {
            m_countDown -= (float)gt.ElapsedGameTime.TotalSeconds;                                    // Reduce countdown 


            if (!m_blocksBroken)                                                                      //    if Blocks still need to be destroyed 
            {                                                                                         //
                // break block east                                                                   //
                for (int i = 1; i <= m_expRange; i++)                                                 //        search tiles within explosion range
                {                                                                                     //
                    if (!currentMap.isWalkable(new Vector2(m_pos.X + i, m_pos.Y)))                    //            if valid block found..
                    {                                                                                 //
                        currentMap.CellSwitch(new Vector2(m_pos.X + i, m_pos.Y));                     //                activate cell switch function (if block is breakable it will be switched to wooden floor block)                
                        m_rangeEast = i;                                                              //                set eastward explosion limit to this tile position 
                        break;                                                                        //
                    }                                                                                 //
                    else { m_rangeEast = m_expRange; }                                                //            if no block is found set eastward explosion limit to full 
                }                                                                                     //
                // break block south                                                                  //----------------------------------------------------------------------------------------------------------------
                for (int i = 1; i <= m_expRange; i++)                                                 //        search tiles within explosion range
                {                                                                                     //
                    if (!currentMap.isWalkable(new Vector2(m_pos.X, m_pos.Y + i)))                    //            if valid block found..
                    {                                                                                 //
                        currentMap.CellSwitch(new Vector2(m_pos.X, m_pos.Y + i));                     //                activate cell switch function (if block is breakable it will be switched to wooden floor block)
                        m_rangeSouth = i;                                                             //                set southward explosion limit to this tile position 
                        break;                                                                        //
                    }                                                                                 //
                    else { m_rangeSouth = m_expRange; }                                               //            if no block is found set southward explosion limit to full 
                }                                                                                     //
                // break block west                                                                   //----------------------------------------------------------------------------------------------------------------
                for (int i = 1; i <= m_expRange; i++)                                                 //        search tiles within explosion range
                {                                                                                     //
                    if (!currentMap.isWalkable(new Vector2(m_pos.X - i, m_pos.Y)))                    //            if valid block found..
                    {                                                                                 //
                        currentMap.CellSwitch(new Vector2(m_pos.X - i, m_pos.Y));                     //                activate cell switch function (if block is breakable it will be switched to wooden floor block)
                        m_rangeWest = i;                                                              //                set westward explosion limit to this tile position 
                        break;                                                                        //
                    }                                                                                 //
                    else { m_rangeWest = m_expRange; }                                                //            if no block is found set westward explosion limit to full 
                }                                                                                     //
                // break block north                                                                  //----------------------------------------------------------------------------------------------------------------
                for (int i = 1; i <= m_expRange; i++)                                                 //        search tiles within explosion range
                {                                                                                     //
                    if (!currentMap.isWalkable(new Vector2(m_pos.X, m_pos.Y - i)))                    //            if valid block found..
                    {                                                                                 //
                        currentMap.CellSwitch(new Vector2(m_pos.X, m_pos.Y - i));                     //                activate cell switch function (if block is breakable it will be switched to wooden floor block)
                        m_rangeNorth = i;                                                             //                set northward explosion limit to this tile position 
                        break;                                                                        //
                    }                                                                                 //
                    else { m_rangeNorth = m_expRange; }                                               //            if no block is found set northward explosion limit to full 
                }                                                                                     //----------------------------------------------------------------------------------------------------------------
                m_blocksBroken = true;                                                                //        set blocknBroken to true to ensure the above only happens once per explosion

                m_collBoxX = new Rectangle(new Point((((int)m_pos.X - m_rangeWest) * 64) + m_collOffset / 2, ((int)m_pos.Y * 64) + 11), new Point(((m_rangeWest + m_rangeEast + 1) * 64) - m_collOffset, 40));
                m_collBoxY = new Rectangle(new Point(((int)m_pos.X * 64) + 11, (((int)m_pos.Y - m_rangeNorth) * 64) + m_collOffset / 2), new Point(40, ((m_rangeNorth + m_rangeSouth + 1) * 64) - m_collOffset));
            }
            
        }                                                                                             


        public override void drawme(SpriteBatch sBatch)
        {
            m_rot += -0.05f;

            sBatch.Draw(m_txr, (m_pos * 64) + m_tilesize / 2, null, Color.White * (m_countDown + 0.4f), m_rot, m_origin, 1, SpriteEffects.None, 1);           // Draw Explosion centre 
        }

        public virtual void drawme(SpriteBatch sBatch, GameTime gt, MapClass currentMap)
        {
            m_updateTrigger += (float)gt.ElapsedGameTime.TotalSeconds * m_framesPerSecond;            //
                                                                                                      //
            if (m_updateTrigger >= 1)                                                                 //
            {                                                                                         //
                m_updateTrigger = 0;                                                                  //    Cycle through animation sequence
                m_srcRect.X += m_srcRect.Width;                                                       //
                if (m_srcRect.X == m_txr.Width)                                                       //
                    m_srcRect.X = 0;                                                                  //
            }                                                                                         //

            #region Check & Draw East
            for (int i = 1; i <= m_rangeEast; i++)                                                                                                  //      Loop up to limit of eastward range
            {                                                                                                                                       //
                if (currentMap.isWalkable(new Vector2((m_pos.X + i), m_pos.Y)))                                                                     //          if the current cell is walkable..
                {                                                                                                                                   //
                    if (i < m_rangeEast)                                                                                                            //              and the cell is not at the maximum specified range..
                    {                                                                                                                               //
                        if (currentMap.isWalkable(new Vector2((m_pos.X + i + 1), m_pos.Y)))                                                         //                  check to see if the beam will continue
                        {                                                                                                                           //
                            sBatch.Draw(m_beamTex, new Vector2((m_pos.X + i) * 64, m_pos.Y * 64), m_srcRect, Color.White * (m_countDown + m_fadeAlt));            //                      if so, draw straight beam texture
                        }                                                                                                                           //
                        else                                                                                                                        //                      if not, draw capped beam texture
                        {                                                                                                                           //
                            sBatch.Draw(m_beamCapTex, new Vector2((m_pos.X + i) * 64, m_pos.Y * 64), m_srcRect, Color.White * (m_countDown + m_fadeAlt));         //
                            break;                                                                                                                  //
                        }                                                                                                                           //
                    }                                                                                                                               //
                    else                                                                                                                            //              if the beam is at its maximum tile position then draw the capped beam texture
                    {                                                                                                                               //
                        sBatch.Draw(m_beamCapTex, new Vector2((m_pos.X + i) * 64, m_pos.Y * 64), m_srcRect, Color.White * (m_countDown + m_fadeAlt));             //
                        break;                                                                                                                      //
                    }                                                                                                                               //
                }                                                                                                                                   //
                else                                                                                                                                //          break loop if tile is not walkable
                {                                                                                                                                   //
                    break;                                                                                                                          //
                }                                                                                                                                   //
            }                                                                                                                                       //
            #endregion

            #region Check & Draw South 
            for (int i = 1; i <= m_rangeSouth; i++)                                                                                                         //      Loop up to limit of eastward range
            {                                                                                                                                               //
                if (currentMap.isWalkable(new Vector2(m_pos.X, m_pos.Y + i)))                                                                               //          if the current cell is walkable..
                {                                                                                                                                           //
                    if (i < m_rangeSouth)                                                                                                                   //              and the cell is not at the maximum specified range..
                    {                                                                                                                                       //
                        if (currentMap.isWalkable(new Vector2(m_pos.X, m_pos.Y + i + 1)))                                                                   //                  check to see if the beam will continue
                        {                                                                                                                                   //
                            sBatch.Draw(m_beamVertTex, new Vector2(m_pos.X * 64, (m_pos.Y + i) * 64), m_srcRect, Color.White * (m_countDown + m_fadeAlt));                //                      if so, draw straight beam texture
                        }                                                                                                                                   //
                        else                                                                                                                                //                      if not, draw capped beam texture
                        {                                                                                                                                   //
                            sBatch.Draw(m_beamCapVertTex, new Vector2(m_pos.X * 64, (m_pos.Y + i) * 64), m_srcRect, Color.White * (m_countDown + m_fadeAlt));             //
                            break;                                                                                                                          //
                        }                                                                                                                                   //
                    }                                                                                                                                       //
                    else                                                                                                                                    //              if the beam is at its maximum tile position then draw the capped beam texture
                    {                                                                                                                                       //
                        sBatch.Draw(m_beamCapVertTex, new Vector2(m_pos.X * 64, (m_pos.Y + i) * 64), m_srcRect, Color.White * (m_countDown + m_fadeAlt));                 //
                        break;                                                                                                                              //
                    }                                                                                                                                       //
                }                                                                                                                                           //                                                                                                    
                else                                                                                                                                        //          break loop if tile is not walkable
                {                                                                                                                                           //
                    break;                                                                                                                                  //
                }                                                                                                                                           //
            }                                                                                                                                               //
            #endregion

            #region Check & Draw West
            for (int i = 1; i <= m_rangeWest; i++)                                                                                                                                                            //      Loop up to limit of eastward range
            {                                                                                                                                                                                                 //
                if (currentMap.isWalkable(new Vector2(m_pos.X - i, m_pos.Y)))                                                                                                                                 //          if the current cell is walkable..
                {                                                                                                                                                                                             //
                    if (i < m_rangeWest)                                                                                                                                                                      //              and the cell is not at the maximum specified range..
                    {                                                                                                                                                                                         //
                        if (currentMap.isWalkable(new Vector2((m_pos.X - i - 1), m_pos.Y)))                                                                                                                   //                  check to see if the beam will continue
                        {                                                                                                                                                                                     //
                            sBatch.Draw(m_beamTex, new Vector2((m_pos.X - i) * 64, m_pos.Y * 64), m_srcRect, Color.White * (m_countDown + m_fadeAlt), 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 1);               //                      if so, draw straight beam texture
                        }                                                                                                                                                                                     //
                        else                                                                                                                                                                                  //                      if not, draw capped beam texture
                        {                                                                                                                                                                                     //
                            sBatch.Draw(m_beamCapTex, new Vector2((m_pos.X - i) * 64, m_pos.Y * 64), m_srcRect, Color.White * (m_countDown + m_fadeAlt), 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 1);            //
                            break;                                                                                                                                                                            //
                        }                                                                                                                                                                                     //
                    }                                                                                                                                                                                         //
                    else                                                                                                                                                                                      //              if the beam is at its maximum tile position then draw the capped beam texture
                    {                                                                                                                                                                                         //
                        sBatch.Draw(m_beamCapTex, new Vector2((m_pos.X - i) * 64, m_pos.Y * 64), m_srcRect, Color.White * (m_countDown + m_fadeAlt), 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 1);                //
                        break;                                                                                                                                                                                //
                    }                                                                                                                                                                                         //
                }                                                                                                                                                                                             //                                                                                                                                                                             
                else                                                                                                                                                                                          //          break loop if tile is not walkable
                {                                                                                                                                                                                             //
                    break;                                                                                                                                                                                    //
                }                                                                                                                                                                                             //
            }                                                                                                                                                                                                 //
            #endregion

            #region Check & Draw North 
            for (int i = 1; i <= m_rangeNorth; i++)                                                                                                                                                            //      Loop up to limit of eastward range
            {                                                                                                                                                                                                  //
                if (currentMap.isWalkable(new Vector2(m_pos.X, m_pos.Y - i)))                                                                                                                                  //          if the current cell is walkable..
                {                                                                                                                                                                                              //
                    if (i < m_rangeNorth)                                                                                                                                                                      //              and the cell is not at the maximum specified range..
                    {                                                                                                                                                                                          //
                        if (currentMap.isWalkable(new Vector2(m_pos.X, m_pos.Y - i - 1)))                                                                                                                      //                  check to see if the beam will continue
                        {                                                                                                                                                                                      //
                            sBatch.Draw(m_beamVertTex, new Vector2(m_pos.X * 64, (m_pos.Y - i) * 64), m_srcRect, Color.White * (m_countDown + m_fadeAlt), 0, Vector2.Zero, 1, SpriteEffects.FlipVertically, 1);              //                      if so, draw straight beam texture
                        }                                                                                                                                                                                      //
                        else                                                                                                                                                                                   //                      if not, draw capped beam texture
                        {                                                                                                                                                                                      //
                            sBatch.Draw(m_beamCapVertTex, new Vector2(m_pos.X * 64, (m_pos.Y - i) * 64), m_srcRect, Color.White * (m_countDown + m_fadeAlt), 0, Vector2.Zero, 1, SpriteEffects.FlipVertically, 1);           //
                            break;                                                                                                                                                                             //
                        }                                                                                                                                                                                      //
                    }                                                                                                                                                                                          //
                    else                                                                                                                                                                                       //              if the beam is at its maximum tile position then draw the capped beam texture
                    {                                                                                                                                                                                          //
                        sBatch.Draw(m_beamCapVertTex, new Vector2(m_pos.X * 64, (m_pos.Y - i) * 64), m_srcRect, Color.White * (m_countDown + m_fadeAlt), 0, Vector2.Zero, 1, SpriteEffects.FlipVertically, 1);               //
                        break;                                                                                                                                                                                 //
                    }                                                                                                                                                                                          //
                }                                                                                                                                                                                              //                                                                                                                                                                   
                else                                                                                                                                                                                           //          break loop if tile is not walkable
                {                                                                                                                                                                                              //
                    break;                                                                                                                                                                                     //
                }                                                                                                                                                                                              //
            }                                                                                                                                                                                                  //
            #endregion
        }
    }
}
