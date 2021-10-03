using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dryv.Validation;

namespace Dryv.Rules
{
    partial class DryvRules<TModel>
    {

		public DryvRules<TModel> Parameter<TResult>(string name, Func<TResult> parameter)
        {
			this.AddParameter(name, parameter);
			return this;
        }
        
		public DryvRules<TModel> Parameter<TResult>(string name, Func<Task<TResult>> parameter)
        {
			this.AddParameter(name, parameter);
			return this;
        }
		public DryvRules<TModel> Parameter<TOptions1, TResult>(string name, Func<TOptions1, TResult> parameter)
        {
			this.AddParameter(name, parameter, typeof(TOptions1));
			return this;
        }
        
		public DryvRules<TModel> Parameter<TOptions1, TResult>(string name, Func<TOptions1, Task<TResult>> parameter)
        {
			this.AddParameter(name, parameter, typeof(TOptions1));
			return this;
        }
		public DryvRules<TModel> Parameter<TOptions1, TOptions2, TResult>(string name, Func<TOptions1, TOptions2, TResult> parameter)
        {
			this.AddParameter(name, parameter, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
        
		public DryvRules<TModel> Parameter<TOptions1, TOptions2, TResult>(string name, Func<TOptions1, TOptions2, Task<TResult>> parameter)
        {
			this.AddParameter(name, parameter, typeof(TOptions1), typeof(TOptions2));
			return this;
        }
		public DryvRules<TModel> Parameter<TOptions1, TOptions2, TOptions3, TResult>(string name, Func<TOptions1, TOptions2, TOptions3, TResult> parameter)
        {
			this.AddParameter(name, parameter, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
        
		public DryvRules<TModel> Parameter<TOptions1, TOptions2, TOptions3, TResult>(string name, Func<TOptions1, TOptions2, TOptions3, Task<TResult>> parameter)
        {
			this.AddParameter(name, parameter, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3));
			return this;
        }
		public DryvRules<TModel> Parameter<TOptions1, TOptions2, TOptions3, TOptions4, TResult>(string name, Func<TOptions1, TOptions2, TOptions3, TOptions4, TResult> parameter)
        {
			this.AddParameter(name, parameter, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
        
		public DryvRules<TModel> Parameter<TOptions1, TOptions2, TOptions3, TOptions4, TResult>(string name, Func<TOptions1, TOptions2, TOptions3, TOptions4, Task<TResult>> parameter)
        {
			this.AddParameter(name, parameter, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4));
			return this;
        }
		public DryvRules<TModel> Parameter<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, TResult>(string name, Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, TResult> parameter)
        {
			this.AddParameter(name, parameter, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }
        
		public DryvRules<TModel> Parameter<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, TResult>(string name, Func<TOptions1, TOptions2, TOptions3, TOptions4, TOptions5, Task<TResult>> parameter)
        {
			this.AddParameter(name, parameter, typeof(TOptions1), typeof(TOptions2), typeof(TOptions3), typeof(TOptions4), typeof(TOptions5));
			return this;
        }

	}
}
