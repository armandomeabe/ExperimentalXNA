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
        public Texture2D Textura { get { return Texturas[0]; } }
        public List<Texture2D> Texturas { get; set; }

        public Vector2 Posicion;
        //public Vector2 Velocidad = new Vector2(50.0f, 50.0f);

        public float Velocidad = 8.0f;

        public ObjetoDibujable(Texture2D Textura)
        {
            Posicion = Vector2.Zero;
        }

        public ObjetoDibujable(Texture2D Textura, Vector2 Posicion)
        {
            this.Texturas = new List<Texture2D> { Textura };
            Posicion = Vector2.Zero;
        }
    }
}
