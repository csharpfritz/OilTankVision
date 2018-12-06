using System;

namespace OilTankVision
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

		public int Value { get; set; }

	}
}
