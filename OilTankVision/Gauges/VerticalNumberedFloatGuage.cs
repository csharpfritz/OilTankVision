using Microsoft.Azure.WebJobs.Host;
using OilTankVision.AzureRecognizeText;
using OilTankVision.Data;
using System;
using System.Linq;

namespace OilTankVision.Gauges
{
	public class VerticalNumberedFloatGuage : IGaugeReader
	{
		private TraceWriter _log;
		private Rootobject _rawData;

		public OilTankReading ProcessTextResult(TraceWriter log, OilTankReading oilTankReading, Rootobject rawData)
		{
			_log = log;
			_rawData = rawData;

			var outValue = new OilTankReading
			{
				ReadingDateTime = oilTankReading.ReadingDateTime,
			};

			var line = _rawData.recognitionResult.lines.FirstOrDefault(l => l.text == "SCULLY" && l.words.Any(w => w.Confidence != "Low")); 
			if (line != null)
			{
				var (topOfGauge, heightOfGauge) = CalculateHeightOfGaugeWindow(line);
				outValue.Value = IdentifyValue(topOfGauge, heightOfGauge);
				return outValue;
			}


			throw new ApplicationException("Reference object not found");

		}

		private (int topOfGauge, int heightOfGauge) CalculateHeightOfGaugeWindow(Line referenceLine)
		{

			(int topOfGauge, int heightOfGauge) outValues = (0, 0);

			int refHeightOfScully = 28;
			int refDistanceToWindow = 66;
			int refHeightGauge = 100;

			int topOfScully = referenceLine.boundingBox[1];
			int bottomOfScully = referenceLine.boundingBox[5];
			int heightOfScully = bottomOfScully - topOfScully;
			double relativePct = refHeightOfScully / (double)heightOfScully;

			outValues.heightOfGauge = (int)(relativePct * refHeightGauge);
			outValues.topOfGauge = (int)(relativePct * refDistanceToWindow) + bottomOfScully;

			return outValues;
		}

		private double IdentifyValue(int topOfGauge, int heightOfGauge)
		{

			var outValue = 0.0D;

			foreach (var line in _rawData.recognitionResult.lines.Where(l => l.words.Any(w => w.Confidence != "Low")))
			{

				if (double.TryParse(line.text, out double gaugeValue))
				{
					// Identify position
					var topOfDigit = line.boundingBox[1]; 
					var bottomOfDigit = line.boundingBox[5];
					var heightDigit = bottomOfDigit - topOfDigit;
					var relativeTopOfDigit = topOfDigit - topOfGauge;
					var relativeBottom = heightOfGauge - heightDigit;
					var pctLocation = relativeTopOfDigit / (double)relativeBottom;
					var modifier = (0.5 - pctLocation) * 10;

					_log.Info($"Found gauge value: {gaugeValue} at position {pctLocation:0%}");

					outValue = gaugeValue + modifier;
				}

			}

			return outValue;

		}
	}
}
