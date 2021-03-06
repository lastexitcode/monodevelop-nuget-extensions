﻿// 
// UninstallPackageCmdlet.cs
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
	[Cmdlet (VerbsLifecycle.Uninstall, "Package", DefaultParameterSetName = ParameterAttribute.AllParameterSets)]
	public class UninstallPackageCmdlet : PackageManagementCmdlet
	{
		public UninstallPackageCmdlet ()
			: this (
				PackageManagementExtendedServices.ConsoleHost,
				null)
		{
		}

		public UninstallPackageCmdlet (
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

		[Parameter]
		public SwitchParameter Force { get; set; }

		[Parameter]
		public SwitchParameter RemoveDependencies { get; set; }

		protected override void ProcessRecord ()
		{
			ThrowErrorIfProjectNotOpen ();
			using (IDisposable monitor = CreateEventsMonitor ()) {
				UninstallPackage ();
			}
		}

		void UninstallPackage ()
		{
			ExtendedPackageManagementProject project = GetProject ();
			UninstallPackageAction2 action = CreateUninstallPackageAction (project);
			ExecuteWithScriptRunner (project, () => {
				action.Execute ();
			});
		}

		ExtendedPackageManagementProject GetProject ()
		{
			string source = null; 
			return (ExtendedPackageManagementProject)ConsoleHost.GetProject (source, ProjectName);
		}

		UninstallPackageAction2 CreateUninstallPackageAction (ExtendedPackageManagementProject project)
		{
			UninstallPackageAction2 action = project.CreateUninstallPackageAction ();
			action.PackageId = Id;
			action.PackageVersion = Version;
			action.ForceRemove = Force.IsPresent;
			action.RemoveDependencies = RemoveDependencies.IsPresent;
//			action.PackageScriptRunner = this;

			return action;
		}
	}
}
