﻿using Microsoft.Azure.WebJobs.Host;
using OilTankVision.AzureRecognizeText;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OilTankVision
{
	public interface IGaugeReader
    {
        OilTankReading ProcessTextResult(TraceWriter log, OilTankReading oilTankReading, Rootobject RawData);
    }
}
