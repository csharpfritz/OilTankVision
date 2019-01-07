using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OilTankVision.Data;
using OilTankVision.Gauges;
using OilTankVision.Weather;

namespace OilTankVision
{
	public static partial class Function1
	{

		[return: Table("OilTankReadings", Connection = "OilTankStorage")]
		[FunctionName("ProcessNewGaugePhoto")]
		[StorageAccount("OilTankStorage")]
		public static OilTankReading Run([BlobTrigger("gauges/{name}", Connection = "OilTankStorage")]Stream myBlob,
			string name, TraceWriter log)
		{

			IGaugeReader gaugeReader = new VerticalNumberedFloatGauge();
			IWeatherProvider weatherProvider = new WeatherBitProvider();

			var pictureDate = DateTime.ParseExact(name.Substring(0, name.Length - 4), "yyyyMMddHHmm", null);

			log.Info($"Processing photo snapped on: {pictureDate}");

			var postalCode = ConfigurationManager.AppSettings["Weather_PostalCode"];
			var apiKey = ConfigurationManager.AppSettings["Weather_Key"];

			var resultTask = SendToRecognizeTextApi(name);
			var weatherTask = weatherProvider.GetWeatherTempInFahrenheit(apiKey, postalCode);
			Task.WaitAll(new Task[] { resultTask, weatherTask });

			var result = resultTask.Result;
			log.Info($"Reporting with weather in {postalCode}: {weatherTask.Result}F");

			var outValue = gaugeReader.ProcessTextResult(log, new OilTankReading { ReadingDateTime = pictureDate }, result);
			outValue.TempF = weatherTask.Result;

			log.Info($"Results from analysis: {result.status}");

			log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

			return outValue;

		}

		private static async Task<AzureRecognizeText.Rootobject> SendToRecognizeTextApi(string name)
		{

			var retries = 0;

			using (var client = new HttpClient())
			{

				var serviceAddress = "https://eastus2.api.cognitive.microsoft.com/vision/v2.0/recognizeText?mode=Printed";
				var fullImageLocation = string.Format(ConfigurationManager.AppSettings["BlobLocation"], name);

				client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["OilTankVisionKey"]);

				var response = await client.PostAsJsonAsync(serviceAddress, new CognitiveServicesPayload { url = fullImageLocation });

				response.EnsureSuccessStatusCode();

				if (response.StatusCode == HttpStatusCode.Accepted)
				{

					var outLocation = response.Headers.GetValues("Operation-Location").First();

					var result = await GetResultsAsync(outLocation);
					return JsonConvert.DeserializeObject<AzureRecognizeText.Rootobject>(result);

				}

				return null;

				async Task<string> GetResultsAsync(string location)
				{

					var result = await client.GetAsync(location);
					if (result.StatusCode == HttpStatusCode.OK)
					{
						var stringResult = await result.Content.ReadAsStringAsync();
						if (JObject.Parse(stringResult)["status"].Value<string>() != "Succeeded")
						{
							retries++;
							await Task.Delay(500);
							return await GetResultsAsync(location);
						}

						return stringResult;

					}
					else if (retries < 20)
					{

						retries++;
						await Task.Delay(500);
						return await GetResultsAsync(location);

					}

					return "";

				}

			}

		}

		private class CognitiveServicesPayload
		{

			public string url;

		}

	}
}
