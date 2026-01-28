using UnityEngine;
using VContainer;

namespace Game.Core.Ticking
{
    public abstract class TickableBehaviour : MonoBehaviour
    {
        private ITickService _tickService;
        private bool _registered;

        [Inject]
        protected void InitializeTicking(ITickService tickService)
        {
            _tickService = tickService;

            if (isActiveAndEnabled)
            {
                Register();
            }
        }

        protected virtual void OnEnable()
        {
            if (_tickService != null)
            {
                Register();
            }
        }

        protected virtual void OnDisable()
        {
            Unregister();
        }

        private void Register()
        {
            if (_registered) return;

            _tickService.Register(this);
            _registered = true;
        }

        private void Unregister()
        {
            if (!_registered) return;

            _tickService.Unregister(this);
            _registered = false;
        }
    }
}
