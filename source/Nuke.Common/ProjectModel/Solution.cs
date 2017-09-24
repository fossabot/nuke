// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Nuke.Core.Utilities;

namespace Nuke.Common.ProjectModel
{
    public class Solution
    {
        public static Solution Parse (string path)
        {
            string GetGuid (string text) =>
                    $@"\{{(?<{Regex.Escape(text)}>[0-9a-fA-F]{{8}}-[0-9a-fA-F]{{4}}-[0-9a-fA-F]{{4}}-[0-9a-fA-F]{{4}}-[0-9a-fA-F]{{12}})\}}";

            string quotedGuidExpr (string name) =>
                    $@"""{GetGuid(name)}""";

            string quotedTextExpr (string name) =>
                    $@"""(?<{Regex.Escape(name)}>[^""]*)""";

            var lines = File.ReadAllLines(path);

            var childToParentMap = lines
                    .SkipWhile(line => !Regex.IsMatch(line, @"^\s*GlobalSection\(NestedProjects\) = preSolution$"))
                    .Skip(count: 1)
                    .TakeWhile(line => !Regex.IsMatch(line, @"^\s*EndGlobalSection$"))
                    .Select(line => Regex.Match(line, $@"^\s*{GetGuid("child")}\s*=\s*{GetGuid("parent")}$"))
                    .ToDictionary(m => Guid.Parse(m.Groups["child"].Value), m => Guid.Parse(m.Groups["parent"].Value));

            var projects = lines
                    .Select(line => Regex.Match(line,
                        $@"^Project\({quotedGuidExpr("typeId")}\)\s*=\s*{quotedTextExpr("name")},\s*{quotedTextExpr("path")},\s*{
                                    quotedGuidExpr("id")
                                }$"))
                    .Where(m => m.Success)
                    .Select(m =>
                    {
                        var id = Guid.Parse(m.Groups["id"].Value);
                        return new Project(
                            id,
                            m.Groups["name"].Value,
                            Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), m.Groups["path"].Value)),
                            Guid.Parse(m.Groups["typeId"].Value),
                            childToParentMap.TryGetValue(id, out var parentId) ? (Guid?) parentId : null);
                    })
                    .Where(p => p.Name != ".build");
            return new Solution(projects);
        }


        public IReadOnlyCollection<Project> Projects { get; }

        public Solution (IEnumerable<Project> projects)
        {
            Projects = projects.ToList();
        }
    }

    [DebuggerDisplay("{" + nameof(Path) + "}")]
    public class Project
    {
        public Project (
            Guid id,
            string name,
            string path,
            Guid typeId,
            Guid? parentId)
        {
            Id = id;
            Name = name;
            Path = path;
            Directory = System.IO.Path.GetDirectoryName(Path);
            TypeId = typeId;
            ParentId = parentId;
        }

        public Guid Id { get; }
        public string Name { get; }
        public string Path { get; }
        public string Directory { get; }
        public Guid TypeId { get; }
        public Guid? ParentId { get; }

        public IEnumerable<Project> GetParents (IEnumerable<Project> projects)
        {
            var idToProjectMap = projects
                    .ToDictionary(p => p.Id, p => p);
            var parentId = ParentId;
            while (parentId != null)
            {
                var parent = idToProjectMap[parentId.Value];
                yield return parent;
                parentId = parent.ParentId;
            }
        }

        public IEnumerable<Project> GetParentsAndSelf (IEnumerable<Project> projects)
        {
            yield return this;
            foreach (var parent in GetParents(projects))
                yield return parent;
        }

        public string GetFullProjectName (IEnumerable<Project> projects)
        {
            return GetParentsAndSelf(projects)
                    .Select(p => p.Name.Replace(".", "_"))
                    .Reverse()
                    .Join("\\");
        }
    }
}
