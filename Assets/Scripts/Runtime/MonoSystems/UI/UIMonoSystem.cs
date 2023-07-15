using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PaperSouls.Core;
using PaperSouls.Runtime.UI.View;

namespace PaperSouls.Runtime.MonoSystems.UI
{
    internal sealed class UIMonoSystem : MonoBehaviour, IUIMonoSystem
    {
        [SerializeField] private View _startingView;
        [SerializeField] private View[] _views;

        private View _currentView;
        private readonly Stack<View> _history = new();

        public bool GetCurrentViewIs<T>() where T : View
        {
            return _currentView is T;
        }

        public T GetCurrentView<T>() where T : View
        {
            if (GetCurrentViewIs<T>()) return _currentView as T;
            else return null;
        }

        public T GetView<T>() where T : View
        {
            foreach (View view in _views)
            {
                if (view is T viewOfType)
                {
                    return viewOfType;
                }
            }

            return null;
        }

        public void Show<T>(bool remeber = true) where T : View
        {
            foreach (View view in _views)
            {
                if (view is T)
                {
                    if (_currentView != null)
                    {
                        if (remeber) _history.Push(_currentView);
                        _currentView.Hide();
                    }

                    view.Show();
                    _currentView = view;
                }
            }
        }

        /// <summary>
        /// Displays a view given the view as an input. Remeber parameter indicates
        /// if to add the view to the history or not. 
        /// </summary>
        private void Show(View view, bool remeber = true)
        {
            if (view != null)
            {
                if (_currentView != null)
                {
                    if (remeber) _history.Push(_currentView);
                    _currentView.Hide();
                }

                view.Show();
                _currentView = view;
            }
        }

        public void ShowLast()
        {
            if (_history.Count != 0)
            {
                Show(_history.Pop(), false);
            }
        }

        public void HideAllViews()
        {
            foreach (View view in _views) view.Hide();
     
        }

        private void ChangeView(ChangeViewMessage msg)
        {
            Show(msg.NextView, msg.Remember);
        }

        private void AddListeners()
        {
            GameManager.AddListener<ChangeViewMessage>(ChangeView);
            GameManager.AddListener<ResetViewMessage>(new(delegate (ResetViewMessage msg) { Init(); }));
        }

        private void Init()
        {
            _views = FindObjectsOfType<MonoBehaviour>().OfType<View>().ToArray();
            foreach (View view in _views)
            {
                view.Init();
                //view.Hide();
            }

            if (_startingView != null) Show(_startingView, true);
        }

        private void Awake()
        {
            AddListeners();
            Init();
        }

        private void Start()
        {
            HideAllViews();
        }
    }
}
