using System;

namespace LuaInstaller.Core
{
    public class VisualStudioToolset
    {
        private readonly string _cl;
        private readonly string _link;
        private readonly string _ml;

        public VisualStudioToolset(string cl, string link, string ml)
        {
            if (cl == null)
            {
                throw new ArgumentNullException("cl");
            }
            
            if (link == null)
            {
                throw new ArgumentNullException("link");
            }
            
            if (ml == null)
            {
                throw new ArgumentNullException("ml");
            }
            
            _cl = cl;
            _link = link;
            _ml = ml;
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

        public string Ml
        {
            get
            {
                return _ml;
            }
        }
    }
}
