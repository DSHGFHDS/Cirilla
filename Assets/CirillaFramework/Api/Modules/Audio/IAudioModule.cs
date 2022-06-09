using UnityEngine;

namespace Cirilla
{
    public interface IAudioModule
    {
        AudioChannel Play(AudioClip audioClip, bool loop = false, int index = 0);
        AudioChannel Play(Vector3 pos, AudioClip audioCli, bool loop = false, int index = 0);
        AudioChannel Play(Transform ownerTrans, AudioClip audioClip, bool loop = false, int index = 0);
        void Clear();
    }
}