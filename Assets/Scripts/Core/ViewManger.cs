using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.UI.View;

namespace PaperSouls.Core
{
    public class ViewManger : MonoBehaviour
    {
        private static ViewManger _instance;
        private static readonly object Padlock = new();

        public static ViewManger Instance
        {
            get
            {
                lock (Padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new();
                    }

                    return _instance;
                }
            }
        }

        [SerializeField] private View _startingView;
        [SerializeField] private View[] _views;

        private View _currentView;
        private readonly Stack<View> _history = new();

        /// <summary>
        /// Checks if current View is of type T
        /// </summary>
        public static bool GetCurrentViewIs<T>() where T : View
        {
            return _instance._currentView is T;
        }

        /// <summary>
        /// Gets the current view if the view is of type T
        /// </summary>
        public static T GetCurrentView<T>() where T : View
        {
            if (GetCurrentViewIs<T>()) return _instance._currentView as T;
            else return null;
        }

        /// <summary>
        /// Gets a View of type T from the master view list
        /// </summary>
        public static T GetView<T>() where T : View
        {
            foreach (View view in _instance._views)
            {
                if (view is T viewOfType)
                {
                    return viewOfType;
                }
            }

            return null;
        }

        /// <summary>
        /// Displays a view of type T. Remeber parameter indicates if to add the view
        /// to the history or not. 
        /// </summary>
        public static void Show<T>(bool remeber = true) where T : View
        {
            foreach (View view in _instance._views)
            {
                if (view is T)
                {
                    if (_instance._currentView != null)
                    {
                        if (remeber) _instance._history.Push(_instance._currentView);
                        _instance._currentView.Hide();
                    }

                    view.Show();
                    _instance._currentView = view;
                }
            }
        }

        /// <summary>
        /// Displays a view given the view as an input. Remeber parameter indicates
        /// if to add the view to the history or not. 
        /// </summary>
        public static void Show(View view, bool remeber = true)
        {
            if (view != null)
            {
                if (_instance._currentView != null)
                {
                    if (remeber) _instance._history.Push(_instance._currentView);
                    _instance._currentView.Hide();
                }

                view.Show();
                _instance._currentView = view;
            }
        }

        /// <summary>
        /// Displays the last view in the history
        /// </summary>
        public static void ShowLast()
        {
            if (_instance._history.Count != 0)
            {
                Show(_instance._history.Pop(), false);
            }
        }

        /// <summary>
        /// Initalizes each view and displays the starting views if one exists.
        /// </summary>
        private void Init()
        {
            foreach (View view in _views)
            {
                view.Init();
                view.Hide();
            }

            if (_startingView != null) Show(_startingView, true);
        }

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            Init();
        }
    }
}
