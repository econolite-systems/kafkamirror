// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace KafkaMirror.Kafka
{
    public interface IProducer<TKey, TValue>
    {
        Task BeginMirroringAsync(IConsumer<byte[], byte[]> consumer, CancellationToken cancellationToken);
    }
}
