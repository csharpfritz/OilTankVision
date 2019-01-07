using OilTankVision;
using OilTankVision.Gauges;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Test.CalculateAbsoluteValue
{

	public class GivenReadingAtCenter : BaseGaugeFixture
	{

		private readonly int[] BoundingBox = new int[] { 534, 268, 612, 269, 617, 318, 532, 323 };

		[Fact]
		public void ShouldReturnExactValue() {

			var outValue = new VerticalNumberedFloatGauge().ProcessTextResult(_TraceWriter, new OilTankVision.Data.OilTankReading(), CreateTextResult(BoundingBox, TextDetected));

			Assert.Equal(150D, outValue.Value);


		}

	}

}
