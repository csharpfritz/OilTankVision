using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OilTankVision.Weather
{
	public interface IWeatherProvider
	{

		Task<int> GetWeatherTempInFahrenheit(string apiKey, string postalCode);

	}

}
