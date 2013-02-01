using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Shooter
{

    class Enemigo
    {
        // Animation representing the enemy
        public Animacion AnimacionEnemigo;

        // The position of the enemy ship relative to the top left corner of thescreen
        public Vector2 Posicion;

        // The state of the Enemy Ship
        public bool Activo;

        // The hit points of the enemy, if this goes to zero the enemy dies
        public int Vida;

        // The amount of damage the enemy inflicts on the player ship
        public int Danios;

        // The amount of score the enemy will give to the player
        public int Puntos;

        // Get the width of the enemy ship
        public int Ancho
        {
            get { return AnimacionEnemigo.AnchoFrame; }
        }

        // Get the height of the enemy ship
        public int Alto
        {
            get { return AnimacionEnemigo.AltoFrame; }
        }

        // The speed at which the enemy moves
        float VelocidadMovimiento;


        public void Inicializar(Animacion animacion, Vector2 posicion)
        {
            // Load the enemy ship texture
            AnimacionEnemigo = animacion;

            // Set the position of the enemy
            Posicion = posicion;

            // We initialize the enemy to be active so it will be updated
            Activo = true;

            // Set the health of the enemy
            Vida = 10;

            // Set the amount of damage the enemy can do
            Danios = 10;

            // Set how fast the enemy moves
            VelocidadMovimiento = 6f;

            // Set the score value of the enemy
            Puntos = 100;
        }


        public void Update(GameTime gameTime)
        {
            // The enemy always moves to the left so decrement x
            Posicion.X -= VelocidadMovimiento;

            // Update the position of the Animation
            AnimacionEnemigo.Posicion = Posicion;

            // Update Animation
            AnimacionEnemigo.Update(gameTime);

            // If the enemy is past the screen or its health reaches 0, deactivate
            if (Posicion.X < -Ancho || Vida <= 0)
            {
                // By setting the Active flag to false, the game will remove
                Activo = false;
            }
        }



        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the animation
            AnimacionEnemigo.Draw(spriteBatch);
        }

    }
}
