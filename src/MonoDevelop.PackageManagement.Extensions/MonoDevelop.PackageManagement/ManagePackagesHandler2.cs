﻿// 
// ManagePackagesHandler.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2013 Matthew Ward
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
using ICSharpCode.PackageManagement;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.PackageManagement.Commands;

namespace MonoDevelop.PackageManagement
{
	public class ManagePackagesHandler2 : PackagesCommandHandler
	{
		protected override void Run ()
		{
			try {
				ManagePackagesViewModel2 viewModel = CreateViewModel ();
				IPackageManagementEvents packageEvents = PackageManagementServices.PackageManagementEvents;
				var dialog = new ManagePackagesDialog2 (viewModel, packageEvents);
				MessageService.ShowCustomDialog (dialog);
			} catch (Exception ex) {
				MessageService.ShowException (ex);
			}
		}

		ManagePackagesViewModel2 CreateViewModel ()
		{
			var packageEvents = new ThreadSafePackageManagementEvents (PackageManagementServices.PackageManagementEvents);
			var viewModels = new PackagesViewModels2 (
				PackageManagementServices.Solution,
				PackageManagementServices.RegisteredPackageRepositories,
				packageEvents,
				PackageManagementServices.PackageActionRunner,
				new PackageManagementTaskFactory());

			return new ManagePackagesViewModel2 (
				viewModels,
				new ManagePackagesViewTitle (PackageManagementServices.Solution),
				packageEvents);
		}

		protected override void Update (CommandInfo info)
		{
			info.Enabled = PackageManagementServices.Solution.IsOpen && !IsDotNetProjectSelected ();
		}
	}
}
