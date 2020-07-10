using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(IEdmEntityType type,
            IList<string> positionalValues, Func<IEdmTypeReference, string, object> convertFunc)
        {
            if (positionalValues.Count == 1 && ResolveUnnamedKey != null)
            {
                string key = positionalValues.Single();
                if (key.StartsWith("'") && key.EndsWith("'"))
                {
                    key = key.Trim('\'');
                    var eventArgs = new ResolveUnnamedKeyEventArgs(key);
                    OnResolveUnnamedKey(this, eventArgs);
                    if (eventArgs.Guid.HasValue)
                    {
                        positionalValues.Clear();
                        positionalValues.Add(eventArgs.Guid.Value.ToString());
                    }
                }
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

        #region ResolveUnnamedKey Event

        /// <summary>
        /// First-pass event when the odata uri resolves unnamed string keys
        /// </summary>
        public static event EventHandler<ResolveUnnamedKeyEventArgs> ResolveUnnamedKey;
        internal static void OnResolveUnnamedKey(object sender, ResolveUnnamedKeyEventArgs eventArgs) => ResolveUnnamedKey?.Invoke(sender, eventArgs);

        #endregion
    }

    public class ResolveUnnamedKeyEventArgs : EventArgs
    {
        public ResolveUnnamedKeyEventArgs(string key)
        {
            Key = key;
        }

        public string Key { get; }
        public Guid? Guid { get; set; }
    }
}
