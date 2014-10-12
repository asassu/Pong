using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ATLS_4519_Lab4
{
    /// <summary> 
    /// This is the main type for your game 
    /// </summary> 

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState oldState;

        // Create a SoundEffect resource 
        //SoundEffect soundEffect;

        //Heres where we access the sound stuff - Michael 3-12
        AudioEngine audioEngine;
        SoundBank soundBank;
        WaveBank waveBank;
        WaveBank streamingWaveBank;
        Cue musicCue;
        Cue cue;

        int bounceCounter = 0;

        bool gamePaused = true;
        bool gameStartScreen = true;

        Random rnd = new Random();

        // Font stuff!
        SpriteFont Font1;
        Vector2 FontPos;
        public string victory; // used to hold the congratulations/blnt message

        // Sprite objects 
        clsSprite ball1;
        clsSprite rightPaddle;
        clsSprite leftPaddle;

        //Victory boolean
        bool pointPlayer = false; //the player has earned a point
        bool pointComputer = false; //the AI has earned a point
        bool resetBall = false; //a point has been earned, reset ball and apply points

        const int WINDOW_WIDTH = 1600;
        const int WINDOW_HEIGHT = 800;


        string beginNotice = "Welcome to Pong! Hit ENTER to begin.";
        string resetNotice = "To restart the game, hit the ENTER button.";

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // changing the back buffer size changes the window size (when in windowed mode) 
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
        }

        /// <summary> 
        /// Allows the game to perform any initialization it needs to before starting to run. 
        /// This is where it can query for any required services and load any non-graphic 
        /// related content. Calling base.Initialize will enumerate through any components 
        /// and initialize them as well. 
        /// </summary> 
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here 

            base.Initialize();
            oldState = Keyboard.GetState();
        }


        /// <summary> 
        /// LoadContent will be called once per game and is the place to load 
        /// all of your content. 
        /// </summary> 
        protected override void LoadContent()
        {
            Font1 = Content.Load<SpriteFont>("Courier New");

            // Load the SoundEffect resource 
            //soundEffect = Content.Load<SoundEffect>("bloop");

            // Load files built from XACT project - Michael 3-12
            audioEngine = new AudioEngine("Content\\soundstuff.xgs");
            waveBank = new WaveBank(audioEngine, "Content\\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content\\Sound Bank.xsb");
            // Load streaming wave bank 
            streamingWaveBank = new WaveBank(audioEngine, "Content\\Music.xwb", 0, 4);
            // The audio engine must be updated before the streaming cue is ready 
            audioEngine.Update();
            // Get cue for streaming music 
            musicCue = soundBank.GetCue("Seans_Adventure");
            // Start the background music 
            musicCue.Play();
            // Get an instance of the cue from the XACT project 
            //cue = soundBank.GetCue("Default_Bump");



            // Create a new SpriteBatch, which can be used to draw textures. 
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ball1 = new clsSprite(Content.Load<Texture2D>("starryball"), new Vector2(WINDOW_WIDTH / 2, WINDOW_HEIGHT / 2), new
           Vector2(64f, 64f),
           graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            rightPaddle = new clsSprite(Content.Load<Texture2D>("nightpaddleRight"), new Vector2(WINDOW_WIDTH - 100, 118f), new
            Vector2(64f, 64f),
            graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            //Added Left Paddle
            leftPaddle = new clsSprite(Content.Load<Texture2D>("nightpaddleLeft"), new Vector2(100, 118f), new
            Vector2(64f, 64f),
            graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            // set the speed the sprites will move 
            //ball1.velocity = new Vector2(5, 5);
            int angleVariable = rnd.Next(6, 13);
            Vector2 randomHelper = new Vector2(-5, angleVariable);
            ball1.velocity = randomHelper;
        }

        /// <summary> 
        /// UnloadContent will be called once per game and is the place to unload 
        /// all content. 
        /// </summary> 
        protected override void UnloadContent()
        {
            // Free the previously alocated resources 
            ball1.texture.Dispose();
            rightPaddle.texture.Dispose();
            leftPaddle.texture.Dispose();
        }

        /// <summary> 
        /// Allows the game to run logic such as updating the world, 
        /// checking for collisions, gathering input, and playing audio. 
        /// </summary> 
        /// <param name="gameTime">Provides a snapshot of timing values.</param> 
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit 
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //Updating keyboard state
            KeyboardState keyboardState = Keyboard.GetState();

            // Is the ENTER key down?
            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.Enter))
                {
                    ball1.position = new Vector2(WINDOW_WIDTH / 2, WINDOW_HEIGHT / 2);
                    //ball1.velocity = new Vector2(5, 5);
                    //Modfied initial ball angle to be slightly random - Michael 3/11
                    int angleVariable = rnd.Next(6, 13);
                    Vector2 randomHelper = new Vector2(-5, angleVariable);
                    ball1.velocity = randomHelper;
                    ball1.velocity *= -1;
                    ball1.scorePlayer = 0;
                    ball1.scoreComputer = 0;
                    gamePaused = false;
                    gameStartScreen = false;
                    bounceCounter = 0;
                }
            }
            else if (oldState.IsKeyDown(Keys.Enter))
            {
                // Key was down last update, but not down now, so
                // it has just been released.
            }

            // Update saved state.
            oldState = keyboardState;

            if(gamePaused == false)
            {
                if(bounceCounter > 10)
                {
                    cue = soundBank.GetCue("Sunny_Bump");
                }
                else
                {
                    cue = soundBank.GetCue("Default_Bump");
                }

                // Move the sprite 
                ball1.Move();
                if (ball1.hitWall)
                {
                    cue.Play();
                }

                // Change the sprite 2 position using the left thumbstick 
                //Vector2 LeftThumb = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left; 
                //rightPaddle.position += new Vector2(LeftThumb.X, -LeftThumb.Y) * 5; 

                // Change the sprite 2 position using the keyboard 
                Vector2 window_checker = new Vector2(0, WINDOW_HEIGHT);


                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    if (rightPaddle.position.Y - 5 > 0)
                    {
                        rightPaddle.position += new Vector2(0, -5);
                    }
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    if (rightPaddle.position.Y + 5 + rightPaddle.size.Y < WINDOW_HEIGHT)
                    {
                        rightPaddle.position += new Vector2(0, 5);
                    }
                }

                // Make sprite 2 follow the mouse 
                //if (rightPaddle.position.X < Mouse.GetState().X) 
                // rightPaddle.position += new Vector2(5, 0); 
                //if (rightPaddle.position.X > Mouse.GetState().X) 
                // rightPaddle.position += new Vector2(-5, 0); 
                //if (rightPaddle.position.Y < Mouse.GetState().Y) 
                // rightPaddle.position += new Vector2(0, 5); 
                //if (rightPaddle.position.Y > Mouse.GetState().Y) 
                // rightPaddle.position += new Vector2(0, -5); 


                if (ball1.Collides1(rightPaddle))
                {
                    //soundEffect.Play();

                    bounceCounter += 1;

                    int angleVariable;
                    int speedVariable;
                    Vector2 randomHelper;
                    //soundEffect1.Play(); 
                    cue = soundBank.GetCue("Default_Bump");

                    if (ball1.scorePlayer >= 3)
                    {
                        angleVariable = rnd.Next(11, 18);
                        randomHelper = new Vector2(5, angleVariable);

                    }
                    else if (bounceCounter > 10)
                    {
                        angleVariable = rnd.Next(3, 25);
                        speedVariable = rnd.Next(2,7);
                        randomHelper = new Vector2(speedVariable, angleVariable);
                        cue = soundBank.GetCue("Sunny_Bump");
                    }
                    else
                    {
                        angleVariable = rnd.Next(8, 13);
                        randomHelper = new Vector2(5, angleVariable);
                        
                    }

                    cue.Play();

                    //int[] randomNumbers = new int[] { 5, 10, 15 };
                    //int nextIndex = rnd.Next(0, randomNumbers.Count() - 1);
                    ball1.velocity = randomHelper;
                    ball1.velocity *= -1;

                    GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
                }
                else
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);

                if (ball1.Collides2(leftPaddle))
                {
                    //soundEffect.Play();

                    bounceCounter += 1;

                   // int[] randomNumbers = new int[] { 5, 10, 15 };
                   // int nextIndex = rnd.Next(0, randomNumbers.Count() - 1);

                    int angleVariable;
                    int speedVariable;
                    Vector2 randomHelper;
                    //Play sound effect

                    cue = soundBank.GetCue("Default_Bump");
                    if (ball1.scorePlayer >= 3)
                    {
                        angleVariable = rnd.Next(11, 18);
                        randomHelper = new Vector2(-5, angleVariable);
                    }
                    else if (bounceCounter > 10)
                    {
                        angleVariable = rnd.Next(3, 25);
                        speedVariable = rnd.Next(2, 7);
                        randomHelper = new Vector2(-speedVariable, angleVariable);
                        cue = soundBank.GetCue("Sunny_Bump");
                    }
                    else
                    {
                        angleVariable = rnd.Next(6, 13);
                        randomHelper = new Vector2(-5, angleVariable);
                    }
                    cue.Play(); 

                    ball1.velocity = randomHelper;
                    ball1.velocity *= -1;
                }

                if (ball1.position.X > WINDOW_WIDTH + ball1.size.X)
                {
                    resetBall = true;
                    pointComputer = true;

                }
                //If ball moves past AI's goal, add one point to Player's Score
                if (ball1.position.X < 0)
                {
                    resetBall = true;
                    pointPlayer = true;
                }

                //left Paddle AI - Michael 10:00am 2/20
                //AI typically looks for the ball when it's past 1/3 of the screen.
                //If the play is doing poorly and the computer is doing well, this is shortened
                //to only be the closest 1/4 of the screen.
                int windowRatio = 3;
                if(ball1.scoreComputer >= 4 && ball1.scorePlayer <= 3)
                {
                    windowRatio = 4;
                }
                if (ball1.position.X < WINDOW_WIDTH / windowRatio)
                {
                    //Figure out leftPaddle Y position. If it is above ball, go down. If it is below ball, go up.
                    if (leftPaddle.position.Y > ball1.position.Y)
                    {
                        leftPaddle.position += new Vector2(0, -5);
                    }
                    else
                    {
                        leftPaddle.position += new Vector2(0, +5);
                    }

                }

            }

            victory = "GAME OVER. Your Score: " + ball1.scorePlayer + " Computer Score: " + ball1.scoreComputer;

            //Update the audio engine - Michael 3-12
            audioEngine.Update(); 

            base.Update(gameTime);
        }

        /// <summary> 
        /// This is called when the game should draw itself. 
        /// </summary> 
        /// <param name="gameTime">Provides a snapshot of timing values.</param> 
        protected override void Draw(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            // Draw running score string

            spriteBatch.DrawString(Font1, "Computer: " + ball1.scoreComputer, new Vector2(5, 10), Color.Black);

            spriteBatch.DrawString(Font1, "Player: " + ball1.scorePlayer,
                new Vector2(graphics.GraphicsDevice.Viewport.Width - Font1.MeasureString("Player: " + ball1.scorePlayer).X - 5, 10), Color.Black);

            if(gameStartScreen)
            {
                FontPos = new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - 300,
                (graphics.GraphicsDevice.Viewport.Height / 2) - 50);
                spriteBatch.DrawString(Font1, beginNotice, FontPos, Color.Black);
            }

            if (resetBall) //point given, but game not over
            {
                if (pointComputer)
                {
                    ball1.scoreComputer += 1;
                }
                else if (pointPlayer)
                {
                    ball1.scorePlayer += 1;
                }

                //Opponent Difficulty AI - Michael 3/11
                int angleVariable;
                if (ball1.scorePlayer >= 3)
                {
                    //Put something here to make the computer a bit harder. Faster speed and wider reaction window
                    angleVariable = rnd.Next(12, 18);
                }
                else
                {
                    angleVariable = rnd.Next(6, 13);

                }
                Vector2 randomHelper = new Vector2(-5, angleVariable);
                ball1.velocity = randomHelper;
                ball1.velocity *= -1;
                ball1.position = new Vector2(WINDOW_WIDTH / 2, WINDOW_HEIGHT / 2);
                //ball1.velocity = new Vector2(5, 5);
                //Modified ball initial velocity angle to be slightly random - Michael 3/11

                resetBall = false;
                pointComputer = false;
                pointPlayer = false;
                bounceCounter = 0;

            }

            if (ball1.scoreComputer == 5 || ball1.scorePlayer == 5)
            {
                FontPos = new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - 300,
                (graphics.GraphicsDevice.Viewport.Height / 2) - 50);
                spriteBatch.DrawString(Font1, victory, FontPos, Color.Black);

                FontPos = new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - 300,
                (graphics.GraphicsDevice.Viewport.Height / 2));
                spriteBatch.DrawString(Font1, resetNotice, FontPos, Color.Black);

                spriteBatch.End();

                gamePaused = true;
                
            }
            else
            {
                // Draw the sprites 
                //spriteBatch.Begin();
                ball1.Draw(spriteBatch);
                rightPaddle.Draw(spriteBatch);
                leftPaddle.Draw(spriteBatch);
                spriteBatch.End();
            }
            

            base.Draw(gameTime);
        }
    }
}
