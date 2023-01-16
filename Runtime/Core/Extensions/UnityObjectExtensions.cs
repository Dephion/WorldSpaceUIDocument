using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dephion.Ui.Core.Extensions
{
    public static class UnityObjectExtensions
    {
        public static T AddComponent<T>(this Object uo) where T : Component
        {
            return uo switch
            {
                GameObject go => go.AddComponent<T>(),
                Component co => co.gameObject.AddComponent<T>(),
                _ => throw new NotSupportedException()
            };
        }
    }
}