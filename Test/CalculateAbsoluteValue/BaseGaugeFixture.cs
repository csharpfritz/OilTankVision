using Microsoft.Azure.WebJobs.Host;
using Moq;
using System;

namespace Test.CalculateAbsoluteValue
{
	public abstract class BaseGaugeFixture {

		protected readonly TraceWriter _TraceWriter;
		protected readonly string TextDetected = "150";
		protected readonly DateTime PictureDate = new DateTime(2018, 12, 28, 19, 21, 0);

		protected BaseGaugeFixture() {

			var newMock = new Mock<TraceWriter>(null);
			//newMock.Setup(w => w.Info(It.IsAny<string>(), null))
			//.Callback<string, string>((x, y) => Console.WriteLine(x));
			_TraceWriter = newMock.Object;

		}

		protected static OilTankVision.TextResult.Rootobject CreateTextResult(int[] boundingBox, string textDetected)
		{

			return new OilTankVision.TextResult.Rootobject
			{
				status = "Succeeded",
				recognitionResult = new OilTankVision.TextResult.Recognitionresult
				{
					lines = new OilTankVision.TextResult.Line[] {
						new OilTankVision.TextResult.Line {
							boundingBox = boundingBox,
							text = textDetected,
							words = new OilTankVision.TextResult.Word[] {
								new OilTankVision.TextResult.Word {
								boundingBox = boundingBox,
								text = textDetected
								}
							}
						}
					}
				}

			};

		}


	}

}
