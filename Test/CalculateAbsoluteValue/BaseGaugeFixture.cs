using Microsoft.Azure.WebJobs.Host;
using Moq;
using System;

namespace Test.CalculateAbsoluteValue
{
	public abstract class BaseGaugeFixture {

		protected TraceWriter _TraceWriter = new Mock<TraceWriter>(null).Object;
		protected readonly string TextDetected = "150";
		protected readonly DateTime PictureDate = new DateTime(2018, 12, 28, 19, 21, 0);


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
