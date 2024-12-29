using System.Collections.Generic;

namespace DataForge.Tests
{
    public class Resource
    {
        public int amount;

        public Dictionary<string, int> map = new()
        {
            { "1", 1 },
            { "2", 2 },
            { "3", 3 }
        };
    }
}