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
        // Partículas!!!!
        ParticleEngine particleEngine;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Personaje player;

        KeyboardState estadoActualDelTeclado;
        KeyboardState estadoPrevioDelTeclado;

        GamePadState estadoActualGamePad;
        GamePadState estadoPrevioGamePad;

        float velocidadPersonaje;

        Texture2D fondoEstatico;

        //String para ver el tiempo total de juego
        string totalTime;

        // Fondos "Parallax" whatever that means :P
        ParallaxingBackground fondoCapa1;
        ParallaxingBackground fondoCapa2;

        // Enemigos
        Texture2D texturaEnemigo;
        List<Enemigo> Enemigos;

        // La frecuencia en que aparecen los enemigos
        TimeSpan EnemigoFrecuenciaSpawn;
        TimeSpan EnemigoTiempoDeUltimaAparicion;

        Random random;

        Texture2D TexturaProyectil;
        List<Projectil> Proyectiles;

        // El rate de disparo del personaje
        TimeSpan DisparosFrecuencia;
        TimeSpan DisparoTiempoDeUltimaAparicion;

        // Lista de texturas de una explosión
        Texture2D texturaExplosion;
        List<Animacion> Explosiones;

        // Sonidos
        SoundEffect SonidoLaser;
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
            //Initialize the player class
            player = new Personaje();
            velocidadPersonaje = 8.0f;

            //Enable the FreeDrag gesture. -->> Esto parece que sirve si jugás en un Windows Phone o algo con touch.
            TouchPanel.EnabledGestures = GestureType.FreeDrag;

            fondoCapa1 = new ParallaxingBackground();
            fondoCapa2 = new ParallaxingBackground();

            Enemigos = new List<Enemigo>();

            EnemigoTiempoDeUltimaAparicion = TimeSpan.Zero;
            EnemigoFrecuenciaSpawn = TimeSpan.FromSeconds(1.0f);

            random = new Random();

            Proyectiles = new List<Projectil>();
            DisparosFrecuencia = TimeSpan.FromSeconds(.15f);

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
            fondoEstatico = Content.Load<Texture2D>("mainbackground");
            texturaEnemigo = Content.Load<Texture2D>("mineAnimation");
            TexturaProyectil = Content.Load<Texture2D>("laser");
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
            player.Inicializar(AnimacionPersonaje, PosicionPersonaje);

            InicializarFondos();

            // Partículas
            List<Texture2D> textures = new List<Texture2D>()
            {
                Content.Load<Texture2D>("spark"),
                //Content.Load<Texture2D>("star"),
                //Content.Load<Texture2D>("diamond")
            };
            particleEngine = new ParticleEngine(textures, new Vector2(400, 240));

            // Sonidos
            MusicaEnJuego = Content.Load<Song>("sound/gameMusic");
            SonidoLaser = Content.Load<SoundEffect>("sound/laserFire");
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
            AnimacionEnemigo.Inicializar(texturaEnemigo, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);
            var Posicion = new Vector2(GraphicsDevice.Viewport.Width + texturaEnemigo.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100));

            var Enemigo = new Enemigo();
            Enemigo.Inicializar(AnimacionEnemigo, Posicion, Content);
            Enemigos.Add(Enemigo);
        }

        private void ActualizarEnemigos(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - EnemigoTiempoDeUltimaAparicion > EnemigoFrecuenciaSpawn)
            {
                EnemigoTiempoDeUltimaAparicion = gameTime.TotalGameTime;
                AgregarEnemigo();
            }

            // Actualizar enemigos
            for (int i = Enemigos.Count - 1; i >= 0; i--)
            {
                Enemigos[i].VelocidadMovimiento = -1 * fondoCapa1.speed * 2;
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

        private void Disparar(Vector2 Posicion)
        {
            Projectil projectile = new Projectil();
            projectile.Inicializar(GraphicsDevice.Viewport, TexturaProyectil, Posicion);
            Proyectiles.Add(projectile);
        }

        private void ActualizarProyectiles()
        {
            // Update the Projectiles
            for (int i = Proyectiles.Count - 1; i >= 0; i--)
            {
                Proyectiles[i].Update();
                if (Proyectiles[i].Activo == false)
                {
                    Proyectiles.RemoveAt(i);
                }
            }
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            particleEngine.EmitterLocation = new Vector2(player.Posicion.X, player.Posicion.Y);
            particleEngine.Update();

            if (estadoActualDelTeclado.IsKeyDown(Keys.S))
            {
                fondoCapa1.AlterSpeed(1.2f);
                fondoCapa2.AlterSpeed(1.2f);
            }
            fondoCapa1.AlterSpeed(1f + estadoActualGamePad.Triggers.Left);
            fondoCapa2.AlterSpeed(1f + estadoActualGamePad.Triggers.Left);

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

                ActualizarEnemigos(gameTime);
                UpdateCollision();
                ActualizarProyectiles();
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
            player.Update(gameTime);

            // Windows Phone Controls
            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();
                if (gesture.GestureType == GestureType.FreeDrag)
                {
                    player.Posicion += gesture.Delta;
                }
            }

            // Analógico
            player.Posicion.X += estadoActualGamePad.ThumbSticks.Left.X * velocidadPersonaje;
            player.Posicion.Y -= estadoActualGamePad.ThumbSticks.Left.Y * velocidadPersonaje;

            // Teclado / Dpad
            if (estadoActualDelTeclado.IsKeyDown(Keys.Left) ||
            estadoActualGamePad.DPad.Left == ButtonState.Pressed)
            {
                player.Posicion.X -= velocidadPersonaje;
            }
            if (estadoActualDelTeclado.IsKeyDown(Keys.Right) ||
            estadoActualGamePad.DPad.Right == ButtonState.Pressed)
            {
                player.Posicion.X += velocidadPersonaje;
            }
            if (estadoActualDelTeclado.IsKeyDown(Keys.Up) ||
            estadoActualGamePad.DPad.Up == ButtonState.Pressed)
            {
                player.Posicion.Y -= velocidadPersonaje;
            }
            if (estadoActualDelTeclado.IsKeyDown(Keys.Down) ||
            estadoActualGamePad.DPad.Down == ButtonState.Pressed)
            {
                player.Posicion.Y += velocidadPersonaje;
            }


            // Evitar que el personaje salga de la ventana
            player.Posicion.X = MathHelper.Clamp(player.Posicion.X, 0, GraphicsDevice.Viewport.Width - player.Ancho);
            player.Posicion.Y = MathHelper.Clamp(player.Posicion.Y, 0, GraphicsDevice.Viewport.Height - player.Alto);

            var shoots = estadoActualGamePad.Triggers.Right;
            if (shoots > 0 && shoots < 0.2f) shoots = 1;
            else if (shoots >= 0.2f && shoots < 0.5f) shoots = 3;
            else if (shoots >= 0.5f && shoots < 1.0f) shoots = 6;
            else if (shoots.Equals(1.0f) || estadoActualDelTeclado.IsKeyDown(Keys.D)) shoots += 30; // MEGA MEGA SHOOT!!

            for (int i = 0; i < shoots; i++)
            {
                // Actualiza el tiempo de cuando se disparó por última vez
                DisparoTiempoDeUltimaAparicion = gameTime.TotalGameTime;
                // Nuevo proyectil en la parte delantera de la navecita
                Disparar(player.Posicion + new Vector2(player.Ancho / 2, random.Next(-15, 15)));
                // Piiuuu!!
                SonidoLaser.Play();
            }

            if (estadoActualGamePad.Buttons.A.Equals(ButtonState.Pressed) || estadoActualDelTeclado.IsKeyDown(Keys.A))
                if (Proyectiles.Count <= 10) // FORMA DE ARMANDO
                {
                    // Actualiza el tiempo de cuando se disparó por última vez
                    DisparoTiempoDeUltimaAparicion = gameTime.TotalGameTime;
                    // Nuevo proyectil en la parte delantera de la navecita
                    Disparar(player.Posicion + new Vector2(player.Ancho / 2, random.Next(-5, 5)));
                    // Piiuuu!!
                    SonidoLaser.Play();
                }
            if (estadoActualGamePad.Buttons.B.Equals(ButtonState.Pressed) || estadoActualDelTeclado.IsKeyDown(Keys.LeftShift) || estadoActualDelTeclado.IsKeyDown(Keys.W))
                if (gameTime.TotalGameTime - DisparoTiempoDeUltimaAparicion > DisparosFrecuencia) // FORMA DE MAXI
                {
                    // Actualiza el tiempo de cuando se disparó por última vez
                    DisparoTiempoDeUltimaAparicion = gameTime.TotalGameTime;
                    // Nuevo proyectil en la parte delantera de la navecita
                    Disparar(player.Posicion + new Vector2(player.Ancho / 2, random.Next(-5, 5)));
                    // Piiuuu!!
                    SonidoLaser.Play();
                }

            // En vez de morirte reseteás la vida
            if (player.Vida <= 0)
            {
                player.Vida = 100;
                score = 0;
            }

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

            // Projectile vs Enemy Collision
            for (int i = 0; i < Proyectiles.Count; i++)
            {
                for (int j = 0; j < Enemigos.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)Proyectiles[i].Posicion.X -
                    Proyectiles[i].Ancho / 2, (int)Proyectiles[i].Posicion.Y -
                    Proyectiles[i].Alto / 2, Proyectiles[i].Ancho, Proyectiles[i].Alto);

                    rectangle2 = new Rectangle((int)Enemigos[j].Posicion.X - Enemigos[j].Ancho / 2,
                    (int)Enemigos[j].Posicion.Y - Enemigos[j].Alto / 2,
                    Enemigos[j].Ancho, Enemigos[j].Alto);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        Enemigos[j].Vida -= Proyectiles[i].Danios;
                        Proyectiles[i].Activo = false;
                        //El puntaje se suma sólo si el enemigo es alcanzado por un proyectil y su vida es 0
                        if (Enemigos[j].Vida == 0)
                            score += Enemigos[j].Puntos;
                    }
                }
            }
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            //spriteBatch.Draw(fondoEstatico, Vector2.Zero, Color.White,);
            spriteBatch.Draw(fondoEstatico, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

            fondoCapa1.Draw(spriteBatch, GraphicsDevice);
            fondoCapa2.Draw(spriteBatch, GraphicsDevice);

            particleEngine.Draw(spriteBatch);

            player.Draw(spriteBatch);

            for (int i = 0; i < Enemigos.Count; i++)
            {
                Enemigos[i].Draw(spriteBatch);
            }

            for (int i = 0; i < Proyectiles.Count; i++)
            {
                Proyectiles[i].Draw(spriteBatch);
            }

            for (int i = 0; i < Explosiones.Count; i++)
            {
                Explosiones[i].Draw(spriteBatch);
            }

            if (player.Activo)
            {
                spriteBatch.DrawString(font, "Puntaje: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
                spriteBatch.DrawString(font, "Vida: " + player.Vida, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);
                spriteBatch.DrawString(font, "Tiempo: " + totalTime, new Vector2(GraphicsDevice.Viewport.Width - 190, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            }
            else
            {
                gameOver = true;
                spriteBatch.DrawString(font, "Puntaje Total: " + score, new Vector2(GraphicsDevice.Viewport.Width / 2 - 300, GraphicsDevice.Viewport.Height / 2 - 15), Color.White);
                spriteBatch.DrawString(font, "Tiempo Total: " + totalTime, new Vector2(GraphicsDevice.Viewport.Width / 2 - 300, GraphicsDevice.Viewport.Height / 2 + 15), Color.White);
                spriteBatch.DrawString(font, "Presione N para\njugar nuevamente", new Vector2(GraphicsDevice.Viewport.Width / 2 - 300, GraphicsDevice.Viewport.Height / 2 + 45), Color.Red);
            }

            // Por algún motivo por cada frame hay que llamar a .Begin y .End del spritebatch para que termine de dibujar...
            spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
