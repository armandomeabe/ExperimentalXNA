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
using Microsoft.Xna.Framework.Input.Touch;

namespace Shooter
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //box2d
        Box2DX.Common.Vec2 gravedad = new Box2DX.Common.Vec2(0f, -10.0f);
        Box2DX.Dynamics.World world = new Box2DX.Dynamics.World(new Box2DX.Collision.AABB(), new Box2DX.Common.Vec2(0f, -10.0f), true);


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Personaje player;

        KeyboardState estadoActualDelTeclado;
        KeyboardState estadoPrevioDelTeclado;

        GamePadState estadoActualGamePad;
        GamePadState estadoPrevioGamePad;

        //String para ver el tiempo total de juego
        string totalTime;

        // Fondos
        Texture2D fondoEstatico;
        Texture2D fondoMenuPrincipal;

        // Fondos "Parallax" whatever that means :P
        ParallaxingBackground fondoCapa1;
        ParallaxingBackground fondoCapa2;
        ParallaxingBackground tierraFirme;

        // Enemigos
        Texture2D texturaEnemigo;
        Texture2D texturaEnemigoTerrestre;
        List<Enemigo> Enemigos;
        List<Enemigo> EnemigosTerrestres;

        // La frecuencia en que aparecen los enemigos
        TimeSpan EnemigoFrecuenciaSpawn;
        TimeSpan EnemigoTiempoDeUltimaAparicion;

        Random random;

        // Lista de texturas de una explosión
        Texture2D texturaExplosion;
        List<Animacion> Explosiones;

        // Sonidos
        SoundEffect SonidoExplosion;
        Song MusicaEnJuego;

        // Puntaje y fuente para mostrarlo
        int score;
        SpriteFont font;

        //Bandera para Pausa y gameOver
        bool pause;
        bool gameOver;
        TimeSpan timer;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 480
            };
            Window.AllowUserResizing = true;
            Content.RootDirectory = "Content";

            Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            InicializarFondos();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // El juego empieza pausado y muestra el fondo.
            pause = true;

            //Initialize the player class
            player = new Personaje();

            //Enable the FreeDrag gesture. -->> Esto parece que sirve si jugás en un Windows Phone o algo con touch.
            TouchPanel.EnabledGestures = GestureType.FreeDrag;

            fondoCapa1 = new ParallaxingBackground();
            fondoCapa2 = new ParallaxingBackground();
            tierraFirme = new ParallaxingBackground();

            Enemigos = new List<Enemigo>();
            EnemigosTerrestres = new List<Enemigo>();

            EnemigoTiempoDeUltimaAparicion = TimeSpan.Zero;
            EnemigoFrecuenciaSpawn = TimeSpan.FromSeconds(1.0f);

            random = new Random();

            Explosiones = new List<Animacion>();
            score = 0;
            pause = false;
            gameOver = false;
            timer = new TimeSpan(0);
            totalTime = "";
            base.Initialize();
        }

        private void InicializarFondos()
        {
            // Texturas de fondos y objetos jugables
            fondoCapa1.Initialize(Content, "bgLayer1extraWide", GraphicsDevice.Viewport.Width, -1);
            fondoCapa2.Initialize(Content, "bgLayer2extraWide", GraphicsDevice.Viewport.Width, -2);
            tierraFirme.Initialize(Content, "Floor1extraWide", GraphicsDevice.Viewport.Width, -3.5f, 150);
            fondoEstatico = Content.Load<Texture2D>("mainbackground");
            fondoMenuPrincipal = Content.Load<Texture2D>("mainMenuTall");
            texturaEnemigo = Content.Load<Texture2D>("mineAnimation");
            texturaEnemigoTerrestre = Content.Load<Texture2D>("buildings");
            texturaExplosion = Content.Load<Texture2D>("explosion");
        }

        protected override void LoadContent()
        {
            // El SpriteBatch se usa para dibujar todo.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Animacion AnimacionPersonaje = new Animacion();
            Texture2D TexturaPersonaje = Content.Load<Texture2D>("shipAnimation");
            AnimacionPersonaje.Inicializar(TexturaPersonaje, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

            Vector2 PosicionPersonaje = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y
            + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            player.Inicializar(AnimacionPersonaje, PosicionPersonaje, Content, GraphicsDevice);

            InicializarFondos();

            // Sonidos
            MusicaEnJuego = Content.Load<Song>("sound/gameMusic");
            SonidoExplosion = Content.Load<SoundEffect>("sound/explosion");
            font = Content.Load<SpriteFont>("gameFont");
            PlayMusic(MusicaEnJuego);
        }

        private void PlayMusic(Song song)
        {
            // No se bien que quieren decir con esto:
            // Due to the way the MediaPlayer plays music,
            // we have to catch the exception. Music will play when the game is not tethered
            try
            {
                // Play the music
                MediaPlayer.Play(song);
                // Loop the currently playing song
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }

        private void AddExplosion(Vector2 position)
        {
            Animacion Explosion = new Animacion();
            Explosion.Inicializar(texturaExplosion, position, 134, 134, 12, 45, Color.White, 1f, false);
            Explosiones.Add(Explosion);
        }

        private void AgregarEnemigo()
        {
            var AnimacionEnemigo = new Animacion();
            var Posicion = new Vector2(GraphicsDevice.Viewport.Width + texturaEnemigo.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100));
            AnimacionEnemigo.Inicializar(texturaEnemigo, Posicion, 47, 61, 8, 30, Color.White, 1f, true);

            var Enemigo = new Enemigo();
            Enemigo.Inicializar(AnimacionEnemigo, Content, GraphicsDevice);
            Enemigos.Add(Enemigo);
        }

        private void AgregarEnemigoTerrestre()
        {
            var AnimacionEnemigo = new Animacion();
            var Posicion = new Vector2(800, random.Next(400,480));
            AnimacionEnemigo.Inicializar(texturaEnemigoTerrestre, Posicion, 100, 138, 1, 1, Color.White, .5f, true);

            var Enemigo = new Enemigo();
            Enemigo.Inicializar(AnimacionEnemigo, Content, GraphicsDevice, true);
            EnemigosTerrestres.Add(Enemigo);
        }

        private void ActualizarEnemigos(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - EnemigoTiempoDeUltimaAparicion > EnemigoFrecuenciaSpawn)
            {
                EnemigoTiempoDeUltimaAparicion = gameTime.TotalGameTime;
                AgregarEnemigo();
            }

            if (((int)random.Next(0, 50)).Equals(10))
                AgregarEnemigoTerrestre();


            // Actualizar enemigos
            for (int i = Enemigos.Count - 1; i >= 0; i--)
            {
                Enemigos[i].VelocidadMovimiento = -1 * fondoCapa2.speed * 4;
                Enemigos[i].Update(gameTime);
                if (!Enemigos[i].Activo)
                {
                    // Si no está activo y la vida es menor o igual a 0
                    if (Enemigos[i].Vida <= 0)
                    {
                        // Boom!
                        AddExplosion(Enemigos[i].Posicion);
                        // Boom audible!
                        SonidoExplosion.Play();
                    }
                    // Eliminar el enemigo caído en acción
                    Enemigos.RemoveAt(i);
                }
            }

            for (int i = EnemigosTerrestres.Count - 1; i >= 0; i--)
            {
                EnemigosTerrestres[i].VelocidadMovimiento = -1 * tierraFirme.speed;
                EnemigosTerrestres[i].Update(gameTime);
                if (!EnemigosTerrestres[i].Activo)
                {
                    // Si no está activo y la vida es menor o igual a 0
                    if (EnemigosTerrestres[i].Vida <= 0)
                    {
                        // Boom!
                        AddExplosion(EnemigosTerrestres[i].Posicion);
                        // Boom audible!
                        SonidoExplosion.Play();
                    }
                    // Eliminar el enemigo caído en acción
                    EnemigosTerrestres.RemoveAt(i);
                }
            }
        }

        private void ActualizarExplosiones(GameTime gameTime)
        {
            for (int i = Explosiones.Count - 1; i >= 0; i--)
            {
                Explosiones[i].Update(gameTime);
                if (Explosiones[i].Activo == false)
                {
                    Explosiones.RemoveAt(i);
                }
            }
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (estadoActualDelTeclado.IsKeyDown(Keys.S))
            {
                fondoCapa1.AlterSpeed(1.2f);
                fondoCapa2.AlterSpeed(1.2f);
            }
            if (estadoActualGamePad.Triggers.Left > 0)
            {
                fondoCapa1.AlterSpeed(1f + estadoActualGamePad.Triggers.Left / 5);
                fondoCapa2.AlterSpeed(1f + estadoActualGamePad.Triggers.Left / 5);
                tierraFirme.AlterSpeed(1f + estadoActualGamePad.Triggers.Left / 5);
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || estadoActualDelTeclado.IsKeyDown(Keys.Q))
                this.Exit();

            // Esto es medio al vicio por ahora:
            // Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
            estadoPrevioGamePad = estadoActualGamePad;
            estadoPrevioDelTeclado = estadoActualDelTeclado;

            // Read the current state of the keyboard and gamepad and store it
            estadoActualDelTeclado = Keyboard.GetState();
            estadoActualGamePad = GamePad.GetState(PlayerIndex.One);

            //Cambia la flag cuando se presiona pausa y pausa o reproduce la música de fondo
            if (estadoActualDelTeclado.IsKeyDown(Keys.P) || estadoActualGamePad.Buttons.Start == ButtonState.Pressed)
            {
                pause = !pause;
                if (pause && !gameOver)
                {
                    MediaPlayer.Pause();
                }
                else
                {
                    MediaPlayer.Resume();
                }
            }

            //Si está en pausa no actualiza nada
            if (!pause && !gameOver)
            {
                ActualizarPersonaje(gameTime);

                fondoCapa1.Update();
                fondoCapa2.Update();
                tierraFirme.Update();

                ActualizarEnemigos(gameTime);
                UpdateCollision();
                player.ActualizarProyectiles();
                ActualizarExplosiones(gameTime);

                EnemigoFrecuenciaSpawn = TimeSpan.FromSeconds(1.0f * random.Next(30));

                timer += gameTime.ElapsedGameTime;
                totalTime = timer.ToString(@"mm\:ss");

                base.Update(gameTime);
            }

            //Para debuggear - Tecla N reinicia el juego
            if (estadoActualDelTeclado.IsKeyDown(Keys.N) || estadoActualGamePad.Buttons.X == ButtonState.Pressed)
            {
                pause = gameOver = false;
                player.Activo = true;
                player.Vida = 0;
                score = 0;
                timer = new TimeSpan(0);
            }
        }

        private void ActualizarPersonaje(GameTime gameTime)
        {
            player.Update(gameTime, estadoActualGamePad, estadoActualDelTeclado, GraphicsDevice);
        }

        /// <summary>
        /// Método sacado del tutorial para calcular colisiones, funciona perfecto!
        /// </summary>
        private void UpdateCollision()
        {
            // Use the Rectangle's built-in intersect functionto 
            // determine if two objects are overlapping
            Rectangle rectangle1;
            Rectangle rectangle2;

            // Only create the rectangle once for the player
            rectangle1 = new Rectangle((int)player.Posicion.X,
            (int)player.Posicion.Y,
            player.Ancho,
            player.Alto);

            // Do the collision between the player and the enemies
            for (int i = 0; i < Enemigos.Count; i++)
            {
                rectangle2 = new Rectangle((int)Enemigos[i].Posicion.X,
                (int)Enemigos[i].Posicion.Y,
                Enemigos[i].Ancho,
                Enemigos[i].Alto);

                // Determine if the two objects collided with each
                // other
                if (rectangle1.Intersects(rectangle2))
                {
                    // Subtract the health from the player based on
                    // the enemy damage
                    player.Vida -= Enemigos[i].Danios;

                    // Since the enemy collided with the player
                    // destroy it
                    Enemigos[i].Vida = 0;

                    // If the player health is less than zero we died
                    if (player.Vida <= 0)
                        player.Activo = false;
                }

            }

            for (int i = 0; i < EnemigosTerrestres.Count; i++)
            {
                rectangle2 = new Rectangle((int)EnemigosTerrestres[i].Posicion.X,
                (int)EnemigosTerrestres[i].Posicion.Y,
                EnemigosTerrestres[i].Ancho,
                EnemigosTerrestres[i].Alto);

                if (rectangle1.Intersects(rectangle2))
                {
                    player.Vida -= EnemigosTerrestres[i].Danios;
                    EnemigosTerrestres[i].Vida = 0;
                    if (player.Vida <= 0)
                        player.Activo = false;
                }

            }

            // Proyectiles enemigos contra el personaje
            // Linq: De los enemigos y los enemigos terrestres seleccionar sus proyectiles
            foreach (var proyectiles in (from e in Enemigos.Concat(EnemigosTerrestres) select e.proyectiles))
            {
                foreach (var proyectil in proyectiles)
                {
                    if (rectangle1.Intersects(new Rectangle((int)proyectil.Posicion.X, (int)proyectil.Posicion.Y,(int)proyectil.Ancho, (int)proyectil.Alto)))
                    {
                        player.Vida -= proyectil.DaniosQueCausa;
                        proyectil.Activo = false;
                        SonidoExplosion.Play();
                    }
                }
            }

            // Projectile vs Enemy Collision
            for (int i = 0; i < player.Proyectiles.Count; i++)
            {
                for (int j = 0; j < Enemigos.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)player.Proyectiles[i].Posicion.X -
                    player.Proyectiles[i].Ancho / 2, (int)player.Proyectiles[i].Posicion.Y -
                    player.Proyectiles[i].Alto / 2, player.Proyectiles[i].Ancho, player.Proyectiles[i].Alto);

                    rectangle2 = new Rectangle((int)Enemigos[j].Posicion.X - Enemigos[j].Ancho / 2,
                    (int)Enemigos[j].Posicion.Y - Enemigos[j].Alto / 2,
                    Enemigos[j].Ancho, Enemigos[j].Alto);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        Enemigos[j].Vida -= player.Proyectiles[i].DaniosQueCausa;
                        player.Proyectiles[i].Activo = false;
                        //El puntaje se suma sólo si el enemigo es alcanzado por un proyectil y su vida es 0
                        if (Enemigos[j].Vida == 0)
                            score += Enemigos[j].Puntos;
                    }
                }
                for (int j = 0; j < EnemigosTerrestres.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)player.Proyectiles[i].Posicion.X -
                    player.Proyectiles[i].Ancho / 2, (int)player.Proyectiles[i].Posicion.Y -
                    player.Proyectiles[i].Alto / 2, player.Proyectiles[i].Ancho, player.Proyectiles[i].Alto);

                    rectangle2 = new Rectangle((int)EnemigosTerrestres[j].Posicion.X - EnemigosTerrestres[j].Ancho / 2,
                    (int)EnemigosTerrestres[j].Posicion.Y - EnemigosTerrestres[j].Alto / 2,
                    EnemigosTerrestres[j].Ancho, EnemigosTerrestres[j].Alto);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        EnemigosTerrestres[j].Vida -= player.Proyectiles[i].DaniosQueCausa;
                        player.Proyectiles[i].Activo = false;
                        //El puntaje se suma sólo si el enemigo es alcanzado por un proyectil y su vida es 0
                        if (EnemigosTerrestres[j].Vida == 0)
                            score += EnemigosTerrestres[j].Puntos;
                    }
                }
            }
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            if (pause || gameOver)
            {   // Si estamos en pausa solo dibujo el fondo de menú principal y termina el método.
                spriteBatch.Draw(fondoMenuPrincipal, Vector2.Zero, Color.White);
            }
            if (player.Activo && !(pause || gameOver))
            {
                spriteBatch.Draw(fondoEstatico, Vector2.Zero, Color.White);
                //spriteBatch.Draw(fondoEstatico, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

                fondoCapa1.Draw(spriteBatch, GraphicsDevice);
                fondoCapa2.Draw(spriteBatch, GraphicsDevice);

                tierraFirme.Draw(spriteBatch, GraphicsDevice);

                player.Draw(spriteBatch);

                foreach (var enemigo in (from enemigos in Enemigos.Concat(EnemigosTerrestres) select enemigos))
                {
                    enemigo.Draw(spriteBatch);
                }

                for (int i = 0; i < Explosiones.Count; i++)
                {
                    Explosiones[i].Draw(spriteBatch);
                }

                spriteBatch.DrawString(font, "Puntaje: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
                spriteBatch.DrawString(font, "Vida: " + player.Vida, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 20), Color.White);
                spriteBatch.DrawString(font, "Tiempo: " + totalTime, new Vector2(GraphicsDevice.Viewport.Width - 110, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
                spriteBatch.DrawString(font, "Municiones: " + player.municiones, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 40), Color.White);
            }
            else
            {
                gameOver = true;
                spriteBatch.DrawString(font, "Puntaje Total: " + score, new Vector2(GraphicsDevice.Viewport.Width / 2 - 300, GraphicsDevice.Viewport.Height / 2 - 15), Color.White);
                spriteBatch.DrawString(font, "Tiempo Total: " + totalTime, new Vector2(GraphicsDevice.Viewport.Width / 2 - 300, GraphicsDevice.Viewport.Height / 2 + 15), Color.White);
                spriteBatch.DrawString(font, "Presione N para\njugar nuevamente", new Vector2(GraphicsDevice.Viewport.Width / 2 - 300, GraphicsDevice.Viewport.Height / 2 + 45), Color.Red);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
