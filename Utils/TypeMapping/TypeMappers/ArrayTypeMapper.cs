﻿using System.Collections.Generic;

namespace Utils.TypeMapping.TypeMappers
{
    public class ArrayTypeMapper<TSource, TDest> : TypeMapperBase<IEnumerable<TSource>, IList<TDest>>
    {
        private readonly ITypeMapper<TSource, TDest> elementMapper;

        public ArrayTypeMapper(ITypeMapper<TSource, TDest> elementMapper)
        {
            this.elementMapper = elementMapper;
        }

        public override IOperationResult<IList<TDest>> TryMap(IEnumerable<TSource> source)
        {
            var list= new List<TDest>();
            foreach (TSource sourceItem in source)
            {
                var mappingValue = elementMapper.TryMap(sourceItem);
                list.Add(mappingValue.Value);
            }
            return OperationResult.Successfull(list.ToArray());
        }
    }
}
