using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    class Animacion
    {
        // Esta imagen contiene la colección completa de sprites (la imagen con todos los sprites)
        Texture2D spriteStrip;
        // La escala que se le aplica a la imagen de sprites
        float scale;
        // El tiempo desde que se actualizó el último sprite
        int elapsedTime;
        // El tiempo durante el cual se muestra un sprite antes del próximo
        int frameTime;
        // El número de sprites que contiene la imagen
        int frameCount;
        // El índice del sprite que se muestra actualmente
        int currentFrame;
        // El color del sprite que estamos mostrando, esto es útil si se quieren hacer efectos como que se vuelva rojo si colisiona.
        Color color;
        // El área de la tira de imágenes que queremos mostrar
        Rectangle sourceRect = new Rectangle();
        // El area donde queremos dibujar la tira de imágenes en el juego.
        Rectangle destinationRect = new Rectangle();
        // Ancho del sprite
        public int FrameWidth;
        // Alto del sprite (frame)
        public int FrameHeight;
        // El estado de la animación
        public bool Active;
        // Vamos a loopear o se reproduce una vez y se queda quieto?
        public bool Looping;
        // Posición, para que era esto?!? (Por que se duplicaba?)
        public Vector2 Position;

        public void Initialize(Texture2D texture, Vector2 position,
        int frameWidth, int frameHeight, int frameCount,
        int frametime, Color color, float scale, bool looping)
        {
            this.color = color;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.frameCount = frameCount;
            this.frameTime = frametime;
            this.scale = scale;

            Looping = looping;
            Position = position;
            spriteStrip = texture;

            // Ponemos el tiempo en cero.
            elapsedTime = 0;
            currentFrame = 0;

            // Por defecto las animaciones están activas.
            Active = true;
        }
        public void Update(GameTime gameTime)
        {
            // Do not update the game if we are not active
            if (!Active) return;

            // Update the elapsed time
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            // If the elapsed time is larger than the frame time
            // we need to switch frames
            if (elapsedTime > frameTime)
            {
                // Move to the next frame
                currentFrame++;

                // If the currentFrame is equal to frameCount reset currentFrame to zero
                if (currentFrame == frameCount)
                {
                    currentFrame = 0;
                    // If we are not looping deactivate the animation
                    if (Looping == false)
                        Active = false;
                }

                // Reset the elapsed time to zero
                elapsedTime = 0;
            }

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
            sourceRect = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
            destinationRect = new Rectangle((int)Position.X - (int)(FrameWidth * scale) / 2,
            (int)Position.Y - (int)(FrameHeight * scale) / 2,
            (int)(FrameWidth * scale),
            (int)(FrameHeight * scale));
        }
        // Draw the Animation Strip
        public void Draw(SpriteBatch spriteBatch)
        {
            // Only draw the animation when we are active
            if (Active) spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);
        }
    }
}
