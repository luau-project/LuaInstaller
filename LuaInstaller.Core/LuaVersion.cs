using System;

namespace LuaInstaller.Core
{
    public sealed class LuaVersion : IComparable<LuaVersion>
    {
        private readonly int _major;
        private readonly int _minor;
        private readonly int? _build;

        public LuaVersion(int major, int minor, int? build)
        {
            _major = major;
            _minor = minor;
            _build = build;
        }

        public int Major
        {
            get
            {
                return _major;
            }
        }

        public int Minor
        {
            get
            {
                return _minor;
            }
        }

        public int? Build
        {
            get
            {
                return _build;
            }
        }

        public string ShortVersion
        {
            get
            {
                return string.Format("{0}.{1}", _major, _minor);
            }
        }

        public string ShortVersionWithoutDot
        {
            get
            {
                return string.Format("{0}{1}", _major, _minor);
            }
        }

        public string Version
        {
            get
            {
                string result;
                if (_build.HasValue)
                {
                    result = string.Format(
                        "{0}.{1}.{2}",
                        _major,
                        _minor,
                        _build.Value
                    );
                }
                else
                {
                    result = string.Format(
                        "{0}.{1}",
                        _major,
                        _minor
                    );
                }
                return result;
            }
        }

        public override string ToString()
        {
            return Version;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            bool result = false;

            if (!(obj == null || GetType() != obj.GetType()))
            {
                LuaVersion other = (LuaVersion)obj;
                result = _major == other._major &&
                    _minor == other._minor &&
                    _build == other._build;
            }

            return result;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return (_build.HasValue ? _build.Value : 0) | (_minor << 8) | (_major << 16);
        }

        public int CompareTo(LuaVersion other)
        {
            return GetHashCode() - other.GetHashCode();
        }
    }
}
