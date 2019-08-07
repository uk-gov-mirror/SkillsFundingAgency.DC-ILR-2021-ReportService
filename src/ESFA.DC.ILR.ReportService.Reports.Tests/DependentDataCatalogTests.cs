﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests
{
    public class DependentDataCatalogTests
    {
        [Fact]
        public void Ilr()
        {
            DependentDataCatalog.ValidIlr.Should().Be<IMessage>();
        }

        [Fact]
        public void ValidationErrors()
        {
            DependentDataCatalog.ValidationErrors.Should().Be<List<ValidationError>>();
        }

        [Fact]
        public void ReferenceDataRoot()
        {
            DependentDataCatalog.ReferenceData.Should().Be<ReferenceDataRoot>();
        }
    }
}
