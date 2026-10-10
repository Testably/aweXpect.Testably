using System.IO.Abstractions;
using aweXpect.Core;

namespace aweXpect.Testably.Results;

public partial class DirectoryResult<TParent>
{
	/// <summary>
	///     Further expectations on the <see cref="IDirectoryInfo" /> identified by this directory result.
	/// </summary>
	public IThat<IDirectoryInfo> Which
		=> new ThatSubject<IDirectoryInfo>(
			_expectationBuilder.ForWhich<TParent, IDirectoryInfo>(
				ResolveDirectoryInfo, " which ", "the directory"));

	private IDirectoryInfo? ResolveDirectoryInfo(TParent source)
	{
		(IFileSystem fs, string fullPath) = _resolver(source);
		return fs.DirectoryInfo.New(fullPath);
	}
}
