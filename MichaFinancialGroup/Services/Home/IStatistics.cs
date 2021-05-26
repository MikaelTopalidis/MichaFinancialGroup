using MichaFinancialGroup.Models;
using SharedLibrary.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.Services
{
    public interface IStatistics
    {
        public IQueryable<Dispositions> GetAll();
    }
}
