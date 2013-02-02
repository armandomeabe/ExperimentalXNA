using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace Shooter
{
    class Enemigo
    {
        GraphicsDevice graphicsDevice;
        bool puedeDisparar;
        ParticleEngine particleEngine;
        public List<Projectil> proyectiles;
        public Texture2D TexturaProyectil;

        public Animacion AnimacionEnemigo;
        public Vector2 Posicion;
        //float Rotacion;

        public bool Activo;
        public int Vida;
        public int Danios;
        public int Puntos;

        public int Ancho
        {
            get { return AnimacionEnemigo.AnchoFrame; }
        }

        public int Alto
        {
            get { return AnimacionEnemigo.AltoFrame; }
        }

        public float VelocidadMovimiento;

        public void Inicializar(Animacion animacion, ContentManager content, GraphicsDevice graphicsDevice, bool puedeDisparar = false)
        {
            this.puedeDisparar = puedeDisparar;
            this.graphicsDevice = graphicsDevice;
            AnimacionEnemigo = animacion;
            Posicion = animacion.Posicion;
            Activo = true;
            Vida = 100;
            Danios = 10;
            VelocidadMovimiento = 6f;
            Puntos = 100;

            // Rastro de partículas :D
            var textures = new List<Texture2D>();
            textures.Add(content.Load<Texture2D>("spark"));
            particleEngine = new ParticleEngine(textures, new Vector2(400, 240));

            // Proyectiles
            proyectiles = new List<Projectil>();
            TexturaProyectil = content.Load<Texture2D>("minibullet");
        }

        public void Disparar()
        {
            proyectiles.Add(new Projectil(graphicsDevice.Viewport, TexturaProyectil, Posicion, 1));
        }

        public void Update(GameTime gameTime)
        {


            if (puedeDisparar && ((int)new Random().Next(0, 10)).Equals(5))
                Disparar();

            Posicion.X -= VelocidadMovimiento;
            AnimacionEnemigo.Posicion = Posicion;
            AnimacionEnemigo.Update(gameTime);
            if (Posicion.X < -Ancho || Vida <= 0)
            {
                Activo = false;
            }

            foreach (var p in proyectiles.ToArray())
            {
                if (!p.Activo)
                    proyectiles.Remove(p);
                else
                    p.Update(-3, -2);
            }

            // Partículas
            particleEngine.EmitterLocation = new Vector2((int)Posicion.X, (int)Posicion.Y);
            particleEngine.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            AnimacionEnemigo.Draw(spriteBatch);
            particleEngine.Draw(spriteBatch);

            foreach (var p in proyectiles)
            {
                p.Draw(spriteBatch);
            }
        }
    }
}
