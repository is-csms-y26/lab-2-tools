using SourceKit.Generators.Builder.Annotations;

namespace Lab2.Tools.Abstractions.Persistence;

[GenerateBuilder]
public partial record ConfigurationQuery([RequiredValue] int PageSize, [RequiredValue] int Cursor);