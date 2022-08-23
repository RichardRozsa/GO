using System;
using Microsoft.Win32;
using System.Collections.Generic;

namespace Go.Tools
{
	public class RegistryRoot
	{
		public RegistryRoot(string name, RegistryKey root)
		{
			this.name	= name;
			this.root	= root;
		}
		public string		name;
		public RegistryKey	root;
	}

	public class RegistryRoots
	{
		// Expand Aliases, Registry values and Environment variables
		private static RegistryRoot[] regRoots = new RegistryRoot[]
		{
			new RegistryRoot("HKEY_CLASSES_ROOT",		Registry.ClassesRoot),
			new RegistryRoot("HKEY_CURRENT_CONFIG",		Registry.CurrentConfig),
			new RegistryRoot("HKEY_CURRENT_USER",		Registry.CurrentUser),
			//new RegistryRoot("HKEY_DYN_DATA",			Registry.DynData),
			new RegistryRoot("HKEY_LOCAL_MACHINE",		Registry.LocalMachine),
			new RegistryRoot("HKEY_PERFORMACE_DATA",	Registry.PerformanceData),
			new RegistryRoot("HKEY_USERS",				Registry.Users),
			new RegistryRoot("HKCR",					Registry.ClassesRoot),
			new RegistryRoot("HKCC",					Registry.CurrentConfig),
			new RegistryRoot("HKCU",					Registry.CurrentUser),
			//new RegistryRoot("HKDD",					Registry.DynData),
			new RegistryRoot("HKLM",					Registry.LocalMachine),
			new RegistryRoot("HKPD",					Registry.PerformanceData),
			new RegistryRoot("HKU",						Registry.Users)
		};

		public static bool GetRoot(ref string name, out RegistryKey regKey)
		{
			foreach (RegistryRoot regRoot in regRoots)
			{
				if (	name.Length > regRoot.name.Length 
					&&	name.Substring(0, regRoot.name.Length + 1).ToUpper().CompareTo(regRoot.name + "\\") == 0)
				{
					regKey = regRoot.root;
					name = name.Substring(regRoot.name.Length + 1);
					return true;
				}
			}
			regKey = regRoots[0].root;
			return false;
		}

	    public static string GetValue(string path, string name)
	    {
	        RegistryKey regKey;
            if (!GetRoot(ref path, out regKey))
                throw new Exception("Missing leading registry hive name");

	        var pathKey = regKey.OpenSubKey(path);
	        if (pathKey != null)
	        {
	            string value = (string) pathKey.GetValue(name);
	            // If null value, there may be a (Default) value.
	            if (value == null)
	            {
	                var subKey = pathKey.OpenSubKey(name);
	                if (subKey != null)
	                {
	                    value = (string)subKey.GetValue(null);
	                    subKey.Close();
	                }
	            }
	            pathKey.Close();
	            return value;
	        }

	        return null;
	    }

        public static string GetValue(string name)
		{
			RegistryKey	regKey;
			if (!GetRoot(ref name, out regKey))
				throw new Exception("Missing leading registry hive name");

			string value = (string)regKey.GetValue(name);
            // If null value, there may be a (Default) value.
            if (value == null)
            {
                var subKey = regKey.OpenSubKey(name);
                if (subKey != null)
                {
                    value = (string)subKey.GetValue(null);
                    subKey.Close();
                }
            }

            return value;
		}

        public static string[] GetNames(string name, string keyName)
        {
            RegistryKey regKey;
            if (!GetRoot(ref name, out regKey))
                throw new Exception("Missing leading registry hive name");

            var subKey = regKey.OpenSubKey(name);
            if (subKey != null)
            {
                var subKeyNames = subKey.GetSubKeyNames();
                var list = new List<string>();
                foreach (var subKeyName in subKeyNames)
                {
                    var subSubkey = subKey.OpenSubKey(subKeyName);
                    if (subSubkey != null)
                    {
                        var value = (string)subSubkey.GetValue(keyName);
                        if (value != null)
                        {
                            list.Add(value);
                        }
                    }
                }
                subKey.Close();
                return list.ToArray();
            }

            return null;
        }
	}
}
