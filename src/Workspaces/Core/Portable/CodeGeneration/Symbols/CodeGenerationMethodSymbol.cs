﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.Editing;

namespace Microsoft.CodeAnalysis.CodeGeneration
{
    internal partial class CodeGenerationMethodSymbol : CodeGenerationAbstractMethodSymbol
    {
        public override ITypeSymbol ReturnType { get; }
        public override ImmutableArray<ITypeParameterSymbol> TypeParameters { get; }
        public override ImmutableArray<IParameterSymbol> Parameters { get; }
        public override ImmutableArray<IMethodSymbol> ExplicitInterfaceImplementations { get; }
        public override bool ReturnsByRef { get; }
        public override MethodKind MethodKind { get; }

        public CodeGenerationMethodSymbol(
            INamedTypeSymbol containingType,
            ImmutableArray<AttributeData> attributes,
            Accessibility declaredAccessibility,
            DeclarationModifiers modifiers,
            ITypeSymbol returnType,
            bool returnsByRef,
            IMethodSymbol explicitInterfaceSymbolOpt,
            string name,
            ImmutableArray<ITypeParameterSymbol> typeParameters,
            ImmutableArray<IParameterSymbol> parameters,
            ImmutableArray<AttributeData> returnTypeAttributes,
            MethodKind methodKind = MethodKind.Ordinary)
            : base(containingType, attributes, declaredAccessibility, modifiers, name, returnTypeAttributes)
        {
            this.ReturnType = returnType;
            this.ReturnsByRef = returnsByRef;
            this.TypeParameters = typeParameters.NullToEmpty();
            this.Parameters = parameters.NullToEmpty();
            this.MethodKind = methodKind;

            this.ExplicitInterfaceImplementations = explicitInterfaceSymbolOpt == null
                ? ImmutableArray.Create<IMethodSymbol>()
                : ImmutableArray.Create(explicitInterfaceSymbolOpt);

            this.OriginalDefinition = this;
        }

        protected override CodeGenerationSymbol Clone()
        {
            var result = new CodeGenerationMethodSymbol(this.ContainingType,
                this.GetAttributes(), this.DeclaredAccessibility, this.Modifiers,
                this.ReturnType, this.ReturnsByRef, this.ExplicitInterfaceImplementations.FirstOrDefault(),
                this.Name, this.TypeParameters, this.Parameters, this.GetReturnTypeAttributes());

            CodeGenerationMethodInfo.Attach(result,
                CodeGenerationMethodInfo.GetIsNew(this),
                CodeGenerationMethodInfo.GetIsUnsafe(this),
                CodeGenerationMethodInfo.GetIsPartial(this),
                CodeGenerationMethodInfo.GetIsAsync(this),
                CodeGenerationMethodInfo.GetStatements(this),
                CodeGenerationMethodInfo.GetHandlesExpressions(this));

            return result;
        }

        public override int Arity => this.TypeParameters.Length;

        public override bool ReturnsVoid
        {
            get
            {
                return this.ReturnType == null || this.ReturnType.SpecialType == SpecialType.System_Void;
            }
        }

        public override ImmutableArray<ITypeSymbol> TypeArguments
            => this.TypeParameters.As<ITypeSymbol>();

        public override IMethodSymbol ConstructedFrom => this;

        public override IMethodSymbol OverriddenMethod => null;

        public override IMethodSymbol ReducedFrom => null;

        public override ITypeSymbol GetTypeInferredDuringReduction(ITypeParameterSymbol reducedFromTypeParameter)
        {
            throw new InvalidOperationException();
        }

        public override IMethodSymbol ReduceExtensionMethod(ITypeSymbol receiverType)
        {
            return null;
        }

        public override IMethodSymbol PartialImplementationPart => null;

        public override IMethodSymbol PartialDefinitionPart => null;
    }
}