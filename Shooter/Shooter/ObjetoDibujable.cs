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
        public Texture2D Textura { get; set; }
        public Vector2 Posicion { get; set; }
        public Vector2 Velocidad = new Vector2(50.0f, 50.0f);

        public ObjetoDibujable(Texture2D Textura)
        {
            Posicion = Vector2.Zero;
        }

        public ObjetoDibujable(Texture2D Textura, Vector2 Posicion)
        {
            this.Textura = Textura;
            Posicion = Vector2.Zero;
        }
    }
}
