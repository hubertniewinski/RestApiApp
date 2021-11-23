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
using Catalog.Api.Dtos;
using System.Collections.Generic;

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

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetItemAsync_ExistingItem_ReturnsExpectedItem()
        {
            Item item = CreateRandomItem();

             repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<System.Guid>()))
                .ReturnsAsync(item);

            var itemController = new ItemsController(repositoryStub.Object, loggerStub.Object);

            var result = await itemController.GetItemAsync(System.Guid.NewGuid());

            result.Value.Should().BeEquivalentTo(item); 
        }

        [Fact]
        public async Task GetItemsAsync_WithExistingItems_ReturnsAllItems()
        {
            var expectedItems = new[]{CreateRandomItem(), CreateRandomItem(), CreateRandomItem()};

            repositoryStub.Setup(repo => repo.GetItemsAsync()).ReturnsAsync(expectedItems);

            var itemController = new ItemsController(repositoryStub.Object, loggerStub.Object);

            var result = await itemController.GetItemsAsync();

            result.Should().BeEquivalentTo(expectedItems); 
        }

        [Fact]
        public async Task GetItemsAsync_WithMatchingItems_ReturnsMatchingItems()
        {
            var allItems = new[]
            {
                new Item(){Name = "Potion"},
                new Item(){Name = "Antidote"},
                new Item(){Name = "Hi-Potion"}
            };
            
            var nameToMatch = "Potion";

            repositoryStub.Setup(repo => repo.GetItemsAsync()).ReturnsAsync(allItems);

            var itemController = new ItemsController(repositoryStub.Object, loggerStub.Object);

            IEnumerable<ItemDto> foundItems = await itemController.GetItemsAsync(nameToMatch);

            foundItems.Should().OnlyContain(
                item => item.Name == allItems[0].Name || item.Name == allItems[2].Name
            );
        }

        [Fact]
        public async Task CreateItemAsync_WithItemToCreate_ReturnsCreatedItem()
        {
            var itemToCreate = new CreateItemDto(
                Guid.NewGuid().ToString(), 
                Guid.NewGuid().ToString(), 
                rand.Next(1,1000));

            var itemController = new ItemsController(repositoryStub.Object, loggerStub.Object);

            var result = await itemController.CreateItemAsync(itemToCreate);

            var createdItem = (result.Result as CreatedAtActionResult).Value as ItemDto;
             
            createdItem.Should().BeEquivalentTo(
                itemToCreate,
                options => options.ComparingByMembers<ItemDto>().ExcludingMissingMembers() 
            );

            createdItem.Id.Should().NotBeEmpty();
            createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, new TimeSpan(0,0,0,0,1000));
        }

        [Fact]
        public async Task UpdateItemAsync_WithExistingItem_ReturnsNoContent()
        { 
            Item item = CreateRandomItem();

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<System.Guid>()))
                .ReturnsAsync(item);

            var itemId = item.Id;
            var itemToUpdate = new UpdateItemDto(
                Guid.NewGuid().ToString(), 
                Guid.NewGuid().ToString(),
                item.Price + 3);

            var itemController = new ItemsController(repositoryStub.Object, loggerStub.Object);

            var result = await itemController.UpdateItemAsync(itemId, itemToUpdate);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteItemAsync_WithExistingItem_ReturnsNoContent()
        { 
            Item item = CreateRandomItem();

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<System.Guid>()))
                .ReturnsAsync(item);

            var itemController = new ItemsController(repositoryStub.Object, loggerStub.Object);

            var result = await itemController.DeleteItemAsync(item.Id);

            result.Should().BeOfType<NoContentResult>();
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