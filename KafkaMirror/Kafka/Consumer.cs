// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Confluent.Kafka;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Extensions;
using Econolite.Ode.Monitoring.Metrics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaMirror.Kafka
{
    public class Consumer : IConsumer<byte[], byte[]>
    {
        private readonly IBuildMessagingConfig _buildMessagingConfig;
        private readonly string _topic;
        private readonly IMetricsCounter _metricsCounter;
        private readonly ILogger _logger;
        private readonly ConsumerOptions<byte[], byte[]> _options;

        public Consumer(IBuildMessagingConfig buildMessagingConfig, string topic, IMetricsCounter metricsCounter, ILogger logger, IOptions<ConsumerOptions<byte[], byte[]>> options)
        {
            _buildMessagingConfig = buildMessagingConfig;
            _topic = topic;
            _metricsCounter = metricsCounter;
            _logger = logger;
            _options = options.Value;
        }

        public async Task ConsumeOnAsync(Func<Confluent.Kafka.ConsumeResult<byte[], byte[]>, Task> consumingFunc, CancellationToken cancellationToken)
        {
            try
            {
                var consumer = new ConsumerBuilder<byte[], byte[]>(_buildMessagingConfig.BuildConsumerClientConfig(_options))
                    .AddLogging(_logger)
                    .Build();
                consumer.Subscribe(_topic);
                _logger.LogInformation("Subscribed: {@}", _topic);
                await ConsumeLoopAsync(consumingFunc, consumer, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unable to setup consumption");
                throw;
            }
        }

        private async Task ConsumeLoopAsync(Func<Confluent.Kafka.ConsumeResult<byte[], byte[]>, Task> consumingFunc, Confluent.Kafka.IConsumer<byte[], byte[]> consumer, CancellationToken cancellationToken)
        {
            do
            {
                try
                {
                    var consumeresult = consumer.Consume(cancellationToken);
                    await consumingFunc(consumeresult);
                    _metricsCounter.Increment();
                    consumer.StoreOffset(consumeresult);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogTrace(ex, "Lost connection to topic");
                }
                catch (OperationCanceledException)
                {
                    // nothing to see here.
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                }
            } while (!cancellationToken.IsCancellationRequested);
        }
    }
}
