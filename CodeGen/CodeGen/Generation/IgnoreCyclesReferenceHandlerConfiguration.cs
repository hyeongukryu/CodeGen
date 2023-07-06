namespace CodeGen.Generation;

internal class IgnoreCyclesReferenceHandlerConfiguration : IReferenceHandlerConfiguration
{
    public bool PreserveReferences => false;
}