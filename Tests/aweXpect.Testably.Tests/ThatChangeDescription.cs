using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Tests;

public sealed partial class ThatChangeDescription
{
	/// <summary>
	///     Runs <paramref name="trigger" /> on a fresh <see cref="MockFileSystem" /> and returns
	///     the first <see cref="ChangeDescription" /> that fired during it.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IAwaitableCallback{T}.Wait(int, System.TimeSpan?)" /> so the helper does
	///     not depend on Testably delivering notifications synchronously on the trigger thread.
	/// </remarks>
	internal static ChangeDescription Capture(Action<MockFileSystem> trigger)
	{
		MockFileSystem fileSystem = new();
		using IAwaitableCallback<ChangeDescription> registration = fileSystem.Notify.OnEvent(_ => { });
		trigger(fileSystem);
		ChangeDescription[] captured = registration.Wait(1, TimeSpan.FromSeconds(30));
		if (captured.Length == 0)
		{
			throw new InvalidOperationException("No notification was captured during the trigger action.");
		}

		return captured[0];
	}
}
