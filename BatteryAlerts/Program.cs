using LoggerLite.EventLog;
using System.Management;

var logger = new EventLogLogger("Battery-Logger", "Monitor");

async Task StartMonitoring()
{

    var query = new ObjectQuery("Select * FROM Win32_Battery");
    var searcher = new ManagementObjectSearcher(query);
    var collection = searcher.Get();

    var battery = collection.OfType<ManagementObject>().FirstOrDefault();
    var percentage = (UInt16) battery.GetPropertyValue("EstimatedChargeRemaining");
    var status = (UInt16)battery.GetPropertyValue("BatteryStatus");

    Console.WriteLine($"Status {status}, Level {percentage}%");
    logger.LogInfo($"Status {(status == 2 ? "Charging" : "On Battery")}, Level {percentage}%");

    if(status == 1)
    {
        logger.LogError($"Status {(status == 2 ? "Charging" : "On Battery")}, Level {percentage}%");
    }



    await Task.Delay(2 * 60 * 1000);
    await StartMonitoring();
}

await StartMonitoring();