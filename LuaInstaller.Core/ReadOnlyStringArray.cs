using System;
using System.Collections;

namespace LuaInstaller.Core
{
    public class ReadOnlyStringArray: IEnumerable
    {
        private readonly string[] _array;

        public ReadOnlyStringArray(string[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            _array = new string[array.Length];
            Array.Copy(array, _array, array.Length);
        }

        public string this[int index]
        {
            get
            {
                return _array[index];
            }
        }

        public int Length
        {
            get
            {
                return _array.Length;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _array.GetEnumerator();
        }
    }
}
