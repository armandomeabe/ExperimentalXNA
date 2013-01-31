using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Shooter
{
    class ObjetoDibujable
    {
        // La vida misma del objeto.
        public Texture2D Textura;

        // Información dimensional.
        //public Vector2 Velocidad = new Vector2(50.0f, 50.0f); // Esto podría ser mejor... o no. El operador += está sobrecargado para
        public float Velocidad = 8.0f;
        public Vector2 Posicion;
        public float Ancho { get { return this.Textura.Width; } }
        public float Alto { get { return this.Textura.Height; } }

        // Estado del personaje o lo que represente el objeto.
        public bool Activo;
        public float Vida;

        public ObjetoDibujable(Texture2D Textura)
        {
            Posicion = Vector2.Zero;
            this.Inicializar();
        }

        public ObjetoDibujable(Texture2D Textura, Vector2 Posicion)
        {
            this.Textura = Textura;
            this.Posicion = Posicion;
            this.Inicializar();
        }

        private void Inicializar()
        {
            this.Activo = true;
            this.Vida = 100.0f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.Activo)
                spriteBatch.Draw(this.Textura, this.Posicion, Color.White);
        }

        public void Update(GameTime gameTime)
        {

        }

        public void RecibirDanios(float Intensidad = 5) // C# Soporta caracteres locos como la ñ pero igual...
        {
            Vida -= Intensidad;
            this.Activo = Vida > 0;
        }

        public void NoHuirDeLaVentana(float Ancho, float Alto)
        {
            Posicion.X = MathHelper.Clamp(this.Posicion.X, 0, Ancho - this.Ancho / 2);
            Posicion.Y = MathHelper.Clamp(this.Posicion.Y, 0, Alto - this.Alto / 2);
        }
    }
}
