using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;

namespace LocationTwin
{
    class Program
    {
        static string DeviceConnectionString = "===Insert Your DeviceConnectionString===";
        static DeviceClient Client = null;
        static TwinCollection reportedProperties = new TwinCollection();
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Connecting to hub");
                Client = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Mqtt);
                //Device가 offline 상태에서 위치 정보 업데이트하였을 시 처음 프로그램 실행할 때 확인
                initTwin();
                Console.WriteLine("Wait for desired telemetry...");
                //Device가 online 상태에서 위치 정보 업데이트하였을 시 프로그램 실행되어 있는 상태에서 바로 업데이트
                Client.SetDesiredPropertyUpdateCallback(OnDesiredPropertyChanged, null).Wait();
                Console.ReadKey();
            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    Console.WriteLine("Error in sample: {0}", exception);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in sample: {0}", ex.Message);
            }
        }
        private static async Task initTwin()
        {
            try
            {
                Twin twin = await Client.GetTwinAsync().ConfigureAwait(false);
                TwinCollection desiredProperties = new TwinCollection();
                TwinCollection reportedPropertiesorg = new TwinCollection();

                desiredProperties = twin.Properties.Desired;
                reportedPropertiesorg = twin.Properties.Reported;
                if ((desiredProperties["windows"]["Location"]["latitude"] != reportedPropertiesorg["Device"]["Location"]["Latitude"]) 
                    && (desiredProperties["windows"]["Location"]["longitude"] != reportedPropertiesorg["Device"]["Location"]["Longitude"]))
                {
                    Console.WriteLine("Desired property change:");
                    Console.WriteLine(JsonConvert.SerializeObject(desiredProperties));
                    reportedProperties["Device"] = new
                    {
                        DeviceState = "Normal",
                        Location = new
                        {
                            Status = "true",
                            Latitude = desiredProperties["windows"]["Location"]["latitude"],
                            Longitude = desiredProperties["windows"]["Location"]["longitude"]
                        }
                    };
                    await Client.UpdateReportedPropertiesAsync(reportedProperties);
                }
            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    Console.WriteLine("Error in sample: {0}", exception);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in sample: {0}", ex.Message);
            }
        }
        private static async Task OnDesiredPropertyChanged(TwinCollection desiredProperties, object userContext)
        {
            try
            {
                Console.WriteLine("Desired property change:");
                Console.WriteLine(JsonConvert.SerializeObject(desiredProperties));

                reportedProperties["Device"] = new
                {
                    DeviceState = "Normal",
                    Location = new
                    {
                        Status = "true",
                        Latitude = desiredProperties["windows"]["Location"]["latitude"],
                        Longitude = desiredProperties["windows"]["Location"]["longitude"]
                    }
                };
                await Client.UpdateReportedPropertiesAsync(reportedProperties);
            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    Console.WriteLine("Error in sample: {0}", exception);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in sample: {0}", ex.Message);
            }
        }
    }
}
