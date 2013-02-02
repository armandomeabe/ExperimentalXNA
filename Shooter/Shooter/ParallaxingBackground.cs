// ParallaxingBackground.cs
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    /// <summary>
    /// Esta clase viene del tutorial de XNA 2D: http://xbox.create.msdn.com/en-US/education/tutorial/2dgame/getting_started
    /// Es la que hace el efecto interesante con los dos fondos movibles mas el fondo estático de atrás.
    /// </summary>
    class ParallaxingBackground
    {
        // The image representing the parallaxing background
        Texture2D texture;

        // An array of positions of the parallaxing background
        Vector2[] positions;

        // Ahora agregué initialSpeed para que cuando se acelere el fondo sepa hasta donde tiene que ir bajando la velocidad
        public float speed { get; private set; }
        public float initialSpeed { get; private set; }

        public void Initialize(ContentManager content, String texturePath, int screenWidth, float speed, float deltaY = 0)
        {
            // Load the background texture we will be using
            texture = content.Load<Texture2D>(texturePath);

            // Set the speed of the background
            speed = initialSpeed = speed;

            // If we divide the screen with the texture width then we can determine the number of tiles need.
            // We add 1 to it so that we won't have a gap in the tiling
            positions = new Vector2[screenWidth / texture.Width + 1];

            // Set the initial positions of the parallaxing background
            for (int i = 0; i < positions.Length; i++)
            {
                // We need the tiles to be side by side to create a tiling effect
                positions[i] = new Vector2(i * texture.Width, deltaY);
            }
        }

        public void AlterSpeed(float factor)
        {
            speed -= factor;
        }

        public void Update()
        {
            // Esto es así porque la velocidad tiene que 'tender' a regresar a initialSpeed pero podría estar moviéndose
            // para la derecha o para la izquierda.
            if (speed != initialSpeed)
            {
                if (speed > initialSpeed)
                    speed -= .25f;
                else
                    speed += .25f;
            }

            // Update the positions of the background
            for (int i = 0; i < positions.Length; i++)
            {
                // Update the position of the screen by adding the speed
                positions[i].X += speed;
                // If the speed has the background moving to the left
                if (speed <= 0)
                {
                    // Check the texture is out of view then put that texture at the end of the screen
                    if (positions[i].X <= -texture.Width/2)
                    {
                        positions[i].X = (texture.Width/2) * (positions.Length - 1);
                    }
                }

                // If the speed has the background moving to the right
                else
                {
                    // Check if the texture is out of view then position it to the start of the screen
                    if (positions[i].X >= (texture.Width/2) * (positions.Length - 1))
                    {
                        positions[i].X = -(texture.Width/2);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                spriteBatch.Draw(texture, positions[i], Color.White);
                //spriteBatch.Draw(texture, new Rectangle((int)positions[i].X, (int)positions[i].Y, graphics.Viewport.Width, graphics.Viewport.Height), Color.White);

            }
        }
    }
}
