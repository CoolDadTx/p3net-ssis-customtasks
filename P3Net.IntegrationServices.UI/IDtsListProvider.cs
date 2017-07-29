using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3Net.IntegrationServices.UI
{
    public interface IDtsListProvider
    {
        List<string> ValueList { get; set; }
    }
}
