﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESFA.DC.ILR.ReportService.Reports.Comparer;
using ESFA.DC.ILR.ReportService.Service.Interface.Builders;
using ESFA.DC.ILR.ReportService.Service.Model.ReportModels;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Reports.Builders
{
    public class ValidationSchemaErrorsReportBuilder : IValidationSchemaErrorsReportBuilder
    {
        private static readonly ValidationErrorsModelComparer ValidationErrorsModelComparer = new ValidationErrorsModelComparer();

        public IEnumerable<ValidationErrorModel> Build(IEnumerable<ValidationError> ilrValidationErrors)
        {
            return ilrValidationErrors
                .Select(e => new ValidationErrorModel()
                {
                    AimSequenceNumber = e.AimSequenceNumber,
                    ErrorMessage = string.Empty,
                    FieldValues = e.ValidationErrorParameters == null
                        ? string.Empty
                        : GetValidationErrorParameters(e.ValidationErrorParameters),
                    LearnerReferenceNumber = e.LearnerReferenceNumber,
                    RuleName = e.RuleName,
                    Severity = e.Severity
                }).OrderBy(v => v, ValidationErrorsModelComparer);
        }

        private string GetValidationErrorParameters(IEnumerable<ValidationErrorParameter> validationErrorParameters)
        {
            StringBuilder result = new StringBuilder();

            foreach (var parameter in validationErrorParameters)
            {
                result.Append($"{parameter.PropertyName}={parameter.Value}|");
            }
          
            return result.ToString();
        }
    }
}
