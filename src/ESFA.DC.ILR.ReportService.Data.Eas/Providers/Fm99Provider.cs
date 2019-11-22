﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Models.Fm99;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ReportService.Data.Eas.Providers
{
    public class Fm99Provider : IExternalDataProvider
    {
        private readonly Func<IILR1920_DataStoreEntities> _ilrContext;

        public Fm99Provider(Func<IILR1920_DataStoreEntities> ilrContext)
        {
            _ilrContext = ilrContext;
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            using (var context = _ilrContext())
            {
                return new ALBGlobal()
                {
                    Learners = await context.ALB_Learners
                        .Where(fml => fml.UKPRN == reportServiceContext.Ukprn)
                        .Select(learner => new ALBLearner()
                        {
                            LearnRefNumber = learner.LearnRefNumber,
                            LearningDeliveries = learner.ALB_LearningDeliveries.Select(learningDelivery => new LearningDelivery()
                            {
                                AimSeqNumber = learningDelivery.AimSeqNumber,
                                LearningDeliveryValue = new LearningDeliveryValue()
                                {
                                    ApplicFactDate = learningDelivery.ApplicFactDate,
                                    ApplicProgWeightFact = learningDelivery.ApplicProgWeightFact,
                                    AreaCostFactAdj = learningDelivery.AreaCostFactAdj,
                                    FundLine = learningDelivery.FundLine,
                                    LiabilityDate = learningDelivery.LiabilityDate,
                                    PlannedNumOnProgInstalm = learningDelivery.PlannedNumOnProgInstalm,
                                    WeightedRate = learningDelivery.WeightedRate,
                                },
                                LearningDeliveryPeriodisedValues = learningDelivery.ALB_LearningDelivery_PeriodisedValues.Select(ldpv => new LearningDeliveryPeriodisedValue()
                                {
                                    AttributeName = ldpv.AttributeName,
                                    Period1 = ldpv.Period_1,
                                    Period2 = ldpv.Period_2,
                                    Period3 = ldpv.Period_3,
                                    Period4 = ldpv.Period_4,
                                    Period5 = ldpv.Period_5,
                                    Period6 = ldpv.Period_6,
                                    Period7 = ldpv.Period_7,
                                    Period8 = ldpv.Period_8,
                                    Period9 = ldpv.Period_9,
                                    Period10 = ldpv.Period_10,
                                    Period11 = ldpv.Period_11,
                                    Period12 = ldpv.Period_12
                                }).ToList()
                            }).ToList(),
                        }).ToListAsync(cancellationToken)
                };
            }
        }
    }
}
