namespace Nexus.Logging;

/// <summary>
///     Represents the Serilog settings for logging to Elasticsearch.
/// </summary>
public class SerilogSettings
{
    /// <summary>
    ///     Gets or sets the Elasticsearch settings for Serilog logging.
    /// </summary>
    public SerilogElasticSearchSettings ElasticSearchSettings { get; set; } = new ()
    {
        Password = string.Empty,
        IndexFormat = string.Empty,
        Uri = string.Empty,
        Username = string.Empty,
    };

    /// <summary>
    ///     Represents the Elasticsearch settings for Serilog logging.
    /// </summary>
    public class SerilogElasticSearchSettings
    {
        /// <summary>
        ///     Gets or sets the URI of the Elasticsearch server.
        /// </summary>
        /// <remarks>
        ///     This is the URI of the Elasticsearch server that Serilog will send log events to, e.g. "http://localhost:9200".
        /// </remarks>
        required public string Uri { get; set; }

        /// <summary>
        ///     Gets or sets the username to use for authentication with Elasticsearch.
        /// </summary>
        required public string Username { get; set; }

        /// <summary>
        ///     Gets or sets the password to use for authentication with Elasticsearch.
        /// </summary>
        required public string Password { get; set; }

        /// <summary>
        ///     Gets or sets the format string to use for the index name in Elasticsearch.
        /// </summary>
        /// <remarks>
        ///     This is the format string that Serilog will use to create the index name for log events in Elasticsearch.
        ///     The default value is an empty string, which means the index name will not contain a timestamp.
        /// </remarks>
        public string IndexFormat { get; set; } = string.Empty;
    }
}