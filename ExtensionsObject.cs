using System;

namespace RA
{
    public static class ObjectExtensions
    {
        public static double ToDouble(this object obj)
        {
            switch (obj)
            {
                case double @_:return @_;
                case string @_:return double.Parse(@_);
                default: throw new NotImplementedException($"object.ToDouble is not implemented for type of {obj.GetType()}");
            }

        }

    }
}
