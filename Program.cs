using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Configuration;

namespace NuGetList
{
    class Program
    {
        static void Main(string[] args)
        {

            

           // string strRootpath = @"\\WIN201613\1ES.SCOMMigration_071718\SystemCenter\Migration\SCOM\src\";
            string strRootpath = ConfigurationManager.AppSettings["Rootpath"];
            string[] allfileslist;
            List<string> allInternalItemList = new List<string>();
            List<string> allExternal_dllPath = new List<string>();
            List<string> allresultsExternal = new List<string>();
            List<string> allresultsInternal = new List<string>();
           // string strCDMBuild_dllPath = @"D:\CDM_SFE\Branches\OnPrem\1ES_Baseline\OnPrem\SCOM\private\product\external\";
            string strExternal_dllPath = ConfigurationManager.AppSettings["External_dllPath"];
            if (Directory.Exists(strExternal_dllPath))
            {
                allfileslist = Directory.GetFiles(strExternal_dllPath, "*.dll", SearchOption.AllDirectories);
                foreach (string line1 in allfileslist)
                {
                    allExternal_dllPath.Add(line1);
                }
            }
            List<string> allfileslist1 = Directory.GetFiles(strRootpath, "*.*", SearchOption.AllDirectories)
      .Where(file => new string[] { ".csproj" /*, ".mpproj"*/ }
      .Contains(Path.GetExtension(file)))
      .ToList();
           for(int i=0;i< allfileslist1.Count;i++)
          
            {
                string line1 = allfileslist1[i];
                string s1, strHit, strStatus1, strStatus2;
                string[] Internal_lines = System.IO.File.ReadAllLines(line1);
                foreach (string line2 in Internal_lines)
                {
                    if (line2.Contains("HintPath"))
                    {
                        s1 = line2.Replace("<HintPath>", string.Empty).Replace("</HintPath>", string.Empty);
                        strHit = line2.Split('\\')[line2.Split('\\').Length - 1].Replace("</HintPath>", string.Empty);
                        strStatus1 = allExternal_dllPath.Where(x => x.Contains(strHit)).FirstOrDefault();
                        if (strStatus1 != null)
                        {
                            //External
                            strStatus2 = allresultsExternal.Where(x => x.Contains(strHit)).FirstOrDefault();
                            if (strStatus2 == null)
                            {
                                allresultsExternal.Add(strHit + ",External");
                            }
                        }
                        else
                        {
                            strStatus2 = allresultsInternal.Where(x => x.Contains(strHit)).FirstOrDefault();
                            //Internal
                            if (strStatus2 == null)
                            {
                                 allresultsInternal.Add(strHit + ",Internal");
                            }
                        }
                    } 
                }
            }
            Assembly ass = Assembly.GetExecutingAssembly();
            string path = System.IO.Path.GetDirectoryName(ass.Location);
            File.WriteAllLines(path + "\\NuGetListforExternal.txt", allresultsExternal.ToList());
            File.WriteAllLines(path + "\\NuGetListforInternal.txt", allresultsInternal.ToList());
            Console.WriteLine(path + "\\NuGetListforExternal.txt", allresultsInternal.ToList());
            Console.WriteLine(path + "\\NuGetListforInternal.txt", allresultsInternal.ToList());
        }
    }
}
