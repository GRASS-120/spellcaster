using Player;
using UnityEngine;

namespace FiniteStateMachine
{
    // пока универсальное состояние
    // TODO: делать через дженерики - так себе. убрать вообще поля?
    public abstract class BaseState : IState
    {
        // protected readonly T1 _actor;
        // protected readonly T2 _data;
        // protected BaseState(T1 actor, T2 data)
        // {
        //     _actor = actor;
        //     _data = data;
        // }

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