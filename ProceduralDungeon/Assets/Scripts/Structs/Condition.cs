using System;

[Serializable]
public struct Condition
{
    public Type type;
    public Possibility possibility;
    public Position position;
}