// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace KafkaMirror.Kafka
{
    public interface IConsumerFactory
    {
        IConsumer<byte[], byte[]> CreateLocalConsumer(string topic);
        IConsumer<byte[], byte[]> CreateRemoteConsumer(string topic);
    }
}
