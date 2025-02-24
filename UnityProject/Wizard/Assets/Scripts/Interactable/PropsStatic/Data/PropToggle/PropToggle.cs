﻿using System;
using DG.Tweening;
using Entity.Player;
using FiniteStateMachine;
using Player;
using UnityEngine;

namespace Interactable.PropsStatic.Data.PropToggle
{
    // еще нужно чтобы нельзя было взаимодействовать пока воспроизводится анимация
    
    public class PropToggle : PropStatic, IHasState
    {
        private PropToggleSO _propData;
        private bool _openClose;
        private StateMachine _stateMachine;
        private OpenState _openState;
        private CloseState _closeState;
        // private LockedState _lockedState;

        private void Awake()
        {
            _propData = (PropToggleSO) propData;
            
            _stateMachine = new StateMachine();
            _openState = new OpenState(transform, _propData);
            _closeState = new CloseState(transform, _propData);
            // _lockedState = new LockedState(transform, _propData);
            
            // Any(_lockedState, new FuncPredicate(() => _propData.isLocked));  // дорабоать нужно когда сделаю замок -> когда сделааю инвентарь
            At(_closeState, _openState, new FuncPredicate(() => _openClose && !_propData.isLocked));
            At(_openState, _closeState, new FuncPredicate(() => !_openClose && !_propData.isLocked));
            // + locked state
            
            _stateMachine.SetState(_closeState);
        }
        
        private void Start()
        {
            OnPropAction += Toggle;
        }

        private void Update()
        {
            _stateMachine.Update();
        }

        private void Toggle(PlayerManager player)
        {
            _openClose = !_openClose;
        }
        
        public void At(IState from, IState to, IPredicate condition) =>
            _stateMachine.AddTransition(from, to, condition);
        
        public void Any(IState to, IPredicate condition) =>
            _stateMachine.AddAnyTransition(to, condition);
        
        // TODO: сделать базовый класс статов для interactable'ов
        private class OpenState : BaseState
        {
            private Transform _tr;
            private PropToggleSO _data;

            public OpenState(Transform tr, PropToggleSO data)
            {
                _tr = tr;
                _data = data;
            }

            public override void OnEnter()
            {
                Debug.Log("open");
                _tr.DORotate(_data.animatedObjectPos1, _data.duration, RotateMode.Fast);
            }
        }
        
        private class CloseState : BaseState
        {
            private Transform _tr;
            private PropToggleSO _data;

            public CloseState(Transform tr, PropToggleSO data)
            {
                _tr = tr;
                _data = data;
            }

            public override void OnEnter()
            {
                Debug.Log("close");

                _tr.DORotate(_data.animatedObjectPos2, _data.duration, RotateMode.Fast);
            }
        }

        // private class LockedState : BaseState<Transform, PropToggleSO>
        // {
        //     public LockedState(Transform tr, PropToggleSO data) : base(tr, data) { }
        //
        //     public override void OnEnter()
        //     {
        //         // _actor.DORotate(_data.animatedObjectPos2, _data.duration, RotateMode.Fast);
        //     }
        // }
    }
}