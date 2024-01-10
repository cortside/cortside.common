# Release 6.2

* Update and cleanup nuget dependencies to latest stable versions
* Remove any direct serilog references to break Serilog coupling, needed for making cortside libraries easier to multitarget net6.0 and net8.0
* Include README.md in nuget pagckage in attempt to make changes more evident


|Commit|Date|Author|Message|
|---|---|---|---|
| b58632a | <span style="white-space:nowrap;">2023-11-08</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update version
| 4b22be9 | <span style="white-space:nowrap;">2023-11-08</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'release/6.1' into develop
| f46254d | <span style="white-space:nowrap;">2023-11-08</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'master' into develop
| 9583465 | <span style="white-space:nowrap;">2023-11-08</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'master' into develop
| eb2dd89 | <span style="white-space:nowrap;">2023-11-09</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Update XunitLogger to make it public
| d68abce | <span style="white-space:nowrap;">2023-11-09</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  use readme.md for nuget.org
| 87b08fa | <span style="white-space:nowrap;">2023-11-09</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add and move extension classes
| 4f141e6 | <span style="white-space:nowrap;">2023-11-09</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'master' into develop
| d064072 | <span style="white-space:nowrap;">2023-11-09</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'develop' of github.com:cortside/cortside.common into develop
| f893534 | <span style="white-space:nowrap;">2023-11-14</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'master' into develop
| 8d673e2 | <span style="white-space:nowrap;">2023-12-22</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (HEAD -> release/6.2, origin/develop, origin/HEAD, develop) update to latest nuget packages and remove any direct serilog references
****

# Release 6.1

* Update nuget dependencies to latest stable versions
* Add ScopedLocalTimeZone for use with tests to set predictable timezone regardless of environment
    ```csharp
    using (new ScopedLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById("UTC+12"))) {
        var localDateTime = new DateTime(2020, 12, 31, 23, 59, 59, DateTimeKind.Local);
        var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime);

        Assert.Equal("UTC+12", TimeZoneInfo.Local.Id);
        Assert.Equal(utcDateTime.AddHours(12), localDateTime);
    }
    ```
* Add XunitLogger for capturing log output to xUnit's ITestOutputHelper
	```csharp
	// Create a logger factory with a debug provider
	loggerFactory = LoggerFactory.Create(builder => {
		builder
			.SetMinimumLevel(LogLevel.Trace)
			.AddFilter("Microsoft", LogLevel.Warning)
			.AddFilter("System", LogLevel.Warning)
			.AddFilter("Cortside.Common", LogLevel.Trace)
			.AddXunit(output);
	});

	// Create a logger with the category name of the current class
	var logger = loggerFactory.CreateLogger<XunitLoggerTest>();

	... user logger as you would any other logger ...
	```
* changed hierarchichal organization of loggers, so LogEventLogger has new namespace of Cortside.Common.Testing.Logging.LogEvent
* Added helper class RandomValues for generating "random" data
* Added IServiceCollection extension method Unregister<T> for unregistering something already in service collection
	```csharp
	// Remove the app's DbContext registration.
	services.Unregister<DbContextOptions<DatabaseContext>>();
	services.Unregister<DbContext>();
	```

|Commit|Date|Author|Message|
|---|---|---|---|
| 9b1a714 | <span style="white-space:nowrap;">2023-08-29</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update version
| 1007bdd | <span style="white-space:nowrap;">2023-08-29</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update release notes
| f0f453f | <span style="white-space:nowrap;">2023-09-04</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'master' into develop
| 3c480ba | <span style="white-space:nowrap;">2023-09-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add new project for common unit and integration test related classes
| a11f4cd | <span style="white-space:nowrap;">2023-10-16</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Add LogEventLogger to help in validating and testing log output
| e2f06ca | <span style="white-space:nowrap;">2023-10-31</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add extension method for unregistering a service from service collection
| 184c4a2 | <span style="white-space:nowrap;">2023-11-08</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add new XunitLogger for capturing ITestOutputHelper output
| 5efb5a7 | <span style="white-space:nowrap;">2023-11-08</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (HEAD -> develop, origin/develop, origin/HEAD) update to latest nuget packages
****

# Release 6.0

* Update version number to match framework version (6.x)
* Update projects to be net6.0
* Update nuget dependencies to latest stable versions
* Use FrameworkReference to remove deprecated nuget references

|Commit|Date|Author|Message|
|---|---|---|---|
| 5fa0531 | <span style="white-space:nowrap;">2023-06-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  merge from release
| 8ff9b4c | <span style="white-space:nowrap;">2023-06-20</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'master' into develop
| efab617 | <span style="white-space:nowrap;">2023-06-26</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [net6] update to net6
| fc87c1e | <span style="white-space:nowrap;">2023-06-26</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [net6] update to net6
| 4ec9f20 | <span style="white-space:nowrap;">2023-06-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [net6] cleanup from build lint analysis
| a5f003f | <span style="white-space:nowrap;">2023-06-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [net6] cleanup from build lint analysis
| cb0e249 | <span style="white-space:nowrap;">2023-06-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [net6] add some unit tests to address lint analysis
| aace252 | <span style="white-space:nowrap;">2023-06-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [net6] cleanup from build lint analysis
| cc57bd9 | <span style="white-space:nowrap;">2023-06-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [net6] update to net6
| a1e6f00 | <span style="white-space:nowrap;">2023-06-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [net6] cleanup of lint warnings
| 56e9250 | <span style="white-space:nowrap;">2023-06-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [net6] fix namespace casing
| ee8d225 | <span style="white-space:nowrap;">2023-06-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [net6] fix namespace casing
| 2033755 | <span style="white-space:nowrap;">2023-06-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [net6] fix namespace casing
| 471b316 | <span style="white-space:nowrap;">2023-06-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (origin/net6, net6) [net6] fix namespace casing
| 7c41b93 | <span style="white-space:nowrap;">2023-06-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #53 from cortside/net6
| 0c4fbdc | <span style="white-space:nowrap;">2023-07-17</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update version to 6.x to be in line with dotnet and net6 version numbers
| aa6bbd3 | <span style="white-space:nowrap;">2023-07-17</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update version to 6.x to be in line with dotnet and net6 version numbers
| c2ddf75 | <span style="white-space:nowrap;">2023-08-29</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  use FrameworkReference to remove deprecated nuget references; update to latest nuget packages
| 8b5c3b2 | <span style="white-space:nowrap;">2023-08-29</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (HEAD -> release/6.0, origin/develop, origin/HEAD, develop) use FrameworkReference to remove deprecated nuget references; update to latest nuget packages
****

