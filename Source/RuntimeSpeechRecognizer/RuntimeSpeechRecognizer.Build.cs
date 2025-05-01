// Georgy Treshchev 2024.

using UnrealBuildTool;
using System.IO;

public class RuntimeSpeechRecognizer : ModuleRules
{
	private string WhisperLibPath
	{
		get { return Path.GetFullPath(Path.Combine(ModuleDirectory, "../ThirdParty/whisper.cpp/Lib/Win64/")); }
	}

	public RuntimeSpeechRecognizer(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;
		
		// Enable CPU instruction sets
		MinCpuArchX64 = MinimumCpuArchitectureX64.Default;

		PrivateDependencyModuleNames.AddRange(
			new string[]
			{
				"CoreUObject",
				"Engine",
				"Core",
				"SignalProcessing",
				"AudioPlatformConfiguration"
			}
		);

		if (Target.Type == TargetType.Editor)
		{
			PrivateDependencyModuleNames.AddRange(
				new string[]
				{
					"UnrealEd",
					"Slate",
					"SlateCore"
				});

			if (Target.Version.MajorVersion >= 5 && Target.Version.MinorVersion >= 0)
			{
				PrivateDependencyModuleNames.AddRange(
					new string[]
					{
						"DeveloperToolSettings"
					}
				);
			}
		}

		PrivateIncludePaths.Add(Path.Combine(ModuleDirectory, "..", "ThirdParty", "whisper.cpp"));


		PublicAdditionalLibraries.Add(Path.Combine(WhisperLibPath, "ggml.lib"));
		PublicAdditionalLibraries.Add(Path.Combine(WhisperLibPath, "ggml-base.lib"));
		PublicAdditionalLibraries.Add(Path.Combine(WhisperLibPath, "ggml-cpu.lib"));
		PublicAdditionalLibraries.Add(Path.Combine(WhisperLibPath, "ggml-vulkan.lib"));

	}
}