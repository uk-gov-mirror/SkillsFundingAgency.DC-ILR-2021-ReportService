﻿using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Desktop.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Desktop
{
    public class ReportServiceDesktopTask: IDesktopTask
    {
        private readonly IReportServiceContextFactory<IDesktopContext> _reportServiceContextFactory;
        private readonly IEntryPoint _entryPoint;

        public ReportServiceDesktopTask(IReportServiceContextFactory<IDesktopContext> reportServiceContextFactory, IEntryPoint entryPoint)
        {
            _reportServiceContextFactory = reportServiceContextFactory;
            _entryPoint = entryPoint;
        }
        public async Task<IDesktopContext> ExecuteAsync(IDesktopContext desktopContext, CancellationToken cancellationToken)
        {
            var reportServiceContext = _reportServiceContextFactory.Build(desktopContext);

            await _entryPoint.Callback(reportServiceContext, cancellationToken);

            return desktopContext;
        }
    }
}
