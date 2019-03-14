﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR1819.DataStore.EF.Valid;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Context;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public abstract class BaseLearnersService
    {
        private readonly string _filename;
        private readonly ILogger _logger;
        private readonly IKeyValuePersistenceService _redis;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly DataStoreConfiguration _dataStoreConfiguration;

        private readonly SemaphoreSlim _getDataLock;

        private bool _loadedDataAlready;

        private List<string> _loadedData;

        protected BaseLearnersService(
            string key,
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Redis)] IKeyValuePersistenceService redis,
            IJsonSerializationService jsonSerializationService,
            DataStoreConfiguration dataStoreConfiguration)
        {
            _filename = key;
            _logger = logger;
            _redis = redis;
            _jsonSerializationService = jsonSerializationService;
            _dataStoreConfiguration = dataStoreConfiguration;
            _loadedData = null;
            _getDataLock = new SemaphoreSlim(1, 1);
        }

        public async Task<List<string>> GetLearnersAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            await _getDataLock.WaitAsync(cancellationToken);

            try
            {
                if (_loadedDataAlready)
                {
                    return _loadedData;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                _loadedDataAlready = true;
                int ukPrn = reportServiceContext.Ukprn;

                if (await _redis.ContainsAsync(_filename, cancellationToken))
                {
                    string learnersValidStr = await _redis.GetAsync(_filename, cancellationToken);
                    _loadedData = _jsonSerializationService.Deserialize<List<string>>(learnersValidStr);
                }
                else
                {
                    var validLearnersList = new List<string>();

                    DbContextOptions<ILR1819_DataStoreEntitiesValid> validContextOptions = new DbContextOptionsBuilder<ILR1819_DataStoreEntitiesValid>().UseSqlServer(_dataStoreConfiguration.ILRDataStoreValidConnectionString).Options;
                    using (var ilrValidContext = new ILR1819_DataStoreEntitiesValid(validContextOptions))
                    {
                        validLearnersList = ilrValidContext.Learners.Where(x => x.Ukprn == ukPrn).Select(x => x.LearnRefNumber).ToList();
                    }

                    _loadedData = validLearnersList;
                }
            }
            catch (Exception ex)
            {
                // Todo: Check behaviour
                _logger.LogError($"Failed to get & deserialise {_filename}", ex);
            }
            finally
            {
                _getDataLock.Release();
            }

            return _loadedData;
        }
    }
}
