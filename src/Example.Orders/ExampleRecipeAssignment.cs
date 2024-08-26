using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.Orders;
using Moryx.Orders.Assignment;

namespace Example.Orders
{
    /// <summary>
    /// Recipe assignment for the Example
    /// </summary>
    [Plugin(LifeCycle.Singleton, typeof(IRecipeAssignment), Name = nameof(ExampleRecipeAssignment))]
    public class ExampleRecipeAssignment : RecipeAssignmentBase<RecipeAssignmentConfig>
    {
        /// <inheritdoc />
        public override async Task<IReadOnlyList<IProductRecipe>> SelectRecipes(Operation operation, IOperationLogger operationLogger)
        {
            return new[] { await LoadDefaultRecipe(operation.Product) };
        }

        /// <inheritdoc />
        public override Task<bool> ProcessRecipe(IProductRecipe clone, Operation operation, IOperationLogger operationLogger)
        {
            return Task.FromResult(true);
        }
    }
}