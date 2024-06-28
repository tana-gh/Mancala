using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace tana_gh.Mancala
{
    public static class RoleAttributeUtil
    {
        public static IEnumerable<Type> GetAllTypesWithRole(string role, SceneKind sceneKind)
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var roleAttributes =
                    type.GetCustomAttributes(typeof(RoleAttribute), false)
                    .Cast<RoleAttribute>()
                    .Where(attr => attr.Role == role && attr.SceneKind == sceneKind);
                foreach (var attr in roleAttributes)
                {
                    var genericParams = attr.GenericParams;
                    yield return genericParams.Length == 0 ? type : type.MakeGenericType(genericParams);
                }
            }
        }
    }
}
