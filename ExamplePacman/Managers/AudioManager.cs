using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace Pacman.Managers
{
    /// <summary>
    /// Centralized audio management system
    /// Provides better control over sound effects and music
    /// </summary>
    public class AudioManager
    {
        private readonly Dictionary<string, SoundEffect> _soundEffects;
        private readonly Dictionary<string, SoundEffectInstance> _soundInstances;

        // Sound effects
        public SoundEffect GameStart { get; private set; }
        public SoundEffect Munch { get; private set; }
        public SoundEffect Credit { get; private set; }
        public SoundEffect Death1 { get; private set; }
        public SoundEffect Death2 { get; private set; }
        public SoundEffect EatFruit { get; private set; }
        public SoundEffect EatGhost { get; private set; }
        public SoundEffect PowerPellet { get; private set; }
        public SoundEffect Extend { get; private set; }
        public SoundEffect Intermission { get; private set; }
        public SoundEffect Retreating { get; private set; }
        public SoundEffect Siren1 { get; private set; }
        public SoundEffect Siren2 { get; private set; }
        public SoundEffect Siren3 { get; private set; }
        public SoundEffect Siren4 { get; private set; }
        public SoundEffect Siren5 { get; private set; }

        // Sound instances (for looping sounds)
        public SoundEffectInstance MunchInstance { get; private set; }
        public SoundEffectInstance PowerPelletInstance { get; private set; }
        public SoundEffectInstance RetreatingInstance { get; private set; }
        public SoundEffectInstance Siren1Instance { get; private set; }

        public AudioManager()
        {
            _soundEffects = new Dictionary<string, SoundEffect>();
            _soundInstances = new Dictionary<string, SoundEffectInstance>();
        }

        public void LoadContent(ContentManager content)
        {
            // Load all sound effects
            Credit = LoadSound(content, "Sounds/credit");
            Death1 = LoadSound(content, "Sounds/death_1");
            Death2 = LoadSound(content, "Sounds/death_2");
            EatFruit = LoadSound(content, "Sounds/eat_fruit");
            EatGhost = LoadSound(content, "Sounds/eat_ghost");
            Extend = LoadSound(content, "Sounds/extend");
            GameStart = LoadSound(content, "Sounds/game_start");
            Intermission = LoadSound(content, "Sounds/intermission");
            Munch = LoadSound(content, "Sounds/munch");
            PowerPellet = LoadSound(content, "Sounds/power_pellet");
            Retreating = LoadSound(content, "Sounds/retreating");
            Siren1 = LoadSound(content, "Sounds/siren_1");
            Siren2 = LoadSound(content, "Sounds/siren_2");
            Siren3 = LoadSound(content, "Sounds/siren_3");
            Siren4 = LoadSound(content, "Sounds/siren_4");
            Siren5 = LoadSound(content, "Sounds/siren_5");

            // Create looping instances
            MunchInstance = CreateInstance(Munch, "Munch", isLooped: true, volume: 0.35f);
            PowerPelletInstance = CreateInstance(PowerPellet, "PowerPellet", isLooped: true);
            RetreatingInstance = CreateInstance(Retreating, "Retreating", isLooped: true);
            Siren1Instance = CreateInstance(Siren1, "Siren1", isLooped: true, volume: 0.8f);
        }

        private SoundEffect LoadSound(ContentManager content, string assetName)
        {
            var sound = content.Load<SoundEffect>(assetName);
            _soundEffects[assetName] = sound;
            return sound;
        }

        private SoundEffectInstance CreateInstance(SoundEffect sound, string name, bool isLooped = false, float volume = 1.0f)
        {
            var instance = sound.CreateInstance();
            instance.IsLooped = isLooped;
            instance.Volume = volume;
            _soundInstances[name] = instance;
            return instance;
        }

        public void PlaySound(SoundEffect sound)
        {
            sound?.Play();
        }

        public void StopAll()
        {
            foreach (var instance in _soundInstances.Values)
            {
                instance.Stop();
            }
        }

        public void PauseAll()
        {
            foreach (var instance in _soundInstances.Values)
            {
                if (instance.State == SoundState.Playing)
                {
                    instance.Pause();
                }
            }
        }

        public void ResumeAll()
        {
            foreach (var instance in _soundInstances.Values)
            {
                if (instance.State == SoundState.Paused)
                {
                    instance.Resume();
                }
            }
        }
    }
}
