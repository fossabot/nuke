﻿// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;
using static Nuke.Common.IO.SerializationTasks;

namespace Nuke.Common.Tests
{
    public class SerializationTest
    {
        [Theory]
        [MemberData(nameof(Serializers))]
        public void Test(string name, Func<Data, Data> serializationChain)
        {
            var obj = new Data
                      {
                          String = "mytext",
                          Number = 5,
                          Boolean = true,
                          Nested = new Data
                                   {
                                       Boolean = false
                                   }
                      };

            var obj2 = serializationChain(obj);

            obj2.String.Should().Be(obj.String);
            obj2.Number.Should().Be(obj.Number);
            obj2.Boolean.Should().Be(obj.Boolean);
            obj2.Nested.Should().NotBeNull();
            obj2.Nested.Boolean.Should().Be(obj.Nested.Boolean);
            obj2.Nested.String.Should().Be(obj.Nested.String);
        }

        public class Data
        {
            public string String { get; set; }
            public int Number { get; set; }
            public bool Boolean { get; set; }

            public Data Nested { get; set; }
        }

        public static IEnumerable<object[]> Serializers
        {
            [UsedImplicitly]
            get
            {
                object[] GetSerialization (string name, Func<Data, Data> serialization) => new object[] { name, serialization };

                yield return GetSerialization("Json", x => JsonDeserialize<Data>(JsonSerialize(x)));
                yield return GetSerialization ("Yaml", x => YamlDeserialize<Data> (YamlSerialize (x)));
                //yield return GetSerialization(x => XmlSerialize(x), x => XmlDeserialize<Data>(x));
            }
        }
    }
}
