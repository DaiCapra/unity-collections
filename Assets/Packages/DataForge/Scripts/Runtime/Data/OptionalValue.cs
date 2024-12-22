namespace DataForge.Data
{
    public struct OptionalValue<T> where T : new()
    {
        private T _value;
        public bool isSet;

        public override string ToString()
        {
            return $"value: {_value}: set: {isSet}";
        }

        public static implicit operator bool(OptionalValue<T> obj)
        {
            return obj.isSet;
        }

        public static implicit operator T(OptionalValue<T> obj)
        {
            return obj._value;
        }

        public static implicit operator OptionalValue<T>(T value)
        {
            return new OptionalValue<T>()
            {
                _value = value,
                isSet = true
            };
        }
    }
}