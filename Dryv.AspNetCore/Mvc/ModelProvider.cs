namespace Dryv.AspNetCore.Mvc
{
    internal class ModelProvider : IModelProvider
    {
        private object model;
        public void SetModel(object m) => this.model = m;
        public object GetModel() => this.model;
    }
}