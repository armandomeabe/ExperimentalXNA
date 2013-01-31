using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Shooter
{
    class ObjetoDibujableAnimado
    {
        // La vida misma del objeto.
        public Texture2D Textura;
        public Animacion AnimacionObjeto;

        // Información dimensional.
        //public Vector2 Velocidad = new Vector2(50.0f, 50.0f); // Esto podría ser mejor... o no. El operador += está sobrecargado para
        public float Velocidad = 8.0f;
        public Vector2 Posicion;
        public float Ancho { get { return this.AnimacionObjeto.FrameWidth; } }
        public float Alto { get { return this.AnimacionObjeto.FrameHeight; } }

        // Estado del personaje o lo que represente el objeto.
        public bool Activo;
        public float Vida;

        public ObjetoDibujableAnimado(Texture2D Textura)
        {
            Posicion = Vector2.Zero;
            this.Inicializar(new Animacion());
        }

        public ObjetoDibujableAnimado(Texture2D Textura, Vector2 Posicion)
        {
            this.Textura = Textura;
            this.Posicion = Posicion;
            this.Inicializar(new Animacion());
        }

        public void Inicializar(Animacion Animacion)
        {
            this.AnimacionObjeto = Animacion;
            this.Activo = true;
            this.Vida = 100.0f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.Activo)
                AnimacionObjeto.Draw(spriteBatch);
            //spriteBatch.Draw(Textura, Posicion, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        // Actualiza la animación
        public void Update(GameTime gameTime)
        {
            AnimacionObjeto.Position = Posicion;
            AnimacionObjeto.Update(gameTime);
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
