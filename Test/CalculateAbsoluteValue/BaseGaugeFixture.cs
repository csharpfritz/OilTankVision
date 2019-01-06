using Microsoft.Azure.WebJobs.Host;
using Moq;
using System;

namespace Test.CalculateAbsoluteValue
{
	public abstract class BaseGaugeFixture
	{

		protected readonly TraceWriter _TraceWriter;
		protected readonly string TextDetected = "150";
		protected readonly DateTime PictureDate = new DateTime(2018, 12, 28, 19, 21, 0);

		protected BaseGaugeFixture()
		{

			var newMock = new Mock<TraceWriter>(null);
			//newMock.Setup(w => w.Info(It.IsAny<string>(), null))
			//.Callback<string, string>((x, y) => Console.WriteLine(x));
			_TraceWriter = newMock.Object;

		}

		protected static OilTankVision.AzureRecognizeText.Rootobject CreateTextResult(int[] boundingBox, string textDetected)
		{

			return new OilTankVision.AzureRecognizeText.Rootobject
			{
				status = "Succeeded",
				recognitionResult = new OilTankVision.AzureRecognizeText.Recognitionresult
				{
					lines = new OilTankVision.AzureRecognizeText.Line[] {

						ScullyLine,

						new OilTankVision.AzureRecognizeText.Line {
							boundingBox = boundingBox,
							text = textDetected,
							words = new OilTankVision.AzureRecognizeText.Word[] {
								new OilTankVision.AzureRecognizeText.Word {
								boundingBox = boundingBox,
								text = textDetected
								}
							}
						}
					}
				}

			};

		}

		/// <summary>
		/// A default position for the reference "SCULLY" word at the top of the Gauge
		/// </summary>
		private static readonly OilTankVision.AzureRecognizeText.Line ScullyLine = new OilTankVision.AzureRecognizeText.Line
		{
			boundingBox = new int[] { 502, 149, 629, 145, 630, 177, 504, 182 },
			text = "SCULLY",
			words = new OilTankVision.AzureRecognizeText.Word[] {
				new OilTankVision.AzureRecognizeText.Word {
					boundingBox= new int[] { 508, 152, 625, 146, 626, 179, 508, 182 },
					text = "SCULLY",
					Confidence = ""
				}
			}
		};

	}

}
