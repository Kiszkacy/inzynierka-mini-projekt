using System;
using System.Collections.Generic;

public sealed class EventManager : Singleton<EventManager>
{
    private readonly List<Observable> observers = new();
    
    public void RegisterEvent(Event @event)
    {
        foreach (Observable observer in observers)
        {
            observer.Notify(@event);
        }
    }

    public void Subscribe(Observable observer)
    {
        if (this.observers.Contains(observer)) return;
        
        this.observers.Add(observer);
    }
    
    public void Unsubscribe(Observable observer)
    {
        if (!this.observers.Contains(observer)) return;
        
        this.observers.Remove(observer);
    }
}