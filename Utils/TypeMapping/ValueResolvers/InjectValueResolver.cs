using System;
using System.Linq;

namespace Utils.TypeMapping.ValueResolvers
{
    class InjectValueResolver : SourceMappingResolverBase
    {

        protected override bool IsMemberSuitable(BuilderMemberInfo memberInfo)
        {
            return memberInfo.Attributes.Any(x => x is InjectValueAttribute);
        }


        protected override ISourceInfo ResolveSourceValue(MappingMemberInfo memberInfo)
        {
            var attribute = memberInfo.Attributes.FirstOrDefault(x => x is InjectValueAttribute) as InjectValueAttribute;

            if (attribute == null || attribute.Value == null)
            {
                throw new ArgumentException( "Mappling value is not set");
            }

            if (attribute.Value is string && string.IsNullOrWhiteSpace(attribute.Value.ToString()))
            {
                throw new ArgumentException("Mappling value cannot be empty");
            }

            var convertedValue = Convert.ChangeType(attribute.Value, memberInfo.Type);
            return SourceInfo.Create(convertedValue);;
        }

    }
}
