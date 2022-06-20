using System;

namespace LuaInstaller.Core
{
    public class VisualStudioToolset
    {
        private readonly string _cl;
        private readonly string _link;

        public VisualStudioToolset(string cl, string link)
        {
            if (cl == null)
            {
                throw new ArgumentNullException("cl");
            }
            
            if (link == null)
            {
                throw new ArgumentNullException("link");
            }

            _cl = cl;
            _link = link;
        }
        
        public string Cl
        {
            get
            {
                return _cl;
            }
        }

        public string Link
        {
            get
            {
                return _link;
            }
        }
    }
}
