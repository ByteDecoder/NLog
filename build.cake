//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var outputDirectory = Argument<string>("outputDirectory", "./artifacts");
var samplesDirectory = Argument<string>("samplesDirectory", "./samples");
var dnxVersion = Argument<string>("dnxVersion", "1.0.0-rc1-update1");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory(outputDirectory);
var samplesDir = Directory(samplesDirectory);

// Define runtime
DNRuntime runtime = DNRuntime.Clr;
if(IsRunningOnUnix())
{
    runtime = DNRuntime.Mono;
}

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
	CleanDirectory(samplesDir);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("uap10");

Task("uap10")
	.ContinueOnError()
	.IsDependentOn("sl5")
    .WithCriteria(IsRunningOnWindows())
	.Does(() =>
{
    // Restore
    DNURestoreSettings restoreSettings = new DNURestoreSettings()
    {
        Architecture = DNArchitecture.X64,
        Runtime = runtime,
        Version = dnxVersion
    };
	DNURestore(restoreSettings);

	// Build
	DNUBuildSettings dnuBuildSettings = new DNUBuildSettings
	{
        Architecture = DNArchitecture.X64,
        Runtime = runtime,
        Version = dnxVersion,
	    Frameworks = new [] { "uap10" },
	    Configurations = new[] { configuration },
	    OutputDirectory = buildDir,
	    Quiet = false
	};
        
    DNUBuild("./src/NLog", dnuBuildSettings);
    DNUBuild("./src/NLogAutoLoadExtension", dnuBuildSettings);
	DNUBuild("./tests/SampleExtensions", dnuBuildSettings);
	
    dnuBuildSettings = new DNUBuildSettings
	{
        Architecture = DNArchitecture.X64,
        Runtime = runtime,
        Version = dnxVersion,
	    Frameworks = new [] { "uap10" },
	    Configurations = new[] { configuration },
	    OutputDirectory = buildDir,
	    Quiet = true
	};
    DNUBuild("./tests/NLog.UnitTests", dnuBuildSettings);

	// Test
	var settings = new DNXRunSettings
	{ 
        Architecture = DNArchitecture.X64,
        Runtime = runtime,
        Version = dnxVersion	
    };
	DNXRun("./tests/NLog.UnitTests/", "test", settings);

});
   
Task("sl5")
	.ContinueOnError()
	.IsDependentOn("net35")
    .WithCriteria(IsRunningOnWindows())
	.Does(() =>
{
    // Restore
    DNURestoreSettings restoreSettings = new DNURestoreSettings()
    {
        Architecture = DNArchitecture.X64,
        Runtime = runtime,
        Version = dnxVersion
    };
	DNURestore(restoreSettings);

	// Build
	DNUBuildSettings dnuBuildSettings = new DNUBuildSettings
	{
        Architecture = DNArchitecture.X64,
        Runtime = runtime,
        Version = dnxVersion,
	    Frameworks = new [] { "sl5" },
	    Configurations = new[] { configuration },
	    OutputDirectory = buildDir,
	    Quiet = false
	};
        
    DNUBuild("./src/NLog", dnuBuildSettings);
    DNUBuild("./src/NLogAutoLoadExtension", dnuBuildSettings);
	DNUBuild("./tests/SampleExtensions", dnuBuildSettings);
	
    dnuBuildSettings = new DNUBuildSettings
	{
        Architecture = DNArchitecture.X64,
        Runtime = runtime,
        Version = dnxVersion,
	    Frameworks = new [] { "sl5" },
	    Configurations = new[] { configuration },
	    OutputDirectory = buildDir,
	    Quiet = true
	};
    DNUBuild("./tests/NLog.UnitTests", dnuBuildSettings);

	// Test
	var settings = new DNXRunSettings
	{ 
        Architecture = DNArchitecture.X64,
        Runtime = runtime,
        Version = dnxVersion	
    };
	DNXRun("./tests/NLog.UnitTests/", "test", settings);

});

