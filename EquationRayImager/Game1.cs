using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace EquationRayImager
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public const int WIDTH = 512;
        public const int HEIGHT = 512;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D texture;
        Camera camera;
        Vector3 newCamPos;
        Quaternion newCamRot;
        Effect effect;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            texture = new Texture2D(GraphicsDevice, WIDTH, HEIGHT);
            camera = new Camera(new Vector3(5, 5, 6), new Vector3(0, 0, 0));

            Color[] colors = new Color[WIDTH * HEIGHT];

            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    colors[x + y * WIDTH] = new Color(0, 0, 0);
                }
            }

            texture.SetData(colors);

            ImageGenerator.CompileKernel();

            Mouse.SetPosition(WIDTH / 2, HEIGHT / 2);
            //IsMouseVisible = false;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //effect = Content.Load<Effect>("EquationPixelShader");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Point change = Mouse.GetState().Position - new Point(WIDTH/2, HEIGHT/2);
           
            var y = Quaternion.CreateFromAxisAngle(-Vector3.Right, change.Y * MathHelper.ToRadians(0.5f));
            var x = Quaternion.CreateFromAxisAngle(Vector3.Up, change.X * MathHelper.ToRadians(0.5f));

            newCamRot = camera.Rotation * x * y;

            camera.Rotation = newCamRot;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            Vector3 moveVector = new Vector3(0, 0, 0);
            KeyboardState keyState = Keyboard.GetState();
            /*if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
                moveVector += new Vector3(0, 0, -1);
            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
                moveVector += new Vector3(0, 0, 1);
            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
                moveVector += new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
                moveVector += new Vector3(-1, 0, 0);
            if (keyState.IsKeyDown(Keys.Q))
                moveVector += new Vector3(0, 1, 0);
            if (keyState.IsKeyDown(Keys.Z))
                moveVector += new Vector3(0, -1, 0);*/

            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
                moveVector += camera.Forward;
            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
                moveVector += -camera.Forward;
            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
                moveVector += camera.Left;
            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
                moveVector += -camera.Left;
            if (keyState.IsKeyDown(Keys.Q))
                moveVector += camera.Up;
            if (keyState.IsKeyDown(Keys.Z))
                moveVector += -camera.Up;

            //var p = Camera.ApplyQuaternion(camera.Position, q);
            newCamPos = camera.Position + moveVector * (float)gameTime.ElapsedGameTime.TotalSeconds * 2;

            // TODO: Add your update logic here

            Mouse.SetPosition(WIDTH / 2, HEIGHT / 2);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            camera.Position = newCamPos;

            Color[] colors = ImageGenerator.CalcGPU(WIDTH, HEIGHT, camera, null);
            
            texture.SetData(colors);


            spriteBatch.Begin();
            spriteBatch.Draw(texture, new Rectangle(0, 0, WIDTH, HEIGHT), Color.White);
            spriteBatch.End();

            /*effect.Parameters["campos"].SetValue(camera.Position);
            effect.Parameters["near"].SetValue(camera.Near);
            effect.Parameters["left"].SetValue(camera.Left);
            effect.Parameters["up"].SetValue(camera.Up);
            effect.Parameters["forward"].SetValue(camera.Forward);
            effect.Parameters["width"].SetValue(WIDTH);
            effect.Parameters["height"].SetValue(HEIGHT);
            //effect.Parameters["interations"].SetValue(50);

            spriteBatch.Begin(effect: effect);
            spriteBatch.Draw(texture, new Rectangle(0, 0, WIDTH, HEIGHT), Color.White);
            spriteBatch.End();
            // TODO: Add your drawing code here
            */
            base.Draw(gameTime);
        }
    }
}
