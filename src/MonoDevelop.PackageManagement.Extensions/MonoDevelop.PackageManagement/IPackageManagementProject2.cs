﻿// 
// IPackageManagementProject.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2012-2013 Matthew Ward
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
using System.Collections.Generic;
using System.Linq;

using NuGet;
using System.Runtime.Versioning;
using MonoDevelop.Projects;

namespace ICSharpCode.PackageManagement
{
	public interface IPackageManagementProject2
	{
		event EventHandler<PackageOperationEventArgs> PackageInstalled;
		event EventHandler<PackageOperationEventArgs> PackageUninstalled;
		event EventHandler<PackageOperationEventArgs> PackageReferenceAdded;
		event EventHandler<PackageOperationEventArgs> PackageReferenceRemoved;

		string Name { get; }
		ILogger Logger { get; set; }
		IPackageRepository SourceRepository { get; }
		FrameworkName TargetFramework { get; }

		DotNetProject DotNetProject { get; }

		IPackageConstraintProvider ConstraintProvider { get; }

		bool IsPackageInstalled(IPackage package);
		bool IsPackageInstalled(string packageId);
		bool HasOlderPackageInstalled(IPackage package);

		IQueryable<IPackage> GetPackages();
		IEnumerable<IPackage> GetPackagesInReverseDependencyOrder();
		IPackage FindPackage(string packageId);

		IEnumerable<PackageOperation> GetInstallPackageOperations(IPackage package, InstallPackageAction2 installAction);
		IEnumerable<PackageOperation> GetUpdatePackagesOperations(IEnumerable<IPackage> packages, IUpdatePackageSettings settings);

		void InstallPackage(IPackage package, InstallPackageAction2 installAction);
		void UpdatePackage(IPackage package, UpdatePackageAction2 updateAction);
		void UninstallPackage(IPackage package, UninstallPackageAction2 uninstallAction);
		void UpdatePackages(UpdatePackagesAction2 action);

		void UpdatePackageReference(IPackage package, IUpdatePackageSettings settings);

		InstallPackageAction2 CreateInstallPackageAction();
		UninstallPackageAction2 CreateUninstallPackageAction();
		UpdatePackageAction2 CreateUpdatePackageAction();
		UpdatePackagesAction2 CreateUpdatePackagesAction();
		ReinstallPackageAction2 CreateReinstallPackageAction();

		void RunPackageOperations(IEnumerable<PackageOperation> expectedOperations);

		IPackage FindPackage (string packageId, SemanticVersion version);
	}
}