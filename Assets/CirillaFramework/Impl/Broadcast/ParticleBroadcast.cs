using UnityEngine;

namespace Cirilla
{
    [AddComponentMenu("Cirilla/Broadcast/ParticleBroadcast")]
    public class ParticleBroadcast : MonoBehaviour
    {
        private IObserverModule observer;
        private IContainer containerIns;
        private void Start()
        {
            containerIns = IocContainer.instance;
            ParticleSystem particle = GetComponent<ParticleSystem>();
            ParticleSystem.MainModule mainModule = particle.main;
            mainModule.loop = false;
            mainModule.stopAction = ParticleSystemStopAction.Callback;
            observer = containerIns.Resolve<IObserverModule>();
        }

        private void OnParticleSystemStopped(){
            observer.Dispatch(ParticleEvent.Stop, gameObject);
        }
    }
}