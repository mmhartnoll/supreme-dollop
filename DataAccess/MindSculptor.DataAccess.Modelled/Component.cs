using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MindSculptor.DataAccess.Modelled
{
    public abstract class Component
    {
        private readonly Lazy<FieldInfo> fieldInfoLoader;
        private readonly Lazy<Container> componentContainerLoader;

        protected Type DefiningType { get; }
        protected Container Container => componentContainerLoader.Value;
        protected FieldInfo FieldInfo => fieldInfoLoader.Value;

        public string Name => FieldInfo.Name;

        protected Component(Definition definition)
        {
            DefiningType = definition.DefiningType;

            fieldInfoLoader = new Lazy<FieldInfo>(LoadFieldInfo);
            componentContainerLoader = new Lazy<Container>(LoadComponentContainer);
        }

        private Container LoadComponentContainer() => Activator.CreateInstance(DefiningType) as Container ??
            throw new InvalidCastException($"DefiningType '{DefiningType.Name}' is not an instance of the type '{nameof(Container)}'.");

        private FieldInfo LoadFieldInfo()
        {
            var componentFieldInfo = DefiningType
                .GetFields()
                .Where(fieldInfo => fieldInfo.FieldType.IsSubclassOf(typeof(Component)));
            foreach(var fieldInfo in componentFieldInfo)
            {
                var component = fieldInfo.GetValue(Container) as Component;
                if (component != null && Equals(component))
                    return fieldInfo;
            }
            throw new Exception($"Failed to find '{GetType().Name}' field expected to be defined in '{DefiningType.Name}'.");
        }

        public abstract class Definition
        {
            internal Type DefiningType { get; }

            protected Definition()
            {
                // Move up the call stack until we find a method that is not defined in a sub-class of this class. This should be the initalizer of the class where the component is defined.

                Type? definingRecordType = null;
                foreach (var stackFrame in new StackTrace().GetFrames())
                {
                    var typeDeclaringMethod = stackFrame?.GetMethod()?.DeclaringType;
                    if (typeDeclaringMethod != null && typeDeclaringMethod != typeof(Definition) && !typeDeclaringMethod.IsSubclassOf(typeof(Definition)))
                    {
                        definingRecordType = typeDeclaringMethod;
                        break;
                    }
                }
                DefiningType = definingRecordType ?? throw new Exception($"Failed to find the class defining the instance of '{GetType().Name}'");
            }
        }
    }
}
