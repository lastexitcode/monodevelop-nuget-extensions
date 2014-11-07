//
// ScriptCsHost.cs
//
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2014 Matthew Ward
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
using NuGet;
using ScriptCs;
using ScriptCs.Contracts;
using ScriptCs.Engine.Mono;
using ScriptCs.Hosting;

namespace MonoDevelop.PackageManagement
{
	public class ScriptCsHost
	{
		ScriptServicesBuilder builder;
		ILogger logger;
		NuGetScriptPack scriptPack;

		public ScriptCsHost (ILogger logger)
		{
			this.logger = logger;
			scriptPack = new NuGetScriptPack (logger);
		}

		public void AddVariable (string name, object value)
		{
			scriptPack.AddVariable (name, value);
		}

		public void RemoveVariable (string name)
		{
			scriptPack.RemoveVariable (name);
		}

		public void InvokeScript (string fileName)
		{
			Init ();
			RunScript (fileName);
		}

		void Init ()
		{
			if (builder != null) {
				return;
			}

			var console = new ScriptConsole ();

			var log = new ScriptCsLogger (logger);
			var consoleLogger = new ScriptConsoleLogger (LogLevel.Debug, console, log);
			builder = new ScriptServicesBuilder (console, consoleLogger, null);
			builder
				.LogLevel (LogLevel.Debug)
				.Repl (false)
				.Cache (false);
		}

		void RunScript (string fileName)
		{
			try {
				builder.ScriptEngine<MonoScriptEngine> ();
				ScriptServices services = builder.Build ();
				services.Executor.AddReferences (typeof(NuGetScriptPackContext));

				services.Executor.Initialize (new string[0], new [] { scriptPack });

				ScriptResult result = services.Executor.Execute (fileName);
				if (result.CompileExceptionInfo != null) {
					LogError (result.CompileExceptionInfo.SourceException);
				}
				if (result.ExecuteExceptionInfo != null) {
					LogError (result.ExecuteExceptionInfo.SourceException);
				}
			} catch (Exception ex) {
				LogError (ex);
			}
		}

		void LogError (Exception ex)
		{
			logger.Log (MessageLevel.Error, ex.ToString ());
		}
	}
}
