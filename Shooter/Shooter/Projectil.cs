// Projectile.cs
//Using declarations
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Shooter
{
    class Projectil
    {
        public Texture2D Textura;
        public Vector2 Posicion;
        public bool Activo;
        public int Danios;
        Viewport viewport;
        public int Ancho
        {
            get { return Textura.Width; }
        }
        public int Alto
        {
            get { return Textura.Height; }
        }
        float VelocidadDeMovimiento;

        public void Inicializar(Viewport viewport, Texture2D textura, Vector2 posicion)
        {
            Textura = textura;
            Posicion = posicion;
            this.viewport = viewport;
            Activo = true;
            Danios = 2;
            VelocidadDeMovimiento = 20f;
        }

        public void Update()
        {
            // Los proyectiles siempre se mueven hacia la derecha
            Posicion.X += VelocidadDeMovimiento;
            //Posicion.Y += (new Random(DateTime.Now.Millisecond).Next(-1, 1)); // No queda muy bueno...

            // Si se van de la pantalla los desactivo para que después se borren del vector.
            if (Posicion.X + Textura.Width / 2 > viewport.Width)
                Activo = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Textura, Posicion, null, Color.White, 0f,
            new Vector2(Ancho / 2, Alto / 2), 1f, SpriteEffects.None, 0f);
        }
    }
}
