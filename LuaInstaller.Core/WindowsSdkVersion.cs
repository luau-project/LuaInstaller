using System;

namespace LuaInstaller.Core
{
    public sealed class WindowsSdkVersion: IComparable<WindowsSdkVersion>
    {
        private readonly int _major;
        private readonly int _minor;
        private readonly string _installationDir;
        private readonly string _productVersion;

        public string InstallationDir
        {
            get
            {
                return _installationDir;
            }
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

        public string ProductVersion
        {
            get
            {
                return _productVersion;
            }
        }

        public WindowsSdkVersion(int major, int minor, string installationDir, string productVersion)
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

            _major = major;
            _minor = minor;
            _installationDir = installationDir;
            _productVersion = productVersion;
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
                result = _minor == other._minor && _major == other._major && _installationDir == other._installationDir && _productVersion == other._productVersion;
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

        public int CompareTo(WindowsSdkVersion other)
        {
            return other.GetHashCode() - GetHashCode();
        }
    }
}
