using Moryx.AbstractionLayer.Products;

namespace Example.Products
{
    /// <summary>
    /// Config for the <see cref="ExampleProductImporter"/>
    /// </summary>
    public class ExampleProductImporterConfig : ProductImporterConfig
    {
        public override string PluginName => nameof(ExampleProductImporter);
    }
}