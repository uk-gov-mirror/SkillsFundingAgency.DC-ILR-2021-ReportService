﻿using System.Reflection;

namespace ESFA.DC.ILR1819.ReportService.Model.Generation
{
    public sealed class ModelProperty
    {
        public string[] Names { get; }

        public PropertyInfo MethodInfo { get; }

        public ModelProperty(string[] names, PropertyInfo methodInfo)
        {
            Names = names;
            MethodInfo = methodInfo;
        }
    }
}