Task("net35")
	.ContinueOnError()
	.IsDependentOn("Dnx451")
    .WithCriteria(IsRunningOnWindows())
	.Does(() =>
{
    // Restore
    DNURestoreSettings restoreSettings = new DNURestoreSettings()
    {
        Architecture = DNArchitecture.X64,
        Runtime = runtime,
        Version = dnxVersion
    };
	DNURestore(restoreSettings);

	// Build
	DNUBuildSettings dnuBuildSettings = new DNUBuildSettings
	{
        Architecture = DNArchitecture.X64,
        Runtime = runtime,
        Version = dnxVersion,
	    Frameworks = new [] { "net35" },
	    Configurations = new[] { configuration },
	    OutputDirectory = buildDir,
	    Quiet = false
	};
        
    DNUBuild("./src/NLog", dnuBuildSettings);
    DNUBuild("./src/NLogAutoLoadExtension", dnuBuildSettings);
	DNUBuild("./tests/SampleExtensions", dnuBuildSettings);
	
    dnuBuildSettings = new DNUBuildSettings
	{
        Architecture = DNArchitecture.X64,
        Runtime = runtime,
        Version = dnxVersion,
	    Frameworks = new [] { "dnx451" },
	    Configurations = new[] { configuration },
	    OutputDirectory = (buildDir + Directory("net35")).ToString(),
	    Quiet = true
	};
    DNUBuild("./tests/NLog.UnitTests", dnuBuildSettings);

	// Test
	var settings = new DNXRunSettings
	{ 
        Architecture = DNArchitecture.X64,
        Runtime = runtime,
        Version = dnxVersion	
    };
	DNXRun("./tests/NLog.UnitTests/", "test", settings);

});

Task("Dnx451")
	.ContinueOnError()
	.IsDependentOn("Dnxcore50")
	.Does(() =>
{
	
	// Restore
    DNURestoreSettings restoreSettings = new DNURestoreSettings()
    {
        Architecture = DNArchitecture.X64,
        Runtime = runtime,
        Version = dnxVersion
    };
	DNURestore(restoreSettings);

	// Build
	DNUBuildSettings dnuBuildSettings = new DNUBuildSettings
	{
        Architecture = DNArchitecture.X64,
        Runtime = runtime,
        Version = dnxVersion,
	    Frameworks = new [] { "dnx451" },
	    Configurations = new[] { configuration },
	    OutputDirectory = buildDir,
	    Quiet = true
	};
        
    DNUBuild("./src/NLog", dnuBuildSettings);
	DNUBuild("./src/NLog.Extended", dnuBuildSettings);
    DNUBuild("./src/NLogAutoLoadExtension", dnuBuildSettings);
	DNUBuild("./tests/SampleExtensions", dnuBuildSettings);
	DNUBuild("./tests/NLog.UnitTests", dnuBuildSettings);

	// Test
	var settings = new DNXRunSettings
	{	
        Architecture = DNArchitecture.X64,
        Runtime = runtime,
        Version = dnxVersion
    };
	DNXRun("./tests/NLog.UnitTests/", "test", settings);

});

Task("Dnxcore50")
    .ContinueOnError()
	.IsDependentOn("Clean")
	.Does(() =>
{
	// Restore
    DNURestoreSettings restoreSettings = new DNURestoreSettings()
    {
        Architecture = DNArchitecture.X64,
        Runtime = DNRuntime.CoreClr,
        Version = dnxVersion
    };
	DNURestore("./src/NLog/project.json", restoreSettings);
	
	// Build
	DNUBuildSettings dnuBuildSettings = new DNUBuildSettings
	{
        Architecture = DNArchitecture.X64,
        Runtime = DNRuntime.CoreClr,
        Version = dnxVersion,
	    Frameworks = new [] { "dnxcore50" },
	    Configurations = new[] { configuration },
	    OutputDirectory = buildDir,
	    Quiet = true
	};
        
    DNUBuild("./src/NLog", dnuBuildSettings);
	
	DNURestore("./test/SampleExtensions/project.json", restoreSettings);
	DNUBuild("./tests/SampleExtensions", dnuBuildSettings);
	DNUBuild("./tests/NLog.UnitTests", dnuBuildSettings);

	// Test
	var settings = new DNXRunSettings
	{	
        Architecture = DNArchitecture.X64,
        Runtime = DNRuntime.CoreClr,
        Version = dnxVersion
    };
	DNXRun("./tests/NLog.UnitTests/", "test", settings);

});
//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