# Release 1.3

|Commit|Date|Author|Message|
|---|---|---|---|
| fa86848 | <span style="white-space:nowrap;">2022-12-29</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update version
| 0800b0e | <span style="white-space:nowrap;">2022-12-30</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'release/1.2' into develop
| b864ad3 | <span style="white-space:nowrap;">2023-01-02</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'master' into develop
| 2e93192 | <span style="white-space:nowrap;">2023-01-02</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'master' into develop
| 2eac9c0 | <span style="white-space:nowrap;">2023-01-03</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  minor cleanup
| 7622185 | <span style="white-space:nowrap;">2023-01-03</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  minor cleanup
| f71b825 | <span style="white-space:nowrap;">2023-01-03</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  merge from master
| 442455c | <span style="white-space:nowrap;">2023-01-03</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (origin/release/1.2) Merge branch 'master' into release/1.2
| 813dc7f | <span style="white-space:nowrap;">2023-01-03</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'release/1.2' into develop
| 30dd98d | <span style="white-space:nowrap;">2023-01-04</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'master' into develop
| 1c97007 | <span style="white-space:nowrap;">2023-01-04</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'master' into release/1.2
| 92ec10e | <span style="white-space:nowrap;">2023-01-04</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'release/1.2' into develop
| 88090e9 | <span style="white-space:nowrap;">2023-06-05</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add helper method to MessageListException for inspecting message types
| b685b7b | <span style="white-space:nowrap;">2023-06-05</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  cleanup of lint warnings
| 44297e8 | <span style="white-space:nowrap;">2023-06-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Create dotnet.yml
| e013a6a | <span style="white-space:nowrap;">2023-06-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Update dotnet.yml
| 09ede25 | <span style="white-space:nowrap;">2023-06-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Update dotnet.yml
| 6df25f6 | <span style="white-space:nowrap;">2023-06-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Create codeql.yml
| 6fe59bd | <span style="white-space:nowrap;">2023-06-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Update codeql.yml
| ceb5423 | <span style="white-space:nowrap;">2023-06-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Create codeql-config.yml
| 335efc3 | <span style="white-space:nowrap;">2023-06-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update from template
| df0df6a | <span style="white-space:nowrap;">2023-06-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (HEAD -> release/1.3, origin/develop, origin/HEAD, develop) update from template
****

