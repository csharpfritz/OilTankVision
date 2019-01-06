using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OilTankVision;
using OilTankVision.AzureRecognizeText;
using OilTankVision.Data;
using OilTankVision.Gauges;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.CalculateAbsoluteValue;
using Xunit;

namespace Test
{

	public class VerticalNumberedFloatGaugeFixture : BaseGaugeFixture
	{

		[Fact]
		public void ShouldReturn150()
		{

			// Arrange
			var azureReadings = JsonConvert.DeserializeObject<Rootobject>(File.ReadAllText(@"ExampleData.json"));

			var analyzer = new VerticalNumberedFloatGuage();

			// Act
			var sut = analyzer.ProcessTextResult(_TraceWriter, new OilTankReading(), azureReadings);

			// Assert
			Assert.Equal(150, sut.Value);

		}

	}

}
