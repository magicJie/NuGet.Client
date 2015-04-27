﻿using NuGet.Versioning;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace NuGet.RuntimeModel.Test
{
    public class JsonRuntimeFormatTests
    {
        [Theory]
        [InlineData("{}")]
        [InlineData("{\"runtimes\":{}}")]
        public void CanParseEmptyRuntimeJsons(string content)
        {
            Assert.Equal(new RuntimeGraph(), ParseRuntimeJsonString(content));
        }

        [Fact]
        public void CanParseSimpleRuntimeJson()
        {
            const string content = @"
{
    ""runtimes"": {
        ""any"": {},
        ""win8-x86"": {
            ""#import"": [
                ""win8"",
                ""win7-x86""
            ],
            ""Some.Package"": {
                ""Some.Package.For.win8-x86"": ""4.2""
            }
        },
        ""win8"": {
            ""#import"": [
                ""win7""
            ]
        }
    }
}";

            Assert.Equal(
                new RuntimeGraph(new []
                {
                    new RuntimeDescription("any"),
                    new RuntimeDescription("win8-x86", new[]
                        {
                            "win8",
                            "win7-x86"
                        }, new[] {
                            new RuntimeDependencySet("Some.Package", new [] {
                                new RuntimePackageDependency("Some.Package.For.win8-x86", new VersionRange(new NuGetVersion("4.2")))
                            })
                        }),
                    new RuntimeDescription("win8", new[] { "win7" })
                }), ParseRuntimeJsonString(content));
        }

        private RuntimeGraph ParseRuntimeJsonString(string content)
        {
            using (var reader = new StringReader(content))
            {
                return JsonRuntimeFormat.ReadRuntimeGraph(reader);
            }
        }
    }
}
