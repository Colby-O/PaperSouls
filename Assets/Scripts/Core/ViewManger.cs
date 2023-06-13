using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManger : MonoBehaviour
{
    private static ViewManger instance;
    public static ViewManger Instance
    {
        get
        {
            if (instance == null) Debug.Log("UI Manger is null!!!");

            return instance;
        }

        private set { }
    }

    public View startingView;
    public View[] views;

    [SerializeField] private View currentView;
    private readonly Stack<View> history = new();

    public static bool GetCurrentViewIs<T>() where T : View
    {
        return instance.currentView is T;
    }

    public static T GetCurrentView<T>() where T : View
    {
        if (GetCurrentViewIs<T>()) return instance.currentView as T;
        else return null;
    }

    public static T GetView<T>() where T : View
    {
        foreach (View view in instance.views)
        {
            if (view is T viewOfType)
            {
                return viewOfType;
            }
        }

        return null;
    }

    public static void Show<T>(bool remeber = true) where T : View
    {
        foreach (View view in instance.views)
        {
            if (view is T)
            {
                if (instance.currentView != null)
                {
                    if (remeber) instance.history.Push(instance.currentView);
                    instance.currentView.Hide();
                }

                view.Show();
                instance.currentView = view;
            }
        }
    }
    public static void Show(View view, bool remeber = true)
    {
        if (view != null)
        {
            if (instance.currentView != null)
            {
                if (remeber) instance.history.Push(instance.currentView);
                instance.currentView.Hide();
            }

            view.Show();
            instance.currentView = view;
        }
    }

    public static void ShowLast()
    {
        if (instance.history.Count != 0)
        {
            Show(instance.history.Pop(), false);
        }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach (View view in views)
        {
            view.Init();
            view.Hide();
        }

        if (startingView != null) Show(startingView, true);
    }
}
