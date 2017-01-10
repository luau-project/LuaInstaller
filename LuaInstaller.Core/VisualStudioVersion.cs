using System;

namespace LuaInstaller.Core
{
    public sealed class VisualStudioVersion : IComparable<VisualStudioVersion>
    {
        private readonly int _major;
        private readonly int _minor;
        private readonly string _vsDir;
        private readonly string _vcDir;

        public VisualStudioVersion(int major, int minor, string vsDir, string vcDir)
        {
            if (major < 0)
            {
                throw new ArgumentNullException("major");
            }

            if (minor < 0)
            {
                throw new ArgumentNullException("minor");
            }

            if (vsDir == null)
            {
                throw new ArgumentNullException("vsDir");
            }
            
            if (vcDir == null)
            {
                throw new ArgumentNullException("vcDir");
            }

            _major = major;
            _minor = minor;
            _vsDir = vsDir;
            _vcDir = vcDir;
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

        public string VcDir
        {
            get
            {
                return _vcDir;
            }
        }

        public string VsDir
        {
            get
            {
                return _vsDir;
            }
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
                VisualStudioVersion other = (VisualStudioVersion)obj;
                result = _minor == other._minor && _major == other._major;
            }

            return result;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return (_minor & 0xFF) | ((_major & 0xFF) << 16);
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", _major, _minor);
        }

        public int CompareTo(VisualStudioVersion other)
        {
            return other.GetHashCode() - GetHashCode();
        }
    }
}
