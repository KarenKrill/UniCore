using System;
using UnityEngine;

namespace KarenKrill.UniCore.Utilities
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SerializeInterfaceAttribute : PropertyAttribute { }
}
