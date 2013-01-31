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

namespace Shooter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ObjetoDibujable Fondo;
        ObjetoDibujable Personaje;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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

            this.Fondo = new ObjetoDibujable(Content.Load<Texture2D>("bgLayer1"), Vector2.Zero);
            this.Personaje = new ObjetoDibujable(Content.Load<Texture2D>("player"), Vector2.Zero);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            

            // TODO: Add your update logic here
            // Move the sprite by speed, scaled by elapsed time.
            //Personaje.Posicion += Personaje.Velocidad *(float)gameTime.ElapsedGameTime.TotalSeconds;

            Personaje.Posicion = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            int MaxX = graphics.GraphicsDevice.Viewport.Width - Personaje.Textura.Width;
            int MinX = 0;
            int MaxY = graphics.GraphicsDevice.Viewport.Height - Personaje.Textura.Height;
            int MinY = 0;
            
            // Check for bounce.
            if (Personaje.Posicion.X > MaxX)
            {
                Personaje.Velocidad.X *= -1;
                //Personaje.Posicion.X = MaxX;
            }

            else if (Personaje.Posicion.X < MinX)
            {
                Personaje.Velocidad.X *= -1;
                //spritePosition.X = MinX;
            }

            if (Personaje.Posicion.Y > MaxY)
            {
                Personaje.Velocidad.Y *= -1;
                //spritePosition.Y = MaxY;
            }

            else if (Personaje.Posicion.Y < MinY)
            {
                Personaje.Velocidad.Y *= -1;
                //spritePosition.Y = MinY;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(Fondo.Textura, Fondo.Posicion, Color.White);
            spriteBatch.Draw(Personaje.Textura, Personaje.Posicion, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
