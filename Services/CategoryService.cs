using Microsoft.EntityFrameworkCore;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Categories;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Web.Data;
using SmartPOS.Web.Models;

namespace SmartPOS.Services
{
    /// <summary>
    /// Service for managing categories in the SmartPOS system.
    /// Supports hierarchical (parentâ€“child) category structures.
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly IDbContextFactory<AppDbContext> _factory;

        public CategoryService(IDbContextFactory<AppDbContext> factory)
        public Task<ApiResponse<CategoryDto>> Create(CreateCategoryDto dto)
        {
            throw new NotImplementedException();
            _factory = factory;
        }

        /// <summary>
        /// Retrieves a single category by its unique identifier,
        /// including its parent name and immediate sub-categories.
        /// </summary>
        /// <param name="id">The category ID.</param>
        /// <returns>An ApiResponse containing the CategoryDto, or a failure message if not found.</returns>
        public async Task<ApiResponse<CategoryDto>> GetById(int id)
        {
            try
            {
                using var context = _factory.CreateDbContext();
                var category = await context.Categories
                    .Include(c => c.ParentCategory)
                    .Include(c => c.SubCategories)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                    return ApiResponse<CategoryDto>.Fail("Category not found.");

                return ApiResponse<CategoryDto>.Ok(MapToDto(category));
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoryDto>.Fail($"Error retrieving category: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all categories as a flat list.
        /// Each category includes its parent name and immediate sub-categories.
        /// </summary>
        /// <returns>An ApiResponse containing a list of CategoryDto objects.</returns>
        public async Task<ApiResponse<List<CategoryDto>>> GetAll()
        {
            try
            {
                using var context = _factory.CreateDbContext();
                var categories = await context.Categories
                    .Include(c => c.ParentCategory)
                    .Include(c => c.SubCategories)
                    .ToListAsync();

                return ApiResponse<List<CategoryDto>>.Ok(
                    categories.Select(MapToDto).ToList());
            throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CategoryDto>>.Fail($"Error retrieving categories: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns the full category hierarchy as a nested tree structure.
        /// Root categories (ParentCategoryId == null) sit at the top level,
        /// with their children recursively nested inside SubCategories.
        /// </summary>
        /// <returns>An ApiResponse containing a list of root-level CategoryDto objects with nested sub-categories.</returns>
        public async Task<ApiResponse<List<CategoryDto>>> GetTree()
        {
            try
            {
                using var context = _factory.CreateDbContext();
                // Load every category with its parent in a single query
                var allCategories = await context.Categories
                    .Include(c => c.ParentCategory)
                    .ToListAsync();

                // Build a lookup of children keyed by parent ID
                var childrenLookup = allCategories
                    .Where(c => c.ParentCategoryId != null)
                    .GroupBy(c => c.ParentCategoryId!.Value)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Start from root categories and recurse
                var roots = allCategories
                    .Where(c => c.ParentCategoryId == null)
                    .Select(c => BuildTreeNode(c, childrenLookup))
                    .ToList();

                return ApiResponse<List<CategoryDto>>.Ok(roots);
            throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CategoryDto>>.Fail($"Error building category tree: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new category. If a ParentCategoryId is provided, verifies
        /// the parent exists before persisting.
        /// </summary>
        /// <param name="dto">The category creation data transfer object.</param>
        /// <returns>An ApiResponse containing the newly created CategoryDto.</returns>
        public async Task<ApiResponse<CategoryDto>> Create(CreateCategoryDto dto)
        {
            try
            {
                using var context = _factory.CreateDbContext();
                // Validate parent exists when specified
                if (dto.ParentCategoryId.HasValue)
                {
                    var parentExists = await context.Categories
                        .AnyAsync(c => c.Id == dto.ParentCategoryId.Value);

                    if (!parentExists)
                        return ApiResponse<CategoryDto>.Fail("Parent category not found.");
            throw new NotImplementedException();
                }

                var category = new Category
        public Task<ApiResponse<CategoryDto>> Update(UpdateCategoryDto dto)
                {
            throw new NotImplementedException();
                    Name = dto.Name,
                    Description = dto.Description,
                    ImageURL = dto.ImageURL,
                    ParentCategoryId = dto.ParentCategoryId
                };

                context.Categories.Add(category);
                await context.SaveChangesAsync();

                // Re-fetch with includes for a complete DTO
                return await GetById(category.Id);
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoryDto>.Fail($"Error creating category: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing category's name, description, image, and parent.
        /// Prevents a category from being set as its own parent.
        /// </summary>
        /// <param name="dto">The category update data transfer object.</param>
        /// <returns>An ApiResponse containing the updated CategoryDto.</returns>
        public async Task<ApiResponse<CategoryDto>> Update(UpdateCategoryDto dto)
        {
            try
            {
                using var context = _factory.CreateDbContext();
                var category = await context.Categories.FindAsync(dto.Id);
                if (category == null)
                    return ApiResponse<CategoryDto>.Fail("Category not found.");

                // Guard against circular self-reference
                if (dto.ParentCategoryId.HasValue && dto.ParentCategoryId.Value == dto.Id)
                    return ApiResponse<CategoryDto>.Fail("A category cannot be its own parent.");

                // Validate parent exists when specified
                if (dto.ParentCategoryId.HasValue)
                {
                    var parentExists = await context.Categories
                        .AnyAsync(c => c.Id == dto.ParentCategoryId.Value);

                    if (!parentExists)
                        return ApiResponse<CategoryDto>.Fail("Parent category not found.");
                }

                category.Name = dto.Name;
                category.Description = dto.Description;
                category.ImageURL = dto.ImageURL;
                category.ParentCategoryId = dto.ParentCategoryId;

                await context.SaveChangesAsync();

                return await GetById(category.Id);
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoryDto>.Fail($"Error updating category: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a category by its ID.
        /// Deletion will fail if the category still has products assigned
        /// (enforced by the database Restrict delete behaviour).
        /// </summary>
        /// <param name="id">The category ID.</param>
        /// <returns>An ApiResponse indicating success or failure.</returns>
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                using var context = _factory.CreateDbContext();
                var category = await context.Categories
                    .Include(c => c.SubCategories)
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                    return ApiResponse.Fail("Category not found.");

                if (category.Products.Any())
                    return ApiResponse.Fail("Cannot delete a category that still has products. Reassign or remove products first.");

                if (category.SubCategories.Any())
                    return ApiResponse.Fail("Cannot delete a category that has sub-categories. Remove or reassign sub-categories first.");

                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return ApiResponse.Ok("Category deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail($"Error deleting category: {ex.Message}");
            }
        }

        // ──────────────────────────────────────────────────────
        //  Private helpers
        // ──────────────────────────────────────────────────────

        /// <summary>
        /// Maps a Category entity to a flat CategoryDto
        /// (immediate sub-categories only, no deep recursion).
        /// </summary>
        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageURL = category.ImageURL,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategoryName = category.ParentCategory?.Name ?? string.Empty,
                SubCategories = category.SubCategories
                    .Select(sc => new CategoryDto
                    {
                        Id = sc.Id,
                        Name = sc.Name,
                        Description = sc.Description,
                        ImageURL = sc.ImageURL,
                        ParentCategoryId = sc.ParentCategoryId,
                        ParentCategoryName = category.Name
                    })
                    .ToList()
            };
        }

        /// <summary>
        /// Recursively builds a CategoryDto tree node from a Category entity
        /// using a pre-built children lookup dictionary.
        /// </summary>
        private static CategoryDto BuildTreeNode(
            Category category,
            Dictionary<int, List<Category>> childrenLookup)
        {
            var dto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageURL = category.ImageURL,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategoryName = category.ParentCategory?.Name ?? string.Empty
            };

            if (childrenLookup.TryGetValue(category.Id, out var children))
            {
                dto.SubCategories = children
                    .Select(child => BuildTreeNode(child, childrenLookup))
                    .ToList();
            }

            return dto;
        }
    }
}
