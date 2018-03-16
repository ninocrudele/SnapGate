using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace GrabCaster.Framework.Security
{
    class Registry
    {
        private const string MAIN_KEY = "GrabCater";

        public static void WriteKey(string Key,string Value)
        {
            Microsoft.Win32.RegistryKey key;
            key = CreateKeyIfNotExist(Key);
            key.SetValue(Key,Value);
            key.Close();
        }

        public static Microsoft.Win32.RegistryKey CreateKeyIfNotExist(string keyName)
        {
            string key = string.Concat(MAIN_KEY, "\\", keyName);
            RegistryKey keyToCheck = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(key);
            if (keyToCheck == null)
            {
                Microsoft.Win32.Registry.LocalMachine.CreateSubKey(keyToCheck.Name);
            }
            return keyToCheck;
        }
    }
}
