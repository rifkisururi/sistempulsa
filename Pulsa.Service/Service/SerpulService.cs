using Pulsa.Data;
using Pulsa.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsa.Service.Service
{
    public class SerpulService : ISerpulService
    {
        private PulsaDataContext context;
        public SerpulService() { }
        public int getSaldo() {
            return 0;
        }
    }
}
