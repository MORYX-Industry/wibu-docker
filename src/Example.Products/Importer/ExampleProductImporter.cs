using System.Collections.Generic;
using System.Threading.Tasks;
using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Products.Management;

namespace Example.Products
{
    /// <summary>
    /// Imports products for Example
    /// </summary>
    [ExpectedConfig(typeof(ExampleProductImporterConfig))]
    [Plugin(LifeCycle.Transient, typeof(IProductImporter), Name = nameof(ExampleProductImporter))]
    public  class ExampleProductImporter :  ProductImporterBase<ExampleProductImporterConfig, ExampleImportParameters>, ILoggingComponent
    {
        /// <inheritdoc />
        public IModuleLogger Logger { get; set; }

        public IProductStorage Storage { get; set; }

        protected override Task<ProductImporterResult> Import(ProductImportContext context, ExampleImportParameters parameters)
        {
            var products = new List<ProductType>();

            // TODO: Create objects from parameters, file or endpoint

            return Task.FromResult(new ProductImporterResult { ImportedTypes = products });
        }
    }
}