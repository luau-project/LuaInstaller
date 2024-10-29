using System;

namespace LuaInstaller.Core
{
    public sealed class WindowsSdkVersion: IComparable<WindowsSdkVersion>
    {
        private readonly int _major;
        private readonly int _minor;
        private readonly int? _build;
        private readonly int? _revision;
        private readonly string _installationDir;
        private readonly string _productVersion;

        public int Major { get { return _major; } }

        public int Minor { get { return _minor; } }

        public int? Build { get { return _build; } }

        public int? Revision { get { return _revision; } }

        public string InstallationDir { get { return _installationDir; } }

        public string ProductVersion { get { return _productVersion; } }

        public bool HasUniversalCRT { get { return _major >= 10; } }

        public WindowsSdkVersion(int major, int minor, string installationDir, string productVersion, int? build = null, int? revision = null)
        {
            if (major < 0)
            {
                throw new ArgumentOutOfRangeException("major");
            }

            if (minor < 0)
            {
                throw new ArgumentOutOfRangeException("minor");
            }
            
            if (installationDir == null)
            {
                throw new ArgumentNullException("installationDir");
            }
            
            if (productVersion == null)
            {
                throw new ArgumentNullException("productVersion");
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
            _installationDir = installationDir;
            _productVersion = productVersion;
            _build = build;
            _revision = revision;
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
                WindowsSdkVersion other = (WindowsSdkVersion)obj;
                result = _minor == other._minor &&
                    _major == other._major &&
                    _installationDir == other._installationDir &&
                    _productVersion == other._productVersion &&
                    _build == other._build &&
                    _revision == other._revision;
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
            return HasUniversalCRT ?
                string.Format("{0}.{1} ({0}.{1}.{2}.{3})", _major, _minor, _build.Value, _revision.Value) :
                string.Format("{0}.{1} ({2})", _major, _minor, _productVersion);
        }

        public int CompareTo(WindowsSdkVersion other)
        {
            int[] versionDigits = new int[4] { _major, _minor, _build == null ? 0 : _build.Value, _revision == null ? 0 : _revision.Value };
            int[] otherVersionDigits = new int[4] { other._major, other._minor, other._build == null ? 0 : other._build.Value, other._revision == null ? 0 : other._revision.Value };

            int result = 0;
            int i = 0;
            int len = versionDigits.Length;

            while (result == 0 && i < len)
            {
                result = versionDigits[i] - otherVersionDigits[i];
                i++;
            }
            
            return result;
        }
    }
}
