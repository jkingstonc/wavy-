using System;

namespace wavynet.vm.data.items
{
    [Serializable]
    public class Wnull : WItem
    {
        private Wnull()
        {
        }

        public static implicit operator Wnull(int value)
        {
            return null;
        }

        public override string ToString()
        {
            return "null";
        }
    }

    [Serializable]
    public class Wbool : WItem
    {
        private Wbool(bool value)
        {
            this.value = value;
        }

        private Wbool()
        {

        }

        public static implicit operator Wbool(bool value)
        {
            return new Wbool(value);
        }

        public static implicit operator bool(Wbool wbool)
        {
            return wbool.value;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    [Serializable]
    public class Wint : WItem
    {
        private Wint(int value)
        {
            this.value = value;
        }

        private Wint()
        {

        }

        public static implicit operator Wint(int value)
        {
            return new Wint(value);
        }

        public static implicit operator int(Wint wint)
        {
            return wint.value;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    [Serializable]
    public class Wdouble : WItem
    {
        private Wdouble(double value)
        {
            this.value = value;
        }

        private Wdouble()
        {

        }

        public static implicit operator Wdouble(double value)
        {
            return new Wdouble(value);
        }

        public static implicit operator double(Wdouble wdouble)
        {
            return wdouble.value;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    [Serializable]
    public class Wstring : WItem
    {
        private Wstring(string value)
        {
            this.value = value;
        }

        private Wstring()
        {

        }

        public static implicit operator Wstring(string value)
        {
            return new Wstring(value);
        }

        public static implicit operator string(Wstring wstring)
        {
            return wstring.value;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}