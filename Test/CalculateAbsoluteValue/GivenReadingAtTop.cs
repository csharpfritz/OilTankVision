using OilTankVision;
using OilTankVision.Gauges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Test.CalculateAbsoluteValue
{
	public class GivenReadingAtTop : BaseGaugeFixture
	{

		private readonly int[] BoundingBox = new int[] { 534, 243, 612, 269, 617, 322, 532, 323 };

		[Fact]
		public void ShouldReturnFiveAfterValue()
		{

			var outValue = new VerticalNumberedFloatGauge().ProcessTextResult(_TraceWriter, new OilTankVision.Data.OilTankReading(), CreateTextResult(BoundingBox, TextDetected));

			Assert.Equal(155D, outValue.Value);


		}

	}

}
