// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Linq;
using JetBrains.Annotations;
using Nuke.Core;
using Nuke.Core.Injection;

namespace Nuke.Common.ProjectModel
{
    /// <inheritdoc/>
    [PublicAPI]
    [UsedImplicitly(ImplicitUseKindFlags.Assign)]
    public class SolutionAttribute : StaticInjectionAttributeBase
    {
        public static Solution Value { get; private set; }

        [CanBeNull]
        public override object GetStaticValue ()
        {
            return Value = Value ?? Solution.Parse(NukeBuild.Instance.SolutionFile);
        }
    }
}
