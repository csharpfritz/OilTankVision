using OilTankVision.AzureRecognizeText;
using System;

namespace OilTankVision.Data
{
	public class OilTankReading {

		public string RowKey { get
			{
				return ReadingDateTime.ToString("yyyyMMddHHmm");
			}
		}

		public string PartitionKey {
			get { return ReadingDateTime.ToString("yyyyMM"); }
		}

		public DateTime ReadingDateTime { get; set; }

		public double Value { get; set; }

		public int TempF { get; set; }

	}



}
