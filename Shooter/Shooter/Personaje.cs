using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;


namespace Shooter
{
    class Personaje
    {
        ParticleEngine particleEngine;
        Vector2 DireccionDisparos;
        // Disparos
        public long municiones = 1000000;
        public Texture2D TexturaProyectil;
        public List<Projectil> Proyectiles;
        SoundEffect SonidoLaser;
        // El rate de disparo del personaje
        public TimeSpan[] DisparosFrecuencia;
        public TimeSpan[] TiempoDeUltimoDisparo;
        public int[] shoots;

        public Animacion Animacion;
        public Vector2 Posicion;
        public bool Activo;
        public int Vida;
        float velocidadMovimiento = 8.0f;

        // Referencia al GraphicsDevice principal del juego.
        GraphicsDevice graphicsDevice;

        public int Ancho
        {
            get { return Animacion.AnchoFrame; }
        }

        public int Alto
        {
            get { return Animacion.AltoFrame; }
        }

        public void Inicializar(Animacion animation, Vector2 position, ContentManager content, GraphicsDevice graphicsDevice)
        {
            this.DireccionDisparos = new Vector2(15, 0);
            this.graphicsDevice = graphicsDevice;
            Animacion = animation;
            Posicion = position;
            Activo = true;
            Vida = 100;

            // Proyectiles
            Proyectiles = new List<Projectil>();
            TexturaProyectil = content.Load<Texture2D>("laser");
            SonidoLaser = content.Load<SoundEffect>("sound/laserFire");

            DisparosFrecuencia = new TimeSpan[3];
            TiempoDeUltimoDisparo = new TimeSpan[3];
            shoots = new int[3];

            DisparosFrecuencia[0] = new TimeSpan(0, 0, 0, 0, 250);
            shoots[0] = 4;
            DisparosFrecuencia[1] = new TimeSpan(0, 0, 0, 0, 25);
            shoots[1] = 40;
            DisparosFrecuencia[2] = new TimeSpan(0, 0, 0, 0, 2);
            shoots[2] = 500;

            // Rastro de partículas :D
            var textures = new List<Texture2D>();
            textures.Add(content.Load<Texture2D>("spark"));
            particleEngine = new ParticleEngine(textures, new Vector2(400, 240));
        }

        public void Disparar(int type, GameTime gameTime)
        {
            if (type < 0 || type > 2)
                return;

            TimeSpan delayShoot = gameTime.TotalGameTime - TiempoDeUltimoDisparo[type];

            if (delayShoot < DisparosFrecuencia[type])
                return;
            TiempoDeUltimoDisparo[type] = gameTime.TotalGameTime;

            //for (int i=0;i<shoots[type];i++)
            //{
                Proyectiles.Add(new Projectil(graphicsDevice.Viewport, TexturaProyectil, Posicion, DireccionDisparos));
                SonidoLaser.Play();
                municiones--;
            //}
        }
        public void DispararJoystick(int shoots, GameTime gameTime)
        {
            for (int i = 0; i < shoots; i++)
            {
            Proyectiles.Add(new Projectil(graphicsDevice.Viewport, TexturaProyectil, Posicion, DireccionDisparos));
            SonidoLaser.Play();
            municiones--;
            }
        }

        public void Update(GameTime gameTime, GamePadState gamepadState, KeyboardState keyboardState, GraphicsDevice graphics, Vector2 DireccionDisparos)
        {
            Animacion.Posicion = Posicion;
            Animacion.Update(gameTime);

            // Partículas
            particleEngine.EmitterLocation = Posicion;
            particleEngine.Update();

            ActualizarProyectiles();

            // Windows Phone Controls
            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();
                if (gesture.GestureType == GestureType.FreeDrag)
                {
                    Posicion += gesture.Delta;
                }
            }

            // Analógico
            Posicion.X += gamepadState.ThumbSticks.Left.X * velocidadMovimiento;
            Posicion.Y -= gamepadState.ThumbSticks.Left.Y * velocidadMovimiento;

            // Teclado / Dpad
            if (keyboardState.IsKeyDown(Keys.Left) ||
            gamepadState.DPad.Left == ButtonState.Pressed)
            {
                Posicion.X -= velocidadMovimiento;
            }
            if (keyboardState.IsKeyDown(Keys.Right) ||
            gamepadState.DPad.Right == ButtonState.Pressed)
            {
                Posicion.X += velocidadMovimiento;
            }
            if (keyboardState.IsKeyDown(Keys.Up) ||
            gamepadState.DPad.Up == ButtonState.Pressed)
            {
                Posicion.Y -= velocidadMovimiento;
            }
            if (keyboardState.IsKeyDown(Keys.Down) ||
            gamepadState.DPad.Down == ButtonState.Pressed)
            {
                Posicion.Y += velocidadMovimiento;
            }

            // Evitar que el personaje salga de la ventana
            Posicion.X = MathHelper.Clamp(Posicion.X, 0, graphics.Viewport.Width - Ancho);
            Posicion.Y = MathHelper.Clamp(Posicion.Y, 0, graphics.Viewport.Height - Alto);

            var shoots = gamepadState.Triggers.Right;
            if (shoots > 0 && shoots < 0.2f) shoots = 1;
            else if (shoots >= 0.2f && shoots < 0.5f) shoots = 3;
            else if (shoots >= 0.5f && shoots < 1.0f) shoots = 6;
            else if (shoots.Equals(1.0f) || keyboardState.IsKeyDown(Keys.D)) shoots += 30; // MEGA MEGA SHOOT!!
            DispararJoystick((int)shoots, gameTime);

            if (gamepadState.Buttons.A.Equals(ButtonState.Pressed) || keyboardState.IsKeyDown(Keys.A))
                Disparar(0, gameTime);
            if (gamepadState.Buttons.B.Equals(ButtonState.Pressed) || keyboardState.IsKeyDown(Keys.W))
                Disparar(1, gameTime);
            if (gamepadState.Buttons.X.Equals(ButtonState.Pressed) || keyboardState.IsKeyDown(Keys.D))
                Disparar(2, gameTime);

        }

        public void ActualizarProyectiles()
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

        public void Draw(SpriteBatch spriteBatch)
        {
            particleEngine.Draw(spriteBatch);

            foreach (var p in Proyectiles)
            {
                p.Draw(spriteBatch);
            }

            Animacion.Draw(spriteBatch);
        }
    }
}
