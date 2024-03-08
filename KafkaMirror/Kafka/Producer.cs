// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Extensions;
using Econolite.Ode.Monitoring.Metrics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaMirror.Kafka
{
    public class Producer : IProducer<byte[], byte[]>
    {
        private readonly IBuildMessagingConfig _buildMessagingConfig;
        private readonly string _topic;
        private readonly IMetricsCounter _metricsCounter;
        private readonly ILogger _logger;
        private readonly ProducerOptions<byte[], byte[]> _options;

        public Producer(IBuildMessagingConfig buildMessagingConfig, string topic, IMetricsCounter metricsCounter, ILogger logger, IOptions<ProducerOptions<byte[], byte[]>> options)
        {
            _buildMessagingConfig = buildMessagingConfig;
            _topic = topic;
            _metricsCounter = metricsCounter;
            _logger = logger;
            _options = options.Value;
        }

        public async Task BeginMirroringAsync(IConsumer<byte[], byte[]> consumer, CancellationToken cancellationToken)
        {
            try
            {
                var producer = new Confluent.Kafka.ProducerBuilder<byte[], byte[]>(_buildMessagingConfig.BuildProducerClientConfig(_options))
                    .AddLogging(_logger)
                    .Build();
                _logger.LogInformation("Set to produce on {@}", _topic);
                await consumer.ConsumeOnAsync(async (result) =>
                {
                    await ConsumingAndProducingFuncAsync(producer, result, cancellationToken);
                    _metricsCounter.Increment();
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unable to setup consumption");
                throw;
            }
        }

        public async Task ConsumingAndProducingFuncAsync(Confluent.Kafka.IProducer<byte[], byte[]> producer, Confluent.Kafka.ConsumeResult<byte[], byte[]> consumeResult, CancellationToken cancellationToken)
        {
            var message = new Confluent.Kafka.Message<byte[], byte[]>
            {
                Headers = consumeResult.Message.Headers,
                Key = consumeResult.Message.Key,
                Value = consumeResult.Message.Value,
                Timestamp = consumeResult.Message.Timestamp,
            };
            await producer.ProduceAsync(_topic, message, cancellationToken);
        }
    }
}
