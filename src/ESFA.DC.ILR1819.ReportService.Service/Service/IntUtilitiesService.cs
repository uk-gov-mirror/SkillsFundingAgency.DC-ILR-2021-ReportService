﻿using System;
using ESFA.DC.ILR1819.ReportService.Interface.Service;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class IntUtilitiesService : IIntUtilitiesService
    {
        public int ObjectToInt(object value)
        {
            if (value is int i)
            {
                return i;
            }

            return Convert.ToInt32(value);
        }
    }
}