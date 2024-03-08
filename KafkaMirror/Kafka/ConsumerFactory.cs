// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;
using Econolite.Ode.Monitoring.Metrics;
using KafkaMirror.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaMirror.Kafka
{
    public class ConsumerFactory : IConsumerFactory
    {
        private readonly IBuildMessagingConfig<string, Local> _buildLocalMessagingConfig;
        private readonly IBuildMessagingConfig<string, Remote> _buildRemoteMessagingConfig;
        private readonly IMetricsFactory _metricsFactory;
        private readonly IOptions<ConsumerOptions<byte[], byte[]>> _optionsConsummer;
        private readonly ILoggerFactory _loggerFactory;

        public ConsumerFactory(IBuildMessagingConfig<string, Local> buildLocalMessagingConfig, IBuildMessagingConfig<string, Remote> buildRemoteMessagingConfig, IMetricsFactory metricsFactory, IOptions<ConsumerOptions<byte[], byte[]>> optionsConsummer, ILoggerFactory loggerFactory)
        {
            _buildLocalMessagingConfig = buildLocalMessagingConfig;
            _buildRemoteMessagingConfig = buildRemoteMessagingConfig;
            _metricsFactory = metricsFactory;
            _optionsConsummer = optionsConsummer;
            _loggerFactory = loggerFactory;
        }

        public IConsumer<byte[], byte[]> CreateLocalConsumer(string topic) => new Consumer(_buildLocalMessagingConfig, topic, _metricsFactory.GetMetricsCounter(topic), _loggerFactory.CreateLogger($"LocalConsummer-{topic}"), _optionsConsummer);
        public IConsumer<byte[], byte[]> CreateRemoteConsumer(string topic) => new Consumer(_buildRemoteMessagingConfig, topic, _metricsFactory.GetMetricsCounter(topic), _loggerFactory.CreateLogger($"RemoteConsummer-{topic}"), _optionsConsummer);
    }
}
