using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OilTankVision.Weather
{
	public class WeatherBitProvider : IWeatherProvider
	{
		public async Task<int> GetWeatherTempInFahrenheit(string apiKey, string postalCode)
		{

			using (var client = new HttpClient())
			{

				var outString = await client.GetStringAsync($"https://api.weatherbit.io/v2.0/current?postal_code={postalCode}&key={apiKey}");
				var o = JObject.Parse(outString);

				var tempC = o["data"][0]["temp"].ToObject<decimal>();
				return (int)Math.Ceiling((tempC * 9) / 5 + 32);

			}

		}
	}

}
