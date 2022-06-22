using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WizWars
{
    enum Direction
    {
        South,
        West,
        North,
        East
    }

    class PlayerClass : Animated2D
    {
        //class variables

        private Direction m_facing;
        private Vector2 m_destPos;
        private bool m_isMoving;

        private float m_speed;
        private int m_caulPlaced, m_caulMax;
        private int m_expRange;
        private int m_shieldsRemaining;
        private float m_shieldCount;

        public Vector2 Destination
        {
            get
            {
                return m_destPos;
            }
        }

        public Direction Facing
        {
            get
            {
                return m_facing;
            }
        }

        // Upgrade Accessors 
        public float Speed                  // used to get / set player speed
        {
            get
            {
                return m_speed;
            }
            set
            {
                m_speed = value;
            }
        }
        public int CauldronsPlaced            // used to track how many active cauldrons the player has
        {
            get
            {
                return m_caulPlaced;
            }
            set
            {
                m_caulPlaced = value;
            }
        }
        public int CauldronMax            // used to get / set player max cauldron limit
        {
            get
            {
                return m_caulMax;
            }
            set
            {
                m_caulMax = value;
            }
        }
        public int ExplosionRange           // used to get / set player explosion range
        {
            get
            {
                return m_expRange;
            }
            set
            {
                m_expRange = value;
            }
        }

        // Shields and Damage
        public float ShieldCount            
        {
            get
            {
                return m_shieldCount;
            }
        }
        public int ShieldsRemaining            // used to track remaining lives
        {
            get
            {
                return m_shieldsRemaining;
            }
        }

        public void takeDamage()
        {
            m_shieldsRemaining--;
            m_shieldCount = 3;

            m_destPos = m_pos;
        }

        public PlayerClass(Texture2D spriteSheet, Vector2 position, int rows, int cols, int fps) : base(spriteSheet, position, rows, cols, fps)
        {
            m_facing = Direction.South;
            m_destPos = m_pos;
            m_isMoving = false;

            m_shieldCount = 0;
            m_shieldsRemaining = 3;

            m_speed = 0.04f;
            m_caulMax = 2;
            m_expRange = 3;
        }

        public void moveme(Direction moveDir)
        {
            m_facing = moveDir;

            switch (moveDir)
            {
                case Direction.North:
                    m_destPos.Y--;
                    break;
                case Direction.South:
                    m_destPos.Y++;
                    break;
                case Direction.East:
                    m_destPos.X++;
                    break;
                case Direction.West:
                    m_destPos.X--;
                    break;
            }
        }

        public void updateme(GameTime gameTime, MapClass currentMap, GamePadState padCurr, GamePadState padOld)
        {
            if (m_pos == m_destPos)
            {
                m_isMoving = false;
            }
            else { m_isMoving = true; }


            if (!m_isMoving)
            {

                if (padCurr.DPad.Up == ButtonState.Pressed)                                          // move north from stationary
                {
                    if (currentMap.isWalkable(new Vector2(m_pos.X, m_pos.Y - 1)))
                    {
                        moveme(Direction.North);
                    }
                    else { m_facing = Direction.North; }
                }
                else if (padCurr.DPad.Down == ButtonState.Pressed)                                   // move south from stationary
                {
                    if (currentMap.isWalkable(new Vector2(m_pos.X, m_pos.Y + 1)))
                    {
                        moveme(Direction.South);
                    }
                    else { m_facing = Direction.South; }
                }
                else if (padCurr.DPad.Left == ButtonState.Pressed)                                   // move west from stationary
                {
                    if (currentMap.isWalkable(new Vector2(m_pos.X - 1, m_pos.Y)))
                    {
                        moveme(Direction.West);
                    }
                    else { m_facing = Direction.West; }
                }                                                                                    
                else if (padCurr.DPad.Right == ButtonState.Pressed)                                  // move east from stationary
                {
                    if (currentMap.isWalkable(new Vector2(m_pos.X + 1, m_pos.Y)))
                    {
                        moveme(Direction.East);
                    }
                    else { m_facing = Direction.East; }
                }
            }
            else
            {
                if (padCurr.DPad.Up == ButtonState.Pressed)       
                {
                    if ((m_pos.Y < m_destPos.Y + 0.1f && m_facing == Direction.North) || m_facing == Direction.South)            // continue north while traveling north (to avoid pause) or switch from south to north instantly
                    {
                        if (currentMap.isWalkable(new Vector2(m_destPos.X, m_destPos.Y - 1)))
                            moveme(Direction.North);
                    }
                }
                else if (padCurr.DPad.Down == ButtonState.Pressed)     
                {
                    if ((m_pos.Y > m_destPos.Y - 0.1f && m_facing == Direction.South) || m_facing == Direction.North)            // continue south while traveling south (to avoid pause) or switch from north to south instantly
                    {
                        if (currentMap.isWalkable(new Vector2(m_destPos.X, m_destPos.Y + 1)))
                            moveme(Direction.South);
                    }
                }
                else if (padCurr.DPad.Left == ButtonState.Pressed)
                {
                    if((m_pos.X < m_destPos.X + 0.1f && m_facing == Direction.West) || m_facing == Direction.East)               // continue west while traveling west (to avoid pause) or switch from east to west instantly
                    {
                        if (currentMap.isWalkable(new Vector2(m_destPos.X - 1, m_destPos.Y)))
                            moveme(Direction.West);
                    }
                }
                else if (padCurr.DPad.Right == ButtonState.Pressed)
                {
                    if ((m_pos.X > m_destPos.X - 0.1f && m_facing == Direction.East) || m_facing == Direction.West)              // continue east while traveling east (to avoid pause) or switch from west to east instantly
                    {
                        if (currentMap.isWalkable(new Vector2(m_destPos.X + 1, m_destPos.Y)))
                            moveme(Direction.East);
                    }
                }

                if (m_facing == Direction.East)                  // This code actually moves the player while the code above sets the players destination based on controler input
                {                                                //
                    if(m_destPos.X > m_pos.X)                    //
                    {                                            //
                        m_pos.X += m_speed;                      //
                    }                                            //
                    else { m_pos = m_destPos; }                  //
                }                                                //
                else if (m_facing == Direction.West)             //
                {                                                //
                    if (m_destPos.X < m_pos.X)                   //
                    {                                            //
                        m_pos.X -= m_speed;                      //
                    }                                            //
                    else { m_pos = m_destPos; }                  //
                }                                                //
                else if (m_facing == Direction.South)            //
                {                                                //
                    if (m_destPos.Y > m_pos.Y)                   //
                    {                                            //
                        m_pos.Y += m_speed;                      //
                    }                                            //
                    else { m_pos = m_destPos; }                  //
                }                                                //
                else if (m_facing == Direction.North)            //
                {                                                //
                    if (m_destPos.Y < m_pos.Y)                   //
                    {                                            //
                        m_pos.Y -= m_speed;                      //
                    }                                            //
                    else { m_pos = m_destPos; }                  //
                }                                                //
            }

            // Shield Counter
            if (m_shieldCount > 0)
            {
                m_shieldCount -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            m_srcRect.Y = (int)m_facing * m_srcRect.Height;                          // position source rect for spritesheet animation 

            m_collBox.X = (int)(m_pos.X * 64)+16;                                    // bind collision box to position 
            m_collBox.Y = (int)(m_pos.Y * 64)+16;                                    //
        }

        public override void drawme(SpriteBatch sBatch, GameTime gt)
        {
            if (m_isMoving)
            {
                m_updateTrigger += (float)gt.ElapsedGameTime.TotalSeconds * m_framesPerSecond;
            }
            
            if (m_updateTrigger >= 1)
            {
                m_updateTrigger = 0;
                m_srcRect.X += m_srcRect.Width;
                if (m_srcRect.X == m_txr.Width)
                    m_srcRect.X = 0;
            }

            if (m_shieldCount <= 0)
            {
                sBatch.Draw(m_txr, new Vector2(m_pos.X * 64, m_pos.Y * 64), m_srcRect, Color.White);
            }
            else
            {
                sBatch.Draw(m_txr, new Vector2(m_pos.X * 64, m_pos.Y * 64), m_srcRect, Color.OrangeRed);
            }
        }
    }
}
