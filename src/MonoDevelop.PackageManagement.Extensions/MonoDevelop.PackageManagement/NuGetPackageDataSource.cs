﻿//
// NuGetPackageDataSource.cs
//
// Author:
//       Matt Ward <matt.ward@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.PackageManagement;
using MonoDevelop.Components.MainToolbar;
using MonoDevelop.Core;
using MonoDevelop.Ide.CodeCompletion;
using Xwt.Drawing;

namespace MonoDevelop.PackageManagement
{
	public class NuGetPackageDataSource : ISearchDataSource
	{
		readonly SearchPopupSearchPattern searchPattern;
		readonly PackageSearchCommand command;

		public NuGetPackageDataSource (SearchPopupSearchPattern searchPattern)
		{
			this.searchPattern = searchPattern;
			command = new PackageSearchCommand (searchPattern.Pattern);
		}

		Image ISearchDataSource.GetIcon (int item)
		{
			return null;
		}

		string ISearchDataSource.GetMarkup (int item, bool isSelected)
		{
			return GettextCatalog.GetString (
				"Add Package <b>{0}</b>{1}",
				command.PackageId, GetPackageVersionMarkup ());
		}

		string GetPackageVersionMarkup ()
		{
			if (command.HasVersion ()) {
				return " <b>" + command.Version + "</b>";
			}
			return String.Empty;
		}

		string ISearchDataSource.GetDescriptionMarkup (int item, bool isSelected)
		{
			return null;
		}

		TooltipInformation ISearchDataSource.GetTooltip (int item)
		{
			return null;
		}

		double ISearchDataSource.GetWeight (int item)
		{
			return 0;
		}

		DomRegion ISearchDataSource.GetRegion (int item)
		{
			return DomRegion.Empty;
		}

		bool ISearchDataSource.CanActivate (int item)
		{
			return IsProjectSelected ();
		}

		bool IsProjectSelected ()
		{
			try {
				return PackageManagementExtendedServices.ProjectService.CurrentProject != null;
			} catch (Exception ex) {
				LoggingService.LogError ("Error getting current project.", ex);
			}
			return false;
		}

		void ISearchDataSource.Activate (int item)
		{
			var runner = new PackageInstaller ();
			runner.Run (command);
		}

		int ISearchDataSource.ItemCount {
			get {
				if (IsProjectSelected () && command.IsValid) {
					return 1;
				}
				return 0;
			}
		}
	}
}



