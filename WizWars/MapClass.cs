using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WizWars
{
    class MapClass
    {
        private int[,] m_Cells;
        private int m_width;
        private int m_height;
        private Vector2 m_dropPos;
        private List<Vector2> m_caulLocs;
        private List<Vector2> m_playerPositions;

        public Vector2 DropPosition 
        {
            get
            {
                return m_dropPos;
            }
        }

        public bool isWalkable(Vector2 idx)                                                         //     Check to see if a given tile is walkable for the player,
        {                                                                                           //     also used to determine the pathing for explosions
            switch (m_Cells[(int)idx.X, (int)idx.Y])                                                //
            {                                                                                       //       
                case 0:                                                                             //      if tile = 0 return as walkable 
                    return true;                                                                    //
                case 1:                                                                             //      if the tile = 1
                    bool result = true;                                                             //          set result variable to true
                    for (int i = 0; i < m_caulLocs.Count; i++)                                      //              cycle through the list of cauldron locations
                    {                                                                               //                  
                        if (idx == m_caulLocs[i])                                                   //                  check to see if the given tile matches the location of a cauldron
                        {                                                                           //                      
                            result = false;                                                         //                      if it does, set result to false
                            break;                                                                  //
                        }                                                                           //
                    }                                                                               //
                    for (int i = 0; i < m_playerPositions.Count; i++)                               //              then cycle through the list of player positions
                    {                                                                               //
                        if (idx == m_playerPositions[i])                                            //                  check to see if the given tile matches the location of a player
                        {                                                                           //
                            result = false;                                                         //                      if it does, set result to false
                            break;                                                                  //
                        }                                                                           //
                    }                                                                               //
                    return result;                                                                  //          return the result 
                default:                                                                            //
                    return false;                                                                   //          if the tile = another number return as not walkable
            }                                                                                       //
        }                                                                                           //

        public void CellSwitch(Vector2 idx)                          //
        {                                                            //
            if (m_Cells[(int)idx.X, (int)idx.Y] == 4)                //
            {                                                        //     check to see if a cell is a breakable wall (4), if so change to wooden floor (1)
                m_Cells[(int)idx.X, (int)idx.Y] = 1;                 //
                
                int m_temp = Game1.RNG.Next(1, 7);                   //     generate a 1 in 6 chance for an item to drop..
                if (m_temp == 1) { m_dropPos = idx; }                //         if an item is to drop, set drop position (this will be accessed by gameone's drop item function)
            }                                                        //
        }                                                            //

        public MapClass(int[,] floorplan)
        {
            m_dropPos = Vector2.Zero;

            m_width = floorplan.GetLength(0);
            m_height = floorplan.GetLength(1);

            m_Cells = new int[m_width, m_height];
            for (int x = 0; x < m_width; x++)
            {
                for (int y = 0; y < m_height; y++)
                {
                    m_Cells[x, y] = floorplan[y, x];
                }
            }
        }

        public void updateMe(List<Vector2> caulLocs, List<Vector2> playPos)
        {
            m_caulLocs = caulLocs;
            m_playerPositions = playPos;
        }

        public void drawme(SpriteBatch sb, List<Texture2D> tiles)
        {
            for (int x = 0; x < m_width; x++)
            {
                for (int y = 0; y < m_height; y++)
                {
                    sb.Draw(tiles[m_Cells[x, y]], new Vector2(x * tiles[m_Cells[x, y]].Width, y * tiles[m_Cells[x, y]].Height), Color.White);
                }
            }
        }
    }
}
