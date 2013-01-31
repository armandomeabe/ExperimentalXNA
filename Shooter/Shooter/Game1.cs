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
        ObjetoDibujableAnimado Personaje;

        List<ObjetoDibujableAnimado> Enemigos;
        //ObjetoDibujableAnimado Enemigo;

        // La textura del fondo estático
        ObjetoDibujable FondoPrincipal;
        // Parallaxing backs
        ParallaxingBackground bgLayer1;
        ParallaxingBackground bgLayer2;

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
            bgLayer1 = new ParallaxingBackground();
            bgLayer2 = new ParallaxingBackground();
            Enemigos = new List<ObjetoDibujableAnimado>();
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

            // Sobre el personaje. Este código está un poquito sucio che...
            this.Personaje = new ObjetoDibujableAnimado(Content.Load<Texture2D>("shipAnimation"), new Vector2(150));
            var AnimacionPersonaje = new Animacion();
            AnimacionPersonaje.Initialize(Personaje.Textura, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);
            Personaje.AnimacionObjeto = AnimacionPersonaje;
            
            // Cargamos los parallaxing backs (Como traduzco esto?)
            bgLayer1.Initialize(Content, "bgLayer1", GraphicsDevice.Viewport.Width, -1);
            bgLayer2.Initialize(Content, "bgLayer2", GraphicsDevice.Viewport.Width, -2);

            FondoPrincipal = new  ObjetoDibujable(Content.Load<Texture2D>("mainbackground"), Vector2.Zero);

            NuevoEnemigo();
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            // Teclado ó DPad
            if (Keyboard.GetState().IsKeyDown(Keys.Left) || GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed)
                Personaje.Posicion.X -= Personaje.Velocidad;
            if (Keyboard.GetState().IsKeyDown(Keys.Right) || GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed)
                Personaje.Posicion.X += Personaje.Velocidad;
            if (Keyboard.GetState().IsKeyDown(Keys.Up) || GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)
                Personaje.Posicion.Y -= Personaje.Velocidad;
            if (Keyboard.GetState().IsKeyDown(Keys.Down) || GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)
                Personaje.Posicion.Y += Personaje.Velocidad;

            // Stick analógico :D
            Personaje.Posicion = new Vector2(
                Personaje.Posicion.X + GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X * 15,
                Personaje.Posicion.Y + -1 * GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y * 15);

            Personaje.Update(gameTime);
            foreach (var Enemigo in Enemigos)
            {
                Enemigo.MoverRelativo(-1, (new Random()).Next(-1,1));
                Enemigo.Update(gameTime);
                Enemigo.RecibirDanios();
            }

            // Asegurarse que el personaje no se escapa de la pantalla. "Clamp" significa algo así como "Abrazadera".
            Personaje.NoHuirDeLaVentana(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            // Fondos animados
            bgLayer1.Update();
            bgLayer2.Update();

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
            
            // Fondos
            FondoPrincipal.Draw(this.spriteBatch);
            bgLayer1.Draw(spriteBatch);
            bgLayer2.Draw(spriteBatch);

            Personaje.Draw(this.spriteBatch);
            foreach (var Enemigo in Enemigos)
            {
                Enemigo.Draw(this.spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void NuevoEnemigo(string Textura = "mineAnimation")
        {
            var R = new Random(DateTime.Now.Millisecond);
            Vector2 posicionAleatoria = new Vector2(R.Next(0, GraphicsDevice.Viewport.Width), R.Next(0, GraphicsDevice.Viewport.Height));
            var Enemigo = new ObjetoDibujableAnimado(Content.Load<Texture2D>(Textura), posicionAleatoria);
            var AnimacionEnemigo = new Animacion();
            AnimacionEnemigo.Initialize(Enemigo.Textura, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);
            Enemigo.AnimacionObjeto = AnimacionEnemigo;
            Enemigos.Add(Enemigo);
        }
    }
}
