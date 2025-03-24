using System;

namespace KindMen.Uxios.Http
{
    public class Header : Tuple<string, string>
    {
        public string Name => Item1;
        public string Value => Item2;

        public Header(string name, string value) : base(name, value)
        {
        }
        
        public static explicit operator Header((string, string) tuple)
        {
            return new Header(tuple.Item1, tuple.Item2);
        }

        public override string ToString()
        {
            return $"{this.Name}: {this.Value}";
        }
    }
}