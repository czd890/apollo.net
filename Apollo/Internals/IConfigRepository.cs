using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Enums;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public interface IConfigRepository : IDisposable
    {

        string Namespace { get; }
        ConfigFileFormat Format { get; }
        /// <summary>
        /// Get the config from this repository. </summary>
        /// <returns> config </returns>
        Properties GetConfig();

    Task Initialize();

    /// <summary>
    /// Add change listener. </summary>
    /// <param name="listener"> the listener to observe the changes </param>
    void AddChangeListener(IRepositoryChangeListener listener);

    /// <summary>
    /// Remove change listener. </summary>
    /// <param name="listener"> the listener to remove </param>
    void RemoveChangeListener(IRepositoryChangeListener listener);
}