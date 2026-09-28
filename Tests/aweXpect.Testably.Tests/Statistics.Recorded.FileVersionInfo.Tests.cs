using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed class FileVersionInfo
		{
			public sealed class FactoryTests
			{
				[Fact]
				public async Task GetVersionInfo_WithFileNameFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo.GetVersionInfo(f => f == "a.exe").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileVersionInfo.GetVersionInfo with fileName matching f => f == "a.exe" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task GetVersionInfo_WithFileNameFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					fileSystem.File.WriteAllText("b.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("b.exe");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo.GetVersionInfo(f => f == "a.exe").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class PropertyTests
			{
				[Fact]
				public async Task Comments_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").Comments;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].Comments.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task CompanyName_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").CompanyName;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].CompanyName.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task FileBuildPart_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").FileBuildPart;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].FileBuildPart.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task FileDescription_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").FileDescription;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].FileDescription.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task FileMajorPart_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").FileMajorPart;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].FileMajorPart.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task FileMinorPart_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").FileMinorPart;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].FileMinorPart.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task FileName_Get_ShouldRecordOnInstance()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").FileName;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].FileName.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task FilePrivatePart_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").FilePrivatePart;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].FilePrivatePart.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task FileVersion_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").FileVersion;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].FileVersion.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task InternalName_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").InternalName;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].InternalName.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task IsDebug_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").IsDebug;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].IsDebug.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task IsPatched_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").IsPatched;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].IsPatched.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task IsPreRelease_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").IsPreRelease;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].IsPreRelease.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task IsPrivateBuild_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").IsPrivateBuild;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].IsPrivateBuild.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task IsSpecialBuild_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").IsSpecialBuild;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].IsSpecialBuild.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Language_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").Language;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].Language.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task LegalCopyright_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").LegalCopyright;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].LegalCopyright.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task LegalTrademarks_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").LegalTrademarks;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].LegalTrademarks.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task OriginalFilename_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").OriginalFilename;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].OriginalFilename.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task PrivateBuild_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").PrivateBuild;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].PrivateBuild.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ProductBuildPart_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").ProductBuildPart;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].ProductBuildPart.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ProductMajorPart_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").ProductMajorPart;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].ProductMajorPart.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ProductMinorPart_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").ProductMinorPart;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].ProductMinorPart.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ProductName_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").ProductName;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].ProductName.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ProductPrivatePart_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").ProductPrivatePart;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].ProductPrivatePart.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ProductVersion_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").ProductVersion;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].ProductVersion.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task SpecialBuild_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");
					_ = fileSystem.FileVersionInfo.GetVersionInfo("a.exe").SpecialBuild;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].SpecialBuild.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNotAccessed_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.exe", "");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileVersionInfo["a.exe"].Comments.Get().Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a get of FileVersionInfo["a.exe"].Comments exactly once,
						             but it was recorded 0 times
						             """);
				}
			}
		}
	}
}
