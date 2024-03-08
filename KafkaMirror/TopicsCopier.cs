// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using KafkaMirror.Config;
using KafkaMirror.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaMirror
{
    public class TopicsCopier : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IConsumerFactory _consumerFactory;
        private readonly IProducerFactory _producerFactory;
        private readonly Local _optionsLocal;
        private readonly Remote _optionsRemote;

        public TopicsCopier(IConsumerFactory consumerFactory, IProducerFactory producerFactory, IOptions<Local> optionsLocal, IOptions<Remote> optionsRemote, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType().Name);
            _consumerFactory = consumerFactory;
            _producerFactory = producerFactory;
            _optionsLocal = optionsLocal.Value;
            _optionsRemote = optionsRemote.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                List<Task> tasks = new List<Task>();
                foreach (var topic in _optionsLocal.SourceTopics)
                {
                    tasks.Add(Task.Run(() => _producerFactory.CreateRemoteProducer(topic).BeginMirroringAsync(_consumerFactory.CreateLocalConsumer(topic), stoppingToken), stoppingToken));
                }
                foreach (var topic in _optionsRemote.SourceTopics)
                {
                    tasks.Add(Task.Run(() =>_producerFactory.CreateLocalProducer(topic).BeginMirroringAsync(_consumerFactory.CreateRemoteConsumer(topic), stoppingToken), stoppingToken));
                }
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unable to setup the mirroring exiting");
            }
        }
    }
}
