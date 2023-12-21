using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Setup.Configuration;

namespace LuaInstaller.Core
{
	public class VisualStudioVersionsFromSetupApi : IVisualStudioVersionLocator
	{
		private static readonly Regex _rgx;

		static VisualStudioVersionsFromSetupApi()
        {
			_rgx = new Regex(@"^(\d+)\.(\d+)\.(\d+)\.(\d+)$");
		}

		private VisualStudioVersion ProcessSetupInstance(ISetupInstance2 setupInstance)
		{
			VisualStudioVersion result = null;

			ISetupPackageReference[] packageReferences = setupInstance.GetPackages();

			int len = packageReferences.Length;
			int i = 0;

			while (i < len)
			{
				ISetupPackageReference packageReference = packageReferences[i];
				string id = packageReference.GetId();

				if (id.Equals("Microsoft.VisualStudio.Component.VC.Tools.x86.x64", StringComparison.InvariantCultureIgnoreCase))
				{
					string vsDir = setupInstance.GetInstallationPath();
					
					if (vsDir != null)
					{
						string vcToolsVersionDefaultFile = Path.Combine(vsDir, "VC", "Auxiliary", "Build", "Microsoft.VCToolsVersion.default.txt");

						if (File.Exists(vcToolsVersionDefaultFile))
                        {
							string vcVersion = File.ReadAllText(vcToolsVersionDefaultFile).Trim();

                            Match match = _rgx.Match(setupInstance.GetInstallationVersion());

                            if (match.Success)
                            {
                                GroupCollection groups = match.Groups;

                                int major = int.Parse(groups[1].Value);
                                int minor = int.Parse(groups[2].Value);
                                int build = int.Parse(groups[3].Value);
								int revision = int.Parse(groups[4].Value);

								string vcDir = Path.Combine(vsDir, "VC", "Tools", "MSVC", vcVersion);

                                result = new VisualStudioVersion(
                                    major,
                                    minor,
                                    vsDir,
                                    vcDir,
                                    build,
									revision
								);
                            }

                            i = len;
                        }
					}
				}
				else
				{
					i++;
				}
			}
			
			return result;
		}

		public VisualStudioVersion[] GetVersions()
		{
			ICollection<VisualStudioVersion> versions = new List<VisualStudioVersion>();

			try
			{
				SetupConfiguration query = new SetupConfiguration();

				IEnumSetupInstances e = query.EnumAllInstances();
				int fetched;

				do
				{
					fetched = 0;
					ISetupInstance[] instances = new ISetupInstance[1];

					e.Next(1, instances, out fetched);
					if (fetched != 0)
					{
						VisualStudioVersion instanceVersion = ProcessSetupInstance((ISetupInstance2)instances[0]);

						if (instanceVersion != null)
						{
							versions.Add(instanceVersion);
						}
					}
				}
				while (fetched != 0);
			}
			catch
			{

			}

			VisualStudioVersion[] result = new VisualStudioVersion[versions.Count];
			versions.CopyTo(result, 0);

			return result;
		}
	}
}
