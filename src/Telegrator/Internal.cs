#pragma warning disable IDE0130
namespace System.Diagnostics.CodeAnalysis;
#pragma warning restore IDE0130

[Flags]
internal enum DynamicallyAccessedMemberTypes
{
    None = 0,
    PublicParameterlessConstructor = 1,
    PublicConstructors = 3,
    NonPublicConstructors = 4,
    PublicMethods = 8,
    NonPublicMethods = 0x10,
    PublicFields = 0x20,
    NonPublicFields = 0x40,
    PublicNestedTypes = 0x80,
    NonPublicNestedTypes = 0x100,
    PublicProperties = 0x200,
    NonPublicProperties = 0x400,
    PublicEvents = 0x800,
    NonPublicEvents = 0x1000,
    Interfaces = 0x2000,
    All = -1
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.GenericParameter | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue | AttributeTargets.Struct, Inherited = false)]
internal sealed class DynamicallyAccessedMembersAttribute : Attribute
{
    public DynamicallyAccessedMemberTypes MemberTypes { get; }

    public DynamicallyAccessedMembersAttribute(DynamicallyAccessedMemberTypes memberTypes)
    {
        MemberTypes = memberTypes;
    }
}
