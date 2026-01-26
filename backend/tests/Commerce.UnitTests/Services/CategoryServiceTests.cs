using Commerce.Repositories;
using Commerce.Repositories.Entities;
using Commerce.Services;
using Commerce.Shared.Enums;
using Commerce.Shared.Requests;
using Commerce.Shared.Responses;

namespace Commerce.UnitTests.Services;

public class CategoryServiceTests
{
    [Fact]
    public async Task GetCategoriesAsync_MapsAllCategories_AndPassesQueryParams()
    {
        // Arrange
        var repo = new FakeCategoryRepository
        {
            Categories =
            [
                new Category { Id = 1, Name = "Tools", Description = "Hand tools", IsActive = true },
                new Category { Id = 2, Name = "Power Tools", Description = "Electric tools", IsActive = true }
            ],
            TotalCount = 2,
            Page = 1,
            PageSize = 10
        };

        var sut = new CategoryService(repo);
        var queryParams = new GetCategoriesQueryParams { Page = 1, PageSize = 10 };

        // Act
        var results = await sut.GetCategoriesAsync(queryParams);

        // Assert
        Assert.Equal(2, results.Items.Count);

        Assert.Equal(1, results.Items[0].Id);
        Assert.Equal("Tools", results.Items[0].Name);
        Assert.Equal("Hand tools", results.Items[0].Description);

        Assert.Equal(2, results.Items[1].Id);
        Assert.Equal("Power Tools", results.Items[1].Name);
        Assert.Equal("Electric tools", results.Items[1].Description);

        Assert.Equal(2, results.TotalCount);
        Assert.Equal(1, results.Page);
        Assert.Equal(10, results.PageSize);

        Assert.Same(queryParams, repo.LastQueryParams);
    }

    [Fact]
    public async Task GetCategoriesAsync_WhenRepoReturnsEmpty_ReturnsEmptyPagedResult()
    {
        // Arrange
        var repo = new FakeCategoryRepository
        {
            Categories = [],
            TotalCount = 0,
            Page = 1,
            PageSize = 10
        };

        var sut = new CategoryService(repo);

        // Act
        var results = await sut.GetCategoriesAsync(new GetCategoriesQueryParams());

        // Assert
        Assert.Empty(results.Items);
        Assert.Equal(0, results.TotalCount);
        Assert.Equal(1, results.Page);
        Assert.Equal(10, results.PageSize);
    }

    [Fact]
    public async Task GetCategoryAdminDetailsAsync_WhenRepoReturnsNull_ReturnsNull()
    {
        // Arrange
        var repo = new FakeCategoryRepository
        {
            CategoryGraphById = null
        };

        var sut = new CategoryService(repo);

        // Act
        var result = await sut.GetCategoryAdminDetailsAsync(categoryId: 123);

        // Assert
        Assert.Null(result);
        Assert.Equal(123, repo.LastGraphCategoryId);
    }

