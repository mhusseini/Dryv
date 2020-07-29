using System;
using System.Collections;
using System.Collections.Generic;

namespace Dryv.Configuration
{
    public class DryvServiceCollection : IEnumerable<Type>
    {
        private readonly List<Type> items = new List<Type>();

        protected virtual void OnAdd(Type type)
        {

        }
        protected virtual void OnRemove(Type type)
        {

        }
        protected virtual void OnClear()
        {

        }

        public void Add(Type type)
        {
            //if (!typeof(IDryvCustomTranslator).IsAssignableFrom(type) &&
            //    !typeof(IDryvMethodCallTranslator).IsAssignableFrom(type))
            //{
            //    throw new ArgumentException($"The supplied type must implement {nameof(IDryvCustomTranslator)} or {nameof(IDryvMethodCallTranslator)}.");
            //}

            this.OnAdd(type);

            this.items.Add(type);
        }

        public void Add<T>() => this.Add(typeof(T));

        public void Clear()
        {
            this.OnClear();
            this.items.Clear();
        }

        public IEnumerator<Type> GetEnumerator() => this.items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.items).GetEnumerator();

        public void Remove(Type type)
        {
            this.OnRemove(type);
            this.items.Remove(type);
        }

        public void Remove<T>() => this.Remove(typeof(T));
    }
}