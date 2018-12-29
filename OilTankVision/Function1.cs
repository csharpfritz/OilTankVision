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

			var result = SendToRecognizeTextApi(name).GetAwaiter().GetResult();
			var outValue = CalculateAbsoluteValue(log, pictureDate, result, int.Parse(ConfigurationManager.AppSettings["Gauge_Top"]), int.Parse(ConfigurationManager.AppSettings["Gauge_Height"]), int.Parse(ConfigurationManager.AppSettings["Gauge_DigitHeight"]));

			log.Info($"Results from analysis: {result.status}");

			log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

			return outValue;

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

					// Wait 5 seconds for the processing
					await Task.Delay(5000);

					var result = await client.GetStringAsync(outLocation);
					return JsonConvert.DeserializeObject<TextResult.Rootobject>(result);

				}

				return null;

			}



		}

	}
}
