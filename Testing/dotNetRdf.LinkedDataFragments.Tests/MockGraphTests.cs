﻿/*
dotNetRDF is free and open source software licensed under the MIT License

-----------------------------------------------------------------------------

Copyright (c) 2009-2012 dotNetRDF Project (dotnetrdf-developer@lists.sf.net)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is furnished
to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.IO;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;
using Xunit.Abstractions;

namespace VDS.RDF.LDF
{
    [Collection("QpfServer")]
    public class MockGraphTests : GraphTests
    {
        private readonly MockQpfServer qpfServer;

        public MockGraphTests(MockQpfServer qpfServer, ITestOutputHelper output) : base(output) => this.qpfServer = qpfServer;

        protected override LdfGraph Graph => new(qpfServer.BaseUri + "2016-04/en");
    }

    [CollectionDefinition("QpfServer")]
    public class QpfServerCollection : ICollectionFixture<MockQpfServer> { }

    public class MockQpfServer : IDisposable
    {
        private readonly WireMockServer Server;

        public MockQpfServer()
        {
            Server = WireMockServer.Start();

            var file = (string q) => Response.Create()
                .WithHeader("Content-Type", "application/ld+json")
                .WithTransformer(true)
                .WithBodyFromFile(Path.Combine("resources", $"{q}.jsonld"));

            var root = () => Request.Create().WithPath("/2016-04/en");
            Server.Given(root()).RespondWith(file("root"));
            Server.Given(root().WithParam("page")).RespondWith(file("root{{request.query.page}}"));

            var containsTriple = root().WithParam("subject", "http://dbpedia.org/ontology/extinctionDate").WithParam("predicate", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type").WithParam("object", "http://www.w3.org/1999/02/22-rdf-syntax-ns#Property");
            Server.Given(containsTriple).RespondWith(file("containsTriple"));

            var triplesWithObject = () => root().WithParam("object", "\"1997-02-04\"^^http://www.w3.org/2001/XMLSchema#date");
            Server.Given(triplesWithObject()).RespondWith(file("getTriplesWithObject"));
            Server.Given(triplesWithObject().WithParam("page")).RespondWith(file("getTriplesWithObject{{request.query.page}}"));

            var triplesWithPredicate = () => root().WithParam("predicate", "http://dbpedia.org/ontology/extinctionDate");
            Server.Given(triplesWithPredicate()).RespondWith(file("getTriplesWithPredicate"));
            Server.Given(triplesWithPredicate().WithParam("page")).RespondWith(file("getTriplesWithPredicate{{request.query.page}}"));

            var triplesWithPredicateObject = () => root().WithParam("predicate", "http://dbpedia.org/ontology/extinctionDate").WithParam("object", "\"2011-10-05\"^^http://www.w3.org/2001/XMLSchema#date");
            Server.Given(triplesWithPredicateObject()).RespondWith(file("getTriplesWithPredicateObject"));

            var triplesWithSubject = () => root().WithParam("subject", "http://0-access.newspaperarchive.com.topcat.switchinc.org/Viewer.aspx?img=7578853");
            Server.Given(triplesWithSubject()).RespondWith(file("getTriplesWithSubject"));

            var triplesWithSubjectObject = () => root().WithParam("subject", "http://dbpedia.org/resource/123_Democratic_Alliance").WithParam("object", "\"707366241\"^^http://www.w3.org/2001/XMLSchema#integer");
            Server.Given(triplesWithSubjectObject()).RespondWith(file("getTriplesWithSubjectObject"));

            var triplesWithSubjectPredicate = () => root().WithParam("subject", "http://dbpedia.org/resource/123_Democratic_Alliance").WithParam("predicate", "http://dbpedia.org/ontology/extinctionDate");
            Server.Given(triplesWithSubjectPredicate()).RespondWith(file("getTriplesWithSubjectPredicate"));
        }

        public Uri BaseUri => new(Server.Url);

        void IDisposable.Dispose() => Server.Stop();
    }
}
