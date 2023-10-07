using UnityEngine;

namespace Core.Utils
{
    public struct Floating
    {
        private float m_offset;
        private float m_value;

        public Floating(float value = 0)
        {
            System.Random rand = new System.Random();
            m_offset = rand.Next(-1000, 1000) * 1.0f;
            this.m_value = value + m_offset;
        }

        public float Value
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

        public static bool nearlyEqual(float a, float b, float epsilon)
        {
            float absA = Mathf.Abs(a);
            float absB = Mathf.Abs(b);
            float diff = Mathf.Abs(a - b);

            if (a == b)
            { // shortcut, handles infinities
                return true;
            }
            else if (a == 0 || b == 0 || diff < float.MinValue)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * float.MinValue);
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }

        public static explicit operator Floating(float a) { return new Floating(a); }
        public static implicit operator float(Floating a) { return a.Value; }

        public static Floating operator +(Floating a, Floating b) { return new Floating(a.Value + b.Value); }
        public static Floating operator +(Floating a, float b) { return new Floating(a.Value + b); }

        public static Floating operator -(Floating a, Floating b) { return new Floating(a.Value - b.Value); }
        public static Floating operator -(Floating a, float b) { return new Floating(a.Value - b); }

        public static Floating operator *(Floating a, Floating b) { return new Floating(a.Value * b.Value); }
        public static Floating operator *(Floating a, float b) { return new Floating(a.Value * b); }

        public static Floating operator /(Floating a, Floating b) { return new Floating(a.Value * b.Value); }
        public static Floating operator /(Floating a, float b) { return new Floating(a.Value * b); }

        public static bool operator ==(Floating a, Floating b) { return a.Value == b.Value; }
        public static bool operator ==(Floating a, float b) { return a.Value == b; }

        public static bool operator !=(Floating a, Floating b) { return a.Value != b.Value; }
        public static bool operator !=(Floating a, float b) { return a.Value != b; }

        public static bool operator >=(Floating a, Floating b) { return a.Value >= b.Value; }
        public static bool operator >=(Floating a, float b) { return a.Value >= b; }

        public static bool operator >(Floating a, Floating b) { return a.Value > b.Value; }
        public static bool operator >(Floating a, float b) { return a.Value > b; }

        public static bool operator <=(Floating a, Floating b) { return a.Value <= b.Value; }
        public static bool operator <=(Floating a, float b) { return a.Value <= b; }

        public static bool operator <(Floating a, Floating b) { return a.Value < b.Value; }
        public static bool operator <(Floating a, float b) { return a.Value < b; }

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
