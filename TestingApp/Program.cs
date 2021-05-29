using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingApp
{
  public class InstalledApp
  {
    public string DisplayName { get; set; }
    public string InstallationLocation { get; set; }
  }
  class Program
  {
    static void Main(string[] args)
    {
      foreach (var item in GetFullListInstalledApplication())
      {
        Console.WriteLine(item.DisplayName + " --- " + item.InstallationLocation);
      }

      Console.ReadLine();
    }

    private static List<InstalledApp> GetFullListInstalledApplication()
    {
      IEnumerable<InstalledApp> finalList = new List<InstalledApp>();

      string registry_key_32 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
      string registry_key_64 = @"SOFTWARE\WoW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

      List<InstalledApp> win32AppsCU = GetInstalledApplication(Registry.CurrentUser, registry_key_32);
      List<InstalledApp> win32AppsLM = GetInstalledApplication(Registry.LocalMachine, registry_key_32);
      List<InstalledApp> win64AppsCU = GetInstalledApplication(Registry.CurrentUser, registry_key_64);
      List<InstalledApp> win64AppsLM = GetInstalledApplication(Registry.LocalMachine, registry_key_64);

      finalList = win32AppsCU.Concat(win32AppsLM).Concat(win64AppsCU).Concat(win64AppsLM);

      finalList = finalList.GroupBy(d => d.DisplayName).Select(d => d.First());

      return finalList.OrderBy(o => o.DisplayName).ToList();
    }

    private static List<InstalledApp> GetInstalledApplication(RegistryKey regKey, string registryKey)
    {
      List<InstalledApp> list = new List<InstalledApp>();
      using (Microsoft.Win32.RegistryKey key = regKey.OpenSubKey(registryKey))
      {
        if (key != null)
        {
          foreach (string name in key.GetSubKeyNames())
          {
            using (RegistryKey subkey = key.OpenSubKey(name))
            {
              string displayName = (string)subkey.GetValue("DisplayName");
              string installLocation = (string)subkey.GetValue("InstallLocation");

              if (!string.IsNullOrEmpty(displayName)) // && !string.IsNullOrEmpty(installLocation)
              {
                list.Add(new InstalledApp()
                {
                  DisplayName = displayName.Trim(),
                  InstallationLocation = installLocation
                });
              }
            }
          }
        }
      }

      return list;
    }
  }
}
