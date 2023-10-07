namespace Core.Utils
{
    public struct Integering
    {
        private int m_offset;
        private int m_value;

        public Integering(int value = 0)
        {
            System.Random rand = new System.Random();
            m_offset = rand.Next(-1000, 1000);
            this.m_value = value + m_offset;
        }

        public int Value
        {
            get
            {
                return m_value - m_offset;
            }
        }

        public void Dispose()
        {
            m_offset = 0;
            m_value = 0;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static explicit operator Integering(int a) { return new Integering(a); }
        public static implicit operator int(Integering a) { return a.Value; }

        public static Integering operator +(Integering a, Integering b) { return new Integering(a.Value + b.Value); }
        public static Integering operator +(Integering a, int b) { return new Integering(a.Value + b); }

        public static Integering operator -(Integering a, Integering b) { return new Integering(a.Value - b.Value); }
        public static Integering operator -(Integering a, int b) { return new Integering(a.Value - b); }

        public static Integering operator *(Integering a, Integering b) { return new Integering(a.Value * b.Value); }
        public static Integering operator *(Integering a, int b) { return new Integering(a.Value * b); }

        public static Integering operator /(Integering a, Integering b) { return new Integering(a.Value * b.Value); }
        public static Integering operator /(Integering a, int b) { return new Integering(a.Value * b); }

        public static bool operator ==(Integering a, Integering b) { return a.Value == b.Value; }
        public static bool operator ==(Integering a, int b) { return a.Value == b; }

        public static bool operator !=(Integering a, Integering b) { return a.Value != b.Value; }
        public static bool operator !=(Integering a, int b) { return a.Value != b; }

        public static bool operator >=(Integering a, Integering b) { return a.Value >= b.Value; }
        public static bool operator >=(Integering a, int b) { return a.Value >= b; }

        public static bool operator >(Integering a, Integering b) { return a.Value > b.Value; }
        public static bool operator >(Integering a, int b) { return a.Value > b; }

        public static bool operator <=(Integering a, Integering b) { return a.Value <= b.Value; }
        public static bool operator <=(Integering a, int b) { return a.Value <= b; }

        public static bool operator <(Integering a, Integering b) { return a.Value < b.Value; }
        public static bool operator <(Integering a, int b) { return a.Value < b; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
