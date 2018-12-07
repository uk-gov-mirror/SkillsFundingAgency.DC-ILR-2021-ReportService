﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.ReportService.Interface.Context;
using ESFA.DC.ILR1819.ReportService.Model.ILR;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IAllbProviderService
    {
        Task<ALBGlobal> GetAllbData(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);

        Task<List<ALBLearningDeliveryValues>> GetALBDataFromDataStore(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}
