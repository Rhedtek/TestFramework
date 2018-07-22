using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutomationTestCommon;
using AutomationTestPowerCLI;

namespace ReImageVM
{
    class Program
    {
        static void Main(string[] args)
        {
            string vmName = args[0];

            Console.WriteLine("ReImaging VM " + vmName);
            Console.WriteLine("Hit ENTER to roll back VM to the Service snapshot.");
            Console.ReadLine();

            NetworkCredential mNetworkCredentials = new NetworkCredential("user", "password");

            Console.WriteLine("Rolling VM back to the Service snapshot...");
            // First step: roll back to service
            PowerCLI.Instance(mNetworkCredentials).RollbackVM(vmName, "Service", null);
            Console.WriteLine("Rolled back VM to snapshot Service.");
            Console.WriteLine("Hit ENTER to remove the Service snapshot.");
            Console.ReadLine();

            Console.WriteLine("Removing snapshot Service...");
            PowerCLI.Instance(mNetworkCredentials).RemoveSnapshot(vmName, "Service", null);
            Console.WriteLine("Removed snapshot Service from VM " + vmName);
            Console.WriteLine("Hit ENTER to create a new Service snapshot.");
            Console.ReadLine();

            Console.WriteLine("Creating new snapshot Service...");
            PowerCLI.Instance(mNetworkCredentials).CreateSnapshot(vmName, "Service", null);
            Console.WriteLine("Removed snapshot Service from VM " + vmName);
            Console.WriteLine("All done.");
        }
    }
}
