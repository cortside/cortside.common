namespace Cortside.Common.Testing.Logging.Xunit {
    /// <summary>
    /// Extension methods for the <see cref="ILoggerFactory"/> class.
    /// </summary>
    public static class XunitLoggerFactoryExtensions {
        /// <summary>
        /// Adds a debug logger named 'Debug' to the factory.
        /// </summary>
        /// <param name="builder">The extension method argument.</param>
        public static ILoggingBuilder AddXunit(this ILoggingBuilder builder, ITestOutputHelper output) {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider>(new XunitLoggerProvider(output)));

            return builder;
        }
    }
}
