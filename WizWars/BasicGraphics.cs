using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WizWars
{
    // A basic class set up to handle displayign static images on screen 
    class StaticGraphic
    {
        protected Vector2 m_pos;
        protected Texture2D m_txr;

        public Vector2 Position
        {
            get
            {
                return m_pos;
            }
            set
            {
                m_pos = value;
            }
        }

        public StaticGraphic(Vector2 position, Texture2D txrImage)
        {
            m_pos = position;
            m_txr = txrImage;
        }

        public virtual void drawme(SpriteBatch sBatch)
        {
            sBatch.Draw(m_txr, m_pos, Color.White);
        }
    }

    // A class set up to handle moving sprites
    class MotionGraphic : StaticGraphic
    {
        protected Vector2 m_velocity;
        protected Rectangle m_collBox;

        public Rectangle CollisionBox
        {
            get
            {
                return m_collBox;
            }
        }

        public MotionGraphic(Vector2 position, Texture2D txr) : base(position, txr)
        {
            m_velocity = Vector2.Zero;
            m_collBox = new Rectangle(((int)m_pos.X * 64) + 16, ((int)m_pos.Y * 64) + 16, 32, 32);
        }

        public virtual void updateme(Vector2 vel)
        {
            m_velocity = vel;

            m_pos = m_pos + m_velocity;
        }

        public override void drawme(SpriteBatch sBatch)
        {
            sBatch.Draw(m_txr, m_pos, Color.White * 0.3f);
        }
    }

    // A class set up to handle animated sprites
    class Animated2D : MotionGraphic
    {
        protected Rectangle m_srcRect;
        protected float m_updateTrigger;
        protected int m_framesPerSecond;
        protected int m_rows, m_cols;

        public Animated2D(Texture2D spriteSheet, Vector2 position, int rows, int cols ,int fps) : base(position, spriteSheet)
        {
            m_rows = rows;
            m_cols = cols;

            m_srcRect = new Rectangle(0, 0, spriteSheet.Width/m_cols, spriteSheet.Height/m_rows);
            m_updateTrigger = 0;
            m_framesPerSecond = fps;
        }

        public virtual void drawme(SpriteBatch sBatch, GameTime gt)
        {
            m_updateTrigger += (float)gt.ElapsedGameTime.TotalSeconds * m_framesPerSecond;

            if (m_updateTrigger >= 1)
            {
                m_updateTrigger = 0;
                m_srcRect.X += m_srcRect.Width;
                if (m_srcRect.X == m_txr.Width)
                    m_srcRect.X = 0;
            }

            sBatch.Draw(m_txr, new Vector2(m_pos.X * 64, m_pos.Y * 64), m_srcRect, Color.White);
        }
        public virtual void drawme(SpriteBatch sBatch, GameTime gt, Vector2 cursPos)                       //
        {                                                                                                  //
            m_pos = cursPos;                                                                               //
                                                                                                           //
            m_updateTrigger += (float)gt.ElapsedGameTime.TotalSeconds * m_framesPerSecond;                 //
                                                                                                           //
            if (m_updateTrigger >= 1)                                                                      //
            {                                                                                              //   Draw method used for animated cursor
                m_updateTrigger = 0;                                                                       //
                m_srcRect.X += m_srcRect.Width;                                                            //
                if (m_srcRect.X == m_txr.Width)                                                            //
                    m_srcRect.X = 0;                                                                       //
            }                                                                                              //
                                                                                                           //
            sBatch.Draw(m_txr, new Vector2(m_pos.X * 64, m_pos.Y * 64), m_srcRect, Color.White);           //
        }                                                                                                  //
    }

    // A rotating variant of the motion graphic class
    class Rotating2D : MotionGraphic
    {
        private Vector2 m_origin;
        private float m_rot, m_rotSpeed, m_scale, m_life;
        private bool m_isParticle;

        public float Life 
        {
            get
            {
                return m_life;
            }
        }
        public Rotating2D(Texture2D txr, Vector2 position, float rotationSpeed, bool isPari) : base(position, txr)
        {
            m_origin = new Vector2(m_txr.Width / 2, m_txr.Height / 2);
            m_rot = 0;
            m_rotSpeed = rotationSpeed;
            m_scale = 1;
            m_isParticle = isPari;
            m_life = 3;
            if (m_isParticle)
            {
                m_velocity = new Vector2(Game1.RNG.Next(-2, 2), Game1.RNG.Next(-2, 2));
            }
        }

        public override void drawme(SpriteBatch sBatch)
        {
            if (m_isParticle)
            {
                m_scale -= 0.002f;
                m_life -= 0.01f;
                m_pos += m_velocity;
            }
            else                                             //
            {                                                //
                int temp = Game1.RNG.Next(1,3);              //
                if (temp == 2)                               //
                {                                            //     Shudder effect
                    m_scale = 0.994f;                        //
                }                                            //
                else { m_scale = 1; }                        //
            }                                                //

            m_rot += m_rotSpeed;
            sBatch.Draw(m_txr, m_pos, null, Color.White * m_scale, m_rot, m_origin, m_scale, SpriteEffects.None, 1);
        }
    }
}