# Release 1.2
|Commit|Date|Author|Message|
|---|---|---|---|
| 445749c | <span style="white-space:nowrap;">2022-08-01</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [correlationtests] add tests to assert correlationId works; add async suffix to async methods
| c5c9aa6 | <span style="white-space:nowrap;">2022-08-01</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'develop' into correlationtests
| d599421 | <span style="white-space:nowrap;">2022-08-01</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'develop' into correlationtests
| cf68bbd | <span style="white-space:nowrap;">2022-08-01</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'develop' into correlationtests
| 72826d6 | <span style="white-space:nowrap;">2022-08-02</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #47 from cortside/correlationtests
| 74e47a7 | <span style="white-space:nowrap;">2022-12-21</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update nuget api key
| 9a25b58 | <span style="white-space:nowrap;">2022-12-29</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [feature/BOT-20221229] updated nuget packages
| 19ce7e8 | <span style="white-space:nowrap;">2022-12-29</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (origin/feature/BOT-20221229, feature/BOT-20221229) update build and helper scripts
| 3e71e1b | <span style="white-space:nowrap;">2022-12-29</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #49 from cortside/feature/BOT-20221229
| b2709a3 | <span style="white-space:nowrap;">2022-12-29</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (HEAD -> release/1.2, origin/develop, origin/HEAD, develop) initial changelog
****
# Release 1.1
|Commit|Date|Author|Message|
|---|---|---|---|
| 73eec9b | <span style="white-space:nowrap;">2013-01-23</span> | <span style="white-space:nowrap;">cort@Pahia.spring2tech.com</span> |  move from SVN
| 375eefe | <span style="white-space:nowrap;">2013-01-29</span> | <span style="white-space:nowrap;">rolandk</span> |  Updated makefile so it can actually output artifact files for use.
| b656c39 | <span style="white-space:nowrap;">2014-05-20</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add publishing to internal nuget feed
| 9b966f8 | <span style="white-space:nowrap;">2014-05-23</span> | <span style="white-space:nowrap;">bryantc@sevenpeaks.spring2tech.com</span> |  -Moved Web.Hosting to its own folder -Removed Web.Client and Web.Server and Web(empty proj) -Added Spring2.Common.Configuration with IConfigurationProvider, Simple and AppSettings
| 82a2b9b | <span style="white-space:nowrap;">2014-05-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  fixed nuget publishing
| 2815975 | <span style="white-space:nowrap;">2014-06-13</span> | <span style="white-space:nowrap;">justine</span> |  Added Storage project with SqlBlobProvider.
| 98a00fb | <span style="white-space:nowrap;">2014-06-13</span> | <span style="white-space:nowrap;">justine</span> |  Added Storage project with SqlBlobProvider.
| 1990cfc | <span style="white-space:nowrap;">2014-06-16</span> | <span style="white-space:nowrap;">justine</span> |  SQL & File Blob providers implementation.
| cc26ff7 | <span style="white-space:nowrap;">2014-06-17</span> | <span style="white-space:nowrap;">justine</span> |  AzureBlobProvider & Blob Export Utility
| 1b51446 | <span style="white-space:nowrap;">2016-05-02</span> | <span style="white-space:nowrap;">cort</span> |  add build file for appveyor
| 2841172 | <span style="white-space:nowrap;">2016-05-02</span> | <span style="white-space:nowrap;">cort</span> |  enumerate test assemblies
| b147e69 | <span style="white-space:nowrap;">2016-05-02</span> | <span style="white-space:nowrap;">cort</span> |  enumerate test assemblies
| 1087f8b | <span style="white-space:nowrap;">2016-05-02</span> | <span style="white-space:nowrap;">cort</span> |  include nuget packages
| ca28f20 | <span style="white-space:nowrap;">2016-05-02</span> | <span style="white-space:nowrap;">cort</span> |  include nuget packages
| 053d6a7 | <span style="white-space:nowrap;">2016-05-02</span> | <span style="white-space:nowrap;">cort</span> |  add reference for private nuget server
| cd6817e | <span style="white-space:nowrap;">2016-05-02</span> | <span style="white-space:nowrap;">cort</span> |  publish nuget packages
| 375f865 | <span style="white-space:nowrap;">2016-05-02</span> | <span style="white-space:nowrap;">cort</span> |  use version from assembly
| b2fe01d | <span style="white-space:nowrap;">2016-05-02</span> | <span style="white-space:nowrap;">cort</span> |  use version from assembly
| 3eaa262 | <span style="white-space:nowrap;">2016-05-02</span> | <span style="white-space:nowrap;">cort</span> |  add common projects from LA as spring2.common
| 1ee78eb | <span style="white-space:nowrap;">2016-05-02</span> | <span style="white-space:nowrap;">cort</span> |  add dnx to project setup
| 6ef7170 | <span style="white-space:nowrap;">2016-05-02</span> | <span style="white-space:nowrap;">cort</span> |  install rc1-update1
| 32ed553 | <span style="white-space:nowrap;">2016-05-02</span> | <span style="white-space:nowrap;">cort</span> |  add nuspec files for new projects
| 23bfb70 | <span style="white-space:nowrap;">2016-05-02</span> | <span style="white-space:nowrap;">cort</span> |  add all nupkg to artifacts
| e565a68 | <span style="white-space:nowrap;">2016-05-02</span> | <span style="white-space:nowrap;">cort</span> |  only publish spring2 packages
| 8e63653 | <span style="white-space:nowrap;">2016-05-02</span> | <span style="white-space:nowrap;">cort</span> |  set dnx version number
| 0a33f26 | <span style="white-space:nowrap;">2016-05-02</span> | <span style="white-space:nowrap;">cort</span> |  set dnx version number
| 04bedd4 | <span style="white-space:nowrap;">2016-05-02</span> | <span style="white-space:nowrap;">cort</span> |  dnx version format
| 09a1c51 | <span style="white-space:nowrap;">2016-05-03</span> | <span style="white-space:nowrap;">cort</span> |  dnx version format
| 1d42d93 | <span style="white-space:nowrap;">2016-05-03</span> | <span style="white-space:nowrap;">cort</span> |  build number
| 8dd9064 | <span style="white-space:nowrap;">2016-05-03</span> | <span style="white-space:nowrap;">cort</span> |  build number
| b3ecf8d | <span style="white-space:nowrap;">2016-05-03</span> | <span style="white-space:nowrap;">cort</span> |  add net451 target to dnx projects
| b497939 | <span style="white-space:nowrap;">2016-05-10</span> | <span style="white-space:nowrap;">ruifang</span> |  project json test change
| 4167f2e | <span style="white-space:nowrap;">2016-05-10</span> | <span style="white-space:nowrap;">ruifang</span> |  project json test changes
| 73ed2d6 | <span style="white-space:nowrap;">2016-05-10</span> | <span style="white-space:nowrap;">ruifang</span> |  remove references not needed, make System.Runtime as a dev dependency
| 8b6d9c1 | <span style="white-space:nowrap;">2016-05-17</span> | <span style="white-space:nowrap;">ruifang</span> |  test dependency version range
| 804ab36 | <span style="white-space:nowrap;">2016-11-30</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  Convert spring2.common projects to dotnet core 1 enabled for .net 4.5.1 and dnx 4.5.1
| 2866450 | <span style="white-space:nowrap;">2016-11-30</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  Update of appveyor.yml and some final changes to spring2.common for dotnet core support
| 7aaff00 | <span style="white-space:nowrap;">2016-11-30</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  add missing dashes to appveyor.yml
| 2ff2057 | <span style="white-space:nowrap;">2016-11-30</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  another missing dash in appveyor.yml
| c9295c1 | <span style="white-space:nowrap;">2016-11-30</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  remove build script that is no longer used, change dnu restore to dotnet restore
| 89722d4 | <span style="white-space:nowrap;">2016-11-30</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  Reimplement build script
| 96de8e0 | <span style="white-space:nowrap;">2016-11-30</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  update build script target
| 091f3cd | <span style="white-space:nowrap;">2016-12-01</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  Try try again
| 70a5459 | <span style="white-space:nowrap;">2016-12-01</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  more fully populate build script
| 2a3c021 | <span style="white-space:nowrap;">2016-12-01</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  attempt to reset environment path, echo out path
| 88672ef | <span style="white-space:nowrap;">2016-12-01</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  test setting variables in csproj instead of using build script
| 63a0544 | <span style="white-space:nowrap;">2016-12-01</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  update loc csproj
| 8541350 | <span style="white-space:nowrap;">2016-12-01</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  update other xproj files
| 79ef1d8 | <span style="white-space:nowrap;">2016-12-02</span> | <span style="white-space:nowrap;">cort</span> |  remove deprecated project tags and fix version numbers
| 966bc57 | <span style="white-space:nowrap;">2016-12-02</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  add configuration add dotnet pack
| 7e9b6dd | <span style="white-space:nowrap;">2016-12-02</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  Merge
| 8f3eef2 | <span style="white-space:nowrap;">2016-12-02</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  update dotnet pack with configuration path to test
| c868bae | <span style="white-space:nowrap;">2016-12-02</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  fix path for dotnet pack test of configuration project
| ed5a9c5 | <span style="white-space:nowrap;">2016-12-02</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  try try again
| 49572dd | <span style="white-space:nowrap;">2016-12-02</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  readd xproj property groups for web security
| 9a7ba5f | <span style="white-space:nowrap;">2016-12-02</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  test dotnet pack in after-build and with --no-build
| f5efc28 | <span style="white-space:nowrap;">2016-12-02</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  readd bootstrap xproj property groups
| d240215 | <span style="white-space:nowrap;">2016-12-02</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  add dotnet packing for the rest of the spring2.common projects
| 7238b86 | <span style="white-space:nowrap;">2016-12-19</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  Attempt full migration to dotnet core of command project
| 67c060e | <span style="white-space:nowrap;">2016-12-19</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  attempt to publish dll in nuget packages
| aceab00 | <span style="white-space:nowrap;">2016-12-19</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  change build output path for command project
| 91600e7 | <span style="white-space:nowrap;">2016-12-20</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  more changes in attempt to deploy dll with nuget packages
| bba7dd4 | <span style="white-space:nowrap;">2016-12-20</span> | <span style="white-space:nowrap;">joes@spring2.com</span> |  remove no build tag from dotnet pack
| af0b108 | <span style="white-space:nowrap;">2017-02-02</span> | <span style="white-space:nowrap;">cort</span> |  update appveyor build
| cf335aa | <span style="white-space:nowrap;">2017-02-02</span> | <span style="white-space:nowrap;">cort</span> |  add dotnet-version so that we can....version
| dbd006d | <span style="white-space:nowrap;">2017-02-02</span> | <span style="white-space:nowrap;">cort</span> |  remove committed packages
| 0f2d8fe | <span style="white-space:nowrap;">2017-02-02</span> | <span style="white-space:nowrap;">cort</span> |  remove .vs from committed source
| fede118 | <span style="white-space:nowrap;">2017-02-02</span> | <span style="white-space:nowrap;">cort</span> |  update to exclude folders that should not be committed
| 9cdf29d | <span style="white-space:nowrap;">2017-02-02</span> | <span style="white-space:nowrap;">cort</span> |  remove, should be uneeded
| f85e3c2 | <span style="white-space:nowrap;">2017-02-02</span> | <span style="white-space:nowrap;">cort</span> |  update build to be a little more dynamic
| b9f6464 | <span style="white-space:nowrap;">2017-02-02</span> | <span style="white-space:nowrap;">cort</span> |  remove & syntax
| f729412 | <span style="white-space:nowrap;">2017-02-02</span> | <span style="white-space:nowrap;">cort</span> |  fix invocation
| 4bfe949 | <span style="white-space:nowrap;">2017-02-02</span> | <span style="white-space:nowrap;">cort</span> |  comment out stop for a moment
| e34bcae | <span style="white-space:nowrap;">2017-02-02</span> | <span style="white-space:nowrap;">cort</span> |  change in hopes to avoid build warning
| 1e16e9d | <span style="white-space:nowrap;">2017-02-02</span> | <span style="white-space:nowrap;">cort</span> |  revert back to previous build without scripts
| 94c79ff | <span style="white-space:nowrap;">2017-02-03</span> | <span style="white-space:nowrap;">cort</span> |  updated to .netstandard1.6 for all of the libraries
| 3a128ae | <span style="white-space:nowrap;">2017-02-03</span> | <span style="white-space:nowrap;">cort</span> |  ignore a compiler warning and add a note to resolve
| 8e4f1d0 | <span style="white-space:nowrap;">2017-02-03</span> | <span style="white-space:nowrap;">Cort A. Schaefer</span> |  conversion from hg to git
| e8c8e51 | <span style="white-space:nowrap;">2017-02-03</span> | <span style="white-space:nowrap;">josephshull</span> |  add netstandard library to spring2.common.command
| 687f5da | <span style="white-space:nowrap;">2017-02-03</span> | <span style="white-space:nowrap;">josephshull</span> |  remove runtimes from spring2.common.command
| ba9b9c7 | <span style="white-space:nowrap;">2017-02-03</span> | <span style="white-space:nowrap;">josephshull</span> |  remove dotnet version from spring2.common.command
| 4dad475 | <span style="white-space:nowrap;">2017-02-03</span> | <span style="white-space:nowrap;">josephshull</span> |  readd dotnet versioning, add target to spring2.common.command nuspec file
| 2ecb78a | <span style="white-space:nowrap;">2017-02-03</span> | <span style="white-space:nowrap;">josephshull</span> |  update spring2.common.command nuspec target to debug
| 44b70d3 | <span style="white-space:nowrap;">2017-02-06</span> | <span style="white-space:nowrap;">josephshull</span> |  add Rui's changes from orderservice -- addition of IQueryListResult, generic exception handler etc
| 3e2a716 | <span style="white-space:nowrap;">2017-02-06</span> | <span style="white-space:nowrap;">josephshull</span> |  remove runtimes from spring2.common.web.mvc and spring2.common.query
| e8551cf | <span style="white-space:nowrap;">2017-02-06</span> | <span style="white-space:nowrap;">josephshull</span> |  remove runtimes from bootstrap and ioc projects, add netstandard1.4 framework
| 496f02b | <span style="white-space:nowrap;">2017-02-06</span> | <span style="white-space:nowrap;">josephshull</span> |  add netstandard1.4 to command and query
| 2f4f3aa | <span style="white-space:nowrap;">2017-02-06</span> | <span style="white-space:nowrap;">Rui Fang Li</span> |  use configured json settings when writing output in exception handlers
| f6b969f | <span style="white-space:nowrap;">2017-07-27</span> | <span style="white-space:nowrap;">Cort A. Schaefer</span> |  upgrade project using VS2017
| f356c62 | <span style="white-space:nowrap;">2017-07-28</span> | <span style="white-space:nowrap;">rolandk</span> |  Simplified/loosened up constraints in the dispatchers. Added DomainEvent project for publishing/receiving domain events.
| bf0f2c3 | <span style="white-space:nowrap;">2017-07-31</span> | <span style="white-space:nowrap;">rolandk</span> |  Attempt at fixing build from dotnet CLI differences in prerelease
| 4ab285e | <span style="white-space:nowrap;">2017-07-31</span> | <span style="white-space:nowrap;">rolandk</span> |  Added missing parameter
| 15ccc32 | <span style="white-space:nowrap;">2017-07-31</span> | <span style="white-space:nowrap;">rolandk</span> |  Fixed actual location of where dotnet restore was failing. Added DomainEvents to be packagaged
| 118e2e1 | <span style="white-space:nowrap;">2017-07-31</span> | <span style="white-space:nowrap;">rolandk</span> |  Attempt at fixing syntax in the build script
| 83dfa2a | <span style="white-space:nowrap;">2017-07-31</span> | <span style="white-space:nowrap;">rolandk</span> |  Removed .net framework projects from the solution
| 7d1ff26 | <span style="white-space:nowrap;">2017-07-31</span> | <span style="white-space:nowrap;">rolandk</span> |  packaging nuget packages
| b1c6b06 | <span style="white-space:nowrap;">2017-07-31</span> | <span style="white-space:nowrap;">rolandk</span> |  put artifacts string in quotes in build file
| dcaf699 | <span style="white-space:nowrap;">2017-07-31</span> | <span style="white-space:nowrap;">rolandk</span> |  Removed debug test section
| 8d11ead | <span style="white-space:nowrap;">2017-07-31</span> | <span style="white-space:nowrap;">rolandk</span> |  Added backslash to indicate artifacts is a different folder in the build script
| 25dbc9b | <span style="white-space:nowrap;">2017-07-31</span> | <span style="white-space:nowrap;">rolandk</span> |  Package solution instead of individual projects. Added branch suffix to package
| f5167cd | <span style="white-space:nowrap;">2017-07-31</span> | <span style="white-space:nowrap;">rolandk</span> |  Fixed syntax in build script
| 777ffa9 | <span style="white-space:nowrap;">2017-07-31</span> | <span style="white-space:nowrap;">rolandk</span> |  Added missing backslash
| 6e07b1c | <span style="white-space:nowrap;">2017-07-31</span> | <span style="white-space:nowrap;">Roland Kwong</span> |  Merged in upgrade11 (pull request #1)
| 02ee923 | <span style="white-space:nowrap;">2017-08-01</span> | <span style="white-space:nowrap;">rolandk</span> |  Made the type name of the event be suffixed to the address setting when publishing.
| de4e53a | <span style="white-space:nowrap;">2017-08-01</span> | <span style="white-space:nowrap;">rolandk</span> |  Removed Error object passed into the Reject call on DomainEventHandler.Handle() failure, because the receiver stops receiving if there is an error.
| e4aaa0e | <span style="white-space:nowrap;">2017-08-01</span> | <span style="white-space:nowrap;">rolandk</span> |  Removed unneeded project
| 530a517 | <span style="white-space:nowrap;">2017-08-02</span> | <span style="white-space:nowrap;">Roland Kwong</span> |  Merged in c7registration (pull request #2)
| 5aea423 | <span style="white-space:nowrap;">2017-08-02</span> | <span style="white-space:nowrap;">rolandk</span> |  Package only the projects that have a nuspec file
| 990b3ac | <span style="white-space:nowrap;">2017-08-25</span> | <span style="white-space:nowrap;">rolandk</span> |  netcore2 update.
| ec705e2 | <span style="white-space:nowrap;">2017-08-25</span> | <span style="white-space:nowrap;">rolandk</span> |  Updated the image of which to build this project.
| 07e5b4c | <span style="white-space:nowrap;">2017-08-25</span> | <span style="white-space:nowrap;">rolandk</span> |  Added back test section in build file
| 1954583 | <span style="white-space:nowrap;">2017-08-25</span> | <span style="white-space:nowrap;">Roland Kwong</span> |  Merged in netcore2 (pull request #3)
| e48f1f9 | <span style="white-space:nowrap;">2017-08-25</span> | <span style="white-space:nowrap;">Roland Kwong</span> |  README.md edited online with Bitbucket
| dd8c747 | <span style="white-space:nowrap;">2018-01-15</span> | <span style="white-space:nowrap;">cYCL157</span> |  update name for new home
| 6424462 | <span style="white-space:nowrap;">2018-01-15</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  change namespace for new home
| 6cd3fdc | <span style="white-space:nowrap;">2018-01-15</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  reformatting; test develop branch build
| 420a197 | <span style="white-space:nowrap;">2018-02-15</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add a bootstrapper overload to init with an IConfigurationRoot for when it's already built
| 2a49a7c | <span style="white-space:nowrap;">2018-02-15</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #1 from cortside/develop
| 85fd4d2 | <span style="white-space:nowrap;">2018-02-23</span> | <span style="white-space:nowrap;">jnielsen</span> |  Add callback for fault tolerance Add setting for Durable queues
| 9e772d0 | <span style="white-space:nowrap;">2018-02-26</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #2 from nielsenjeff/develop
| 919c09e | <span style="white-space:nowrap;">2018-02-26</span> | <span style="white-space:nowrap;">jnielsen</span> |  add callback to publisher
| 9a68024 | <span style="white-space:nowrap;">2018-03-02</span> | <span style="white-space:nowrap;">jnielsen</span> |  update nuget packages add callback to domain event publisher expand comments on domain event settings
| 6c13072 | <span style="white-space:nowrap;">2018-03-22</span> | <span style="white-space:nowrap;">jnielsen</span> |  Add more detail to readme for azure configuration.
| d2e82ff | <span style="white-space:nowrap;">2018-03-22</span> | <span style="white-space:nowrap;">jnielsen</span> |  skip unit test
| 1ab6787 | <span style="white-space:nowrap;">2018-03-22</span> | <span style="white-space:nowrap;">jnielsen</span> |  fix extra brace in config.json
| 05212ab | <span style="white-space:nowrap;">2018-03-27</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #4 from nielsenjeff/develop
| 65b3c0b | <span style="white-space:nowrap;">2018-12-06</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add some additional logging for troubleshooting
| bdcdd62 | <span style="white-space:nowrap;">2018-12-06</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update nuget packages to latest versions; remove logging of message body
| 9664940 | <span style="white-space:nowrap;">2018-12-06</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  ignore ncrunch files
| 2a9a266 | <span style="white-space:nowrap;">2018-12-07</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  better null handling for handler
| e0e6c64 | <span style="white-space:nowrap;">2018-12-07</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  disable appveyor feeds to try to resolve build errors
| ac9dbef | <span style="white-space:nowrap;">2018-12-07</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  create seperate settings classes for both the receiver and publisher so that they can both be registered in IoC
| e80e514 | <span style="white-space:nowrap;">2019-03-25</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #5 from cortside/develop
| 459cf61 | <span style="white-space:nowrap;">2019-03-25</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add command line utility to publish message
| 71fc3de | <span style="white-space:nowrap;">2019-03-25</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  remove readline wait
| 272ee0e | <span style="white-space:nowrap;">2019-07-30</span> | <span style="white-space:nowrap;">thorton</span> |  fix for the dlq using byte array as the body type
| 427af8a | <span style="white-space:nowrap;">2019-08-06</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #6 from cortside/dlq-byte-array.-fix
| a946c2a | <span style="white-space:nowrap;">2019-08-06</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  allow for message body to come from file
| 61f0a87 | <span style="white-space:nowrap;">2019-08-06</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update nuget api key
| 691b491 | <span style="white-space:nowrap;">2019-08-06</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update nuget api key
| 08868d4 | <span style="white-space:nowrap;">2019-09-18</span> | <span style="white-space:nowrap;">thorton</span> |  added null handling in receiver close
| e12b56f | <span style="white-space:nowrap;">2019-09-18</span> | <span style="white-space:nowrap;">thorton</span> |  added session close in publisher
| bce0aab | <span style="white-space:nowrap;">2019-09-18</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #7 from cortside/add-null-handling-in-close
| fa9e446 | <span style="white-space:nowrap;">2019-11-13</span> | <span style="white-space:nowrap;">cYCL157</span> |  Update appveyor.yml
| c82ecf7 | <span style="white-space:nowrap;">2019-12-07</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  added support for correlationId on messages
| 4b3e6ce | <span style="white-space:nowrap;">2019-12-07</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  added support for correlationId on messages
| 0c18b37 | <span style="white-space:nowrap;">2019-12-07</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  added support for correlationId on messages
| 5a76bbe | <span style="white-space:nowrap;">2019-12-08</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add middleware and context to be able to get correlationId
| 9492e6b | <span style="white-space:nowrap;">2019-12-10</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  if Request-Id header is empty string from httpcontext, set it as if it didn't exist
| fc03b1e | <span style="white-space:nowrap;">2019-12-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  push messageid, correlationId and messageType to logging context on both publish and receive
| 4bee381 | <span style="white-space:nowrap;">2019-12-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  log as trace level the message body
| 5de0fd5 | <span style="white-space:nowrap;">2019-12-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update log levels and add messageId to publish, receive, accept and reject log entries
| 6161150 | <span style="white-space:nowrap;">2019-12-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add ability to set x-opt-scheduled-enqueue-time annotation for scheduled delivery messages
| 7045497 | <span style="white-space:nowrap;">2019-12-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  trying to simplify multiple signatures by using options class
| b96ebce | <span style="white-space:nowrap;">2019-12-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  trying to simplify multiple signatures by using options class
| 6a0d278 | <span style="white-space:nowrap;">2019-12-28</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #9 from cortside/correlationId
| 2dda59a | <span style="white-space:nowrap;">2019-12-29</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  support sending a scheduled message
| a49e6aa | <span style="white-space:nowrap;">2020-02-25</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #8 from cortside/develop
| 4f25709 | <span style="white-space:nowrap;">2020-03-19</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'develop' into schedulemessage
| 36ca29a | <span style="white-space:nowrap;">2020-03-20</span> | <span style="white-space:nowrap;">Ruifang Li</span> |  Merge pull request #10 from cortside/schedulemessage
| d30818d | <span style="white-space:nowrap;">2020-03-25</span> | <span style="white-space:nowrap;">Troy Horton</span> |  added receiverhostedservice
| 1ad8f36 | <span style="white-space:nowrap;">2020-03-25</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #11 from cortside/feature/receiver-hosted-service
| 7d8af2b | <span style="white-space:nowrap;">2020-03-25</span> | <span style="white-space:nowrap;">Troy Horton</span> |  fix for receiver hosted service going in to endless loop
| 7045e14 | <span style="white-space:nowrap;">2020-03-25</span> | <span style="white-space:nowrap;">Troy Horton</span> |  receiverhostedservice loop fix
| fe29667 | <span style="white-space:nowrap;">2020-03-25</span> | <span style="white-space:nowrap;">Troy Horton</span> |  updated receiverhostedservice loop logic
| 02bdd65 | <span style="white-space:nowrap;">2020-03-25</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #12 from cortside/receiver-hosted-service
| 86546a8 | <span style="white-space:nowrap;">2020-06-17</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  better handle deserialize errors; add unit test around message callback handling
| f917c5b | <span style="white-space:nowrap;">2020-06-17</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  better handle deserialize errors; add unit test around message callback handling
| eedf26d | <span style="white-space:nowrap;">2020-06-17</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  better handle deserialize errors; add unit test around message callback handling
| 4af3692 | <span style="white-space:nowrap;">2020-06-17</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add additional test to validate malformed json handling
| 0abe833 | <span style="white-space:nowrap;">2020-06-29</span> | <span style="white-space:nowrap;">Troy Horton</span> |  updated ampqnetlite to latest
| 9e56ea1 | <span style="white-space:nowrap;">2020-06-29</span> | <span style="white-space:nowrap;">Troy Horton</span> |  attempt to fix service blocking host from starting
| ec39a5a | <span style="white-space:nowrap;">2020-07-02</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #13 from cortside/deserialize-error
| 632cfa2 | <span style="white-space:nowrap;">2020-07-02</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #15 from cortside/update-amqpnetlite
| b70361c | <span style="white-space:nowrap;">2020-11-10</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  health check and availability functionality
| b12d7e0 | <span style="white-space:nowrap;">2020-11-11</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update build image to 2019
| 138d147 | <span style="white-space:nowrap;">2020-11-11</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add nuspec files to publish nuget packages
| 8f802d4 | <span style="white-space:nowrap;">2020-11-11</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update nuget key
| f4d19b3 | <span style="white-space:nowrap;">2020-11-11</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  fix namespaces
| 30a12ae | <span style="white-space:nowrap;">2020-11-11</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add project for json handling of iso timestamps
| b907ac6 | <span style="white-space:nowrap;">2020-11-18</span> | <span style="white-space:nowrap;">smendez-dev</span> |  add support for a custom messageId in the SendAsync method
| f43ce6d | <span style="white-space:nowrap;">2020-11-19</span> | <span style="white-space:nowrap;">smendez-dev</span> |  update api key
| ce753a0 | <span style="white-space:nowrap;">2020-11-19</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update nuget api key
| 4542201 | <span style="white-space:nowrap;">2020-11-19</span> | <span style="white-space:nowrap;">smendez-dev</span> |  Merge branch 'develop' into send-async-message-id
| 9bd5e2b | <span style="white-space:nowrap;">2020-11-19</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #17 from smendezp/send-async-message-id
| 0fc57cf | <span style="white-space:nowrap;">2020-11-21</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  make it possible to register new check types without having to implement ICheckFactory
| 8eca68c | <span style="white-space:nowrap;">2020-11-21</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update nuget api key
| a7a8113 | <span style="white-space:nowrap;">2020-11-22</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  use both interval and cache duration for checks
| fc3921c | <span style="white-space:nowrap;">2020-11-22</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  move out health to new repo; add correcltionId to timed hosted service
| a7faf90 | <span style="white-space:nowrap;">2020-11-23</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #18 from cortside/send-async-message-id
| a49bcbc | <span style="white-space:nowrap;">2020-11-25</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  simplify use of GetCorrelationId by allowing it and defaulting to return a new value if one is not set
| 12d9c00 | <span style="white-space:nowrap;">2020-11-25</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #19 from cortside/healthavailability
| 3987491 | <span style="white-space:nowrap;">2020-11-25</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  cleanup and use dotnet pack against solution to get better package dependency
| fafa8da | <span style="white-space:nowrap;">2020-11-25</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #20 from cortside/nuget-packaging
| d65654d | <span style="white-space:nowrap;">2020-11-25</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #21 from cortside/develop
| 35e2ac3 | <span style="white-space:nowrap;">2020-11-26</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  make iso timespan serializer backwards compatable with c# default
| 230392e | <span style="white-space:nowrap;">2020-11-26</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'develop' of github.com:cortside/cortside.common into develop
| 2e15824 | <span style="white-space:nowrap;">2020-11-26</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'master' into develop
| 5b71700 | <span style="white-space:nowrap;">2020-11-27</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #22 from cortside/develop
| e817224 | <span style="white-space:nowrap;">2020-12-07</span> | <span style="white-space:nowrap;">smendez</span> |  add support for a custom messageId in the ScheduleMessageAsync method
| 2dc993d | <span style="white-space:nowrap;">2020-12-08</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #23 from cortside/schedule-async-message-id
| 58fb9d0 | <span style="white-space:nowrap;">2021-01-27</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #24 from cortside/develop
| 96b67e1 | <span style="white-space:nowrap;">2021-02-11</span> | <span style="white-space:nowrap;">cYCL157</span> |  Create LICENSE
| 2b4f1f8 | <span style="white-space:nowrap;">2021-03-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add serializer settings to publisher settings for better control over serialization of message bodies
| 6054d19 | <span style="white-space:nowrap;">2021-03-17</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #26 from cortside/develop
| 7f61c38 | <span style="white-space:nowrap;">2021-04-20</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add Task.Yield to start of TimeHostedService incase implementors don't have async code
| bca7d85 | <span style="white-space:nowrap;">2021-04-21</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  comment out use of semophore that _should_ not be needed with delay that is being used now
| 4bffca1 | <span style="white-space:nowrap;">2021-04-21</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  completely remove semaphore after testing
| 85b87c0 | <span style="white-space:nowrap;">2021-06-04</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #27 from cortside/develop
| 9662e66 | <span style="white-space:nowrap;">2021-07-06</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add Task extension methods for handling async timeouts
| d9bc01d | <span style="white-space:nowrap;">2021-07-06</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Add WithCancellation extension method
| 9ba83ee | <span style="white-space:nowrap;">2021-08-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Add AsyncUtil
| 23ae299 | <span style="white-space:nowrap;">2021-10-05</span> | <span style="white-space:nowrap;">cYCL157</span> |  Merge pull request #28 from cortside/develop
| ae57655 | <span style="white-space:nowrap;">2021-11-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add project for SubjectPrincipal
| bcae312 | <span style="white-space:nowrap;">2021-11-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'develop' of github.com:cortside/cortside.common into develop
| 1e7bbce | <span style="white-space:nowrap;">2021-11-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update nuget api key
| 6bcf56f | <span style="white-space:nowrap;">2021-11-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  expose simplified claims on SubjectPrincipal
| c8a6446 | <span style="white-space:nowrap;">2021-11-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update to netstandard2.1 for all library projects, remove domainevent that was moved to it's own repository as well as remove other currently unused projects
| eb475cd | <span style="white-space:nowrap;">2021-11-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #29 from cortside/develop
| fe6d919 | <span style="white-space:nowrap;">2021-11-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'netstandard21' into develop
| d5a7775 | <span style="white-space:nowrap;">2021-11-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update to 1.1, merge netstandard21 branch
| 6707b61 | <span style="white-space:nowrap;">2021-11-29</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #30 from cortside/develop
| 02a1643 | <span style="white-space:nowrap;">2022-01-03</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add some helper methods for waiting while/until condition or timeout
| 774b0d8 | <span style="white-space:nowrap;">2022-01-06</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #31 from cortside/develop
| b9a930b | <span style="white-space:nowrap;">2022-01-06</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  site source
| 28a44e6 | <span style="white-space:nowrap;">2022-01-06</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  remove commented out config
| 9a0040c | <span style="white-space:nowrap;">2022-01-26</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'develop' of github.com:cortside/cortside.common into develop
| 928e4f5 | <span style="white-space:nowrap;">2022-01-26</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  fork spring2.core.message
| 7a03fee | <span style="white-space:nowrap;">2022-01-26</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  merge in changes
| e888e85 | <span style="white-space:nowrap;">2022-02-09</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add tests for message filter
| 8aeaa6d | <span style="white-space:nowrap;">2022-02-09</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add tests for message filter
| a2e529b | <span style="white-space:nowrap;">2022-03-09</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add aes based encryption class that handles serialized objects and strings
| 70e29aa | <span style="white-space:nowrap;">2022-03-10</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  break Guard into new project and add some extensibility; cleanup
| d493d27 | <span style="white-space:nowrap;">2022-03-10</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add Null<T> guard
| 46a979c | <span style="white-space:nowrap;">2022-03-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #32 from cortside/develop
| 0849aa8 | <span style="white-space:nowrap;">2022-03-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add contract resolver to order properties (thanks braden)
| 94eb330 | <span style="white-space:nowrap;">2022-03-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'develop' of github.com:cortside/cortside.common into develop
| ae47998 | <span style="white-space:nowrap;">2022-04-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  set correlationId on response as header X-Correlation-Id to make log searching easier
| c9d5d74 | <span style="white-space:nowrap;">2022-04-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #33 from cortside/develop
| 092f24e | <span style="white-space:nowrap;">2022-05-05</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add notes
| 98c087f | <span style="white-space:nowrap;">2022-05-05</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'develop' of github.com:cortside/cortside.common into develop
| ebcad2f | <span style="white-space:nowrap;">2022-05-09</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add ToJson() extension method
| bdbcef3 | <span style="white-space:nowrap;">2022-06-27</span> | <span style="white-space:nowrap;">Ruifang Li</span> |  Update MessageExceptionResponseFilter.cs
| 43fafa4 | <span style="white-space:nowrap;">2022-06-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #34 from cortside/feature/PP-2463
| c65e6de | <span style="white-space:nowrap;">2022-06-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #35 from cortside/feature/PP-2463-m
| 828ce77 | <span style="white-space:nowrap;">2022-07-05</span> | <span style="white-space:nowrap;">Juan Gomez</span> |  Add forbidden exception
| b741358 | <span style="white-space:nowrap;">2022-07-05</span> | <span style="white-space:nowrap;">Juan Gabriel Gomez Vargas</span> |  Merge pull request #39 from cortside/feature/PP-2452-m
| 6fa0468 | <span style="white-space:nowrap;">2022-07-05</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ValidationListException] add new exception type and support expectations set in rest guidelines
| 9ec454c | <span style="white-space:nowrap;">2022-07-05</span> | <span style="white-space:nowrap;">Juan Gabriel Gomez Vargas</span> |  Merge pull request #40 from cortside/master
| 9c22816 | <span style="white-space:nowrap;">2022-07-05</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ValidationListException] add test with fields to assert json rendered version
| 9b16a03 | <span style="white-space:nowrap;">2022-07-05</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ValidationListException] remove nesting and description
| bf82d39 | <span style="white-space:nowrap;">2022-07-05</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'master' into ValidationListException
| 8665692 | <span style="white-space:nowrap;">2022-07-05</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ValidationListException] adding result that reshapes model state validation into common error response
| 9d70be9 | <span style="white-space:nowrap;">2022-07-05</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ValidationListException] adding result that reshapes model state validation into common error response
| 0408986 | <span style="white-space:nowrap;">2022-07-05</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ValidationListException] update README with configuration example
| c48603c | <span style="white-space:nowrap;">2022-07-05</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ValidationListException] add filter for unhandled exception
| cdb6948 | <span style="white-space:nowrap;">2022-07-05</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ValidationListException] update README with configuration example
| 13dcd0d | <span style="white-space:nowrap;">2022-07-07</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ValidationListException] removed unneeded class
| c20a793 | <span style="white-space:nowrap;">2022-07-07</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #41 from cortside/ValidationListException
| f93b05b | <span style="white-space:nowrap;">2022-07-07</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update to latest newtonsoft library per dependabot recommendation
| 4f8f087 | <span style="white-space:nowrap;">2022-07-08</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update nuget packages
| 2a166ce | <span style="white-space:nowrap;">2022-07-08</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update nuget packages
| 0bea8c2 | <span style="white-space:nowrap;">2022-07-08</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #42 from cortside/develop
| e9c557a | <span style="white-space:nowrap;">2022-07-13</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [feature/BOT-20220713] updated nuget packages
| 0c6c582 | <span style="white-space:nowrap;">2022-07-13</span> | <span style="white-space:nowrap;">subjectivereality</span> |  add constructor to specify reason for invalidity
| ab855ee | <span style="white-space:nowrap;">2022-07-13</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #43 from cortside/AddReasonToInvalidValue
| cdb12b3 | <span style="white-space:nowrap;">2022-07-13</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #44 from cortside/feature/BOT-20220713
| d6446b5 | <span style="white-space:nowrap;">2022-07-14</span> | <span style="white-space:nowrap;">subjectivereality</span> |  Merge remote-tracking branch 'remotes/origin/master' into NoDefaultMessage
| b3b38a1 | <span style="white-space:nowrap;">2022-07-14</span> | <span style="white-space:nowrap;">subjectivereality</span> |  just use custom message
| 8d20ca9 | <span style="white-space:nowrap;">2022-07-14</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #45 from cortside/NoDefaultMessage
| 312603e | <span style="white-space:nowrap;">2022-07-15</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #46 from cortside/develop
| 445749c | <span style="white-space:nowrap;">2022-08-01</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [correlationtests] add tests to assert correlationId works; add async suffix to async methods
| c5c9aa6 | <span style="white-space:nowrap;">2022-08-01</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'develop' into correlationtests
| 6918714 | <span style="white-space:nowrap;">2022-08-01</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add sonar to build
| 3de31a2 | <span style="white-space:nowrap;">2022-08-01</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add sonar to build
| d599421 | <span style="white-space:nowrap;">2022-08-01</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'develop' into correlationtests
| 56a03aa | <span style="white-space:nowrap;">2022-08-01</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (origin/release/1.0, origin/master, master) Merge pull request #48 from cortside/develop
| cf68bbd | <span style="white-space:nowrap;">2022-08-01</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'develop' into correlationtests
| 72826d6 | <span style="white-space:nowrap;">2022-08-02</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #47 from cortside/correlationtests
| 74e47a7 | <span style="white-space:nowrap;">2022-12-21</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update nuget api key
| 9a25b58 | <span style="white-space:nowrap;">2022-12-29</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [feature/BOT-20221229] updated nuget packages
| 19ce7e8 | <span style="white-space:nowrap;">2022-12-29</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (origin/feature/BOT-20221229, feature/BOT-20221229) update build and helper scripts
| 3e71e1b | <span style="white-space:nowrap;">2022-12-29</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (HEAD -> develop, origin/develop, origin/HEAD) Merge pull request #49 from cortside/feature/BOT-20221229
****
