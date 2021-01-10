#region Usings

using System.Collections;

#endregion


namespace Eshva.Poezd.Core.DependencyInjection
{
  /// <summary>
  /// Represents the context of resolving one root service and can be used throughout the tree to fetch something to be injected.
  /// </summary>
  public interface IResolutionContext
  {
    /// <summary>
    /// Gets all instances resolved within this resolution context at this time.
    /// </summary>
    IEnumerable TrackedInstances { get; }

    /// <summary>
    /// Gets an instance of the specified <typeparamref name="TService"/>.
    /// </summary>
    TService Get<TService>();

    /// <summary>
    /// Gets whether there exists a primary registration for the <typeparamref name="TService"/> type.
    /// </summary>
    bool Has<TService>(bool isPrimary = true); // TODO: Rename to IsRegistered.
  }
}
