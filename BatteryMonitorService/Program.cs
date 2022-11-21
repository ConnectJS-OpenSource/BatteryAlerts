using LoggerLite.EventLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace BatteryMonitorService
{
    internal class Program
    {
        static EventLogLogger logger = new EventLogLogger("Battery-Logger", "Monitor");
        static int Interval = 2 * 60 * 1000;
        static void Main(string[] args)
        {
            int.TryParse(args[0], out Interval);
            StartMonitoring().Wait();
        }

        

        async static Task StartMonitoring()
        {

            var query = new ObjectQuery("Select * FROM Win32_Battery");
            var searcher = new ManagementObjectSearcher(query);
            var collection = searcher.Get();

            var battery = collection.OfType<ManagementObject>().FirstOrDefault();
            var percentage = (UInt16)battery.GetPropertyValue("EstimatedChargeRemaining");
            var status = (UInt16)battery.GetPropertyValue("BatteryStatus");

            Console.WriteLine($"Status {status}, Level {percentage}%");
            logger.LogInfo($"Status {(status == 2 ? "Charging" : "On Battery")}, Level {percentage}%");

            if (status == 1)
            {
                logger.LogError($"Status {(status == 2 ? "Charging" : "On Battery")}, Level {percentage}%");
            }



            await Task.Delay(Interval);
            await StartMonitoring();
        }

    }
}
