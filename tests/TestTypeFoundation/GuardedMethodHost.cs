﻿namespace TestTypeFoundation;

public class GuardedMethodHost
{
    public void ConsumeString(string s)
    {
        if (s is null)
        {
            throw new ArgumentNullException(nameof(s));
        }
        if (s.Length == 0)
        {
            throw new ArgumentException("String cannot be empty.", nameof(s));
        }
    }

    public void ConsumeInt32(int i)
    {
    }

    public void ConsumeGuid(Guid g)
    {
        if (g == Guid.Empty)
        {
            throw new ArgumentException("Guid cannot be empty.", nameof(g));
        }
    }

    public void ConsumeStringAndInt32(string s, int i)
    {
        if (s is null)
        {
            throw new ArgumentNullException(nameof(s));
        }
        if (s.Length == 0)
        {
            throw new ArgumentException("String cannot be empty.", nameof(s));
        }
    }

    public void ConsumeStringAndGuid(string s, Guid g)
    {
        if (s is null)
        {
            throw new ArgumentNullException(nameof(s));
        }
        if (s.Length == 0)
        {
            throw new ArgumentException("String cannot be empty.", nameof(s));
        }
        if (g == Guid.Empty)
        {
            throw new ArgumentException("Guid cannot be empty.", nameof(g));
        }
    }

    public void ConsumeInt32AndGuid(int i, Guid g)
    {
        if (g == Guid.Empty)
        {
            throw new ArgumentException("Guid cannot be empty.", nameof(g));
        }
    }

    public void ConsumeStringAndInt32AndGuid(string s, int i, Guid g)
    {
        if (s is null)
        {
            throw new ArgumentNullException(nameof(s));
        }
        if (s.Length == 0)
        {
            throw new ArgumentException("String cannot be empty.", nameof(s));
        }
        if (g == Guid.Empty)
        {
            throw new ArgumentException("Guid cannot be empty.", nameof(g));
        }
    }
}