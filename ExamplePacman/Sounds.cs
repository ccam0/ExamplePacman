using System;
using Microsoft.Xna.Framework.Audio;

namespace Pacman
{
    /// <summary>
    /// Manages all game audio including sound effects and their instances.
    /// </summary>
    public static class Sounds
    {
        // Sound effects
        public static SoundEffect GameStart;
        public static SoundEffect Munch;
        public static SoundEffect Credit;
        public static SoundEffect Death1;
        public static SoundEffect Death2;
        public static SoundEffect EatFruit;
        public static SoundEffect EatGhost;
        public static SoundEffect PowerPellet;
        public static SoundEffect Extend;
        public static SoundEffect Intermission;
        public static SoundEffect Retreating;
        public static SoundEffect Siren1;
        public static SoundEffect Siren2;
        public static SoundEffect Siren3;
        public static SoundEffect Siren4;
        public static SoundEffect Siren5;

        // Sound effect instances for looping sounds
        public static SoundEffectInstance MunchInstance;
        public static SoundEffectInstance PowerPelletInstance;
        public static SoundEffectInstance RetreatingInstance;
        public static SoundEffectInstance Siren1Instance;

        /// <summary>
        /// Stops all currently playing sound instances.
        /// </summary>
        public static void StopAllSounds()
        {
            MunchInstance?.Stop();
            PowerPelletInstance?.Stop();
            RetreatingInstance?.Stop();
            Siren1Instance?.Stop();
        }
    }
}
