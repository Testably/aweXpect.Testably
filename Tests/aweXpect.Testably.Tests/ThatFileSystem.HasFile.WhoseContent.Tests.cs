using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class ThatFileSystem
{
	public sealed partial class HasFile
	{
		public sealed class WhoseContent
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenContentIsDifferent_ShouldFail()
				{
					string path = "foo.txt";
					MockFileSystem sut = new();
					// ReSharper disable once MethodHasAsyncOverload
					sut.File.WriteAllText(path, "baz");

					async Task Act()
					{
						await That(sut).HasFile(path).WhoseContent(c => c.IsEmpty());
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has file {Formatter.Format(path)} whose content is empty,
						              but content was "baz"
						              """);
				}

				[Test]
				public async Task WhenContentMatches_ShouldSucceed()
				{
					string path = "foo.txt";
					MockFileSystem sut = new();
					// ReSharper disable once MethodHasAsyncOverload
					sut.File.WriteAllText(path, "");

					async Task Act()
					{
						await That(sut).HasFile(path).WhoseContent(c => c.IsEmpty());
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
