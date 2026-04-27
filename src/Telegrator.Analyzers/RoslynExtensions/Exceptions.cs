namespace Telegrator.Analyzers.RoslynExtensions;

#pragma warning disable RCS1194 // Implement exception constructors
public class TargteterNotFoundException() : Exception();
public class BaseClassTypeNotFoundException() : Exception();
public class AncestorNotFoundException() : Exception();
#pragma warning restore RCS1194 // Implement exception constructors