    [Fact]
    public async Task GetCategoryAdminDetailsAsync_WhenRepoReturnsGraph_MapsParentsAndChildren()
    {
        // Arrange
        var electronics = new Category { Id = 1, Name = "Electronics", Description = "Root", IsActive = true };
        var computers = new Category { Id = 2, Name = "Computers", Description = "Child", IsActive = true };
        var laptops = new Category { Id = 3, Name = "Laptops", Description = "Leaf", IsActive = true };

        var graph = new Category
        {
            Id = computers.Id,
            Name = computers.Name,
            Description = computers.Description,
            IsActive = computers.IsActive,
            ParentLinks =
            [
                new CategoryLink { ParentCategoryId = electronics.Id, ParentCategory = electronics, ChildCategoryId = computers.Id, ChildCategory = computers }
            ],
            ChildLinks =
            [
                new CategoryLink { ParentCategoryId = computers.Id, ParentCategory = computers, ChildCategoryId = laptops.Id, ChildCategory = laptops }
            ]
        };

        var repo = new FakeCategoryRepository
        {
            CategoryGraphById = graph
        };

        var sut = new CategoryService(repo);

        // Act
        var result = await sut.GetCategoryAdminDetailsAsync(categoryId: 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result!.Id);
        Assert.Equal("Computers", result.Name);
        Assert.Equal("Child", result.Description);
        Assert.True(result.IsActive);

        Assert.Single(result.Parents);
        Assert.Equal(electronics.Id, result.Parents[0].Id);
        Assert.Equal(electronics.Name, result.Parents[0].Name);

        Assert.Single(result.Children);
        Assert.Equal(laptops.Id, result.Children[0].Id);
        Assert.Equal(laptops.Name, result.Children[0].Name);

        Assert.Equal(2, repo.LastGraphCategoryId);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetRootsAsync_PassesIncludeInactive_AndMaps(bool includeInactive)
    {
        // Arrange
        var repo = new FakeCategoryRepository
        {
            Roots =
            [
                new Category { Id = 10, Name = "Books", Description = "All books", IsActive = true }
            ]
        };

        var sut = new CategoryService(repo);

        // Act
        var result = await sut.GetRootsAsync(includeInactive);

        // Assert
        Assert.Single(result);
        Assert.Equal(10, result[0].Id);
        Assert.Equal("Books", result[0].Name);

        Assert.Equal(includeInactive, repo.LastRootsIncludeInactive);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetChildrenAsync_PassesArgs_AndMaps(bool includeInactive)
    {
        // Arrange
        var repo = new FakeCategoryRepository
        {
            Children =
            [
                new Category { Id = 11, Name = "Sci-Fi", Description = "Science Fiction", IsActive = true }
            ]
        };

        var sut = new CategoryService(repo);

        // Act
        var result = await sut.GetChildrenAsync(parentCategoryId: 5, includeInactive);

        // Assert
        Assert.Single(result);
        Assert.Equal(11, result[0].Id);
        Assert.Equal("Sci-Fi", result[0].Name);

        Assert.Equal(5, repo.LastChildrenParentId);
        Assert.Equal(includeInactive, repo.LastChildrenIncludeInactive);
    }

    [Theory]
    [InlineData(DbResultOption.Success, 55)]
    [InlineData(DbResultOption.NotFound, 0)]
    [InlineData(DbResultOption.Conflict, 0)]
    public async Task AddCategoryAsync_ReturnsRepoResult_AndCategoryId(DbResultOption repoResult, int repoCategoryId)
    {
        // Arrange
        var repo = new FakeCategoryRepository
        {
            AddResult = (repoResult, repoCategoryId)
        };

        var sut = new CategoryService(repo);

        var request = new CreateCategoryRequest
        {
            Name = "New Category",
            Description = "Desc",
            ParentCategoryIds = [1, 2]
        };

        // Act
        var result = await sut.AddCategoryAsync(request);

        // Assert
        Assert.Equal(repoResult, result.Result);
        Assert.Equal(repoCategoryId, result.CategoryId);
        Assert.Same(request, repo.LastCreateRequest);
    }

    [Theory]
    [InlineData(DbResultOption.Success)]
    [InlineData(DbResultOption.NotFound)]
    [InlineData(DbResultOption.Conflict)]
    [InlineData(DbResultOption.Error)]
    public async Task UpdateCategoryAsync_ReturnsRepoResult(DbResultOption repoResult)
    {
        // Arrange
        var repo = new FakeCategoryRepository
        {
            UpdateResult = repoResult
        };

        var sut = new CategoryService(repo);

        var request = new CreateCategoryRequest
        {
            Name = "Updated Category",
            Description = "Updated Desc"
        };

        // Act
        var result = await sut.UpdateCategoryAsync(request, categoryId: 123);

        // Assert
        Assert.Equal(repoResult, result);
        Assert.Same(request, repo.LastUpdateRequest);
        Assert.Equal(123, repo.LastUpdateCategoryId);
    }

    [Theory]
    [InlineData(DbResultOption.Success)]
    [InlineData(DbResultOption.NotFound)]
    [InlineData(DbResultOption.Error)]
    public async Task ToggleCategoryAsync_ReturnsRepoResult(DbResultOption repoResult)
    {
        // Arrange
        var repo = new FakeCategoryRepository
        {
            ToggleResult = repoResult
        };

        var sut = new CategoryService(repo);

        // Act
        var result = await sut.ToggleCategoryAsync(categoryId: 77);

        // Assert
        Assert.Equal(repoResult, result);
        Assert.Equal(77, repo.LastToggleCategoryId);
    }

    [Theory]
    [InlineData(DbResultOption.Success)]
    [InlineData(DbResultOption.NotFound)]
    [InlineData(DbResultOption.Invalid)]
    [InlineData(DbResultOption.AlreadyExists)]
    [InlineData(DbResultOption.Conflict)]
    [InlineData(DbResultOption.Error)]
    public async Task AttachCategoryAsync_ReturnsRepoResult(DbResultOption repoResult)
    {
        // Arrange
        var repo = new FakeCategoryRepository
        {
            AttachResult = repoResult
        };

        var sut = new CategoryService(repo);

        // Act
        var result = await sut.AttachCategoryAsync(parentCategoryId: 1, childCategoryId: 2);

        // Assert
        Assert.Equal(repoResult, result);
        Assert.Equal(1, repo.LastAttachParentId);
        Assert.Equal(2, repo.LastAttachChildId);
    }

    [Theory]
    [InlineData(DbResultOption.Success)]
    [InlineData(DbResultOption.NotFound)]
    [InlineData(DbResultOption.Conflict)]
    [InlineData(DbResultOption.Error)]
    public async Task DetachCategoryAsync_ReturnsRepoResult(DbResultOption repoResult)
    {
        // Arrange
        var repo = new FakeCategoryRepository
        {
            DetachResult = repoResult
        };

        var sut = new CategoryService(repo);

        // Act
        var result = await sut.DetachCategoryAsync(parentCategoryId: 1, childCategoryId: 2);

        // Assert
        Assert.Equal(repoResult, result);
        Assert.Equal(1, repo.LastDetachParentId);
        Assert.Equal(2, repo.LastDetachChildId);
    }

    /// <summary>
    /// Minimal fake for unit tests (same style as ProductServiceTests)
    /// </summary>
    private sealed class FakeCategoryRepository : ICategoryRepository
    {
        // Return data for paging
        public List<Category> Categories { get; set; } = [];
        public int TotalCount { get; set; } = 0;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Return data for graph
        public Category? CategoryGraphById { get; set; }

        // Return data for nav
        public List<Category> Roots { get; set; } = [];
        public List<Category> Children { get; set; } = [];

        // Return results for mutations
        public (DbResultOption Result, int CategoryId) AddResult { get; set; } = (DbResultOption.Error, 0);
        public DbResultOption UpdateResult { get; set; } = DbResultOption.Error;
        public DbResultOption ToggleResult { get; set; } = DbResultOption.Error;
        public DbResultOption AttachResult { get; set; } = DbResultOption.Error;
        public DbResultOption DetachResult { get; set; } = DbResultOption.Error;

        // Captured args
        public GetCategoriesQueryParams? LastQueryParams { get; private set; }
        public int? LastGraphCategoryId { get; private set; }
        public int? LastSimpleCategoryId {get; private set; }

        public bool? LastRootsIncludeInactive { get; private set; }
        public int? LastChildrenParentId { get; private set; }
        public bool? LastChildrenIncludeInactive { get; private set; }

        public CreateCategoryRequest? LastCreateRequest { get; private set; }
        public CreateCategoryRequest? LastUpdateRequest { get; private set; }
        public int? LastUpdateCategoryId { get; private set; }
        public int? LastToggleCategoryId { get; private set; }

        public int? LastAttachParentId { get; private set; }
        public int? LastAttachChildId { get; private set; }
        public int? LastDetachParentId { get; private set; }
        public int? LastDetachChildId { get; private set; }

        public Task<PagedResult<Category>> GetAllCategoriesAsync(GetCategoriesQueryParams queryParams, CancellationToken ct = default)
        {
            LastQueryParams = queryParams;

            return Task.FromResult(
                new PagedResult<Category>(
                    Categories,   // Items
                    Page,         // Page
                    PageSize,     // PageSize
                    TotalCount    // TotalCount
                )
            );
        }


        public Task<Category?> GetCategoryGraphByIdAsync(int id, CancellationToken ct = default)
        {
            LastGraphCategoryId = id;
            return Task.FromResult(CategoryGraphById);
        }

        public Task<IReadOnlyList<Category>> GetRootsAsync(bool includeInactive = false, CancellationToken ct = default)
        {
            LastRootsIncludeInactive = includeInactive;
            return Task.FromResult<IReadOnlyList<Category>>(Roots);
        }

        public Task<IReadOnlyList<Category>> GetChildrenAsync(int parentCategoryId, bool includeInactive = false, CancellationToken ct = default)
        {
            LastChildrenParentId = parentCategoryId;
            LastChildrenIncludeInactive = includeInactive;
            return Task.FromResult<IReadOnlyList<Category>>(Children);
        }

        public Task<(DbResultOption Result, int CategoryId)> AddCategoryAsync(CreateCategoryRequest category, CancellationToken ct = default)
        {
            LastCreateRequest = category;
            return Task.FromResult(AddResult);
        }

        public Task<DbResultOption> UpdateCategoryAsync(CreateCategoryRequest category, int categoryId, CancellationToken ct = default)
        {
            LastUpdateRequest = category;
            LastUpdateCategoryId = categoryId;
            return Task.FromResult(UpdateResult);
        }

        public Task<DbResultOption> ToggleCategoryAsync(int categoryId, CancellationToken ct = default)
        {
            LastToggleCategoryId = categoryId;
            return Task.FromResult(ToggleResult);
        }

        public Task<DbResultOption> AttachCategoryAsync(int parentCategoryId, int childCategoryId, CancellationToken ct = default)
        {
            LastAttachParentId = parentCategoryId;
            LastAttachChildId = childCategoryId;
            return Task.FromResult(AttachResult);
        }

        public Task<DbResultOption> DetachCategoryAsync(int parentCategoryId, int childCategoryId, CancellationToken ct = default)
        {
            LastDetachParentId = parentCategoryId;
            LastDetachChildId = childCategoryId;
            return Task.FromResult(DetachResult);
        }

        public Task<Category?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            LastSimpleCategoryId = id;
            return Task.FromResult<Category?>(CategoryGraphById);
        }
    }
}
