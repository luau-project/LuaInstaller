using System;

namespace LuaInstaller.Core
{
    public sealed class VisualStudioVersion : IComparable<VisualStudioVersion>
    {
        private readonly int _major;
        private readonly int _minor;
        private readonly int? _build;
        private readonly int? _revision;
        private readonly string _vsDir;
        private readonly string _vcDir;

        public VisualStudioVersion(int major, int minor, string vsDir, string vcDir, int? build = null, int? revision = null)
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

            if (build != null && build.HasValue && build.Value < 0)
            {
                throw new ArgumentException("Integer number greater than or equal to zero expected.", "build");
            }

            if (revision != null && revision.HasValue && revision.Value < 0)
            {
                throw new ArgumentException("Integer number greater than or equal to zero expected.", "revision");
            }

            _major = major;
            _minor = minor;
            _vsDir = vsDir;
            _vcDir = vcDir;
            _build = build;
            _revision = revision;
        }

        public int Major { get { return _major; } }

        public int Minor { get { return _minor; } }

        public int? Build { get { return _build; } }

        public int? Revision { get { return _revision; } }

        public string VcDir { get { return _vcDir; } }

        public string VsDir { get { return _vsDir; } }


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
                result = _minor == other._minor && _major == other._major && _build == other._build && _revision == other._revision;
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
            return _build != null && _build.HasValue && _revision != null && _revision.HasValue ? string.Format("{0}.{1}.{2}.{3}", _major, _minor, _build.Value, _revision.Value) : string.Format("{0}.{1}", _major, _minor);
        }

        public int CompareTo(VisualStudioVersion other)
        {
            return other.GetHashCode() - GetHashCode();
        }
    }
}
