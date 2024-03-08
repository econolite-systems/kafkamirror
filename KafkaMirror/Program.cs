// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Extensions.AspNet;
using Econolite.Ode.Messaging;
using Econolite.Ode.Monitoring.Metrics.Extensions;
using KafkaMirror;
using KafkaMirror.Config;
using KafkaMirror.Kafka;
using KafkaMirror.Monitoring;

await AppBuilder.BuildAndRunWebHostAsync(args, options => { options.Source = "Kafka Mirror"; }, (builder, services) =>
{
    services
        .Configure<Local>(builder.Configuration.GetSection("Topics:Local"))
        .Configure<Remote>(builder.Configuration.GetSection("Topics:Remote"))
        .Configure<KafkaConfigOptions<string, Local>>(builder.Configuration.GetSection("Kafka:Local"))
        .PostConfigure<KafkaConfigOptions<string, Local>>(_ => _.CertFilenames = new CertFilenames
        {
            CAFilename = "local.ca.crt",
            CertFilename = "local.cert.crt",
        })
        .Configure<KafkaConfigOptions<string, Remote>>(builder.Configuration.GetSection("Kafka:Remote"))
        .PostConfigure<KafkaConfigOptions<string, Remote>>(_ => _.CertFilenames = new CertFilenames
        {
            CAFilename = "remote.ca.crt",
            CertFilename = "remote.cert.crt",
        })
        .AddMetrics<LocalMonitoring>(builder.Configuration, "KafkaMirror")
        .AddSingleton<IBuildMessagingConfig<string, Local>, BuildMessagingConfig<string, Local>>()
        .AddSingleton<IBuildMessagingConfig<string, Remote>, BuildMessagingConfig<string, Remote>>()
        .AddSingleton<IConsumerFactory, ConsumerFactory>()
        .AddSingleton<IProducerFactory, ProducerFactory>()
        .AddHostedService<TopicsCopier>();
});
