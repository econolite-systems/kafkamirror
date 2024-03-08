// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace KafkaMirror.Kafka
{
    public interface IConsumer<TKey, TValue>
    {
        Task ConsumeOnAsync(Func<Confluent.Kafka.ConsumeResult<byte[], byte[]>, Task> consumingFunc, CancellationToken cancellationToken);
    }
}
