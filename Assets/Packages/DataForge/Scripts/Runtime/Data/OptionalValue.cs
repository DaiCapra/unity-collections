namespace DataForge.Data
{
    public struct OptionalValue<T> where T : new()
    {
        public T value;
        public bool isSet;

        public override string ToString()
        {
            return $"value: {value}: set: {isSet}";
        }

        public static implicit operator bool(OptionalValue<T> obj)
        {
            return obj.isSet;
        }

        public static implicit operator T(OptionalValue<T> obj)
        {
            return obj.value;
        }

        public static implicit operator OptionalValue<T>(T value)
        {
            return new()
            {
                value = value,
                isSet = true
            };
        }
    }
}