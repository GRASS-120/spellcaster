using Player;
using UnityEngine;

namespace FiniteStateMachine
{
    // пока универсальное состояние
    // TODO: делать через дженерики - так себе. убрать вообще поля?
    public abstract class BaseState : IState
    {
        public virtual void OnEnter()
        {
            // nope
        }

        public virtual void Update()
        {
            // nope
        }

        public virtual void FixedUpdate()
        {
            // nope
        }

        public virtual void OnExit()
        {
            // nope
        }
    }
}