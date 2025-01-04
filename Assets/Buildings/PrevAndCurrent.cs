using System;
using System.Collections.Generic;

public class PrevAndCurrent<T>
{

    T prev;
    T current;

    public PrevAndCurrent(T initialValue)
    {
        prev = initialValue;
        current = initialValue;
    }

    public void Set(T newValue)
    {
        prev = current;
        current = newValue;
    }

    public bool HasChanged()
    {
        return EqualityComparer<T>.Default.Equals(prev, current);
    }
}