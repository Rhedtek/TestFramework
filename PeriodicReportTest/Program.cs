using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using AutomationTestServer.DataObjects;

namespace PeriodicReportTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TestReport();
        }

        static void TestReport()
        {
            PeriodicReports reports = new PeriodicReports();
            reports.Start(null);

            while (true)
            {
                Thread.Sleep(10000);
            }
        }

        static void Test()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = "mstsc.exe";
                process.StartInfo.Arguments = "/v:user005";
                Console.WriteLine("Executing: " + process.StartInfo.FileName + " " + process.StartInfo.Arguments);
                process.Start();
                Console.WriteLine("ProcessID: " + process.Id);
                process.WaitForExit();

                ManagementObjectSearcher mos = new ManagementObjectSearcher(String.Format("Select * From Win32_Process Where ParentProcessID={0}", process.Id));

                foreach (ManagementObject mo in mos.Get())
                {
                    var child = Process.GetProcessById(Convert.ToInt32(mo["ProcessID"]));
                    Console.WriteLine("Child: " + child.Id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MSTSC exception: " + ex);
            }
        }
    }
}
