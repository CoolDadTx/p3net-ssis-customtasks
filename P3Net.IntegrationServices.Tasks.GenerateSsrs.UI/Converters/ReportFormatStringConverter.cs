/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace P3Net.IntegrationServices.Tasks.GenerateSsrs.UI.Converters
{
    internal class ReportFormatStringConverter : StringConverter
    {
        public override StandardValuesCollection GetStandardValues ( ITypeDescriptorContext context )
        {
            var values = new[]
            {
                "PDF",
                "HTML5",
                "PPTX",
                "ATOM",
                "HTML4.0",
                "MHTML",
                "IMAGE",
                "EXCEL",
                "WORD",
                "CSV",
                "XML"
            };

            return new StandardValuesCollection(values.OrderBy(x => x).ToArray());
        }

        public override bool GetStandardValuesExclusive ( ITypeDescriptorContext context )
        {
            return false;
        }

        public override bool GetStandardValuesSupported ( ITypeDescriptorContext context )
        {
            return true;
        }
    }
}
