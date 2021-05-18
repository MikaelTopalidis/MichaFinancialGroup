using MichaFinancialGroup.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.Services
{
    public interface IStatistics
    {
        public IEnumerable<Dispositions> GetAll();
    }
}
