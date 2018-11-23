using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KafkaSubscriber.Interfaces;
using KafkaSubscriber.Models;
using KafkaSubscriber.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using SynchronizationSubscriberService.Abstracts;
using SynchronizationSubscriberService.Helpers;
using SynchronizationSubscriberService.Models.appsettings;

namespace SynchronizationSubscriberService
{
    public class Synchronization
    {
        private SynchronizationService _service;
        private Task task;
        private IEnumerable<SynchronizationTableInfo> _tables;

        /// <summary>
        /// Синхронизатор, получает из appsetting все таблицы синхронизации и
        /// запускает сервис синхронизации
        /// </summary>
        /// <param name="provider">провайдер сервисов</param>
        /// <param name="tables">таблицы по которым будет запущена синхронизация</param>
        /// <returns></returns>
        public Task Start(IServiceProvider provider, IEnumerable<SynchronizationTableInfo> tables)
        {
            var logfactory = provider.GetService<ILoggerFactory>();
            _tables = tables;
            var logger = logfactory.CreateLogger<Synchronization>();

            logger.LogInformation("Service started");
            try
            {
                var subscriberSettings = new List<SubscriberSettings>();

                foreach (var table in _tables)
                {
                    var method = ReflectionHelper.GetMethodByName(table.ModelName);
                    var commonServiceType = ReflectionHelper.GenericType(method, typeof(SyncService<>));
                    subscriberSettings.Add(new SubscriberSettings
                    {
                        PointService = (IPointService)provider.GetService(commonServiceType),
                        KafkaTopic = table.KafkaTopic,
                        KafkaGroupId = table.KafkaGroupId
                    });
                }

                _service = new SynchronizationService(subscriberSettings, logfactory);
                task = _service.StartSynchronizationAsync(default(CancellationToken));
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
            }
            return task;
        }
    }
}
