using System;
using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using KafkaSubscriber.Interfaces;
using KafkaSubscriber.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SynchronizationSubscriberService.Abstracts
{
    public class SyncService<T> : IPointService
    {
        protected DateTime _bulkBegin { get; set; } = DateTime.Now;
        protected int _allowedMessageCount { get; } = 10;
        protected readonly SyncRepository<T> _repository;
        protected readonly ILogger _logger;

        public SyncService(SyncRepository<T> repository, ILoggerFactory logger)
        {
            _repository = repository;
            _logger = logger.CreateLogger(this.GetType().Name);
        }

        public ProcessingStatus SaveMessage(Message<string, string> message)
        {
            var result = ProcessingStatus.Retry;
            try
            {
                var data = JsonConvert.DeserializeObject<T>(message.Value);
                _repository.Synchronization(new List<T> { data });
                result = ProcessingStatus.Commit;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error sending data to the database in service {this.GetType().Name}, message: {e.Message}");
            }
            return result;
        }

        public bool SaveCollectedMessages(List<Message<string, string>> messages)
        {
            var result = false;
            try
            {
                if (messages.Count >= _allowedMessageCount ||
                    DateTime.Now - _bulkBegin > TimeSpan.FromMinutes(1))
                {
                    var datalist = messages.Select(message => JsonConvert.DeserializeObject<T>(message.Value))
                        .ToList();
                    _repository.Synchronization(datalist);
                    _bulkBegin = DateTime.Now;
                    result = true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error sending data to the database in service {this.GetType().Name}, message: {e.Message}");
            }
            return result;
        }
    }
}
