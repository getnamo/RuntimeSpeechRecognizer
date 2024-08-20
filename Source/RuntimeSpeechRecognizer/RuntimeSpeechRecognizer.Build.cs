// Georgy Treshchev 2024.

using UnrealBuildTool;
using System.IO;

public class RuntimeSpeechRecognizer : ModuleRules
{

	private string PluginLibPath
	{
		get { return Path.GetFullPath(Path.Combine(ModuleDirectory, "../../ThirdParty/LlamaCpp")); }
	}

	public RuntimeSpeechRecognizer(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;
		
		// Enable CPU instruction sets
#if UE_5_3_OR_LATER
		// Increase to AVX2 OR AVX512 for better performance (if your CPU supports it)
		MinCpuArchX64 = MinimumCpuArchitectureX64.AVX;
#else
		bUseAVX = true;
#endif

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

		if (Target.Platform == UnrealTargetPlatform.Win64)
		{
			//toggle this on for cuda build - copied from llama-unreal
			bool bUseCuda = true;
			if (bUseCuda)
			{
				PublicDelayLoadDLLs.Add("ggml.dll");
				PublicDelayLoadDLLs.Add("llama.dll");

				string WinLibDLLPath = Path.Combine(PluginLibPath, "Win64/Cuda");

				RuntimeDependencies.Add("$(BinaryOutputDir)/ggml.dll", Path.Combine(WinLibDLLPath, "ggml.dll"));
				RuntimeDependencies.Add("$(BinaryOutputDir)/llama.dll", Path.Combine(WinLibDLLPath, "llama.dll"));

				//NB: Creates cuda runtime .dll dependencies, proper import path not defined yet
				//These are usually found in NVIDIA GPU Computing Toolkit\CUDA\v12.2\lib\x64
				//PublicAdditionalLibraries.Add(Path.Combine(PluginLibPath, "Win64/Cuda", "cudart.lib"));
				//PublicAdditionalLibraries.Add(Path.Combine(PluginLibPath, "Win64/Cuda", "cublas.lib"));
				//PublicAdditionalLibraries.Add(Path.Combine(PluginLibPath, "Win64/Cuda", "cuda.lib"));

				PublicAdditionalLibraries.Add(Path.Combine(WinLibDLLPath, "llama.lib"));
				PublicAdditionalLibraries.Add(Path.Combine(WinLibDLLPath, "ggml.lib"));
			}
			else
			{
				//We do not use shared dll atm
				//PublicAdditionalLibraries.Add(Path.Combine(PluginLibPath, "Win64", "ggml_shared.lib"));

				PublicAdditionalLibraries.Add(Path.Combine(PluginLibPath, "Win64", "llama.lib"));
				PublicAdditionalLibraries.Add(Path.Combine(PluginLibPath, "Win64", "ggml_static.lib"));
			}
		}
	}
}