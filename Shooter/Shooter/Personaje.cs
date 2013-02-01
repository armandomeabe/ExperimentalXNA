using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Shooter
{
    class Personaje
    {
        public Animacion Animacion;
        public Vector2 Posicion;
        public bool Activo;
        public int Vida;

        public int Ancho
        {
            get { return Animacion.AnchoFrame; }
        }

        public int Alto
        {
            get { return Animacion.AltoFrame; }
        }

        public void Inicializar(Animacion animation, Vector2 position)
        {
            Animacion = animation;
            Posicion = position;
            Activo = true;
            Vida = 100;
        }

        public void Update(GameTime gameTime)
        {
            Animacion.Posicion = Posicion;
            Animacion.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Animacion.Draw(spriteBatch);
        }
    }
}
