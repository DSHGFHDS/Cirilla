using System;
using UnityEngine;

namespace Cirilla
{
    public class AudioChannel
    {
        public int? index => audioInfo?.index;

        private AudioInfo audioInfo;
        private AudioSource audioSource;

        public float volume
        {
            get => audioSource.volume;
            set => audioSource.volume = value;
        }

        public bool mute
        {
            get => audioSource.mute;
            set => audioSource.mute = value;
        }

        public AudioChannel(AudioInfo audioInfo)
        {
            this.audioInfo = audioInfo;
            audioSource = audioInfo.go.GetComponent<AudioSource>();
        }

        public AudioChannel OnUpdate(Action callback)
        {
            if (audioInfo == null)
                return null;

            audioInfo.onUpdate = callback;
            return this;
        }

        public AudioChannel OnComplete(Action callback)
        {
            if (audioInfo == null)
                return null;

            audioInfo.onComplete = callback;

            return this;
        }

        public void Delete()
        {
            if (audioInfo == null)
                return;

            audioInfo.stop();
            audioInfo = null;
        }
    }
}