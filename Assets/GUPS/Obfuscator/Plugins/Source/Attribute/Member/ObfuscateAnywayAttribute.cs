// System
using System;

namespace GUPS.Obfuscator.Attribute
{
    /// <summary>
    /// Add this to a class or class member to obfuscate it anyway with a new name '_ObfuscateTo', although the settings did not allow to.
    /// If you set the parameter '_ObfuscateTo' to null or an empty string, the class or member will be obfuscated with a random name.
    /// For example if you do not want to obfuscate all public methods beside some specific.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Event)]
    public class ObfuscateAnywayAttribute : System.Attribute
    {
#pragma warning disable
        private String obfuscateTo;

        public ObfuscateAnywayAttribute(String _ObfuscateTo)
        {
            this.obfuscateTo = _ObfuscateTo;
        }
#pragma warning restore
    }
}