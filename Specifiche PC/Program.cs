using System;
using System.Linq;
using System.Management;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== PC Hardware Specifications ===\n");

        Console.WriteLine($"- Operating System: {GetOS()}");
        Console.WriteLine($"- Processor (CPU): {GetCPU()}");
        Console.WriteLine($"- RAM Memory: {GetRam()}");
        Console.WriteLine($"- Graphics Card: {GetGPU()}");
        Console.WriteLine($"- Disk Space: {GetStorageFreeSpace()}");
        Console.WriteLine($"- Motherboard: {GetMotherboard()}");

        Console.WriteLine("\n-------------------------------------------");
        Console.WriteLine("\nPress a key to exit...");
        Console.ReadKey();
    }

    static string GetOS()
    {
        using var searcher = new ManagementObjectSearcher("SELECT Caption, Version, OSArchitecture FROM Win32_OperatingSystem");
        foreach (ManagementObject obj in searcher.Get())
        {
            return $"{obj["Caption"]} {obj["Version"]} ({obj["OSArchitecture"]})";
        }
        return "N/D";
    }

    static string GetCPU()
    {
        using var searcher = new ManagementObjectSearcher("SELECT Name, MaxClockSpeed, NumberOfCores, NumberOfLogicalProcessors FROM Win32_Processor");
        foreach (ManagementObject obj in searcher.Get())
        {
            return $"{obj["Name"]} ({obj["NumberOfCores"]} Core / {obj["NumberOfLogicalProcessors"]} Thread) @ {Convert.ToDouble(obj["MaxClockSpeed"]) / 1000:0.00} GHz";
        }
        return "N/D";
    }

    static string GetRam()
    {
        using var searcher = new ManagementObjectSearcher("SELECT Capacity, Speed, SMBIOSMemoryType FROM Win32_PhysicalMemory");
        double totalBytes = 0;
        string speed = "";
        int stickCount = 0;
        string ramType = "DDR";

        foreach (ManagementObject obj in searcher.Get())
        {
            totalBytes += Convert.ToDouble(obj["Capacity"]);
            speed = obj["Speed"]?.ToString() ?? "";
            stickCount++;

            if (obj["SMBIOSMemoryType"] != null)
            {
                int typeId = Convert.ToInt32(obj["SMBIOSMemoryType"]);
                if (typeId == 26) ramType = "DDR4";
                else if (typeId == 34) ramType = "DDR5";
                else if (typeId == 24) ramType = "DDR3";
            }
        }

        double totalGB = totalBytes / (1024 * 1024 * 1024);
        return $"{totalGB:0} GB {ramType} @ {speed} MHz ({stickCount} banchi)";
    }

    static string GetGPU()
    {
        using var searcher = new ManagementObjectSearcher("SELECT Name, AdapterRAM FROM Win32_VideoController");
        var gpuList = new List<string>();

        foreach (ManagementObject obj in searcher.Get())
        {
            string name = obj["Name"]?.ToString() ?? "N/D";

            long rawRam = 0;
            if (obj["AdapterRAM"] != null)
            {
                long.TryParse(obj["AdapterRAM"].ToString(), out rawRam);
            }

            double vramGB = (double)rawRam / (1024 * 1024 * 1024);

            if (vramGB <= 0)
            {
                gpuList.Add($"{name} (VRAM > 4GB)");
            }
            else
            {
                gpuList.Add($"{name} ({vramGB:0.00} GB VRAM)");
            }
        }
        return gpuList.Count > 0 ? string.Join(" | ", gpuList) : "N/D";
    }

    static string GetStorageFreeSpace()
    {
        string result = "";
        foreach (DriveInfo drive in DriveInfo.GetDrives())
        {
            if (drive.IsReady && drive.DriveType == DriveType.Fixed)
            {
                double freeGB = drive.AvailableFreeSpace / (1024 * 1024 * 1024);
                double totalGB = drive.TotalSize / (1024 * 1024 * 1024);
                result += $"[{drive.Name} {freeGB:0} GB liberi su {totalGB:0} GB] ";
            }
        }
        return string.IsNullOrWhiteSpace(result) ? "N/D" : result;
    }

    static string GetMotherboard()
    {
        using var searcher = new ManagementObjectSearcher("SELECT Manufacturer, Product FROM Win32_BaseBoard");
        foreach (ManagementObject obj in searcher.Get())
        {
            return $"{obj["Manufacturer"]} {obj["Product"]}";
        }
        return "N/D";
    }
}