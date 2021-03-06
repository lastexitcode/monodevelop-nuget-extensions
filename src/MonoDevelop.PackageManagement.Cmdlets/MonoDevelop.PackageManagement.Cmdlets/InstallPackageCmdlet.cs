﻿// 
// InstallPackageCmdlet.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2011-2014 Matthew Ward
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Management.Automation;
using ICSharpCode.PackageManagement.Scripting;
using MonoDevelop.PackageManagement;
using NuGet;

namespace ICSharpCode.PackageManagement.Cmdlets
{
	[Cmdlet (VerbsLifecycle.Install, "Package", DefaultParameterSetName = ParameterAttribute.AllParameterSets)]
	public class InstallPackageCmdlet : PackageManagementCmdlet
	{
		public InstallPackageCmdlet ()
			: this (
				PackageManagementExtendedServices.ConsoleHost,
				null)
		{
		}

		public InstallPackageCmdlet (
			IPackageManagementConsoleHost consoleHost,
			ICmdletTerminatingError terminatingError)
			: base (consoleHost, terminatingError)
		{
		}

		[Parameter (Position = 0, Mandatory = true)]
		public string Id { get; set; }

		[Parameter (Position = 1)]
		public string ProjectName { get; set; }

		[Parameter (Position = 2)]
		public SemanticVersion Version { get; set; }

		[Parameter (Position = 3)]
		public string Source { get; set; }

		[Parameter]
		public SwitchParameter IgnoreDependencies { get; set; }

		[Parameter, Alias ("Prerelease")]
		public SwitchParameter IncludePrerelease { get; set; }

		[Parameter]
		public FileConflictAction FileConflictAction { get; set; }

		[Parameter]
		public DependencyVersion? DependencyVersion { get; set; }

		protected override void ProcessRecord ()
		{
			ThrowErrorIfProjectNotOpen ();
			using (IConsoleHostFileConflictResolver resolver = CreateFileConflictResolver ()) {
				using (IDisposable monitor = CreateEventsMonitor ()) {
					InstallPackage ();
				}
			}
		}

		IConsoleHostFileConflictResolver CreateFileConflictResolver ()
		{
			return ConsoleHost.CreateFileConflictResolver (FileConflictAction);
		}

		void InstallPackage ()
		{
			IPackageManagementProject2 project = GetProject ();
			using (project.SourceRepository.StartInstallOperation (Id)) {
				InstallPackageAction2 action = CreateInstallPackageTask (project);
				ExecuteWithScriptRunner (project, () => {
					action.Execute ();
				});
			}
		}

		IPackageManagementProject2 GetProject ()
		{
			return ConsoleHost.GetProject (Source, ProjectName);
		}

		InstallPackageAction2 CreateInstallPackageTask (IPackageManagementProject2 project)
		{
			InstallPackageAction2 action = project.CreateInstallPackageAction ();
			action.PackageId = Id;
			action.PackageVersion = Version;
			action.IgnoreDependencies = IgnoreDependencies.IsPresent;
			action.AllowPrereleaseVersions = IncludePrerelease.IsPresent;
			if (DependencyVersion.HasValue) {
				action.DependencyVersion = DependencyVersion.Value;
			}
			return action;
		}
	}
}
