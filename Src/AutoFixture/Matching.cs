using System;

namespace Ploeh.AutoFixture
{
    [Flags]
    public enum Matching
    {
        ExactType = 1,
        BaseType = 2,
        ImplementedInterfaces = 4,
        PropertyName = 8,
        ParameterName = 16
    }
}
