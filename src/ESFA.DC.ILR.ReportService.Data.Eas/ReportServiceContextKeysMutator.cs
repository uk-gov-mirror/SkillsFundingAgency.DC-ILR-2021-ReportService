﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR2021.DataStore.EF.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ReportService.Data.Eas
{
    public class ReportServiceContextKeysMutator : IReportServiceContextKeysMutator
    {
        private readonly Func<IILR2021_DataStoreEntities> _ilrContext;

        public ReportServiceContextKeysMutator(Func<IILR2021_DataStoreEntities> ilrContext)
        {
            _ilrContext = ilrContext;
        }

        public async Task<IReportServiceContext> MutateAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData, CancellationToken cancellationToken)
        {
            await AddIlrReportingFilename(reportServiceContext, cancellationToken);
            await AddEasReportingFilename(reportServiceContext, cancellationToken);

            return reportServiceContext;
        }

        private async Task AddIlrReportingFilename(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            using (var ilrContext = _ilrContext())
            {
                var fileDetails = await ilrContext.FileDetails
                    .Where(fd => fd.UKPRN == reportServiceContext.Ukprn)
                    .OrderByDescending(d => d.SubmittedTime)
                    .FirstOrDefaultAsync(cancellationToken);

                reportServiceContext.IlrReportingFilename = Path.GetFileName(fileDetails?.Filename);
                reportServiceContext.LastIlrFileUpdate = fileDetails?.SubmittedTime != null
                    ? fileDetails?.SubmittedTime.Value.ToString(ReportServiceConstants.LastFileUpdateDateTimeFormat)
                    : null;
            }
        }

        private async Task AddEasReportingFilename(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            reportServiceContext.EasReportingFilename = string.IsNullOrWhiteSpace(reportServiceContext.OriginalFilename) ? reportServiceContext.Filename : reportServiceContext.OriginalFilename;
            reportServiceContext.LastEasFileUpdate = reportServiceContext.SubmissionDateTimeUtc.ToString(ReportServiceConstants.LastFileUpdateDateTimeFormat);
        }
    }
}
