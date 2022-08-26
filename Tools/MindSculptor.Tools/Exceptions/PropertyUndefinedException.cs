using System;

namespace MindSculptor.Tools.Exceptions
{
    public class PropertyUndefinedException : InvalidOperationException
    {
        private readonly string? checkPropertyName;
        public string PropertyName { get; }

        public PropertyUndefinedException(string propertyName, string? checkPropertyName = null)
        {
            PropertyName            = propertyName;
            this.checkPropertyName  = checkPropertyName;
        }

        public override string Message => $"Property '{PropertyName}' is undefined." + 
            (checkPropertyName == null ? string.Empty : $" Please check the value of '{checkPropertyName}' prior to accessing this property.");
    }
}
