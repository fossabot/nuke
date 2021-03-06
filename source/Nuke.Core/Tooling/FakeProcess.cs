// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Nuke.Core.Tooling
{
    internal class FakeProcess : IProcess
    {
        public void Dispose ()
        {
        }

        public ProcessStartInfo StartInfo => throw new NotSupportedException();

        public IEnumerable<Output> Output => Enumerable.Empty<Output>();

        public int ExitCode => 0;

        public void Kill ()
        {
        }

        public bool WaitForExit ()
        {
            return true;
        }
    }
}
