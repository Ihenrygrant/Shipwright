using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class ResolutionHandler : MonoBehaviour
{

    private Dictionary<string, UnityEvent> eventDictionary;

    private static ResolutionHandler eventManager;

    private Vector2 resolution;

    private void Awake()
    {
        resolution = new Vector2(Screen.width, Screen.height);
    }

    void Update()
    {
        if (resolution.x != Screen.width || resolution.y != Screen.height)
        {
            ResolutionHandler.TriggerEvent("test");

            resolution.x = Screen.width;
            resolution.y = Screen.height;
        }
    }

    public static ResolutionHandler instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(ResolutionHandler)) as ResolutionHandler;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }

            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }
    }

    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        if (eventManager == null) return;
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}