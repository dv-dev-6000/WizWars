using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace WizWars
{
    enum GameState              // create game states
    {                           //
        StartScreen,            //
        Ingame,                 //
        GameOver                //
    }                           //

    enum GameMode               // create game modes(how many players)
    {                           //
        TwoPlayer,              //
        ThreePlayer,            //
        FourPlayer              //
    }                           //

    struct Camera2d                                                                              //     Camera Implemented to align maze level
    {                                                                                            //     to center screen
        public Vector2 Position;                                                                 //
        public float Zoom;                                                                       //
                                                                                                 //
        public Matrix getCam()                                                                   //
        {                                                                                        //
            Matrix temp;                                                                         //
            temp = Matrix.CreateTranslation(new Vector3(Position.X, Position.Y, 0));             //
            temp *= Matrix.CreateScale(Zoom);                                                    //
            return temp;                                                                         //
        }                                                                                        //
    }                                                                                            //

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static readonly Random RNG = new Random();   // Random Number Generator
        public static SpriteFont debugFont, hudFont, bigFont;
        SoundEffect buttonSwitch, buttonPress, pickUp, dropCaul, hitShield, hitDie, caulExp, gameOver, endTheme;

        GamePadState padOneCurr, padOneOld, padTwoCurr, padTwoOld, padThreeCurr, padThreeOld, padFourCurr, padFourOld;
        GameState currState;
        GameMode currMode;

        #region START SCREEN ASSETS
        StaticGraphic startBackGround, startTitle, gameTitle, overTitle, infoScreen;            // start backgroud and title
        Rotating2D startVortex, charWin;                                                        // large rotating vortex
        List<Rotating2D> starticleList;                                                         // start screen particles
        float starticleCountdown, matchEndCount;
        Animated2D cursorSprite;
        ButtonClass startGameBut, instructBut, twoBut, threeBut, fourBut;
        Vector2 cursorPos;
        #endregion

        Texture2D beamTexMain, beamTexEnd, beamTexVertMain, beamTexVertEnd;
        Texture2D oneShield, twoShields, threeShields, lidTex, p1ShieldState, p2ShieldState, p3ShieldState, p4ShieldState;

        MapClass currMap;

        List<Texture2D> tiles;
        int[,] testfloor, mainLevel;

        Vector2 itemPos, itemPosOld;

        PlayerClass p1, p2, p3, p4;
        HUDClass p1HUD, p2HUD, p3HUD, p4HUD;
        List<CauldronClass> cauldronList;
        List<ExplosionClass> explosionList;
        List<PowerUpClass> PowerUpList;

        List<MotionGraphic> runeList;
        float runeCountDown;

        List<Vector2> caulLocs;
        List<Vector2> playerLocs;

        Camera2d camera;

        bool hasLoaded, isModeSelect, matchEnd, gameShown, overShown, showInfo;
        string winner;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            _graphics.PreferredBackBufferWidth = 1920;       // set screen dimensions and set full screen
            _graphics.PreferredBackBufferHeight = 1080;      //
            _graphics.IsFullScreen = true;                   //
            _graphics.HardwareModeSwitch = false;            //
        }

        protected override void Initialize()
        {
            camera.Position = new Vector2(0, 0);
            camera.Zoom = 1;

            currState = GameState.StartScreen;
            currMode = GameMode.TwoPlayer;

            cursorPos = new Vector2(11.2f, 11.8f);
            starticleCountdown = 0;
            isModeSelect = false;

            winner = "Draw";

            // Initialise Tile List
            tiles = new List<Texture2D>();
            // Initialise Cauldron List
            cauldronList = new List<CauldronClass>();
            // Initialise Explosion List
            explosionList = new List<ExplosionClass>();
            // Initialise Power Up List
            PowerUpList = new List<PowerUpClass>();
            // Initialise Starticle List
            starticleList = new List<Rotating2D>();
            // Initialise Rune List
            runeList = new List<MotionGraphic>();

            caulLocs = new List<Vector2>();
            playerLocs = new List<Vector2>();

            itemPosOld = Vector2.Zero;
            hasLoaded = true;
            matchEnd = false;
            matchEndCount = 3;
            gameShown = false;
            overShown = false;
            showInfo = false;

            testfloor = new int[17, 17] // level constructed for testing
            {
                { 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2},
                { 3, 1, 1, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 1, 1, 3},
                { 3, 1, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 1, 3},
                { 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3},
                { 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3},
                { 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3},
                { 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3},
                { 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3},
                { 3, 4, 3, 4, 3, 4, 3, 1, 2, 1, 3, 4, 3, 4, 3, 4, 3},
                { 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3},
                { 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3},
                { 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3},
                { 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3},
                { 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3},
                { 3, 1, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 1, 3},
                { 3, 1, 1, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 1, 1, 3},
                { 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2},
            };

            mainLevel = new int[31, 31] // this is the grid layout for the main playable level
            {
            //    0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // 0
                { 0, 0, 0, 0, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 0, 0, 0, 0}, // 1
                { 0, 0, 0, 0, 3, 1, 1, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 1, 1, 3, 0, 0, 0, 0}, // 2
                { 0, 0, 0, 0, 3, 1, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 1, 3, 0, 0, 0, 0}, // 3
                { 0, 0, 0, 0, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3, 0, 0, 0, 0}, // 4
                { 0, 0, 0, 0, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 0, 0, 0, 0}, // 5
                { 0, 0, 0, 0, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3, 0, 0, 0, 0}, // 6
                { 0, 0, 0, 0, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 0, 0, 0, 0}, // 7
                { 0, 0, 0, 0, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3, 0, 0, 0, 0}, // 8
                { 0, 0, 0, 0, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 0, 0, 0, 0}, // 9
                { 0, 0, 0, 0, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3, 0, 0, 0, 0}, // 10
                { 0, 0, 0, 0, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 0, 0, 0, 0}, // 11
                { 0, 0, 0, 0, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3, 0, 0, 0, 0}, // 12
                { 0, 0, 0, 0, 3, 1, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 3, 1, 3, 0, 0, 0, 0}, // 13
                { 0, 0, 0, 0, 3, 1, 1, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 1, 1, 3, 0, 0, 0, 0}, // 14
                { 0, 0, 0, 0, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 0, 0, 0, 0}, // 15
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // 16
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            };


            currMap = new MapClass(mainLevel);      // assignign the main level as the current map

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Non Level Specific Load

            debugFont = Content.Load<SpriteFont>("DebugFont");
            hudFont = Content.Load<SpriteFont>("HUDfont");
            bigFont = Content.Load<SpriteFont>("BigText");

            tiles.Add(Content.Load<Texture2D>("CaveTile"));          //0
            tiles.Add(Content.Load<Texture2D>("WoodTile"));          //1
            tiles.Add(Content.Load<Texture2D>("wwBlock"));           //2
            tiles.Add(Content.Load<Texture2D>("BlankBlock"));        //3
            tiles.Add(Content.Load<Texture2D>("StoneBrickTile"));    //4

            buttonSwitch = Content.Load<SoundEffect>("Sounds/ButtonSwitch");
            buttonPress = Content.Load<SoundEffect>("Sounds/ButtonPress");
            pickUp = Content.Load<SoundEffect>("Sounds/UpgradeCollect");
            dropCaul = Content.Load<SoundEffect>("Sounds/DropCaul");
            hitShield = Content.Load<SoundEffect>("Sounds/HitShield");
            hitDie = Content.Load<SoundEffect>("Sounds/HitDie");
            caulExp = Content.Load<SoundEffect>("Sounds/Explode");
            gameOver = Content.Load<SoundEffect>("Sounds/GameOver");
            endTheme = Content.Load<SoundEffect>("Sounds/StageClear");

            beamTexMain = Content.Load<Texture2D>("StraightSheet");
            beamTexEnd = Content.Load<Texture2D>("CapSheet");
            beamTexVertMain = Content.Load<Texture2D>("StraightSheetVert");
            beamTexVertEnd = Content.Load<Texture2D>("CapSheetVert");

            oneShield = Content.Load<Texture2D>("WW_UI_OneShield");
            twoShields = Content.Load<Texture2D>("WW_UI_TwoShields");
            threeShields = Content.Load<Texture2D>("WW_UI_ThreeShields");
            lidTex = Content.Load<Texture2D>("WW_UI_Lid");

            p1ShieldState = threeShields;
            p2ShieldState = threeShields;
            p3ShieldState = threeShields;
            p4ShieldState = threeShields;

            #endregion

            #region Start Screen Load
            if (currState == GameState.StartScreen)
            {
                startBackGround = new StaticGraphic(Vector2.Zero, Content.Load<Texture2D>("WW_TitleScreen"));
                startVortex = new Rotating2D(Content.Load<Texture2D>("VortexTitle"), new Vector2(_graphics.PreferredBackBufferWidth / 2, 350), -0.02f, false);
                startTitle = new StaticGraphic(new Vector2(_graphics.PreferredBackBufferWidth/2 - 465, 250), Content.Load<Texture2D>("Title"));

                startGameBut = new ButtonClass(new Vector2(12.5f, 11.8f), Content.Load<Texture2D>("Start"), Content.Load<Texture2D>("StartSelect"));
                instructBut = new ButtonClass(new Vector2(12.5f, 14f), Content.Load<Texture2D>("Inst"), Content.Load<Texture2D>("InstSelect"));

                infoScreen = new StaticGraphic(Vector2.Zero, Content.Load<Texture2D>("WW_InfoScreen"));

                cursorSprite = new Animated2D(Content.Load<Texture2D>("Wizard_Wanderer_4x4"), cursorPos, 4, 4, 6);
            }
            #endregion

            #region Ingame Load
            if (currState == GameState.Ingame)
            {
                p1 = new PlayerClass(Content.Load<Texture2D>("Wizard_Wanderer_4x4"), new Vector2(5, 2), 4, 4, 6);
                p2 = new PlayerClass(Content.Load<Texture2D>("WW_Gold"), new Vector2(25, 14), 4, 4, 6);

                if (currMode == GameMode.FourPlayer || currMode == GameMode.ThreePlayer)
                {
                    p3 = new PlayerClass(Content.Load<Texture2D>("WW_Blue"), new Vector2(25, 2), 4, 4, 6);

                    if (currMode == GameMode.FourPlayer)
                    {
                        p4 = new PlayerClass(Content.Load<Texture2D>("WW_Brown"), new Vector2(5, 14), 4, 4, 6);
                    }
                }
                

                p1HUD = new HUDClass(new Vector2(48, 64), Content.Load<Texture2D>("WW_HUD_BASE"), Content.Load<Texture2D>("WW_UI_GreenWiz"));
                p2HUD = new HUDClass(new Vector2(1744, 624), Content.Load<Texture2D>("WW_HUD_BASE"), Content.Load<Texture2D>("WW_UI_GoldWiz"));

                if (currMode == GameMode.FourPlayer || currMode == GameMode.ThreePlayer)
                {
                    p3HUD = new HUDClass(new Vector2(1744, 64), Content.Load<Texture2D>("WW_HUD_BASE"), Content.Load<Texture2D>("WW_UI_BlueWiz"));

                    if (currMode == GameMode.FourPlayer)
                    {
                        p4HUD = new HUDClass(new Vector2(48, 624), Content.Load<Texture2D>("WW_HUD_BASE"), Content.Load<Texture2D>("WW_UI_BrownWiz"));
                    }
                }


                playerLocs.Add(new Vector2(5, 2));                                          //0   player1
                playerLocs.Add(new Vector2(25, 14));                                        //1   player2
                if (currMode == GameMode.FourPlayer || currMode == GameMode.ThreePlayer)
                {
                    playerLocs.Add(new Vector2(25, 2));                                     //2   player3

                    if (currMode == GameMode.FourPlayer)
                    {
                        playerLocs.Add(new Vector2(5, 14));                                 //3   player4
                    }
                }
            }
            #endregion

            #region Gameover Load
            if (currState == GameState.GameOver)
            {
                charWin = new Rotating2D(Content.Load<Texture2D>("VortexTitle"), new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), -0.02f, false);
            }
            #endregion
        }

        public void DropItem(Vector2 pos)                                                                                //     Drops arandomly selected item at a given position 
        {                                                                                                                //
            int m_temp = RNG.Next(1, 4);                                                                                 //
            switch (m_temp)                                                                                              //
            {                                                                                                            //
                case 1:                                                                                                  //
                    PowerUpList.Add(new PowerUpClass(pos, Content.Load<Texture2D>("SpeedBonus"), "SpeedBoost"));         //
                    break;                                                                                               //
                case 2:                                                                                                  //
                    PowerUpList.Add(new PowerUpClass(pos, Content.Load<Texture2D>("ExpBonus"), "IncreaseRange"));        //
                    break;                                                                                               //
                case 3:                                                                                                  //
                    PowerUpList.Add(new PowerUpClass(pos, Content.Load<Texture2D>("CaulBonus"), "ExtraCaul"));           //
                    break;                                                                                               //
                default:                                                                                                 //
                    break;                                                                                               //
            }                                                                                                            //
            itemPosOld = itemPos;                                                                                        //
        }                                                                                                                //

        public Vector2 ResetPosition()                                            //       used to reset players positiopn after loosign a shield
        {                                                                         //
            List<Vector2> walkableTiles = new List<Vector2>();                    //            initialise a temporary list to store the vector positions of walkabel tiels
                                                                                  //
            for (int i = 1; i < 15; i++)                                          //                Cycle through the tiles that make up the level
            {                                                                     //        
                for (int j = 4; j < 26; j++)                                      //                
                {                                                                 //
                    if (currMap.isWalkable(new Vector2(j, i)))                    //                    check if each tile is walkable 
                    {                                                             //
                        walkableTiles.Add(new Vector2(j, i));                     //                        if so, add it to the walkable tiles list
                    }                                                             //
                }                                                                 //
            }                                                                     //
                                                                                  //
            return walkableTiles[RNG.Next(walkableTiles.Count)];                  //            choose a random element from the walkable tiles list and return its value
        }                                                                         //

        public bool placementLegal(Vector2 caulDest)        //      checks to see if a tile already contains a cauldron
        {                                                   //
            bool result = true;                             //          initialise result as true
            for (int i = 0; i < caulLocs.Count; i++)        //          cycle through the cauldron locations list
            {                                               //
                if (caulDest == caulLocs[i])                //              for each element, check if it matches the intended cauldron location 
                {                                           //
                    result = false;                         //                  if positions match, set result to false
                    break;                                  //
                }                                           //
            }                                               //
                                                            //
            return result;                                  //          return result
        }                                                   //

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            padOneCurr = GamePad.GetState(PlayerIndex.One);     // set up game pads
            padTwoCurr = GamePad.GetState(PlayerIndex.Two);     //
            padThreeCurr = GamePad.GetState(PlayerIndex.Three); //
            padFourCurr = GamePad.GetState(PlayerIndex.Four);   //

            #region Start Screen

            if (currState == GameState.StartScreen)
            {
                if (!hasLoaded)                  // Loads Content if required
                {                                //
                    Initialize();                //
                    LoadContent();               //
                    hasLoaded = true;            //
                }                                //

                // Start Screen Particle Effects -------------------------------------------------------------------------------------------------------------------------//
                if (starticleCountdown > 0)                                                                                                                               //
                {                                                                                                                                                         //
                    starticleCountdown -= (float)gameTime.ElapsedGameTime.TotalSeconds;                                                                                   //
                }                                                                                                                                                         //
                else                                                                                                                                                      //
                {                                                                                                                                                         //
                    starticleList.Add(new Rotating2D(Content.Load<Texture2D>("PotionExplode"), new Vector2(_graphics.PreferredBackBufferWidth / 2, 350), -0.05f, true));  //
                    starticleCountdown = 0.5f;                                                                                                                            //
                }                                                                                                                                                         //
                starticleList.RemoveAll(s => s.Life <= 0);                                                                                                                //
                                                                                                                                                                          //
                //--------------------------------------------------------------------------------------------------------------------------------------------------------//

                #region Set Button Selection Highlights

                if (cursorSprite.Position.Y == startGameBut.Position.Y)      // 
                {                                                            //
                    startGameBut.IsSelected = true;                          //     Highlights Start Button if selected
                }                                                            //
                else { startGameBut.IsSelected = false; }                    //
                                                                             //
                if (cursorSprite.Position.Y == instructBut.Position.Y)       //
                {                                                            //
                    instructBut.IsSelected = true;                           //     Highlights Info Button if selected
                }                                                            //
                else { instructBut.IsSelected = false; }                     //

                if (isModeSelect)                                                   //      Determines which button to highlight
                {                                                                   //
                    if (cursorSprite.Position.Y == twoBut.Position.Y)               //
                    {                                                               //
                        twoBut.IsSelected = true;                                   //
                    }                                                               //
                    else { twoBut.IsSelected = false; }                             //
                                                                                    //
                    if (cursorSprite.Position.Y == threeBut.Position.Y)             //
                    {                                                               //
                        threeBut.IsSelected = true;                                 //
                    }                                                               //
                    else { threeBut.IsSelected = false; }                           //
                                                                                    //
                    if (cursorSprite.Position.Y == fourBut.Position.Y)              //
                    {                                                               //
                        fourBut.IsSelected = true;                                  //
                    }                                                               //
                    else { fourBut.IsSelected = false; }                            //
                }                                                                   //

                #endregion


                #region START SCREEN CONTROLS
                if ((padOneCurr.Buttons.A == ButtonState.Pressed && padOneOld.Buttons.A == ButtonState.Released) || (padTwoCurr.Buttons.A == ButtonState.Pressed && padTwoOld.Buttons.A == ButtonState.Released))
                {
                    if (!isModeSelect)
                    {
                        if (cursorPos.Y == startGameBut.Position.Y)                                                                                         
                        {                                                                                                                                   //      if A is pressed and start button is highlighted
                            twoBut = new ButtonClass(new Vector2(12.5f, 11.1f), Content.Load<Texture2D>("2p"), Content.Load<Texture2D>("2pSelect"));        //      then mode select buttons appear
                            threeBut = new ButtonClass(new Vector2(12.5f, 12.8f), Content.Load<Texture2D>("3p"), Content.Load<Texture2D>("3pSelect"));      //
                            fourBut = new ButtonClass(new Vector2(12.5f, 14.5f), Content.Load<Texture2D>("4p"), Content.Load<Texture2D>("4pSelect"));       //
                            cursorPos = new Vector2(11.2f, twoBut.Position.Y);                                                                              //
                            buttonPress.Play();                                                                                                             //
                            isModeSelect = true;                                                                                                            //
                        }                                                                                                                                   //
                        else if (cursorPos.Y == instructBut.Position.Y)
                        {                                                                                                                                   // triggers information screen
                            showInfo = true;                                                                                                                // 
                            buttonPress.Play();                                                                                                             // 
                        }                                                                                                                                   // 
                    }
                    else
                    {                                                                   //      
                        starticleList.RemoveAll(s => s.Life >= 0);                      //       Removes Start screen particles
                                                                                        //
                        if (cursorPos.Y == threeBut.Position.Y)                         //       Begins play in 3 player mode if 3 player selected
                        {                                                               //
                            buttonPress.Play();                                         //
                            currMode = GameMode.ThreePlayer;                            //
                            currState = GameState.Ingame;                               //
                            padOneOld = padOneCurr;                                     //
                            padTwoOld = padTwoCurr;                                     //
                            hasLoaded = false;                                          //
                        }                                                               //
                        else if (cursorPos.Y == fourBut.Position.Y)                     //       Begins play in 4 player mode if 4 player selected
                        {                                                               //
                            buttonPress.Play();                                         //
                            currMode = GameMode.FourPlayer;                             //
                            currState = GameState.Ingame;                               //
                            padOneOld = padOneCurr;                                     //
                            padTwoOld = padTwoCurr;                                     //
                            hasLoaded = false;                                          //
                        }                                                               //
                        else                                                            //       Begins play in 2 player mode if 2 player selected 
                        {                                                               //       or in the case of a selection error 2 player is started as default
                            buttonPress.Play();                                         //
                            currMode = GameMode.TwoPlayer;                              //
                            currState = GameState.Ingame;                               //
                            padOneOld = padOneCurr;                                     //
                            padTwoOld = padTwoCurr;                                     //
                            hasLoaded = false;                                          //
                        }                                                               //
                    }                                                                   //
                }

                if (((padOneCurr.DPad.Down == ButtonState.Pressed && padOneOld.DPad.Down == ButtonState.Released) || (padTwoCurr.DPad.Down == ButtonState.Pressed && padTwoOld.DPad.Down == ButtonState.Released)) && !showInfo)
                {
                    if (!isModeSelect)                                                        //    Controls for moving Button selection DOWN
                    {                                                                         //
                        buttonSwitch.Play();                                                  //    
                        cursorPos = new Vector2(11.2f, instructBut.Position.Y);               //
                    }                                                                         //
                    else                                                                      //
                    {                                                                         //
                        if (cursorPos.Y != fourBut.Position.Y)                                //
                        {                                                                     //
                            buttonSwitch.Play();                                              //
                            cursorPos.Y += 1.7f;                                              //
                        }                                                                     //
                    }                                                                         //
                }
                if (((padOneCurr.DPad.Up == ButtonState.Pressed && padOneOld.DPad.Up == ButtonState.Released) || (padTwoCurr.DPad.Up == ButtonState.Pressed && padTwoOld.DPad.Up == ButtonState.Released)) && !showInfo)
                {
                    if (!isModeSelect)                                                        //    Controls for movign button selection UP
                    {                                                                         // 
                        buttonSwitch.Play();                                                  // 
                        cursorPos = new Vector2(11.2f, startGameBut.Position.Y);              // 
                    }                                                                         // 
                    else                                                                      // 
                    {                                                                         // 
                        if (cursorPos.Y != twoBut.Position.Y)                                 // 
                        {                                                                     // 
                            buttonSwitch.Play();                                              // 
                            cursorPos.Y -= 1.7f;                                              // 
                        }                                                                     // 
                    }                                                                         // 
                }
                if (showInfo && (padOneCurr.Buttons.B == ButtonState.Pressed && padOneOld.Buttons.B == ButtonState.Released) || (padTwoCurr.Buttons.B == ButtonState.Pressed && padTwoOld.Buttons.B == ButtonState.Released))
                {
                    showInfo = false;                                                         //    exit show info screen when B pressed
                    buttonPress.Play();                                                       //
                }
                #endregion

            }

            #endregion


            #region InGame

            if (currState == GameState.Ingame)
            {
                if (!hasLoaded)                                       //    Loads Content if required
                {                                                     //
                    LoadContent();                                    //
                    camera.Position = new Vector2(-32, 0);            //
                    hasLoaded = true;                                 //
                }                                                     //

                #region Store Player Positions in List

                playerLocs[0] = p1.Destination;
                playerLocs[1] = p2.Destination;
                if (currMode == GameMode.FourPlayer || currMode == GameMode.ThreePlayer)
                {
                    playerLocs[2] = p3.Destination;

                    if (currMode == GameMode.FourPlayer)
                    {
                        playerLocs[3] = p4.Destination;
                    }
                }
                #endregion

                #region UPDATE calls
                if (!matchEnd)
                {
                    if (p1.ShieldsRemaining > 0) { p1.updateme(gameTime, currMap, padOneCurr, padOneOld); }
                    if (p2.ShieldsRemaining > 0) { p2.updateme(gameTime, currMap, padTwoCurr, padTwoOld); }

                    if (currMode == GameMode.FourPlayer || currMode == GameMode.ThreePlayer)
                    {
                        if (p3.ShieldsRemaining > 0) { p3.updateme(gameTime, currMap, padThreeCurr, padThreeOld); }

                        if (currMode == GameMode.FourPlayer)
                        {
                            if (p4.ShieldsRemaining > 0) { p4.updateme(gameTime, currMap, padFourCurr, padFourOld); }
                        }
                    }

                    cauldronList.ForEach(caul => caul.updateMe(gameTime));
                    PowerUpList.ForEach(pUp => pUp.updateMe(gameTime));
                    currMap.updateMe(caulLocs, playerLocs);
                }
                explosionList.ForEach(exp => exp.updateMe(gameTime, currMap));
                #endregion

                #region Item Drops

                itemPos = currMap.DropPosition;         //  if a new drop position has been determined then..
                if (itemPos != itemPosOld)              //
                {                                       //
                    DropItem(itemPos);                  //  Drop Item at newe location
                }                                       //
                #endregion

                #region PLAYERS PLACING CAULDERONS

                // this section deals with cauldron placement, for each player the code will check to see which way the player is facing
                // before proceeding to then check whether the player is closer to the positioon of its destination tile or the tile it departed from.
                // 
                // the tile determined to be closer becomes the target tile for the cauldron placement, the target tile is then checked to see if it already
                // contains a cauldron.. if it doesn't, a cauldron is placed. 
                //
                // This code is implemented for each player individualy 

                // Player 1
                if (padOneCurr.Buttons.A == ButtonState.Pressed && padOneOld.Buttons.A == ButtonState.Released && p1.CauldronsPlaced < p1.CauldronMax && p1.ShieldCount <= 0)
                {
                    Vector2 caulDest = Vector2.Zero;
                    switch (p1.Facing)
                    {
                        case Direction.East:
                            if (p1.Position.X > p1.Destination.X - 0.5f)
                            {
                                caulDest = p1.Destination;
                                break;
                            }
                            else
                            {
                                caulDest = new Vector2(p1.Destination.X - 1, p1.Destination.Y);
                                break;
                            }
                        case Direction.South:
                            if (p1.Position.Y > p1.Destination.Y - 0.5f)
                            {
                                caulDest = p1.Destination;
                                break;
                            }
                            else
                            {
                                caulDest = new Vector2(p1.Destination.X, p1.Destination.Y - 1);
                                break;
                            }
                        case Direction.West:
                            if (p1.Position.X < p1.Destination.X + 0.5f)
                            {
                                caulDest = p1.Destination;
                                break;
                            }
                            else
                            {
                                caulDest = new Vector2(p1.Destination.X + 1, p1.Destination.Y);
                                break;
                            }
                        case Direction.North:
                            if (p1.Position.Y < p1.Destination.Y + 0.5f)
                            {
                                caulDest = p1.Destination;
                                break;
                            }
                            else
                            {
                                caulDest = new Vector2(p1.Destination.X, p1.Destination.Y + 1);
                                break;
                            }
                    }
                    if (placementLegal(caulDest))
                    {
                        cauldronList.Add(new CauldronClass(Content.Load<Texture2D>("caulSheetAlt"), caulDest, 1, 4, 4, p1.ExplosionRange, 1));
                        p1.CauldronsPlaced++;
                        dropCaul.Play();
                    }
                }

                // PLAYER 2
                if (padTwoCurr.Buttons.A == ButtonState.Pressed && padTwoOld.Buttons.A == ButtonState.Released && p2.CauldronsPlaced < p2.CauldronMax && p2.ShieldCount <= 0)
                {
                    Vector2 caulDest = Vector2.Zero;
                    switch (p2.Facing)
                    {
                        case Direction.East:
                            if (p2.Position.X > p2.Destination.X - 0.5f)
                            {
                                caulDest = p2.Destination;
                                break;
                            }
                            else
                            {
                                caulDest = new Vector2(p2.Destination.X - 1, p2.Destination.Y);
                                break;
                            }
                        case Direction.South:
                            if (p2.Position.Y > p2.Destination.Y - 0.5f)
                            {
                                caulDest = p2.Destination;
                                break;
                            }
                            else
                            {
                                caulDest = new Vector2(p2.Destination.X, p2.Destination.Y - 1);
                                break;
                            }
                        case Direction.West:
                            if (p2.Position.X < p2.Destination.X + 0.5f)
                            {
                                caulDest = p2.Destination;
                                break;
                            }
                            else
                            {
                                caulDest = new Vector2(p2.Destination.X + 1, p2.Destination.Y);
                                break;
                            }
                        case Direction.North:
                            if (p2.Position.Y < p2.Destination.Y + 0.5f)
                            {
                                caulDest = p2.Destination;
                                break;
                            }
                            else
                            {
                                caulDest = new Vector2(p2.Destination.X, p2.Destination.Y + 1);
                                break;
                            }
                    }
                    if (placementLegal(caulDest))
                    {
                        cauldronList.Add(new CauldronClass(Content.Load<Texture2D>("caulSheetAlt"), caulDest, 1, 4, 4, p2.ExplosionRange, 2));
                        p2.CauldronsPlaced++;
                        dropCaul.Play();
                    }
                }

                if (currMode == GameMode.FourPlayer || currMode == GameMode.ThreePlayer)
                {
                    // Player 3
                    if (padThreeCurr.Buttons.A == ButtonState.Pressed && padThreeOld.Buttons.A == ButtonState.Released && p3.CauldronsPlaced < p3.CauldronMax && p3.ShieldCount <= 0)
                    {
                        Vector2 caulDest = Vector2.Zero;
                        switch (p3.Facing)
                        {
                            case Direction.East:
                                if (p3.Position.X > p3.Destination.X - 0.5f)
                                {
                                    caulDest = p3.Destination;
                                    break;
                                }
                                else
                                {
                                    caulDest = new Vector2(p3.Destination.X - 1, p3.Destination.Y);
                                    break;
                                }
                            case Direction.South:
                                if (p3.Position.Y > p3.Destination.Y - 0.5f)
                                {
                                    caulDest = p3.Destination;
                                    break;
                                }
                                else
                                {
                                    caulDest = new Vector2(p3.Destination.X, p3.Destination.Y - 1);
                                    break;
                                }
                            case Direction.West:
                                if (p3.Position.X < p3.Destination.X + 0.5f)
                                {
                                    caulDest = p3.Destination;
                                    break;
                                }
                                else
                                {
                                    caulDest = new Vector2(p3.Destination.X + 1, p3.Destination.Y);
                                    break;
                                }
                            case Direction.North:
                                if (p3.Position.Y < p3.Destination.Y + 0.5f)
                                {
                                    caulDest = p3.Destination;
                                    break;
                                }
                                else
                                {
                                    caulDest = new Vector2(p3.Destination.X, p3.Destination.Y + 1);
                                    break;
                                }
                        }
                        if (placementLegal(caulDest))
                        {
                            cauldronList.Add(new CauldronClass(Content.Load<Texture2D>("caulSheetAlt"), caulDest, 1, 4, 4, p3.ExplosionRange, 3));
                            p3.CauldronsPlaced++;
                            dropCaul.Play();
                        }
                    }

                    if (currMode == GameMode.FourPlayer)
                    {
                        // PLAYER 4
                        if (padFourCurr.Buttons.A == ButtonState.Pressed && padFourOld.Buttons.A == ButtonState.Released && p4.CauldronsPlaced < p4.CauldronMax && p4.ShieldCount <= 0)
                        {
                            Vector2 caulDest = Vector2.Zero;
                            switch (p4.Facing)
                            {
                                case Direction.East:
                                    if (p4.Position.X > p4.Destination.X - 0.5f)
                                    {
                                        caulDest = p4.Destination;
                                        break;
                                    }
                                    else
                                    {
                                        caulDest = new Vector2(p4.Destination.X - 1, p4.Destination.Y);
                                        break;
                                    }
                                case Direction.South:
                                    if (p4.Position.Y > p4.Destination.Y - 0.5f)
                                    {
                                        caulDest = p4.Destination;
                                        break;
                                    }
                                    else
                                    {
                                        caulDest = new Vector2(p4.Destination.X, p4.Destination.Y - 1);
                                        break;
                                    }
                                case Direction.West:
                                    if (p4.Position.X < p4.Destination.X + 0.5f)
                                    {
                                        caulDest = p4.Destination;
                                        break;
                                    }
                                    else
                                    {
                                        caulDest = new Vector2(p4.Destination.X + 1, p4.Destination.Y);
                                        break;
                                    }
                                case Direction.North:
                                    if (p4.Position.Y < p4.Destination.Y + 0.5f)
                                    {
                                        caulDest = p4.Destination;
                                        break;
                                    }
                                    else
                                    {
                                        caulDest = new Vector2(p4.Destination.X, p4.Destination.Y + 1);
                                        break;
                                    }
                            }
                            if (placementLegal(caulDest))
                            {
                                cauldronList.Add(new CauldronClass(Content.Load<Texture2D>("caulSheetAlt"), caulDest, 1, 4, 4, p4.ExplosionRange, 4));
                                p4.CauldronsPlaced++;
                                dropCaul.Play();
                            }
                        }
                    }
                }
                #endregion

                #region Update Player Lives 
                
                // this section of code updates the HUD display for each players Shields

                // Player One
                switch (p1.ShieldsRemaining)
                {
                    case 3:
                        p1ShieldState = threeShields;
                        break;
                    case 2:
                        p1ShieldState = twoShields;
                        break;
                    case 1:
                        p1ShieldState = oneShield;
                        break;
                    case 0:
                        p1ShieldState = lidTex;
                        break;    
                }
                // Player Two
                switch (p2.ShieldsRemaining)
                {
                    case 3:
                        p2ShieldState = threeShields;
                        break;
                    case 2:
                        p2ShieldState = twoShields;
                        break;
                    case 1:
                        p2ShieldState = oneShield;
                        break;
                    case 0:
                        p2ShieldState = lidTex;
                        break;
                }
                if (currMode == GameMode.FourPlayer || currMode == GameMode.ThreePlayer)
                {
                    //Player Three
                    switch (p3.ShieldsRemaining)
                    {
                        case 3:
                            p3ShieldState = threeShields;
                            break;
                        case 2:
                            p3ShieldState = twoShields;
                            break;
                        case 1:
                            p3ShieldState = oneShield;
                            break;
                        case 0:
                            p3ShieldState = lidTex;
                            break;
                    }

                    if (currMode == GameMode.FourPlayer)
                    {
                        // Player Four
                        switch (p4.ShieldsRemaining)
                        {
                            case 3:
                                p4ShieldState = threeShields;
                                break;
                            case 2:
                                p4ShieldState = twoShields;
                                break;
                            case 1:
                                p4ShieldState = oneShield;
                                break;
                            case 0:
                                p4ShieldState = lidTex;
                                break;
                        }
                    }
                }

                #endregion

                #region Explosion Behaviour 

                explosionList.ForEach(exp =>
                {
                    // Powerups are destroyed when colliding with explosions
                    PowerUpList.ForEach(pUp =>
                    {
                        if ((pUp.CollisionBox.Intersects(exp.Xrect) || pUp.CollisionBox.Intersects(exp.Yrect)) && pUp.ShieldTime <= 0)
                        {
                            pUp.Collected = true;
                        }
                    });

                    // Cauldrons are triggered when colliding with explosions
                    cauldronList.ForEach(caul =>
                    {
                        if (caul.CollisionBox.Intersects(exp.Xrect) || caul.CollisionBox.Intersects(exp.Yrect))
                        {
                            caul.Countdown = -1f;
                        }
                    });

                    // Players Take Damage when colliding with explosions
                    if ((p1.CollisionBox.Intersects(exp.Xrect) || p1.CollisionBox.Intersects(exp.Yrect)) && p1.ShieldCount <= 0)
                    {
                        if (p1.ShieldsRemaining > 1)
                        {
                            p1.Position = ResetPosition();
                            hitShield.Play();
                        }
                        else 
                        { 
                            p1.Position = new Vector2(-5, -5);
                            hitDie.Play();
                        }
                        p1.takeDamage();
                    }
                    if ((p2.CollisionBox.Intersects(exp.Xrect) || p2.CollisionBox.Intersects(exp.Yrect)) && p2.ShieldCount <= 0)
                    {
                        if (p2.ShieldsRemaining > 1)
                        {
                            p2.Position = ResetPosition();
                            hitShield.Play();
                        }
                        else 
                        { 
                            p2.Position = new Vector2(-6, -6);
                            hitDie.Play();
                        }
                        p2.takeDamage();
                    }
                    if (currMode == GameMode.FourPlayer || currMode == GameMode.ThreePlayer)
                    {
                        if ((p3.CollisionBox.Intersects(exp.Xrect) || p3.CollisionBox.Intersects(exp.Yrect)) && p3.ShieldCount <= 0)
                        {
                            if (p3.ShieldsRemaining > 1)
                            {
                                p3.Position = ResetPosition();
                                hitShield.Play();
                            }
                            else 
                            { 
                                p3.Position = new Vector2(-7, -7); 
                                hitDie.Play();
                            }
                            p3.takeDamage();
                        }

                        if (currMode == GameMode.FourPlayer)
                        {
                            if ((p4.CollisionBox.Intersects(exp.Xrect) || p4.CollisionBox.Intersects(exp.Yrect)) && p4.ShieldCount <= 0)
                            {
                                if (p4.ShieldsRemaining > 1)
                                {
                                    p4.Position = ResetPosition();
                                    hitShield.Play();
                                }
                                else 
                                { 
                                    p4.Position = new Vector2(-8, -8);
                                    hitDie.Play();
                                }
                                p4.takeDamage();
                            }
                        }
                    }
                });
                explosionList.RemoveAll(exp => exp.ExpCount < 0);   // remove spent explosions

                #endregion

                #region Players Collect Pick Ups

                PowerUpList.ForEach(pUp =>
                {
                    // this section of code determines whether a player has collided with an upgrade 
                    // and applies the bonus if valid. speed is capped.

                    // PLAYER 1
                    if (p1.CollisionBox.Intersects(pUp.CollisionBox))
                    {
                        switch (pUp.Type)   // TYPES: "SpeedBoost", "ExtraCaul", "IncreaseRange"
                        {
                            case "SpeedBoost":
                                if (p1.Speed <= 0.08f)
                                {
                                    p1.Speed += 0.02f;
                                }
                                break;
                            case "ExtraCaul":
                                p1.CauldronMax++;
                                break;
                            case "IncreaseRange":
                                p1.ExplosionRange++;
                                break;
                        }
                        pUp.Collected = true;
                        pickUp.Play();
                    }

                    // PLAYER 2
                    if (p2.CollisionBox.Intersects(pUp.CollisionBox))
                    {
                        switch (pUp.Type)   // TYPES: "SpeedBoost", "ExtraCaul", "IncreaseRange"
                        {
                            case "SpeedBoost":
                                if (p2.Speed <= 0.08f)
                                {
                                    p2.Speed += 0.02f;
                                }
                                break;
                            case "ExtraCaul":
                                p2.CauldronMax++;
                                break;
                            case "IncreaseRange":
                                p2.ExplosionRange++;
                                break;
                        }
                        pUp.Collected = true;
                        pickUp.Play();
                    }

                    if (currMode == GameMode.FourPlayer || currMode == GameMode.ThreePlayer)
                    {
                        // PLAYER 3
                        if (p3.CollisionBox.Intersects(pUp.CollisionBox))
                        {
                            switch (pUp.Type)   // TYPES: "SpeedBoost", "ExtraCaul", "IncreaseRange"
                            {
                                case "SpeedBoost":
                                    if (p3.Speed <= 0.08f)
                                    {
                                        p3.Speed += 0.02f;
                                    }
                                    break;
                                case "ExtraCaul":
                                    p3.CauldronMax++;
                                    break;
                                case "IncreaseRange":
                                    p3.ExplosionRange++;
                                    break;
                            }
                            pUp.Collected = true;
                            pickUp.Play();
                        }

                        if (currMode == GameMode.FourPlayer)
                        {
                            // PLAYER 4
                            if (p4.CollisionBox.Intersects(pUp.CollisionBox))
                            {
                                switch (pUp.Type)   // TYPES: "SpeedBoost", "ExtraCaul", "IncreaseRange"
                                {
                                    case "SpeedBoost":
                                        if (p4.Speed <= 0.08f)
                                        {
                                            p4.Speed += 0.02f;
                                        }
                                        break;
                                    case "ExtraCaul":
                                        p4.CauldronMax++;
                                        break;
                                    case "IncreaseRange":
                                        p4.ExplosionRange++;
                                        break;
                                }
                                pUp.Collected = true;
                                pickUp.Play();
                            }
                        }
                    }
                });
                PowerUpList.RemoveAll(pUp => pUp.Collected == true);

                #endregion

                #region Detonate Cauldrons

                //  this section detonates cauldrons and triggers an explosion when the cauldron count variable hits zero
                //  the ID field is used to reduce the active cauldron count for the appropriate player.
                //
                //  this section is also used to keep the caulLocs List up to date which stores the locations of all active cauldrons

                cauldronList.ForEach(cauls =>
                {
                    if (cauls.Countdown < 0)
                    {
                        explosionList.Add(new ExplosionClass(Content.Load<Texture2D>("PotionExplode"), cauls.Position, 1, 4, 4, cauls.ExplosionRange, beamTexMain, beamTexEnd, beamTexVertMain, beamTexVertEnd));
                        caulExp.Play(0.6f,0,0);

                        switch (cauls.BelongsTo)
                        {
                            case 1:
                                p1.CauldronsPlaced--;
                                break;
                            case 2:
                                p2.CauldronsPlaced--;
                                break;
                            case 3:
                                p3.CauldronsPlaced--;
                                break;
                            case 4:
                                p4.CauldronsPlaced--;
                                break;
                        }
                        
                        caulLocs.RemoveAll(c => c == cauls.Position);
                    }
                    else
                    {
                        caulLocs.Add(cauls.Position);
                    }
                });
                cauldronList.RemoveAll(c => c.Countdown < 0);

                #endregion

                #region Set Match End Scenario

                // this section sets the circumstances that will end a match based on which mode is selected
                // and determines which player should be set as the winner based on each scenario.

                if (currMode == GameMode.TwoPlayer)
                {
                    if (p1.ShieldsRemaining == 0)
                    {
                        matchEnd = true;
                        winner = "Player 2";
                    }
                    else if (p2.ShieldsRemaining == 0)
                    {
                        matchEnd = true;
                        winner = "Player 1";
                    }
                }
                else if (currMode == GameMode.ThreePlayer)
                {
                    if (p1.ShieldsRemaining == 0 && p2.ShieldsRemaining == 0)
                    {
                        matchEnd = true;
                        winner = "Player 3";
                    }
                    else if (p1.ShieldsRemaining == 0 && p3.ShieldsRemaining == 0)
                    {
                        matchEnd = true;
                        winner = "Player 2";
                    }
                    else if (p2.ShieldsRemaining == 0 && p3.ShieldsRemaining == 0)
                    {
                        matchEnd = true;
                        winner = "Player 1";
                    }
                }
                else if (currMode == GameMode.FourPlayer)
                {
                    if (p1.ShieldsRemaining == 0 && p2.ShieldsRemaining == 0 && p3.ShieldsRemaining == 0)
                    {
                        matchEnd = true;
                        winner = "Player 4";
                    }
                    else if (p1.ShieldsRemaining == 0 && p2.ShieldsRemaining == 0 && p4.ShieldsRemaining == 0)
                    {
                        matchEnd = true;
                        winner = "Player 3";
                    }
                    else if (p1.ShieldsRemaining == 0 && p3.ShieldsRemaining == 0 && p4.ShieldsRemaining == 0)
                    {
                        matchEnd = true;
                        winner = "Player 2";
                    }
                    else if (p2.ShieldsRemaining == 0 && p3.ShieldsRemaining == 0 && p4.ShieldsRemaining == 0)
                    {
                        matchEnd = true;
                        winner = "Player 1";
                    }
                }

                #endregion

                #region Match End

                //  this section deals with displayign the gameover message based on a timer and then
                //  triggering the move to the win screen (game over state)

                if (matchEnd)
                {
                    matchEndCount -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (!gameShown)
                    {
                        gameTitle = new StaticGraphic(new Vector2(_graphics.PreferredBackBufferWidth / 2 - 260, 350), Content.Load<Texture2D>("Game"));
                        gameOver.Play();
                        gameShown = true;
                    }
                    if (!overShown && matchEndCount < 2)
                    {
                        overTitle = new StaticGraphic(new Vector2(_graphics.PreferredBackBufferWidth / 2 - 244, 550), Content.Load<Texture2D>("Over"));
                        overShown = true;
                    }

                    if (matchEndCount <= 0)
                    {
                        currState = GameState.GameOver;
                        hasLoaded = false;
                    }
                }
                #endregion
            }

            #endregion


            #region GameOver

            runeList.ForEach(r => r.updateme(new Vector2(0, 2))); // updates the rune list and passes a velocity vector

            if (currState == GameState.GameOver)
            {
                if (!hasLoaded)                                     //  Loads content if required
                {                                                   //
                    LoadContent();                                  //
                    camera.Position = new Vector2(0, 0);            //
                    endTheme.Play(0.8f,0,0);                                //
                    hasLoaded = true;                               //
                }                                                   //

                // the section below sets up the winner particles. the winning players face is shot out of the center of a rotating explosion multiple times 

                if (starticleCountdown > 0)
                {
                    starticleCountdown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    switch (winner) 
                    {
                        case "Player 1":
                            starticleList.Add(new Rotating2D(Content.Load<Texture2D>("WW_UI_GreenWiz"), new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), -0.05f, true));
                            break;
                        case "Player 2":
                            starticleList.Add(new Rotating2D(Content.Load<Texture2D>("WW_UI_GoldWiz"), new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), -0.05f, true));
                            break;
                        case "Player 3":
                            starticleList.Add(new Rotating2D(Content.Load<Texture2D>("WW_UI_BlueWiz"), new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), -0.05f, true));
                            break;
                        case "Player 4":
                            starticleList.Add(new Rotating2D(Content.Load<Texture2D>("WW_UI_BrownWiz"), new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), -0.05f, true));
                            break;
                    }
                    starticleList.Add(new Rotating2D(Content.Load<Texture2D>("PotionExplode"), new Vector2(RNG.Next(50, 1870), RNG.Next(50, 1030)), -0.05f, true));
                    starticleCountdown = 0.5f;                                                                                                                            
                }                                                                                                                                                         
                starticleList.RemoveAll(s => s.Life <= 0);

                // the section below regulates the runes that slowly move in the background of the Win Screen 

                if (runeCountDown > 0)
                {
                    runeCountDown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    int runeID = RNG.Next(1, 4);

                    switch (runeID)
                    {
                        case 1:
                            runeList.Add(new MotionGraphic(new Vector2(RNG.Next(0, 1800), -200), Content.Load<Texture2D>("Shape1")));
                            break;
                        case 2:
                            runeList.Add(new MotionGraphic(new Vector2(RNG.Next(0, 1800), -200), Content.Load<Texture2D>("Shape2")));
                            break;
                        case 3:
                            runeList.Add(new MotionGraphic(new Vector2(RNG.Next(0, 1800), -200), Content.Load<Texture2D>("Shape3")));
                            break;
                    }

                    runeCountDown = 2;
                }
                runeList.RemoveAll(runes => runes.Position.Y > _graphics.PreferredBackBufferHeight + 250);

                // the section below resets the game when the player presses start

                if (padOneCurr.Buttons.Start == ButtonState.Pressed && padOneOld.Buttons.Start == ButtonState.Released || padTwoCurr.Buttons.Start == ButtonState.Pressed && padTwoOld.Buttons.Start == ButtonState.Released)
                {
                    starticleList.RemoveAll(s => s.Life >= 0);
                    currState = GameState.StartScreen;
                    hasLoaded = false;
                }
            }

            #endregion

            padOneOld = padOneCurr;     // archive padCurrs previous state for edge detection 
            padTwoOld = padTwoCurr;     //
            padThreeOld = padOneCurr;   //
            padFourOld = padTwoCurr;    //

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.getCam());

            #region Start Screen
            if (currState == GameState.StartScreen)
            {
                startBackGround.drawme(_spriteBatch);
                starticleList.ForEach(s => s.drawme(_spriteBatch));
                startVortex.drawme(_spriteBatch);
                startTitle.drawme(_spriteBatch);

                if (!isModeSelect)
                {
                    startGameBut.drawme(_spriteBatch);
                    instructBut.drawme(_spriteBatch);
                }
                else
                {
                    twoBut.drawme(_spriteBatch);
                    threeBut.drawme(_spriteBatch);
                    fourBut.drawme(_spriteBatch);
                }

                cursorSprite.drawme(_spriteBatch, gameTime, cursorPos);

                if (showInfo)
                {
                    infoScreen.drawme(_spriteBatch);
                }
            }
            #endregion

            #region InGame
            if (currState == GameState.Ingame)
            {
                currMap.drawme(_spriteBatch, tiles);
                cauldronList.ForEach(caul => caul.drawme(_spriteBatch, gameTime));
                PowerUpList.ForEach(pUp => pUp.drawme(_spriteBatch));

                explosionList.ForEach(exp => exp.drawme(_spriteBatch, gameTime, currMap));
                explosionList.ForEach(exp => exp.drawme(_spriteBatch));

                p1.drawme(_spriteBatch, gameTime);
                p2.drawme(_spriteBatch, gameTime);
                if (currMode == GameMode.FourPlayer || currMode == GameMode.ThreePlayer)
                {
                    p3.drawme(_spriteBatch, gameTime);

                    if (currMode == GameMode.FourPlayer)
                    {
                        p4.drawme(_spriteBatch, gameTime);
                    }
                }
                
                p1HUD.drawme(_spriteBatch, p1ShieldState, p1);
                p2HUD.drawme(_spriteBatch, p2ShieldState, p2);
                if (currMode == GameMode.FourPlayer || currMode == GameMode.ThreePlayer)
                {
                    p3HUD.drawme(_spriteBatch, p3ShieldState, p3);

                    if (currMode == GameMode.FourPlayer)
                    {
                        p4HUD.drawme(_spriteBatch, p4ShieldState, p4);
                    }
                }
                if (gameShown) { gameTitle.drawme(_spriteBatch); }
                if (overShown) { overTitle.drawme(_spriteBatch); }
            }
            #endregion

            #region Game Over

            if (currState == GameState.GameOver)
            {
                runeList.ForEach(r => r.drawme(_spriteBatch));

                charWin.drawme(_spriteBatch);
                starticleList.ForEach(s => s.drawme(_spriteBatch));

                _spriteBatch.DrawString(bigFont, "" + winner, new Vector2(_graphics.PreferredBackBufferWidth / 2 - 175, 100), Color.White);
                _spriteBatch.DrawString(bigFont, "WINS", new Vector2(_graphics.PreferredBackBufferWidth / 2 - 120, 820), Color.White);
                _spriteBatch.DrawString(debugFont, "PRESS START TO PLAY AGAIN", new Vector2(_graphics.PreferredBackBufferWidth / 2 - 135, 970), Color.White);
            }
            #endregion

            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
