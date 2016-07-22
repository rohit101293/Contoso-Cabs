using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCabs.ServiceModels
{
    public class CabEstimateEx
    {
        public CabEstimateEx(CabEstimate estimate)
        {
            double high;
            double.TryParse(estimate.CurrentEstimate.HighRange, out high);
            double low;
            double.TryParse(estimate.CurrentEstimate.LowRange, out low);
            this.AverageEstimate = (high + low) / 2;
            this.estimate = estimate;
        }
        public double AverageEstimate;

        public CabEstimate estimate;
    }
}
