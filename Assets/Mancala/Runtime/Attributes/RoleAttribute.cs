using System;

namespace tana_gh.Mancala
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public class RoleAttribute : Attribute
    {
        public string Role { get; }
        public SceneKind SceneKind { get; }
        public Type[] GenericParams { get; }

        public RoleAttribute(string role, SceneKind sceneKind, params Type[] genericParams)
        {
            Role = role;
            SceneKind = sceneKind;
            GenericParams = genericParams;
        }
    }
}
