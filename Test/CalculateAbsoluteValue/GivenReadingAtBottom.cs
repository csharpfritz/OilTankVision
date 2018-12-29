using OilTankVision;
using Xunit;

namespace Test.CalculateAbsoluteValue
{
	public class GivenReadingAtBottom : BaseGaugeFixture
	{

		private readonly int[] BoundingBox = new int[] { 534, 293, 612, 269, 617, 322, 532, 323 };

		[Fact]
		public void ShouldReturnFiveBeforeValue()
		{

			var outValue = Function1.CalculateAbsoluteValue(_TraceWriter, PictureDate, CreateTextResult(BoundingBox, TextDetected), 243, 100, 50);

			Assert.Equal(145D, outValue.Value);


		}

	}

}
