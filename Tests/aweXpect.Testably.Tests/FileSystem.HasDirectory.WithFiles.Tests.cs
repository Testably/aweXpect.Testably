using System.IO;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed partial class HasDirectory
	{
		public sealed class WithFiles
		{
			public sealed class Tests
			{
				[Fact]
				public async Task AllAreReadOnly_WhenFileIsNotReadOnly_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithFile("bar.txt"));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithFiles(files => files.All().ComplyWith(file => file.IsReadOnly()));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose files all are read-only,
						              but none of at least 1 were*
						              """).AsWildcard()
						.Because("the verb agrees with the plural files");
				}

				[Fact]
				public async Task AllDoNotExist_WhenFileExists_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithFile("bar.txt"));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithFiles(files => files.All().ComplyWith(file => file.DoesNotExist()));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose files all do not exist,
						              but none of at least 1 did*
						              """).AsWildcard()
						.Because("the verb agrees with the plural files");
				}

				[Fact]
				public async Task AllHaveAttribute_WhenAttributeIsMissing_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithFile("bar.txt"));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithFiles(files => files.All().ComplyWith(file => file.HasAttribute(FileAttributes.Hidden)));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose files all have attribute Hidden,
						              but none of at least 1 did*
						              """).AsWildcard()
						.Because("the verb agrees with the plural files");
				}

				[Fact]
				public async Task AllHaveContent_WhenContentIsDifferent_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithFile("bar.txt").Which(f => f.HasStringContent("some-content")));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithFiles(files => files.All().ComplyWith(file
								=> file.HasContent("SOME-CONTENT")));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose files all have content equal to "SOME-CONTENT",
						              but none of at least 1 did

						              Not matching items (files):
						              [
						                foo{Path.DirectorySeparatorChar}bar.txt,
						                (… and maybe more)
						              ]

						              Collection (files):
						              [
						                foo{Path.DirectorySeparatorChar}bar.txt,
						                (… and maybe more)
						              ]

						              File content (files[0]):
						              some-content
						              """).IgnoringNewlineStyle();
				}

				[Fact]
				public async Task AllHaveContent_WhenContentMatches_ShouldSucceed()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithFile("bar.txt").Which(f => f.HasStringContent("some-content")));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithFiles(f => f.All().ComplyWith(x => x.HasContent("SOME-CONTENT").IgnoringCase()));
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task AllHaveContent_WhenNegated_WhenContentIsDifferent_ShouldSucceed()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithFile("bar.txt").Which(f => f.HasStringContent("some-content")));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithFiles(files => files.All().ComplyWith(file
								=> file.DoesNotComplyWith(it => it.HasContent("SOME-CONTENT"))));
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task AllHaveContent_WhenNegated_WhenContentMatches_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithFile("bar.txt").Which(f => f.HasStringContent("some-content")));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithFiles(f
								=> f.All().ComplyWith(x => x.DoesNotComplyWith(it => it.HasContent("some-content"))));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose files all do not have content equal to "some-content",
						              but none of at least 1 did

						              Not matching items (files):
						              [
						                foo{Path.DirectorySeparatorChar}bar.txt,
						                (… and maybe more)
						              ]

						              Collection (files):
						              [
						                foo{Path.DirectorySeparatorChar}bar.txt,
						                (… and maybe more)
						              ]

						              File content (files[0]):
						              some-content
						              """).IgnoringNewlineStyle();
				}

				[Fact]
				public async Task AllHaveContentEqualTo_WhenContentIsDifferent_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithFile("bar.txt").Which(f => f.HasStringContent("some-content")));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithFiles(files => files.All().ComplyWith(file => file.HasContent().EqualTo("other")));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose files all have content equal to "other",
						              but none of at least 1 did*
						              """).AsWildcard()
						.Because("the verb agrees with the plural files and is not preceded by an extra space");
				}

				[Fact]
				public async Task AllHaveContentNotEqualTo_WhenContentMatches_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithFile("bar.txt").Which(f => f.HasStringContent("some-content")));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithFiles(files => files.All().ComplyWith(file => file.HasContent().NotEqualTo("some-content")));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose files all do not have content equal to "some-content",
						              but none of at least 1 did*
						              """).AsWildcard()
						.Because("the negation belongs to the plural verb only");
				}

				[Fact]
				public async Task AllHaveCreationTime_WhenNegated_WhenCreationTimeMatches_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithFile("bar.txt"));
					DateTime creationTime = sut.File.GetCreationTime(Path.Combine(path, "bar.txt"));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithFiles(files => files.All().ComplyWith(file
								=> file.DoesNotComplyWith(it => it.HasCreationTime(creationTime))));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose files all do not have creation time equal to {Formatter.Format(creationTime)},
						              but none of at least 1 did*
						              """).AsWildcard()
						.Because("the negation belongs to the plural verb only");
				}

				[Fact]
				public async Task AllHaveExtension_WhenExtensionDiffers_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithFile("bar.txt"));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithFiles(files => files.All().ComplyWith(file => file.HasExtension(".md")));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose files all have extension equal to ".md",
						              but none of at least 1 did*
						              """).AsWildcard()
						.Because("the verb agrees with the plural files");
				}

				[Fact]
				public async Task AllHaveLength_WhenLengthDiffers_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithFile("bar.txt").Which(f => f.HasStringContent("some-content")));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithFiles(files => files.All().ComplyWith(file => file.HasLength(3)));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose files all have length 3,
						              but none of at least 1 did*
						              """).AsWildcard()
						.Because("the verb agrees with the plural files");
				}

				[Fact]
				public async Task AllHaveName_WhenNameDiffers_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithFile("bar.txt"));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithFiles(files => files.All().ComplyWith(file => file.HasName("baz.txt")));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose files all have name equal to "baz.txt",
						              but none of at least 1 did*
						              """).AsWildcard()
						.Because("the verb agrees with the plural files");
				}

				[Fact]
				public async Task AllHaveName_WhenNegated_WhenNameMatches_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithFile("bar.txt"));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithFiles(files => files.All().ComplyWith(file
								=> file.DoesNotComplyWith(it => it.HasName("bar.txt"))));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose files all do not have name equal to "bar.txt",
						              but none of at least 1 did*
						              """).AsWildcard()
						.Because("the negation belongs to the plural verb only");
				}

				[Fact]
				public async Task AllHaveSameContentAs_WhenContentIsDifferent_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize()
						.WithFile("expected.txt").Which(f => f.HasStringContent("other"))
						.WithSubdirectory(path).Initialized(d => d
							.WithFile("bar.txt").Which(f => f.HasStringContent("some-content")));
					string fullExpectedPath = sut.Path.GetFullPath("expected.txt");

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithFiles(files => files.All().ComplyWith(file => file.HasContent().SameAs("expected.txt")));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose files all have the same content as file {Formatter.Format(fullExpectedPath)},
						              but none of at least 1 did*
						              """).AsWildcard()
						.Because("the verb agrees with the plural files");
				}

				[Fact]
				public async Task BeEmpty_WhenDirectoryIsEmpty_ShouldSucceed()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path);

					async Task Act()
					{
						await That(sut).HasDirectory(path).WithFiles(f => f.IsEmpty());
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task BeEmpty_WhenDirectoryIsNotEmpty_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithFile("bar.txt").Which(f => f.HasStringContent("some-content")));

					async Task Act()
					{
						await That(sut).HasDirectory(path).WithFiles(f => f.IsEmpty());
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose files are empty,
						              but files were [
						                foo{Path.DirectorySeparatorChar}bar.txt
						              ]
						              """);
				}
			}
		}
	}
}
