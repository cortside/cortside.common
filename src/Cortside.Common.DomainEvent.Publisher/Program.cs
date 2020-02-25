using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using Microsoft.Extensions.Logging;

namespace Cortside.Common.DomainEvent.Publisher {
    class Program {
        class Options {
            [Option("appname", Required = false, Default = "Publisher", HelpText = "Application name")]
            public string AppName { get; set; }
            [Option("protocol", Required = false, Default = "amqps", HelpText = "protocol (amqp/amqps)")]
            public string Protocol { get; set; }
            [Option("policy", Required = true, HelpText = "Policy")]
            public string Policy { get; set; }
            [Option("key", Required = true, HelpText = "Key")]
            public string Key { get; set; }
            [Option("namespace", Required = true, HelpText = "namespace")]
            public string Namespace { get; set; }
            [Option("eventtype", Required = true, HelpText = "Event type (event class full name)")]
            public string EventType { get; set; }
            [Option("address", Required = true, HelpText = "Address (event class name)")]
            public string Address { get; set; }
            [Option("data", Required = false, HelpText = "Message data/json")]
            public string Data { get; set; }
            [Option("file", Required = false, HelpText = "filename for Message data/json")]
            public string File { get; set; }

        }

        static void Main(string[] args) {
            CommandLine.Parser.Default.ParseArguments<Options>(args)
              .WithParsed<Options>(opts => RunOptionsAndReturnExitCode(opts))
              .WithNotParsed<Options>((errs) => HandleParseError(errs));
        }

        private static void HandleParseError(IEnumerable<Error> errs) {
            foreach (var err in errs) {
                Console.WriteLine($"ERROR: {err.ToString()}");
            }
        }

        private static void RunOptionsAndReturnExitCode(Options opts) {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole();
            var logger = loggerFactory.CreateLogger<DomainEventComms>();

            var psettings = new ServiceBusPublisherSettings {
                AppName = opts.AppName,
                Protocol = opts.Protocol,
                PolicyName = opts.Policy,
                Key = opts.Key,
                Namespace = opts.Namespace,
                Durable = 1
            };
            var publisher = new DomainEventPublisher(psettings, logger);

            if (!String.IsNullOrWhiteSpace(opts.File)) {
                opts.Data = File.ReadAllText(opts.File);
            }

            if (string.IsNullOrEmpty(opts.Data)) {
                logger.LogWarning("Data or File must be specified and have data");
            }

            try {
                publisher.SendAsync(opts.EventType, opts.Address, opts.Data).GetAwaiter().GetResult();
            } finally {
                if (publisher.Error == null) {
                    logger.LogInformation("message sent");
                } else {
                    logger.LogError($"error sending message: {publisher.Error.Description} :: {publisher.Error.Condition}");
                }
            }
        }
    }
}
