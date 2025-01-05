using System.Collections.Generic;

public class PrevAndCurrent<T>
{
    T prev;
    T current;

    public delegate bool Comparer(T a, T b);
    Comparer customComparer;

    public PrevAndCurrent(T initialValue)
    {
        prev = initialValue;
        current = initialValue;
    }

    public PrevAndCurrent(T initialValue, Comparer customComparer)
    {
        prev = initialValue;
        current = initialValue;
        this.customComparer = customComparer;
    }

    public void Set(T newValue)
    {
        prev = current;
        current = newValue;
    }

    public bool HasChanged()
    {
        if (customComparer != null)
        {
            return customComparer(prev, current);
        }

        return EqualityComparer<T>.Default.Equals(prev, current);
    }
}