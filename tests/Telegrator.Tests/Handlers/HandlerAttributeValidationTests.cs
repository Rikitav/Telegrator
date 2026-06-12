/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using FluentAssertions;
using Telegrator.Attributes;
using Telegrator.Core.Attributes;
using Telegrator.Core.Handlers;
using Telegrator.Handlers;
using Xunit;

namespace Telegrator.Tests.Handlers;

public class HandlerAttributeValidationTests
{
    [Fact]
    public void AllHandlerAttributes_ShouldAllowCorrespondingBranchingHandler()
    {
        // Arrange
        Type baseAttributeType = typeof(UpdateHandlerAttribute<>);
        Type[] handlerAttributeTypes = typeof(MessageHandler).Assembly
            .GetTypes()
            .Where(t => t.IsClass
                        && !t.IsAbstract
                        && t.IsSubclassOfRawGeneric(baseAttributeType)
                        && t != typeof(AnyUpdateHandlerAttribute))
            .ToArray();

        handlerAttributeTypes.Should().NotBeEmpty();

        foreach (Type attributeType in handlerAttributeTypes)
        {
            // Get primary handler type from UpdateHandlerAttribute<T>
            Type? primaryHandlerType = attributeType.BaseType?.GetGenericArguments().FirstOrDefault();
            primaryHandlerType.Should().NotBeNull($"because {attributeType.Name} must inherit from UpdateHandlerAttribute<T>");

            string branchingHandlerName = $"Branching{primaryHandlerType!.Name}";
            Type? branchingHandlerType = typeof(MessageHandler).Assembly
                .GetTypes()
                .FirstOrDefault(t => t.Name == branchingHandlerName
                                     && t.IsSubclassOfRawGeneric(typeof(BranchingUpdateHandler<>)));

            branchingHandlerType.Should().NotBeNull($"because branching handler {branchingHandlerName} should exist for {primaryHandlerType.Name}");

            // Act
            UpdateHandlerAttributeBase? attributeInstance = Activator.CreateInstance(attributeType, 0) as UpdateHandlerAttributeBase;
            attributeInstance.Should().NotBeNull();

            // Assert
            attributeInstance!.ExpectingHandlerType
                .Should().Contain(branchingHandlerType,
                    $"because {attributeType.Name} should allow decorating {branchingHandlerType!.Name}");
        }
    }
}

file static class TypeExtensions
{
    public static bool IsSubclassOfRawGeneric(this Type? toCheck, Type generic)
    {
        while (toCheck is not null && toCheck != typeof(object))
        {
            Type current = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == current)
                return true;

            toCheck = toCheck.BaseType;
        }

        return false;
    }
}
