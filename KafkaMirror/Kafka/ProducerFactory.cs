// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;
using Econolite.Ode.Monitoring.Metrics;
using KafkaMirror.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaMirror.Kafka
{
    public class ProducerFactory : IProducerFactory
    {
        private readonly IBuildMessagingConfig<string, Local> _buildLocalMessagingConfig;
        private readonly IBuildMessagingConfig<string, Remote> _buildRemoteMessagingConfig;
        private readonly IMetricsFactory _metricsFactory;
        private readonly IOptions<ProducerOptions<byte[], byte[]>> _optionsProducer;
        private readonly ILoggerFactory _loggerFactory;

        public ProducerFactory(IBuildMessagingConfig<string, Local> buildLocalMessagingConfig, IBuildMessagingConfig<string, Remote> buildRemoteMessagingConfig, IMetricsFactory metricsFactory, IOptions<ProducerOptions<byte[], byte[]>> optionsProducer, ILoggerFactory loggerFactory)
        {
            _buildLocalMessagingConfig = buildLocalMessagingConfig;
            _buildRemoteMessagingConfig = buildRemoteMessagingConfig;
            _metricsFactory = metricsFactory;
            _optionsProducer = optionsProducer;
            _loggerFactory = loggerFactory;
        }

        public IProducer<byte[], byte[]> CreateLocalProducer(string topic) => new Producer(_buildLocalMessagingConfig, topic, _metricsFactory.GetMetricsCounter(topic), _loggerFactory.CreateLogger($"LocalProducer-{topic}"), _optionsProducer);
        public IProducer<byte[], byte[]> CreateRemoteProducer(string topic) => new Producer(_buildRemoteMessagingConfig, topic, _metricsFactory.GetMetricsCounter(topic), _loggerFactory.CreateLogger($"LocalProducer-{topic}"), _optionsProducer);
    }
}
