using OilTankVision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Test.CalculateAbsoluteValue
{

	public class GivenReadingAtCenter : BaseGaugeFixture
	{

		private readonly int[] BoundingBox = new int[] { 534, 272, 612, 269, 617, 322, 532, 323 };

		[Fact]
		public void ShouldReturnExactValue() {

			var outValue = Function1.CalculateAbsoluteValue(_TraceWriter, PictureDate, CreateTextResult(BoundingBox, TextDetected), 243, 100, 50);

			Assert.Equal(150D, outValue.Value);


		}

	}

}
