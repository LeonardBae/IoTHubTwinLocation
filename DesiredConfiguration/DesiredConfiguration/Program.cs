using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Devices;
using System.Threading;
using Newtonsoft.Json;

namespace DesiredConfiguration
{
    class Program
    {
        static RegistryManager registryManager;
        static string connectionString = "===Insert Device Connection String===";
        static void Main(string[] args)
        {
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            SetDesiredConfigurationAndQuery();
            Console.WriteLine("Press any key to quit.");
            Console.ReadLine();
        }

        static private async Task SetDesiredConfigurationAndQuery()
        {
            var twin = await registryManager.GetTwinAsync("===Insert Device ID===");
            var patch = new
            {
                properties = new
                {
                    desired = new
                    {
                        windows = new
                        { 
                        Location = new
                        {
                            latitude = 37.324323,
                            longitude = 155.454354
                        }
                    } }
                }
            };

            await registryManager.UpdateTwinAsync(twin.DeviceId, JsonConvert.SerializeObject(patch), twin.ETag);
            Console.WriteLine("Updated desired configuration");
        }
    }
}
