using System;
using System.Collections.Generic;
[Serializable]
public struct Condition
{
    public Type type;
    public Possibility possibility;
    public List<Position> position;
}