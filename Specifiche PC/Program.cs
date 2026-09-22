using System;
using System.Linq;
using System.Management;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Specifiche PC ===\n");

        Console.WriteLine($"- OS: {GetOS()}");
        Console.WriteLine($"- CPU: {GetCPU()}");
        Console.WriteLine($"- RAM: {GetRam()}");
        Console.WriteLine($"- GPU: {GetGPU()}");
        Console.WriteLine($"- Storage: {GetStorage()}");
        Console.WriteLine($"- Scheda madre: {GetMotherboard()}");
        Console.WriteLine($"- Alimentatore: (non rilevabile via software)");

        Console.WriteLine("\nPremi un tasto per uscire...");
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
        using var searcher = new ManagementObjectSearcher("SELECT Name, MaxClockSpeed FROM Win32_Processor");
        foreach (ManagementObject obj in searcher.Get())
        {
            return $"{obj["Name"]} @ {Convert.ToDouble(obj["MaxClockSpeed"]) / 1000:0.00} GHz";
        }
        return "N/D";
    }

    static string GetRam()
    {
        using var searcher = new ManagementObjectSearcher("SELECT Capacity, Speed FROM Win32_PhysicalMemory");
        double totalBytes = 0;
        string speed = "";

        foreach (ManagementObject obj in searcher.Get())
        {
            totalBytes += Convert.ToDouble(obj["Capacity"]);
            speed = obj["Speed"]?.ToString() ?? "";
        }

        double totalGB = totalBytes / (1024 * 1024 * 1024);
        return $"{totalGB:0} GB DDR4 {speed} MHz";
    }

    static string GetGPU()
    {
        using var searcher = new ManagementObjectSearcher("SELECT Name, AdapterRAM FROM Win32_VideoController");
        var gpuList = new System.Collections.Generic.List<string>();
        foreach (ManagementObject obj in searcher.Get())
        {
            string name = obj["Name"]?.ToString() ?? "N/D";
            double vramGB = 0;
            if (obj["AdapterRAM"] != null && double.TryParse(obj["AdapterRAM"].ToString(), out double vram))
            {
                vramGB = vram / (1024 * 1024 * 1024);
            }
            gpuList.Add($"{name} ({vramGB:0.00} GB)");
        }
        return gpuList.Count > 0 ? string.Join("; ", gpuList) : "N/D";
    }

    static string GetStorage()
    {
        using var searcher = new ManagementObjectSearcher("SELECT Model, Size FROM Win32_DiskDrive");
        string result = "";

        foreach (ManagementObject obj in searcher.Get())
        {
            double sizeGB = Convert.ToDouble(obj["Size"]) / (1024 * 1024 * 1024);
            result += $"{obj["Model"]} {sizeGB:0}GB; ";
        }

        return result.TrimEnd(' ', ';');
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