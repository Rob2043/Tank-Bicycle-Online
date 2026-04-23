using System;
using System.Collections.Generic;
using CustomEventBus;
using UnityEngine;
using Tanks.Complete;
using System.Security.Permissions;

namespace Game.Level
{
    public class InitGame : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private TankMovement[] tankMovement;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private Timer timer;
        [SerializeField] private RespawnManager respawnManager;


        private EventBus _eventBus;

        private List<IDisposable> _disposables = new();

        private void Awake()
        {
            _eventBus = new EventBus();

            RegisterServices();
            Init();
            AddDisposables();
        }
        private void RegisterServices()
        {
            ServiceLocator.Initialize();
            ServiceLocator.Current.Register(_eventBus);
        }

        private void Init()
        {
            inputManager?.Init();
            scoreManager?.Init();
            timer?.Init();
            respawnManager?.Init();
            foreach (var item in tankMovement)
            {
                item?.Init();
            }
        }

        private void AddDisposables()
        {
            //_disposables.Add(_scoreController);
        }

        private void OnDestroy()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }

        private void OnDisable()
        {
            inputManager?.Disable();
            scoreManager?.Disable();
            respawnManager?.Disable();
            foreach (var item in tankMovement)
            {
                item?.Disable();
            }
        }
    }
}