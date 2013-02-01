using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;


namespace Shooter
{

    class Enemigo
    {
        ParticleEngine particleEngine;

        public Animacion AnimacionEnemigo;
        public Vector2 Posicion;
        float Rotacion;

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
        

        public void Inicializar(Animacion animacion, Vector2 posicion, ContentManager content)
        {
            AnimacionEnemigo = animacion;
            Posicion = posicion;
            Activo = true;
            Vida = 10;
            Danios = 10;
            VelocidadMovimiento = 6f;
            Puntos = 100;

            // Rastro de partículas :D
            var textures = new List<Texture2D>();
            textures.Add(content.Load<Texture2D>("spark"));
            particleEngine = new ParticleEngine(textures, new Vector2(400, 240));
        }

        public void Update(GameTime gameTime)
        {
            Posicion.X -= VelocidadMovimiento;
            AnimacionEnemigo.Posicion = Posicion;
            AnimacionEnemigo.Update(gameTime);
            if (Posicion.X < -Ancho || Vida <= 0)
            {
                Activo = false;
            }

            // Partículas
            particleEngine.EmitterLocation = new Vector2((int)Posicion.X, (int)Posicion.Y);
            particleEngine.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            AnimacionEnemigo.Draw(spriteBatch);
            particleEngine.Draw(spriteBatch);
        }
    }
}
