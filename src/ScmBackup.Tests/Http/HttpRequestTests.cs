﻿using RichardSzalay.MockHttp;
using ScmBackup.Http;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ScmBackup.Tests.Http
{
    public class HttpRequestTests
    {
        [Fact]
        public async Task RequestIsExecuted()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect("http://foo.com/bar")
                .WithHeaders(new Dictionary<string, string>
                {
                    { "h1", "v1" },
                    { "h2", "v2" }
                })
                .Respond(HttpStatusCode.OK, "application/json", "content");

            var logger = new FakeLogger();
            var sut = new HttpRequest(logger);
            sut.HttpClient = new HttpClient(mockHttp);


            sut.SetBaseUrl("http://foo.com");
            sut.AddHeader("h1", "v1");
            sut.AddHeader("h2", "v2");
            var result = await sut.Execute("bar");


            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.OK, result.Status);
            Assert.Equal("content", result.Content);
        }
        
        [Fact]
        public void AddBasicAuthHeader_AddsHeaderWithValues()
        {
            var logger = new FakeLogger();
            var sut = new HttpRequest(logger);
            sut.HttpClient = new HttpClient();

            sut.AddBasicAuthHeader("user", "pass");

            var authHeader = sut.HttpClient.DefaultRequestHeaders.Authorization;
            Assert.NotNull(authHeader);
            Assert.Equal("Basic", authHeader.Scheme);
            Assert.Equal("dXNlcjpwYXNz", authHeader.Parameter);
        }
    }
}
