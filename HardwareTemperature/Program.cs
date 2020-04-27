/*
  Supported Hardware
  -CPU core sensors
   Intel Core 2, Core i3/i5/i7, Atom, Sandy Bridge, Ivy Bridge, Haswell, Broadwell, Silvermont, Skylake, Kaby Lake, Airmont
   AMD K8 (0Fh family), K10 (10h, 11h family), Llano (12h family), Fusion (14h family), Bulldozer (15h family), Jaguar (16h family)

  -GPU sensors
   Nvidia
   AMD (ATI)
*/

using OpenHardwareMonitor.Hardware;
using System;
using System.Timers;
using System.IO;

namespace HardwareTemperature
{
    class Program
    {
        static String line = null;

        static void Main(string[] args)
        {
            // add CPU and GPU as hardware
            // note that, CPU temperature data requires 'highestAvailable' permission.
            Computer computer = new Computer() { CPUEnabled = true, GPUEnabled = true };
            computer.Open();

            String filePath = "tmp.txt";
            filePath = DateTime.Now.ToString("yyyMMddHHmmss") + ".csv";

            Timer timer = new Timer() { Enabled = true, Interval = 1000 };

            timer.Elapsed += delegate (object sender, ElapsedEventArgs e)
            {
                Console.Clear();
                Console.WriteLine("{0}\n", DateTime.Now);

                foreach (IHardware hardware in computer.Hardware)
                {
                    hardware.Update();

                    Console.WriteLine("{0}: {1}", hardware.HardwareType, hardware.Name);
                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        // Celsius is default unit
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            Console.WriteLine("{0}: {1}°C", sensor.Name, sensor.Value);
                            Program.line += sensor.Value + ",";
                            // Console.WriteLine("{0}: {1}°F", sensor.Name, sensor.Value*1.8+32);
                        }

                    }
                    
                    Console.WriteLine();
                    Program.line += "\n";
                    
                    if (Program.line.Length > 10240)
                    {
                        using (StreamWriter sw = new StreamWriter(filePath, true))
                        {
                            sw.WriteLine(Program.line);
                        }
                        
                        Program.line = null;
                    }
                }
                Console.WriteLine("Press Enter to exit");
            };

            // press enter to exit
            Console.ReadLine();
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                sw.WriteLine(Program.line);
            }
        }
    }
}
