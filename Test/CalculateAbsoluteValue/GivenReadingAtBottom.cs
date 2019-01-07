using OilTankVision;
using OilTankVision.Gauges;
using Xunit;

namespace Test.CalculateAbsoluteValue
{
	public class GivenReadingAtBottom : BaseGaugeFixture
	{

		private readonly int[] BoundingBox = new int[] { 534, 293, 612, 269, 617, 343, 532, 323 };

		[Fact]
		public void ShouldReturnFiveBeforeValue()
		{

			var outValue = new VerticalNumberedFloatGauge().ProcessTextResult(_TraceWriter, new OilTankVision.Data.OilTankReading(), CreateTextResult(BoundingBox, TextDetected));

			Assert.Equal(145D, outValue.Value);


		}

	}

}
