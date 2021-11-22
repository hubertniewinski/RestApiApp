using Xunit;
using System;
using Moq;
using Catalog.Api.Repositories;
using System.Threading.Tasks;
using Catalog.Api.Entities;
using Catalog.Api.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;

namespace Catalog.UnitTests{
    public class ItemsControllerTests
    {
        private readonly Mock<IItemsRepository> repositoryStub = new ();
        private readonly Mock<ILogger<ItemsController>> loggerStub = new ();
        private readonly Random rand = new();

        [Fact]
        public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound()
        {
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<System.Guid>()))
                .ReturnsAsync((Item)null);

            var itemController = new ItemsController(repositoryStub.Object, loggerStub.Object);

            var result = await itemController.GetItemAsync(System.Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetItemAsync_ExistingItem_ReturnsExpectedItem()
        {
            Item item = CreateRandomItem();

             repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<System.Guid>()))
                .ReturnsAsync(item);

            var itemController = new ItemsController(repositoryStub.Object, loggerStub.Object);

            var result = await itemController.GetItemAsync(System.Guid.NewGuid());

            result.Value.Should().BeEquivalentTo(
                item,
                options => options.ComparingByMembers<Item>()); 
        }

        private Item CreateRandomItem()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Price = rand.Next(1000),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}