using System;
using System.Collections.Generic;
using System.CodeDom;

namespace DotNetZen.CodeDom
{
    /// <summary>
    /// Acts as a cache for <see cref="CodeTypeReference"/> objects.
    /// </summary>
    public static class CodeTypeReferenceStore
    {
        private static Dictionary<RuntimeTypeHandle, CodeTypeReference> typeReferences = 
            new Dictionary<RuntimeTypeHandle, CodeTypeReference>();
        private static Dictionary<string, CodeTypeReference> nameReferences = 
            new Dictionary<string, CodeTypeReference>();

        /// <summary>
        /// Gets a <see cref="CodeTypeReference"/> by the <see cref="Type"/> it should reference.
        /// </summary>
        /// <param name="type">The type being referenced.</param>
        /// <returns>A <see cref="CodeTypeReference"/> object.</returns>
        public static CodeTypeReference Get(Type type)
        {
            RuntimeTypeHandle handle = type.TypeHandle;

            if (!typeReferences.ContainsKey(handle))
            {
                typeReferences.Add(handle, new CodeTypeReference(type));
            }

            return typeReferences[handle];
        }

        /// <summary>
        /// Gets a <see cref="CodeTypeReference"/> by the name of the <see cref="Type"/> it should reference.
        /// </summary>
        /// <param name="typeName">The name of the type being referenced.</param>
        /// <returns>A <see cref="CodeTypeReference"/> object.</returns>
        public static CodeTypeReference Get(string typeName)
        {
            if (!nameReferences.ContainsKey(typeName))
            {
                nameReferences.Add(typeName, new CodeTypeReference(typeName));
            }

            return nameReferences[typeName];
        }
    }
}
