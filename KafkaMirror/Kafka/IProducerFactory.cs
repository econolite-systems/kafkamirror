// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace KafkaMirror.Kafka
{
    public interface IProducerFactory
    {
        IProducer<byte[], byte[]> CreateLocalProducer(string topic);
        IProducer<byte[], byte[]> CreateRemoteProducer(string topic);
    }
}
