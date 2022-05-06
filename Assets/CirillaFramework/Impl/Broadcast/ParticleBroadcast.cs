﻿using UnityEngine;

namespace Cirilla
{
    [AddComponentMenu("Cirilla/Broadcast/ParticleBroadcast")]
    public class ParticleBroadcast : MonoBehaviour
    {
        private IObserver observer;
        private void Start()
        {
            ParticleSystem particle = GetComponent<ParticleSystem>();
            ParticleSystem.MainModule mainModule = particle.main;
            mainModule.loop = false;
            mainModule.stopAction = ParticleSystemStopAction.Callback;
            observer = IocContainer.instance.Resolve<IObserver>();
        }

        private void OnParticleSystemStopped(){
            observer.Dispatch(ParticleEvent.Stop, gameObject);
        }
    }
}