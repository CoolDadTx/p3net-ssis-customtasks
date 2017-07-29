using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P3Net.IntegrationServices.Tasks.GenerateSsrs
{
    public sealed class ReportArgument
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public ReportArgumentValueType ValueType { get; set; }
    }
}
