// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Nuke.Common.ProjectModel;
using Nuke.Core;
using Nuke.Core.IO;
using Xunit;

namespace Nuke.Common.Tests
{
    public class ProjectModelTest
    {
        [Fact]
        public void Test()
        {
            //var currentDirectory = (PathConstruction.AbsolutePath) Directory.GetCurrentDirectory ();
            //var solutionFile = currentDirectory / ".." / ".." / ".." / ".." / ".." / "Nuke.sln";

            //var solution = Solution.Parse(solutionFile);

            //solution.Projects.Where(x => x.IsSolutionFolder()).Select(x => x.Name)
            //        .Should().BeEquivalentTo("bootstrapping", "misc");

            //solution.Projects.Where(x => x.IsCSharpProject()).Should().HaveCount(6);

            EnvironmentInfo.SetVariable("MSBuildSDKsPath", @"C:\Program Files\dotnet\sdk\2.0.0\Sdks");
            EnvironmentInfo.SetVariable ("VSINSTALLDIR", @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional");
            //EnvironmentInfo.SetVariable("VisualStudioVersion", @"15.0");
            var project = new Microsoft.Build.Evaluation.Project(@"C:\code\nuke-build\nuke\source\Nuke.Core\Nuke.Core.csproj");
            System.Diagnostics.Debugger.Launch();
        }
    }
}
