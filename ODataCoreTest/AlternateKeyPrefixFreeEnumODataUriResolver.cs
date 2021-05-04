using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ODataCoreTest
{
    public class AlternateKeyPrefixFreeEnumODataUriResolver : ODataUriResolver
    {
        StringAsEnumResolver StringAsEnum { get; }
        AlternateKeysODataUriResolver AlternateKeys { get; }

        public AlternateKeyPrefixFreeEnumODataUriResolver(IEdmModel model, bool enableCaseInsensitive = false)
        {
            StringAsEnum = new StringAsEnumResolver();
            AlternateKeys = new AlternateKeysODataUriResolver(model);
            EnableCaseInsensitive = enableCaseInsensitive;
        }

        bool enableCaseInsensitive;
        public override bool EnableCaseInsensitive
        {
            get => enableCaseInsensitive;
            set
            {
                enableCaseInsensitive = value;
                StringAsEnum.EnableCaseInsensitive = enableCaseInsensitive;
                AlternateKeys.EnableCaseInsensitive = enableCaseInsensitive;
            }
        }

        #region AlternateKeysODataUriResolver

        public override IEnumerable<IEdmOperation> ResolveBoundOperations(IEdmModel model, string identifier,
            IEdmType bindingType)
        {
            return AlternateKeys.ResolveBoundOperations(model, identifier, bindingType);
        }

        public override IEnumerable<IEdmOperation> ResolveUnboundOperations(IEdmModel model, string identifier)
        {
            return AlternateKeys.ResolveUnboundOperations(model, identifier);
        }

        public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(IEdmEntityType type, IDictionary<string, string> namedValues, Func<IEdmTypeReference, string, object> convertFunc)
        {
            return AlternateKeys.ResolveKeys(type, namedValues, convertFunc);
        }

        public override IEdmNavigationSource ResolveNavigationSource(IEdmModel model, string identifier)
        {
            return base.ResolveNavigationSource(model, identifier);
        }

        public override IEnumerable<IEdmOperationImport> ResolveOperationImports(IEdmModel model, string identifier)
        {
            return base.ResolveOperationImports(model, identifier);
        }

        public override IEdmProperty ResolveProperty(IEdmStructuredType type, string propertyName)
        {
            return base.ResolveProperty(type, propertyName);
        }

        public override IEdmTerm ResolveTerm(IEdmModel model, string termName)
        {
            return base.ResolveTerm(model, termName);
        }

        public override IEdmSchemaType ResolveType(IEdmModel model, string typeName)
        {
            return base.ResolveType(model, typeName);
        }

        public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(IEdmEntityType type,
            IList<string> positionalValues, Func<IEdmTypeReference, string, object> convertFunc)
        {
            if (positionalValues.Contains("CreateEntity"))
            {
                return new List<KeyValuePair<string, object>>();
            }

            return AlternateKeys.ResolveKeys(type, positionalValues, convertFunc);
        }

        #endregion

        #region StringAsEnumResolver

        public override void PromoteBinaryOperandTypes(BinaryOperatorKind binaryOperatorKind,
            ref SingleValueNode leftNode, ref SingleValueNode rightNode, out IEdmTypeReference typeReference)
        {
            StringAsEnum.PromoteBinaryOperandTypes(binaryOperatorKind, ref leftNode, ref rightNode, out typeReference);
        }

        public override IDictionary<IEdmOperationParameter, SingleValueNode> ResolveOperationParameters(
            IEdmOperation operation, IDictionary<string, SingleValueNode> input)
        {
            return StringAsEnum.ResolveOperationParameters(operation, input);
        }

        #endregion
    }
}
