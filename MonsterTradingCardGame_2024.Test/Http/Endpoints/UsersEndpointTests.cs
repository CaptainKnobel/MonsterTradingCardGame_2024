using System;
using System.Collections.Generic;
using System.IO;
using MonsterTradingCardGame_2024.Business_Logic;
using MonsterTradingCardGame_2024.Http;
using MonsterTradingCardGame_2024.Http.Endpoints;
using MonsterTradingCardGame_2024.Models;
using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024;
using NSubstitute;
using NUnit.Framework;

namespace MonsterTradingCardGame_2024.Test.Http.Endpoints
{
    [TestFixture]
    public class UsersEndpointTests
    {
        private UsersEndpoint _endpoint;
        private UserHandler _userHandler;

        [SetUp]
        public void Setup()
        {
            _userHandler = Substitute.For<UserHandler>(Substitute.For<IUserRepository>());
            _endpoint = new UsersEndpoint(_userHandler);
        }

        [Test]
        public void HandleRequest_ValidRegistration_Returns201()
        {
            // Arrange
            var requestContent = "{\"Username\":\"testuser\",\"Password\":\"password123\"}";
            var request = CreateHttpRequest(MonsterTradingCardGame_2024.Http.HttpMethod.POST, new[] { "", "users" }, requestContent);
            var response = CreateHttpResponse();
            _userHandler.RegisterUser("testuser", "password123").Returns(true);

            // Act
            var result = _endpoint.HandleRequest(request, response);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(response.ResponseCode, Is.EqualTo(201));
            Assert.That(response.ResponseMessage, Is.EqualTo("User created"));
        }

        [Test]
        public void HandleRequest_EmptyContent_Returns400()
        {
            // Arrange
            var request = CreateHttpRequest(MonsterTradingCardGame_2024.Http.HttpMethod.POST, new[] { "", "users" }, "");
            var response = CreateHttpResponse();

            // Act
            var result = _endpoint.HandleRequest(request, response);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(response.ResponseCode, Is.EqualTo(400));
            Assert.That(response.ResponseMessage, Is.EqualTo("No content provided"));
        }

        [Test]
        public void HandleRequest_DuplicateUser_Returns409()
        {
            // Arrange
            var requestContent = "{\"Username\":\"testuser\",\"Password\":\"password123\"}";
            var request = CreateHttpRequest(MonsterTradingCardGame_2024.Http.HttpMethod.POST, new[] { "", "users" }, requestContent);
            var response = CreateHttpResponse();
            _userHandler.RegisterUser("testuser", "password123").Returns(false);

            // Act
            var result = _endpoint.HandleRequest(request, response);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(response.ResponseCode, Is.EqualTo(409));
            Assert.That(response.ResponseMessage, Is.EqualTo("User already exists"));
        }

        private HttpRequest CreateHttpRequest(MonsterTradingCardGame_2024.Http.HttpMethod method, string[] path, string content)
        {
            var reader = new StreamReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)));
            var request = new HttpRequest(reader);
            request.GetType().GetProperty("Method")?.SetValue(request, method);
            request.GetType().GetProperty("Path")?.SetValue(request, path);
            request.GetType().GetProperty("Content")?.SetValue(request, content);
            return request;
        }

        private HttpResponse CreateHttpResponse()
        {
            var writer = new StreamWriter(new MemoryStream());
            return new HttpResponse(writer);
        }
    }
}
