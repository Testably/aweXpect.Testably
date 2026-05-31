using System.IO;
using System.Text;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed partial class HasContent
	{
		public sealed class BinaryPlural
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenContentDiffers_ShouldFail()
				{
					byte[] expected = Encoding.UTF8.GetBytes("bar");
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory("foo").Initialized(d => d
						.WithFile("bar.txt").Which(f => f.HasStringContent("baz")));

					async Task Act()
					{
						await That(sut).HasDirectory("foo")
							.WithFiles(files => files.All().ComplyWith(file => file.HasContent(expected)));
					}

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that sut
						              has directory 'foo' whose files have content equal to expected for all items,
						              but not all were

						              Not matching items:
						              [
						                foo{Path.DirectorySeparatorChar}bar.txt,
						                (… and maybe others)
						              ]

						              Collection:
						              [
						                foo{Path.DirectorySeparatorChar}bar.txt
						              ]
						              """).IgnoringNewlineStyle();
				}

				[Fact]
				public async Task WhenContentMatches_ShouldSucceed()
				{
					byte[] content = Encoding.UTF8.GetBytes("baz");
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory("foo").Initialized(d => d
						.WithFile("bar.txt").Which(f => f.HasStringContent("baz")));

					async Task Act()
					{
						await That(sut).HasDirectory("foo")
							.WithFiles(files => files.All().ComplyWith(file => file.HasContent(content)));
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNegated_WhenContentDiffers_ShouldSucceed()
				{
					byte[] expected = Encoding.UTF8.GetBytes("bar");
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory("foo").Initialized(d => d
						.WithFile("bar.txt").Which(f => f.HasStringContent("baz")));

					async Task Act()
					{
						await That(sut).HasDirectory("foo")
							.WithFiles(files => files.All().ComplyWith(file
								=> file.DoesNotComplyWith(it => it.HasContent(expected))));
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNegated_WhenContentMatches_ShouldFail()
				{
					byte[] content = Encoding.UTF8.GetBytes("baz");
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory("foo").Initialized(d => d
						.WithFile("bar.txt").Which(f => f.HasStringContent("baz")));

					async Task Act()
					{
						await That(sut).HasDirectory("foo")
							.WithFiles(files => files.All().ComplyWith(file
								=> file.DoesNotComplyWith(it => it.HasContent(content))));
					}

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that sut
						              has directory 'foo' whose files have content different from content for all items,
						              but not all were

						              Not matching items:
						              [
						                foo{Path.DirectorySeparatorChar}bar.txt,
						                (… and maybe others)
						              ]

						              Collection:
						              [
						                foo{Path.DirectorySeparatorChar}bar.txt
						              ]
						              """).IgnoringNewlineStyle();
				}
			}
		}
	}
}
