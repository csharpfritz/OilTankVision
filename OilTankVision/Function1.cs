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

namespace OilTankVision
{
	public static partial class Function1
	{

		[return: Table("OilTankReadings", Connection = "OilTankStorage")]
		[FunctionName("ProcessNewGaugePhoto")][StorageAccount("OilTankStorage")]
		public static OilTankReading Run([BlobTrigger("gauges/{name}", Connection = "")]Stream myBlob, string name, TraceWriter log)
		{

			// Full location at:  https://jeffcatimages.blob.core.windows.net/gauges/{name}

			var pictureDate = DateTime.ParseExact(name.Substring(0, name.Length - 4), "yyyyMMddHHmm", null);

			log.Info($"Processing photo snapped on: {pictureDate}");

			var postalCode = ConfigurationManager.AppSettings["Weather_PostalCode"];
			var apiKey = ConfigurationManager.AppSettings["Weather_Key"];

			var resultTask = SendToRecognizeTextApi(name);
			var weatherTask = GetWeatherTempInFahrenheit(apiKey, postalCode);
			Task.WaitAll(new Task[] { resultTask, weatherTask });

			var result = resultTask.Result;
			log.Info($"Reporting with weather in {postalCode}: {weatherTask.Result}F");

			var outValue = CalculateAbsoluteValue(log, pictureDate, result, int.Parse(ConfigurationManager.AppSettings["Gauge_Top"]), int.Parse(ConfigurationManager.AppSettings["Gauge_Height"]), int.Parse(ConfigurationManager.AppSettings["Gauge_DigitHeight"]));
			outValue.TempF = weatherTask.Result;

			log.Info($"Results from analysis: {result.status}");

			log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

			return outValue;

		}

		private static async Task<int> GetWeatherTempInFahrenheit(string apiKey, string postalCode)
		{

			using (var client = new HttpClient()) {

				var outString = await client.GetStringAsync($"https://api.weatherbit.io/v2.0/current?postal_code={postalCode}&key={apiKey}");
				var o = JObject.Parse(outString);

				var tempC = o["data"][0]["temp"].ToObject<decimal>();
				return (int)Math.Ceiling((tempC * 9) / 5 + 32);

			}

		}

		public static OilTankReading CalculateAbsoluteValue(TraceWriter log, DateTime pictureDate, TextResult.Rootobject result, int topOfGauge, int heightOfGauge, int heightDigit)
		{
			var outValue = new OilTankReading
			{
				ReadingDateTime = pictureDate
			};

			foreach (var line in result.recognitionResult.lines.Where(l => l.words.Any(w => w.Confidence != "Low")))
			{

				if (double.TryParse(line.text, out double gaugeValue))
				{

					// Identify position
					var topOfDigit = line.boundingBox[1];
					var relativeTopOfDigit = topOfDigit - topOfGauge;
					var relativeBottom = heightOfGauge - heightDigit;
					var pctLocation = relativeTopOfDigit / (double)relativeBottom;
					var modifier = (0.5 - pctLocation) * 10;

					log.Info($"Found gauge value: {gaugeValue} at position {pctLocation:0%}");

					outValue.Value = gaugeValue + modifier;

				}


			}

			return outValue;
		}

		private static async Task<TextResult.Rootobject> SendToRecognizeTextApi(string name)
		{

			var retries = 0;

			using (var client = new HttpClient())
			{

				var serviceAddress = "https://eastus2.api.cognitive.microsoft.com/vision/v2.0/recognizeText?mode=Printed";
				var fullImageLocation = $"https://jeffcatimages.blob.core.windows.net/gauges/{name}";

				client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["OilTankVisionKey"]);

				var response = await client.PostAsJsonAsync(serviceAddress, new CognitiveServicesPayload {url=fullImageLocation});

				response.EnsureSuccessStatusCode();

				if (response.StatusCode == HttpStatusCode.Accepted)
				{

					var outLocation = response.Headers.GetValues("Operation-Location").First();

					var result = await GetResultsAsync(outLocation);
					return JsonConvert.DeserializeObject<TextResult.Rootobject>(result);

				}

				return null;

				async Task<string> GetResultsAsync(string location) {

					var result = await client.GetAsync(location);
					if (result.StatusCode == HttpStatusCode.OK) {
						var stringResult = await result.Content.ReadAsStringAsync();
						if (JObject.Parse(stringResult)["status"].Value<string>() != "Succeeded") {
							retries++;
							await Task.Delay(500);
							return await GetResultsAsync(location);
						}

						return stringResult;

					} else if (retries < 20) {

						retries++;
						await Task.Delay(500);
						return await GetResultsAsync(location);

					}

					return "";

				}

			}



		}

	}
}
