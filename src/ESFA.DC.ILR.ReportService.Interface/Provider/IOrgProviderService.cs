﻿using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Interface.Provider
{
    public interface IOrgProviderService
    {
        Task<string> GetProviderName(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);

        Task<string> GetVersionAsync(CancellationToken cancellationToken);

        Task<decimal?> GetCofRemoval(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}
