using UnityEngine;

namespace Cirilla
{
    public class ParticleBroadcast : MonoBehaviour
    {
        private IObserver observer;
        void Start()
        {
            ParticleSystem particle = GetComponent<ParticleSystem>();
            ParticleSystem.MainModule mainModule = particle.main;
            mainModule.loop = false;
            mainModule.stopAction = ParticleSystemStopAction.Callback;
            observer = IocContainer.instance.Resolve<IObserver>();
        }

        public void OnParticleSystemStopped(){
            observer.Dispatch(ParticleEvent.Stop, gameObject);
        }
    }
}