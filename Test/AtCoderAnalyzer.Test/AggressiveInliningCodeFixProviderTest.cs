﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = AtCoderAnalyzer.Test.CSharpCodeFixVerifier<
    AtCoderAnalyzer.AggressiveInliningAnalyzer,
    AtCoderAnalyzer.AggressiveInliningCodeFixProvider>;

namespace AtCoderAnalyzer.Test
{
    public class AggressiveInliningCodeFixProviderTest
    {
        [Fact]
        public async Task Empty()
        {
            var source = @"
using System.Collections.Generic;
struct IntComparer : IComparer<int>
{
    public int Compare(int x,  int y) => x.CompareTo(y);
}
";
            await VerifyCS.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NumOperator()
        {
            var source = @"
using AtCoder;
using System;
using System.Runtime.CompilerServices;
struct BoolOp : INumOperator<bool>
{
    public bool MinValue => true;
    public bool MaxValue => false;
    public bool Add(bool x, bool y) => x || y;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(bool x, bool y) => x.CompareTo(y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Decrement(bool x) => false;
    public bool Divide(bool x, bool y) => throw new NotImplementedException();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(bool x, bool y) => x == y;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetHashCode(bool obj) => obj.GetHashCode();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool GreaterThan(bool x, bool y) => x && !y;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool GreaterThanOrEqual(bool x, bool y) => x || !y;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Increment(bool x) => true;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool LessThan(bool x, bool y) => y && !x;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool LessThanOrEqual(bool x, bool y) => y || !x;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Minus(bool x) => false;
    public bool Modulo(bool x, bool y) => true ? true : throw new NotImplementedException();
    public bool Multiply(bool x, bool y)
    {
        throw new NotImplementedException();
    }
    public bool Subtract(bool x, bool y)
    {
        return default(bool?) ?? throw new NotImplementedException();
    }
}
";

            var fixedSource = @"
using AtCoder;
using System;
using System.Runtime.CompilerServices;
struct BoolOp : INumOperator<bool>
{
    public bool MinValue => true;
    public bool MaxValue => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Add(bool x, bool y) => x || y;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(bool x, bool y) => x.CompareTo(y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Decrement(bool x) => false;
    public bool Divide(bool x, bool y) => throw new NotImplementedException();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(bool x, bool y) => x == y;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetHashCode(bool obj) => obj.GetHashCode();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool GreaterThan(bool x, bool y) => x && !y;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool GreaterThanOrEqual(bool x, bool y) => x || !y;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Increment(bool x) => true;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool LessThan(bool x, bool y) => y && !x;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool LessThanOrEqual(bool x, bool y) => y || !x;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Minus(bool x) => false;
    public bool Modulo(bool x, bool y) => true ? true : throw new NotImplementedException();
    public bool Multiply(bool x, bool y)
    {
        throw new NotImplementedException();
    }
    public bool Subtract(bool x, bool y)
    {
        return default(bool?) ?? throw new NotImplementedException();
    }
}
";
            await VerifyCS.VerifyCodeFixAsync(source, new DiagnosticResult[]
            {
                VerifyCS.Diagnostic().WithSpan(9, 17, 9, 20).WithArguments("Add"),
            }, fixedSource);
        }

        [Fact]
        public async Task SegtreeOperator()
        {
            var source = @"
using AtCoder;
using System.Runtime.CompilerServices;
struct OpSeg : ISegtreeOperator<int>
{
    public int Identity => default;
    public int Operate(int x, int y) => System.Math.Max(x, y);
}
";
            var fixedSource = @"
using AtCoder;
using System.Runtime.CompilerServices;
struct OpSeg : ISegtreeOperator<int>
{
    public int Identity => default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Operate(int x, int y) => System.Math.Max(x, y);
}
";
            await VerifyCS.VerifyCodeFixAsync(source, new DiagnosticResult[]
            {
                VerifyCS.Diagnostic("AC0007").WithSpan(7, 16, 7, 23).WithArguments("Operate"),
            }, fixedSource);
        }

        [Fact]
        public async Task SegtreeOperator_With_AggressiveInlining()
        {
            var source = @"
using AtCoder;
using System.Runtime.CompilerServices;
struct OpSeg : ISegtreeOperator<int>
{
    public int Identity => default;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Operate(int x, int y) => System.Math.Max(x, y);
}
";
            await VerifyCS.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task LazySegtreeOperator()
        {
            var source = @"
using AtCoder;
using System.Runtime.CompilerServices;
struct Op : ILazySegtreeOperator<long, int>
{
    public long Identity => 0L;
    public int FIdentity => 0;
    public int Composition(int f, int g) => 0;
    public long Mapping(int f, long x) => 0L;
    public long Operate(long x, long y) => 0L;
}
";
            var fixedSource = @"
using AtCoder;
using System.Runtime.CompilerServices;
struct Op : ILazySegtreeOperator<long, int>
{
    public long Identity => 0L;
    public int FIdentity => 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Composition(int f, int g) => 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Mapping(int f, long x) => 0L;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Operate(long x, long y) => 0L;
}
";
            await VerifyCS.VerifyCodeFixAsync(source, new DiagnosticResult[]
            {
                VerifyCS.Diagnostic("AC0007").WithSpan(8, 16, 8, 27).WithArguments("Composition"),
                VerifyCS.Diagnostic("AC0007").WithSpan(9, 17, 9, 24).WithArguments("Mapping"),
                VerifyCS.Diagnostic("AC0007").WithSpan(10, 17, 10, 24).WithArguments("Operate"),
            }, fixedSource);
        }

        [Fact]
        public async Task LazySegtreeOperator_With_AggressiveInlining()
        {
            var source = @"
using AtCoder;
using System.Runtime.CompilerServices;
struct Op : ILazySegtreeOperator<long, int>
{
    public long Identity => 0L;
    public int FIdentity => 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Composition(int f, int g) => 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Mapping(int f, long x) => 0L;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Operate(long x, long y) => 0L;
}
";
            await VerifyCS.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task LazySegtreeOperator_With_Qualified_AggressiveInlining()
        {
            var source = @"
using AtCoder;
struct Op : ILazySegtreeOperator<long, int>
{
    public long Identity => 0L;
    public int FIdentity => 0;
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public int Composition(int f, int g) => 0;
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public long Mapping(int f, long x) => 0L;
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public long Operate(long x, long y) => 0L;
}
";
            await VerifyCS.VerifyAnalyzerAsync(source);
        }
    }
}